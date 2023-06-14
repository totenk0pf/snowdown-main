using System;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zombie;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState currentState;
    private ZombieBehaviour _behaviour;
    private ZombieBehaviour Behaviour {
        get {
            if (!_behaviour) _behaviour = GetComponent<ZombieBehaviour>();
            return _behaviour;
        }
    }

    private void Awake() {
        if (currentState == null) {
            NCLogger.Log($"Initial state not set.", LogLevel.ERROR);
        }
    }

    private void Update() {
        RunStateMachine();
    }

    private void RunStateMachine() {
        EnemyState nextState = currentState.RunCurrentState();
        if (nextState != null) {
            SwitchState(nextState);
        }
    }

    private void SwitchState(EnemyState state) {
        currentState = state;
    }

    private void OnTriggerEnter(Collider other) {
        currentState.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        currentState.OnTriggerExit(other);
    }   
}
