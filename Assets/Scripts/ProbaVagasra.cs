using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbaVagasra : MonoBehaviour
{
    [SerializeField] private GameObject[] slicedObjects;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void Slice(Vector2 velocity) {
        if (slicedObjects.Length > 0) {
            this.gameObject.SetActive(false);
            foreach (GameObject slicedObj in slicedObjects) {
                Transform containerOfSliced = slicedObj.GetComponentInParent<Transform>().transform; // Ahhoz kell, hogy a sliced darabokat az unsliced darab helyere jelenitsuk meg
                containerOfSliced.position = this.gameObject.transform.position;
                containerOfSliced.rotation = this.gameObject.transform.rotation;
                slicedObj.SetActive(true);
                slicedObj.GetComponent<Rigidbody2D>().velocity += 2*velocity;
            }
        } else {
            _rigidbody.velocity += 5 * velocity;
        }
    }
}
