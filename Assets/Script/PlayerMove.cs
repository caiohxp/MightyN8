using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movimentação")]
    public float Speed = 10f;
    public float pushSpeed = 4f;
    public float JumpForce = 600f;
    [HideInInspector] public bool isPushing = false;

    [Header("Detector de Chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float groundGraceTime = 0.06f;
    public float ignoreGroundAfterJump = 0.10f;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool doubleJump;
    [HideInInspector] public bool isGrounded;

    private Rigidbody2D rig;
    private Animator anim;
    private float lastGroundedTime = -999f;
    private float ignoreGroundUntil = -999f;

    // Conexões com os outros módulos do Otto
    private PlayerCombat combat;
    private PlayerHealth health;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Se a cutscene da plataforma final estiver rolando, trava o movimento
        if (health != null && health.onFinalPlatform) return;

        CheckGround();

        if (!Input.GetButton("Fire3"))
            Move();

        Jump();
    }

    void Move()
    {
        // Prioridade 1: Se estiver sofrendo Knockback, a física de dano assume
        if (health != null && health.isKnockedBack) return;

        // Prioridade 2: Se estiver atacando, freia o boneco
        if (combat != null && (combat.isShooting || combat.isPunching))
        {
            anim.SetBool("walk", false);
            return;
        }

        // Movimento Normal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float velocidadeAtual = isPushing ? pushSpeed : Speed;

        rig.velocity = new Vector2(horizontal * velocidadeAtual, rig.velocity.y);

        bool shouldWalk = Mathf.Abs(horizontal) > 0.01f && isGrounded && !isPushing;
        anim.SetBool("walk", shouldWalk);

        if (horizontal > 0f) transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else if (horizontal < 0f) transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (combat != null) combat.CancelAttacks();

            if (isGrounded)
            {
                ignoreGroundUntil = Time.time + ignoreGroundAfterJump;
                lastGroundedTime = -999f;
                isGrounded = false;
                isJumping = true;

                anim.SetBool("walk", false);
                anim.SetBool("jump", true);

                rig.velocity = new Vector2(rig.velocity.x, 0);
                rig.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
                anim.SetTrigger("TriggerJump");
            }
            else if (doubleJump)
            {
                doubleJump = false;
                rig.velocity = new Vector2(rig.velocity.x, 0);
                rig.AddForce(Vector2.up * JumpForce * 0.7f, ForceMode2D.Impulse);

                anim.SetBool("walk", false);
                anim.SetBool("jump", true);
                anim.SetTrigger("TriggerJump");
            }
        }
    }

    void CheckGround()
    {
        bool touchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Time.time < ignoreGroundUntil) isGrounded = false;
        else
        {
            if (touchingGround) lastGroundedTime = Time.time;
            isGrounded = Time.time - lastGroundedTime <= groundGraceTime;
        }

        isJumping = !isGrounded;
        anim.SetBool("jump", !isGrounded);

        if (isGrounded) doubleJump = true;
    }

    public void SetPushing(bool empurrando)
    {
        if (empurrando && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            isPushing = true;
            anim.SetBool("push", true);
        }
        else
        {
            isPushing = false;
            anim.SetBool("push", false);
        }
    }
}