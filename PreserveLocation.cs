
using UnityEngine;
public class PreserveLocation : MonoBehaviour
{
    private TransformWrapper _initialLocation;

    private void Awake()
    {
        _initialLocation = new TransformWrapper(transform);
    }
    
    public void ResetToInitial()
    {
        var myTransform = transform;
        myTransform.localPosition = _initialLocation.Position;
        myTransform.localRotation = _initialLocation.Rotation;
    }

    private void OnEnable()
    {
        ResetToInitial();
    }
}