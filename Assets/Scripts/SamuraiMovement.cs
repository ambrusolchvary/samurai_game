using UnityEngine;

public class SamuraiMovement : MonoBehaviour
{
    private Camera _camera;
    private Rigidbody2D _rigidbody;
    private Animator animator;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 12f;
    public float maxJumpHeight = 7f;
    public float maxJumpTime = 1f;

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
            Debug.Log(gravity);
            animator.SetBool("Falling", true);
        } else {
            animator.SetBool("Falling", false);
        }

        if (Input.GetKeyDown("q")) {
            animator.SetInteger("AttackType", 0);
            animator.SetTrigger("AttackTriggered");
        }
        else if (Input.GetKeyDown("e")) {
            animator.SetInteger("AttackType", 1);
            animator.SetTrigger("AttackTriggered");
        }

    }

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
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            animator.SetInteger("PlayerAnimState", 1);
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
