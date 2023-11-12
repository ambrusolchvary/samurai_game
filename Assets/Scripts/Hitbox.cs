using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] public SliceableObject sliceable;
    [SerializeField] public GameObject sliced;

    
    public void OnCollisionEnter2D(Collision2D other) {
        if(sliceable.getSlicedObject() == null) {
            sliceable.setSlicedObject(sliced);
            sliceable.Slice(Vector2.zero);
        }
    }
    
}
