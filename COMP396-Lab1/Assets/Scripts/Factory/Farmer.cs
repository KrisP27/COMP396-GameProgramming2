using Core.FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Farmer: MonoBehaviour, IEntity, IStateMachine, ITask
{
        private StateMachine stateMachine;
        [SerializeField] private VoidEventChannel harvestEvent;
        [SerializeField] private bool isHarvestReady;

        private void Awake()
        {
            stateMachine = new StateMachine();
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            HarvestState harvest = new HarvestState(renderer, agent);
            RestState rest = new RestState(renderer, agent);
            StoreState store = new StoreState(renderer, agent);
            MarketState market = new MarketState(renderer, agent);

            stateMachine.AddTransition(rest, harvest, new FuncPredicate(() => isHarvestReady));
            stateMachine.AddTransition(rest, store, new FuncPredicate(() => Keyboard.current.sKey.wasPressedThisFrame));
            stateMachine.AddTransition(rest, market, new FuncPredicate(() => Keyboard.current.mKey.wasPressedThisFrame));

            stateMachine.AddTransition(harvest, rest, new FuncPredicate(() => !isHarvestReady));
            stateMachine.AddTransition(store, rest, new FuncPredicate(() => Keyboard.current.rKey.wasPressedThisFrame));
            stateMachine.AddTransition(market, rest, new FuncPredicate(() => Keyboard.current.rKey.wasPressedThisFrame));


            stateMachine.AddTransition(store, harvest, new FuncPredicate(() => isHarvestReady));

            stateMachine.SetState(rest);

            harvestEvent.OnEventRaised += TransitionToHarvest;
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void OnDisable()
        {
            harvestEvent.OnEventRaised -= TransitionToHarvest;
        }

        private bool CheckHarvestPoint(Vector3 harvestLocation)
        {
            return isHarvestReady;
        }
        private void TransitionToHarvest()
        {
            isHarvestReady = !isHarvestReady;
        }
}
