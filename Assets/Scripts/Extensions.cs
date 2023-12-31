using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");
    private static LayerMask sliceableLayerMask = LayerMask.GetMask("Sliceable");

    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction) {
        if (rigidbody.isKinematic)
            return false;

        float radius = 0.2f;
        float distance = 1.2f;
        
        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        RaycastHit2D sliceableHit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, sliceableLayerMask);
        return (hit.collider != null || sliceableHit.collider != null) && hit.rigidbody != rigidbody;
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection) {
        Vector2 direction = other.position - transform.position;
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }
}
