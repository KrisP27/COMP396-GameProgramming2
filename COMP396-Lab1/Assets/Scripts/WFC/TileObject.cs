using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class TileObject : MonoBehaviour
{
    [SerializeField] private Renderer target;
    [SerializeField] private Material domainMaterial;
    [SerializeField] private TextMesh countLabel;
    [SerializeField] private NavMeshModifierVolume navModifier;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private Tile tile;
    private MaterialPropertyBlock block;

    private void Awake()
    {
        if (target == null) target = GetComponentInChildren<Renderer>();
        if (countLabel == null) countLabel = GetComponentInChildren<TextMesh>();

        block = new MaterialPropertyBlock();
    }

    public void Bind(Tile boundTile) => tile = boundTile;

    public void ShowDomain(int remaining, int total, Color fullColor, Color almostColor)
    {
        if (target == null) return;
        if (domainMaterial != null) target.sharedMaterial = domainMaterial;

        float ration = total <= 0 ? 0f : (float)remaining / total;
        Tint(Color.Lerp(almostColor, fullColor, ration));

        if (countLabel != null) countLabel.text = remaining.ToString();
    }

    public void ShowCollapsed(Material material)
    {
        if (target == null) return;

        ClearTint();
        if (material != null) target.sharedMaterial = material;
        if (countLabel != null) countLabel.text = string.Empty;
        if (tile.currentType.ToString().Contains("Water"))
        {
            navModifier = gameObject.GetOrAddComponent<NavMeshModifierVolume>();
            navModifier.area = 1; // Non-Walkable option.
            string[] tokens = tile.currentType.ToString().Split('_');
            
            //Fix Corners
            if (tokens.Length >= 4 && tokens[1] == "C")
            {
                Vector2 direction = tokens[2] switch
                {
                    "NW" => new Vector2(-1f, 1f),
                    "NE" => new Vector2(1f, 1f),
                    "SE" => new Vector2(1f, -1f),
                    "SW" => new Vector2(-1f, -1f),
                    _ => Vector2.zero,
                };

                //Corner is 1 Land 3 Water
                if (tokens[3] == "AB")
                {
                    //Don't use normal modifier since you have to block 3 areas instead of 1
                    navModifier.enabled = false;

                    //Create Array to find the corners based on calculated direction
                    Vector2[] waterCorners =
                    {
                        -direction, new Vector2(direction.x, -direction.y), new Vector2(-direction.x, direction.y)
                    };

                    //Loop and create empty box component on all 3 water corners
                    foreach (Vector2 dir in waterCorners)
                    {
                        var mod = gameObject.AddComponent<NavMeshModifierVolume>();
                        mod.area = 1;
                        mod.center = new Vector3(2.5f * dir.x, 0.5f, 2.5f * dir.y);
                        mod.size = new Vector3(5f, 2f, 5f);
                    }
                }

                //Corner is 3 Land 1 Water
                else if (tokens[3] == "BA")
                {
                    navModifier.enabled = true;
                    navModifier.center = new Vector3(2.5f * direction.x, 0.5f, 2.5f * direction.y);
                    navModifier.size = new Vector3(5f, 2f, 5f); 
                } 
            }  

            //Fix Sides
            else if (tokens.Length >= 3 && (tokens[1] == "H" || tokens[1] == "V"))
            {
                string side = tokens[2];

                //Separate the sides by Horizontal and Vertical to correctly calculate different types
                //Horizontal
                if (tokens[1] == "H")
                {
                    float z = side == "AB" ? -2.5f : 2.5f;

                    navModifier.center = new Vector3(0f, 0.5f, z);
                    navModifier.size = new Vector3(10f, 2f, 5f);
                }

                //Vertical
                else
                {
                    float x = side == "AB" ? 2.5f : -2.5f;

                    navModifier.center = new Vector3(x, 0.5f, 0f);
                    navModifier.size = new Vector3(5f, 2f, 10f);
                }

            }

            else
            {
                navModifier.center = new Vector3(0f,0.5f,0f);
                navModifier.size = new Vector3(10f, 2f, 10f);
            }
        }
    }

    public void ShowContradiction(Color color)
    {
        if (target == null) return;

        if (domainMaterial != null) target.sharedMaterial = domainMaterial;
        Tint(color);

        if (countLabel != null) countLabel.text = "X";
    }

    private void Tint(Color color)
    {
        if (target == null) return;

        target.GetPropertyBlock(block);
        block.SetColor(BaseColorId, color); // URP Lit / Unlit
        block.SetColor(ColorId, color);     // Built-in
        target.SetPropertyBlock(block);
    }

    private void ClearTint()
    {
        if (target == null) return;

        block.Clear();
        target.SetPropertyBlock(block);
    }
}