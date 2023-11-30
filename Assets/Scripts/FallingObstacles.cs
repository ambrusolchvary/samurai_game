using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacles : MonoBehaviour
{
    public Transform player; //This creates a slot in the inspector where you can add your player
    public float maxDistance = 20f; //This can be changed in the inspector to your liking
    public int minGravityScale = 5;
    public int maxGravityScale = 10;
    Rigidbody2D _rigidbody;
    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0;
    }

    void Update() {
        //Debug.Log(Vector3.Distance(player.position, transform.position));
        if (Vector3.Distance(player.position, transform.position) < maxDistance) { //transform is the object that this script is attached to
            _rigidbody.gravityScale = Random.Range(minGravityScale, maxGravityScale);
        }
    }
}
