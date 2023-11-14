using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] public SliceableObject sliceable;
    [SerializeField] public GameObject sliced;
    [SerializeField] public GameObject destroyable;

    
    public void OnCollisionEnter2D(Collision2D other) {
        //Debug.Log(other.gameObject);
        if(sliceable.getSlicedObject() == null) {
            sliceable.setSlicedObject(sliced);
            sliceable.Slice(Vector2.zero);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("Nezd meg a Hitbox osztalyt");
        if (collision.gameObject.CompareTag("Sword")) {
            //Animator animator = collision.gameObject.GetComponentInParent<Animator>();
            //Animation animation = collision.gameObject.GetComponentInParent<Animation>();
            //Debug.Log(animator.GetAnimatorTransitionInfo(0).IsName("Attack1_Animation"));
            //Debug.Log(animation.IsPlaying("Attack1_Animation"));
        }
    }
}
