using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPhysics : MonoBehaviour
{
    [SerializeField] private GameObject _cannonBallPrefab;
    [SerializeField] private ForceMode _forceMode;
    [SerializeField] private Vector3 _force;

    private Rigidbody _cannonBody;

    void Start()
    {
        _cannonBody = Instantiate(_cannonBallPrefab, transform).GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _cannonBody.AddForce(_force, _forceMode);
        }
    }
}
