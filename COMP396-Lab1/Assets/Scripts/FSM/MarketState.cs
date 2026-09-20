using UnityEngine;
using UnityEngine.AI;

namespace Core.FSM
{
    public class MarketState : BaseState
    {
        private GameObject marketStand;
        public MarketState(MeshRenderer renderer, NavMeshAgent agent) : base(renderer, agent)
        {
            
        }

        //State makes NPC go to market stand location and begin selling goods from the storage
        //NPCs using the Shopping State will go to stands that have sellers at them and purchase goods
        public override void Enter()
        {
            meshRenderer.material.color = Color.red;
            marketStand = GameObject.FindWithTag("MarketStand");
            agent.SetDestination(marketStand.transform.position);
            agent.isStopped = false;
        }

        //Updates inventory by lowering sellable goods and increasing coins earned based on goods price
        public override void Update()
        {
            
        }

        //Once Exited, NPC returns to Rest State
        //State should automatically exit when no more goods can be sold
        public override void Exit()
        {
            base.Exit();
            marketStand = null;
        }
    }
}
