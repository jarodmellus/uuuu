using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class MonoStateController : MonoBehaviour
{
    [HideInInspector]
    public string controllerName;

    [HideInInspector]
    List<MonoState> states = new List<MonoState>();

    [SerializeField]
    MonoState startState;
    [SerializeField]
    MonoState exitState;

    MonoState _currentState;

    public UnityEvent onAnyTransition, onStartState, onExitState;

    //Dictionary<string, MonoState> stateMap = new Dictionary<string, MonoState>();

    void Awake()
    {
        //foreach(MonoState state in states) {
            //stateMap.Add(state.stateName,state);
            //foreach(MonoStateTransition transition in state.transitions) {
                //state.transitionMap.Add(transition.nextState,transition);
            //}
        //}
        states = GetComponentsInChildren<MonoState>().ToList();

        if(startState==null) {
            startState = GetComponentInChildren<MonoState>();
        }

        foreach(MonoState state in states) {
            state.gameObject.SetActive(false);
            state.SetController(this);
        }
        _currentState = startState;
        startState.gameObject.SetActive(true);
    }

    void Start()
    {
        _currentState.events?.onEnter?.Invoke();
    }

    public void ChangeState(MonoState newState) {

        if(_currentState.GetTransition(newState)!=null) {
            _currentState.transitionMap[newState].onTransition?.Invoke();
        }

        if(_currentState.events!=null) 
            _currentState.events?.onExit?.Invoke();

        if (newState.events != null)
        {
            newState.events.onEnter?.Invoke();
            //_updateAction = newState.events.onUpdate;
        }
        _currentState.gameObject.SetActive(false);
        newState.gameObject.SetActive(true);
        _currentState =  newState;

        onAnyTransition?.Invoke();

        if(_currentState == exitState) {
            onExitState?.Invoke();
        }
    }
    

    UnityEvent _updateAction;
/*
    public void Update() {
        if (_updateAction == null) return;
        _updateAction?.Invoke();
    }
    */

    [ExecuteInEditMode]
    void OnValidate()
    {
        states = GetComponentsInChildren<MonoState>().ToList();
    }

    public void Restart() {
        _currentState.gameObject.SetActive(false);
        startState.gameObject.SetActive(true);
        _currentState = startState;
    }

#if UNITY_EDITOR
    [Button]
    public void CreateNewState() {
        if (Application.isPlaying) return;
        GameObject newState = new GameObject("New State", typeof(MonoState));
        newState.transform.parent = transform;
        Selection.activeGameObject = newState;
    }
/*
    [Button]
    public void NextState() {
        
    }

    [Button]
    public void PreviousState() {

    }
*/


#endif
}
