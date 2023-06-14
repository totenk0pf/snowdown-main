using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Zombie;

public abstract class EnemyState : MonoBehaviour
{
    private NavMeshAgent _agent;
    protected NavMeshAgent Agent {
        get {
            if (!_agent) _agent = transform.root.GetComponent<NavMeshAgent>();
            return _agent;
        }
    }

    private ZombieBehaviour _behaviour;
    protected ZombieBehaviour Behaviour {
        get {
            if (!_behaviour) _behaviour = transform.root.GetComponent<ZombieBehaviour>();
            return _behaviour;
        }
    }

    private Animator _anim;
    protected Animator Anim {
        get{
            if (!_anim) _anim = transform.root.GetComponentInChildren<Animator>();
            return _anim;
        }
    }

    public LayerMask playerMask;
    public EnemyState previousState;
    public EnemyState nextState;
    
    [ReadOnly] public PlayerDataHandler target;

    public abstract EnemyState RunCurrentState();

    public virtual void OnTriggerEnter(Collider other) {
    }

    public virtual void OnTriggerExit(Collider other) {
    }

    protected static float GetPathRemainingDistance(NavMeshAgent navMeshAgent) {
        if (navMeshAgent.pathPending ||
            navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            navMeshAgent.path.corners.Length == 0)
            return -1f;
    
        var distance = 0.0f;
        for (var i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }
    
        return distance;
    }
    
    protected static float GetPathRemainingDistance(NavMeshPath path) {
        var distance = 0.0f;
        for (var i = 0; i < path.corners.Length - 1; ++i) {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return distance;
    }
}