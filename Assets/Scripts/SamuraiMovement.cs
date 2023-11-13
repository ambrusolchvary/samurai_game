using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamuraiMovement : MonoBehaviour
{
    

    private Camera _camera;
    private Rigidbody2D _rigidbody;
    private Animator animator;
    private BoxCollider2D swordCollider;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 12f;
    public float maxJumpHeight = 7f;
    public float maxJumpTime = 1f;

    private Vector2 characterDir = new Vector2(1, 0); 

    // Azert osztjuk kettovel, mert az ugras idejenek feleben felfele, a masik feleben pedig lefele mozogjon a karakter a parabolan
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _camera = Camera.main;
        swordCollider = transform.Find("Sword").GetComponent<BoxCollider2D>();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        HorizontalMovement();

        grounded = _rigidbody.Raycast(Vector2.down);

        if (grounded) {
            GroundedMovement(); // EZT **************************************************************************
        }

        ApplyGravity();

        if (jumping)
            VerticalMovement();
        if (!grounded && velocity.y < -4f) { // MEG EZT, OSSZELEHETNE VONNI *************************************
            animator.SetBool("Falling", true);
        } else {
            animator.SetBool("Falling", false);
        }

        if (Input.GetKeyDown("q")) {
            animator.SetInteger("AttackType", 0);
            animator.SetTrigger("AttackTriggered");

            Vector3 centerOfSwordCollider = swordCollider.transform.position + new Vector3(characterDir.x * swordCollider.offset.x, swordCollider.offset.y, 0);
            Debug.Log(centerOfSwordCollider);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(centerOfSwordCollider, swordCollider.size, 0f);
            Vector2 swordVelo = new Vector2(characterDir.x * 3, 2);
            if (colliders.Length > 0)
                TriggerSlicing(colliders, swordVelo);
        }
        else if (Input.GetKeyDown("e")) {
            animator.SetInteger("AttackType", 1);
            animator.SetTrigger("AttackTriggered");

            Vector3 centerOfSwordCollider = swordCollider.transform.position + new Vector3(characterDir.x * swordCollider.offset.x, swordCollider.offset.y, 0);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(centerOfSwordCollider, swordCollider.size, 0f);
            Vector2 swordVelo = new Vector2(characterDir.x * 3, 0);
            if (colliders.Length > 0)
                TriggerSlicing(colliders, swordVelo);
        }

    }

    private void TriggerSlicing(Collider2D[] colliders, Vector2 swordVelo) {
        Collider2D hitColliderFirst = null;
        foreach (Collider2D collider in colliders) {
            Debug.Log(collider.gameObject);
            if (hitColliderFirst == null) {
                if (collider.CompareTag("HitCollider")) {
                    hitColliderFirst = collider;
                    Hitbox hitbox = hitColliderFirst.gameObject.GetComponent<Hitbox>();
                    SliceableObject sliceable = hitbox.sliceable;
                    if (sliceable != null) {
                        sliceable.setSlicedObject(hitbox.sliced);
                        sliceable.Slice(swordVelo);
                    }
                }
                if (collider.CompareTag("HitAbleCollider")) {
                    HitAbleObject hitAble = collider.gameObject.GetComponent<HitAbleObject>();
                    hitAble.Hit(10 * swordVelo);
                }
            } else {
                break;
            }
        }
    }

    /*
    private void TriggerSlicing(Collider2D[] colliders) {
        Collider2D unslicedCollider = null;
        Collider2D hitColliderFirst = null;
        foreach (Collider2D collider in colliders) {
            if(unslicedCollider == null || hitColliderFirst == null) {
                if (collider.CompareTag("Sliceable")) {
                    unslicedCollider = collider;
                }
                if (collider.CompareTag("HitCollider")) {
                    Debug.Log(collider.gameObject);
                    hitColliderFirst = collider;
                }
            } else {
                break;
            }
        }
        SliceableWatermelon watermelon = unslicedCollider.gameObject.GetComponent<SliceableWatermelon>();
        //GameObject watermelonGameObject = unslicedCollider.gameObject;
        //watermelonGameObject.Find("");
        if (watermelon != null) {
            Debug.Log(hitColliderFirst.gameObject.GetComponent<Hitbox>().sliced);
            watermelon.setSlicedObject(hitColliderFirst.gameObject.GetComponent<Hitbox>().sliced);
            watermelon.Slice();
            Debug.Log("Sliced watermelon successfully.");
        } else {
            Debug.LogWarning("SliceableWatermelon component not found on the watermelon GameObject.");
        }
    }
    */


    private void HorizontalMovement() {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        if (inputAxis == 0f)
            velocity.x /= 4f;

        if (_rigidbody.Raycast(Vector2.right * velocity.x))
            velocity.x = 0f;

        if (velocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
            animator.SetInteger("PlayerAnimState", 1);
            characterDir = new Vector2(1, 0);
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            animator.SetInteger("PlayerAnimState", 1);
            characterDir = new Vector2(-1, 0);
        } else
            animator.SetInteger("PlayerAnimState", 0);
    }

    private void VerticalMovement() { // Az ugras animaciohoz kell, hogy tudjuk mikor valt at a jumping animaciorol a falling animaciora ugraskozben
        animator.SetFloat("JumpVelocity", velocity.y/jumpForce); // de vegulis lehet torolheted is ezt a metodust meg a jump and fall blend tree-t es helyette csak hasznald kulon a jump es fall animaciokat kulon kulon a boolean meg a trigger ertekek alapjan
    }

    private void GroundedMovement() {
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump")) {
            animator.SetTrigger("JumpTriggered");
            velocity.y = jumpForce;
            jumping = true;
        }
    }

    private void ApplyGravity() {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void FixedUpdate() {
        Vector2 position = _rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = _camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 1f, rightEdge.x - 1f);

        // if (position.x == leftEdge.x + 1f) velocity.x = 0f;  //////////////////////////////////////// de lehet inkabb a horizontalMovement-be kene implementalni

        _rigidbody.MovePosition(position);
    }

    // Ehelyett es az Extensions.DotTest() helyett lehet eleg csak if(_rigidBody.Raycast(Vector2.up)) velocity.y = 0f; Mert nalam nincs specialbox ami kiveteles ha megfejeli
    private void OnCollisionEnter2D(Collision2D collision) {
        if(transform.DotTest(collision.transform, Vector2.up))
            velocity.y = 0f;
    }
}
