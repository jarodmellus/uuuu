using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimalVision : MonoBehaviour
{
    [SerializeField]
    Animal animal;
    List<VisionRay> rays = new List<VisionRay>();
    public void AddRay(VisionRay ray) => rays.Add(ray);
    public UnityEvent onEnterVision,onExitVision;
    public void OnEnterVision(Rigidbody body) {
        //onEnterVision?.Invoke();
        animal.InterestingItemEnterVicinity(body);
    }

    public void OnExitVision(Rigidbody body) {
        //onExitVision?.Invoke();
        animal.InterestingItemExitVicinity(body);
    }
}

