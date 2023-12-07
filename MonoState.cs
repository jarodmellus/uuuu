using System.ComponentModel.Design;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

[ExecuteAlways]
public class MonoState : MonoBehaviour
{
    [HideInInspector]
    public string stateName;
    [HideInInspector]
    public MonoStateEvents events;
    [HideInInspector]
    public List<MonoStateTransition> transitions = new List<MonoStateTransition>();
    public Dictionary<MonoState,MonoStateTransition> transitionMap = new Dictionary<MonoState, MonoStateTransition>();
    public bool canTransitionToAnyState = false;

    MonoStateController _controller;
    public void SetController(MonoStateController controller) => _controller = controller;

    [Header("Gizmos")]
    [SerializeField]
    float gizmoRadius=.1f;
    [SerializeField]
    Color gizmosColor=new Color(1f,0f,0f,.5f);
    GUIStyle fontStyle;// = new GUIStyle(GUI.skin.label);


    void OnEnable()
    {
        //fontStyle.fixedHeight = 2f;
        //fontStyle.fixedWidth = 4f;
        //fontStyle.fontSize = 24;
        //fontStyle.alignment = TextAnchor.MiddleCenter;
        transitions = GetComponentsInChildren<MonoStateTransition>().ToList();
    }

    void Awake()
    {
        events = GetComponent<MonoStateEvents>();
        transitions = GetComponentsInChildren<MonoStateTransition>().ToList();
        foreach(MonoStateTransition transition in transitions) {
            transitionMap.Add(transition.toState,transition);
        }
        
    }

/*
    void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position,gizmoRadius*transform.lossyScale.magnitude);
        //Handles.Sphere(0,transform.position,gizmoRadius*transform.localScale.magnitude);
        Gizmos.color = Color.black;
        //fontStyle.fontSize = 16*transform.lossyScale.magnitude;
        Handles.Label(transform.position, gameObject.name,fontStyle);
    }
*/

    public MonoStateTransition GetTransition(MonoState toState) {
        if (transitionMap.ContainsKey(toState))
            return transitionMap[toState];
        else
            return null;
    }
#if UNITY_EDITOR
    [Button]
    public void CreateNewTransition() {
        if (Application.isPlaying) return;
        GameObject newTransition = new GameObject("New Transition", typeof(MonoStateTransition));
        newTransition.GetComponent<MonoStateTransition>().fromState = this;
        newTransition.transform.parent = transform;
        Selection.activeGameObject = newTransition;
    }

    [Button]
    public void AddEvents() {
        if (Application.isPlaying) return;
        if(gameObject.GetComponent<MonoStateEvents>()==null) {
            events=gameObject.AddComponent<MonoStateEvents>();
        }
    }
#endif

#if UNITY_EDITOR
    [Button]
#endif
    public void TransitionToThisState() {
        _controller.ChangeState(this);
    }

}