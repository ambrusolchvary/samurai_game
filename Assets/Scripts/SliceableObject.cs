using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    [SerializeField] private GameObject unslicedObject;
    private GameObject slicedObject;

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

    public void Slice(Vector2 velocity) {
        unslicedObject.SetActive(false);
        slicedObject.SetActive(true);

        for (int i = 0; i < slicedObject.transform.childCount; i++) {
            Rigidbody2D sliceRigidbody = slicedObject.transform.GetChild(i).GetComponent<Rigidbody2D>();
            sliceRigidbody.velocity = _rigidbody.velocity + velocity;
        }

        _collider.enabled = false;
    }
}