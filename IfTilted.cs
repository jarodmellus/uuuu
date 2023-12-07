using UnityEngine;
using UnityEngine.Events;

public class IfTilted : MonoBehaviour
{
    bool tilted = false;
    [SerializeField]
    float tiltAngle = .35f;

    public UnityEvent onTilted, onStoppedTilting;
    private void FixedUpdate()
    {
        if (tilted)
        {
            if (Vector3.Dot(transform.up, Vector3.down) <= tiltAngle)
            {
                tilted = false;
                onStoppedTilting?.Invoke();
            }
        }
        else
        {
            if (Vector3.Dot(transform.up, Vector3.down) > tiltAngle)
            {
                tilted = true;
                onTilted?.Invoke();
            }
        }
    }

    void OnDisable()
    {
        onStoppedTilting?.Invoke();
    }
}