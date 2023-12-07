
using UnityEngine;
public class EncapsulationCheck {

    public static bool AreBoundEncapsulatedByOtherBounds( Bounds boundsA, Bounds boundsB)
    {
        // Check if colliderA bounds are entirely encapsulated by colliderB bounds
        return boundsB.Contains(boundsA.min) && boundsB.Contains(boundsA.max);
    }

    public static bool AreBoundEncapsulatedByOtherBounds( Collider[] colsA, Collider[] colsB)
    {
        return AreBoundEncapsulatedByOtherBounds(CombineColliderBounds(colsA),CombineColliderBounds(colsB));
    }

    public static float GetNormalizedEncapsulationVolume( Bounds boundsA, Bounds boundsB)
    {
        // Calculate the intersection bounds
        float minX = Mathf.Max(boundsA.min.x, boundsB.min.x);
        float minY = Mathf.Max(boundsA.min.y, boundsB.min.y);
        float minZ = Mathf.Max(boundsA.min.z, boundsB.min.z);
        float maxX = Mathf.Min(boundsA.max.x, boundsB.max.x);
        float maxY = Mathf.Min(boundsA.max.y, boundsB.max.y);
        float maxZ = Mathf.Min(boundsA.max.z, boundsB.max.z);

        // Calculate the intersection volume for both colliderA and colliderB
        float intersectionVolumeA = Mathf.Max(0f, maxX - minX) *
                                   Mathf.Max(0f, maxY - minY) *
                                   Mathf.Max(0f, maxZ - minZ);

        float volumeA = boundsA.size.x * boundsA.size.y * boundsA.size.z;

        // Normalize the intersection volume relative to colliderA volume
        float normalizedVolume = intersectionVolumeA / volumeA;

        return normalizedVolume;
    }

    public static float GetNormalizedEncapsulationVolume( Collider[] colsA, Collider[] colsB) 
    {
        return GetNormalizedEncapsulationVolume( CombineColliderBounds(colsA),CombineColliderBounds(colsB));
    }

    public static void CombineBounds(Bounds boundsA, Bounds boundsB) {
        boundsA.Encapsulate(boundsB);
    }

    public static Bounds CombineColliderBounds(Collider[] cols) {
        Bounds bounds = cols[0].bounds;
        
        foreach(Collider col in cols) {
            bounds.Encapsulate(col.bounds);
        }
        return bounds;
    }
}