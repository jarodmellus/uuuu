using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentScale : MonoBehaviour
{
    [SerializeField] float scaleMultipler = 1f;
    void LateUpdate()
    {
        transform.localScale = scaleMultipler * new Vector3(
            1f/transform.parent.localScale.x,
            1f/transform.parent.localScale.y,
            1f/transform.parent.localScale.z
        );
    }
}
