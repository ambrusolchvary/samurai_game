using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField] private GameObject[] slicedObjects;
    [SerializeField] private GameObject toBeDestroyed;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void Slice(Vector2 velocity) {
        if (slicedObjects.Length > 0) {
            this.gameObject.SetActive(false);
            bool isSclicedDestroyer = false;
            foreach (GameObject slicedObj in slicedObjects) {
                isSclicedDestroyer = slicedObj.CompareTag("SliceableDestroyer");
                Transform containerOfSliced = slicedObj.GetComponentInParent<Transform>().transform; // Ahhoz kell, hogy a sliced darabokat az unsliced darab helyere jelenitsuk meg
                containerOfSliced.position = this.gameObject.transform.position;
                containerOfSliced.rotation = this.gameObject.transform.rotation;
                slicedObj.SetActive(true);
                if (isSclicedDestroyer) {
                    slicedObj.GetComponent<Animator>().SetTrigger("FlashingTriggered");
                }
                Vector2 bummDir = slicedObj.GetComponent<Transform>().position - transform.position;
                slicedObj.GetComponent<Rigidbody2D>().velocity += 2 * velocity + 10 * bummDir;
                //slicedObj.GetComponent<Transform>().rotation = new Quaternion(1, 1, Random.Range(-10.0f, 10.0f), 1);
            }
            if (isSclicedDestroyer) {
                Destroy(toBeDestroyed, 2.0f);
            }
        } else {
            _rigidbody.velocity += 5 * velocity;
        }
    }
}
