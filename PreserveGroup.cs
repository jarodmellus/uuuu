using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreserveGroup : MonoBehaviour
{
    [SerializeField]
    PreserveLocation[] group;

    void Awake() {
        group = GetComponentsInChildren<PreserveLocation>();
    }

    public void ResetToInitial() {
        foreach(PreserveLocation p in group) {
            p.ResetToInitial();
        }
    }

    void ResetToInitialRPC() {
        
    }

    private void OnEnable() {
        ResetToInitial();
    }
}
