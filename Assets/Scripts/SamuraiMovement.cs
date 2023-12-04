using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Samurai karakter mozgasat megvalosito osztaly
public class SamuraiMovement : MonoBehaviour
{
    private Camera _camera; // main camera, amely koveti a samurai-t
    private Rigidbody2D _rigidbody; // a samurai rigidbody-ja a fizikai hatasokhoz
    private Animator animator; // animator a samurai-hoz tartozo kulonbozo animaciok es azok lejatszanak vezerlesehez

    private Vector2 velocity; // karakter sebessege
    private float inputAxis; // horizontalis mozgatashoz a felhasznaloi input kezelese

    public float moveSpeed = 15f; // a karakter mozgasanak gyorsasagat lehet megadni
    public float maxJumpHeight = 7f; // az ugras parabolajanak maximalis, talajtol szamitott magassaga
    public float maxJumpTime = 1f; // az ugras hossza masodpercben

    // itt taroljuk el az utolso megsebesulesunk (a frame kezdetetol szamitott) idejet, ami ahhoz kell, hogy ne tudjunk egy utkozes soran indokolatlanul tobbszor is sebzodni
    private float timeOfLastHitTaken = 0f; 

    private Vector2 characterDir = new Vector2(1, 0); // a karakter eloretekinto iranya, kezdetben jobbra nez

    // publikus getter a karakter eloretekinto iranyahoz, hogy masik osztalybol is elerheto legyen
    public Vector2 GetCharacterDir() { 
        return characterDir;
    }

    // Azert osztjuk kettovel, mert az ugras idejenek feleben felfele, a masik feleben pedig lefele mozogjon a karakter a parabolan
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    

    public HeartsManager heartsManager;
    public UIManager uiManager;

    private GameObject proceduralGenerator;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _camera = Camera.main;
        proceduralGenerator = GameObject.FindWithTag("ProceduralGeneration");
    }

    private void Start() {
        EnablePlayerMovement();
    }

    private void Update() {
        HorizontalMovement(); // horizontalis mozgas mindig hasznalhato

        // ha atleptunk az utolso platform kezdetet meghatarozo x koordinata +10-edik hataran, azaz beertunk a celba, akkor a jateknak vege es nyert a jatekos
        if (transform.position.x > proceduralGenerator.GetComponent<ProceduralGeneration>().GetEndPstartIdx() + 10) {
            PlayerDead();
            StartCoroutine(uiManager.WinningSequence());
        }
        // Az External osztalyban megirt raycast metodussal folyton vizsgaljuk, hogy talajon allunk-e (vagy sliceable objektumon) es aszerint allitjuk a grounded erteket
        grounded = _rigidbody.Raycast(Vector2.down);

        animator.SetBool("IsGrounded", grounded);

        if (grounded) GroundedMovement();

        VerticalMovement();
        

        ApplyGravity();

    }

    private void HorizontalMovement() {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        if (inputAxis == 0f)
            velocity.x /= 4f;
        

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
        if (velocity.y <= 0) {
            animator.SetTrigger("FallingTriggered"); // de vegulis lehet torolheted is ezt a metodust meg a jump and fall blend tree-t es helyette csak hasznald kulon a jump es fall animaciokat kulon kulon a boolean meg a trigger ertekek alapjan
        }
    }

    private void GroundedMovement() {
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump")) {
            jumping = true;
            grounded = false;
            animator.SetTrigger("JumpTriggered");
            animator.SetBool("IsGrounded", grounded);
            velocity.y = jumpForce;
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

        if (_rigidbody.bodyType == RigidbodyType2D.Dynamic) _rigidbody.MovePosition(position);
    }
    

    // a karakter testevel utkozo objektumok kezelese
    private void OnCollisionEnter2D(Collision2D collision) {
        if(transform.DotTest(collision.transform, Vector2.up)) // ha ugras kozben beleutjuk a fejunket valamibe, akkor a sebesseget az y tengelyen nullazni kell, hogy hagyjuk a
            velocity.y = 0f; // gravitaciot eletbe lepni, kulonben olyan hatas lenne mintha hozzatapadnank egy rovid idore a plafonhoz, mert a sebessegunk meg oda tartana minket
        if (collision.collider.CompareTag("WoundingSliceable") || collision.collider.CompareTag("WoundingObject") || collision.collider.CompareTag("Reward")) {
            animator.SetTrigger("TakeHitTriggered");
            float timeOfCurrentHit = Time.time; // az utolso frame kezdete ota ebben a pillanatban eltelt idot eltaroljuk egy valtozoban, hogy megnezzuk nem telt e el tul keves ido, hogy megserulhessunk ismet az elozo serules ota
            if (timeOfLastHitTaken == 0f || timeOfCurrentHit - timeOfLastHitTaken > 0.5) {
                heartsManager.TakeDamage(1); // Sebzest okoz a szamurajnak
                Vector2 hurtedVelo = new Vector2(-characterDir.x * 10, 20); // konstans, ami megadja mekkora erovel hokkenjunk hatra mikor megserulunk
                velocity += hurtedVelo; // hozzaadjuk es nem pedig egyenlove tesszuk, mert igy ha legalabb pl szakadek felett ugrunk at es tuske van a tuloldalon van eselyunk aterkezni, mert ha egyenlove tennenk, akkor menetirannyal ellenkezo iranyba lokodnenk mindig
                timeOfLastHitTaken = timeOfCurrentHit; // vezetjuk, hogy mindig az epp utolso serulest taroljuk
            }
        }
        if (heartsManager.life == 0) { // ha elfogyott az eletunk meghaltunk
            PlayerDead();
        }
    }

    // A jatekos mozgasanak engedelyezese
    private void EnablePlayerMovement() {
        animator.enabled = true; // engedelyezi az animalast
        _rigidbody.bodyType = RigidbodyType2D.Dynamic; // engedelyezi a mozgast
    }

    // A jatekos halala
    public void PlayerDead() {
        ApplyGravity();
        animator.SetBool("PlayerDied", true); // lejatsza a meghalas animaciot
        _rigidbody.bodyType = RigidbodyType2D.Static; // ne tudjon mozogni a karakter amikor halottak vagyunk
    }
}
