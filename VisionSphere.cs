using UnityEngine;

public class VisionSphere : MonoBehaviour
{
    AnimalVision animalVision;
    [SerializeField]
    float visionRadius=2f;
    void FixedUpdate()
    {
        Physics.OverlapSphere(transform.position,visionRadius);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,0,0.25f);
        Gizmos.DrawSphere(transform.position,visionRadius);
    }

    void Awake()
    {
        animalVision = GetComponentInParent<AnimalVision>();
        //animalVision.AddRay(this);
    }
}