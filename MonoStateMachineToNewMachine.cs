using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MonoStateController))]
public class MonoStateMachineTransitionToNewMachine : MonoBehaviour
{
    public MonoStateController fromMachine,toMachine;
    // Start is called before the first frame update
    void Start()
    {
        fromMachine = GetComponent<MonoStateController>();
        fromMachine.onExitState.AddListener(ToNextMachine);
    }

    void ToNextMachine() {
        fromMachine.gameObject.SetActive(false);
        toMachine.gameObject.SetActive(true);
    }


}
