using UnityEngine;
using UnityEngine.AI;

namespace Core.FSM
{
    public class StoreState : BaseState
    {
        private GameObject storageUnit;
        public StoreState(MeshRenderer renderer, NavMeshAgent agent) : base(renderer, agent)
        {
            
        }

        //State activates after harvesting inventory reaches maximum and moves to bring NPC to storage unit to dump items
        public override void Enter()
        {
            meshRenderer.material.color = Color.orange;
            storageUnit = GameObject.FindWithTag("StorageUnit");
            agent.SetDestination(storageUnit.transform.position);
            agent.isStopped = false;
        }

        //State should be checking inventory space to check if inventory reaches 0 and exit once inventory is empty
        //If Storage unit is full, State transitions to next tagged StorageUnit, if there is none available, just exit
        public override void Update()
        {
            base.Update();
        }

        //Once Exited, NPC returns to harvesting state unless there is nothing left to be harvested
        public override void Exit()
        {
            base.Exit();
            storageUnit = null;
        }
    }
}
