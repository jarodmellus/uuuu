using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(MonoCondition))]
public class MonoStateTransition : MonoBehaviour
{
    public MonoState fromState;
    public MonoState toState;
    public UnityEvent onTransition;
    MonoCondition condition;

    [ExecuteAlways]
    void Awake()
    {
        fromState = GetComponentInParent<MonoState>();
        condition = GetComponent<MonoCondition>();
        condition.onCondition.AddListener(PerformTransition);
        
    }

    [ExecuteInEditMode]
    void OnValidate()
    {
        if (toState == null) return;
        if(fromState == null) return;
        gameObject.name = $"[{fromState.name}] ---> [{toState.name}]";
    }

    void OnDrawGizmos()
    {
        if (toState == null) return;
        if(fromState==null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(fromState.transform.position,toState.transform.position);
    }

    public void PerformTransition() {
        //if(!enabled) return;
        toState.TransitionToThisState();
    }
}