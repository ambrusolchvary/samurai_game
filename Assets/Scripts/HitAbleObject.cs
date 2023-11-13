using UnityEngine;

public class HitAbleObject : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void Hit(Vector2 velocity) {
        _rigidbody.velocity = _rigidbody.velocity + velocity;
    }
}
