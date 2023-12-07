using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleChildren : MonoBehaviour
{
    SetScale[] children;
    void Awake()
    {
        children = GetComponentsInChildren<SetScale>();
    }

    public void SetScale(float val) {
        foreach(SetScale child in children) {
            child.Set(val);
        }
    }
}
