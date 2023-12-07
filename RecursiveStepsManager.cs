using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RecursiveStepsManager : MonoBehaviour
{
    StepsManager mainManager;
    List<StepsManager> managers = new List<StepsManager>();
    StepsManager activeManager;

    private void Awake() {
        mainManager=GetComponent<StepsManager>();
    }

    void Start() {
        managers =mainManager.transform.GetComponentsInChildren<StepsManager>(true).ToList();
        managers.Remove(mainManager);
        foreach(StepsManager manager in managers) {
            manager.onComplete.AddListener(()=>mainManager.Next());
            manager.onStart.AddListener(()=>activeManager=manager);
            //manager.onQuit.AddListener(()=>mainManager.Previous());
        }        
        activeManager=transform.GetChild(0).GetComponent<StepsManager>();
    }

    [Button]
    public void Next() {
        activeManager.Next();
    }

    [Button]
    public void Previous() {
        activeManager.Previous();
    }
}
