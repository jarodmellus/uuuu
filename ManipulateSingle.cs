using UnityEngine;
using UnityEngine.Events;

public class ManipulateSingle : MonoBehaviour {
    public UnityEvent<float> outputEvent;
    public float offset=0f;
    public float multiplier=1f;

    public void DoManipulation(float val) {
        outputEvent?.Invoke((val + offset) * multiplier);
    }
}