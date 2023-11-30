using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    [SerializeField] private GameObject unslicedObject;
    private GameObject slicedObject;
    private GameObject destroyableObject;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void setSlicedObject(GameObject slicedObject) {
        this.slicedObject = slicedObject;
    }

    public GameObject getSlicedObject() {
        return slicedObject;
    }

    public void setDestroyableObject(GameObject destroyableObject) {
        this.destroyableObject = destroyableObject;
    }

    public GameObject getDestroyableObject() {
        return destroyableObject;
    }

    public void Slice(Vector2 velocity) {
        //Debug.Log(unslicedObject);
        //Debug.Log(_collider);
        unslicedObject.SetActive(false);
        slicedObject.SetActive(true);

        for (int i = 0; i < slicedObject.transform.childCount; i++) {
            Rigidbody2D sliceRigidbody = slicedObject.transform.GetChild(i).GetComponent<Rigidbody2D>();
            sliceRigidbody.velocity = _rigidbody.velocity + velocity;
        }

        _collider.enabled = false;
        //Debug.Log(destroyableObject);
        if(destroyableObject != null)
            Destroy(destroyableObject);
    }
}
