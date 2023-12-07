using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScale : MonoBehaviour
{
    [SerializeField]
    float scaleMultiplier = 1f;
    public void Set(float val) {
        transform.localScale = Vector3.one * (val*scaleMultiplier);
    }
}
