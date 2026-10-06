using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Dano e Knockback")]
    public float KBForce = 5f;
    public float KBTotalTime = 0.2f;

    [HideInInspector] public bool isKnockedBack = false;
    [HideInInspector] public bool onFinalPlatform = false;

    private float KBCounter;
    private bool KnockFromRight;
    
    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerCombat combat;
    private PlayerMove movement;

    private float positionYOnFinalPlatform;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMove>();
    }

    void Update()
    {
        // Aplica o Knockback independentemente de outras ações
        if (KBCounter > 0)
        {
            isKnockedBack = true;
            if (KnockFromRight) rig.velocity = new Vector2(-KBForce, KBForce);
            else rig.velocity = new Vector2(KBForce, KBForce);

            KBCounter -= Time.deltaTime;
        }
        else
        {
            isKnockedBack = false;
        }

        // Checa morte e Game Over
        if (PlayerData.instance.health <= 0)
        {
            GameController.instance.ShowGameOver();
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float collisionX)
    {
        // Imunidade de impacto durante o soco
        if (combat != null && combat.isPunching) return;

        PlayerData.instance.health--;

        // Cancela tudo que o Otto estava fazendo
        if (combat != null) combat.CancelAttacks();
        if (movement != null) movement.SetPushing(false);

        anim.SetBool("walk", false);
        anim.SetBool("push", false);
        anim.ResetTrigger("TriggerPunch");
        anim.ResetTrigger("ShootPlus");

        // Calcula a direção para espirrar o Otto para trás
        KnockFromRight = transform.position.x <= collisionX;
        KBCounter = KBTotalTime;

        StartCoroutine(HitedCoRoutine());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Inimigos ou balas do inimigo
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 8)
        {
            TakeDamage(collision.transform.position.x);
        }

        if (collision.gameObject.layer == 8) collision.gameObject.SetActive(false);
        if (collision.gameObject.layer == 15) PlayerData.instance.health = 0; // Espinhos
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FloorBlock fb = collision.gameObject.GetComponent<FloorBlock>();
        if (fb != null) fb.Trampled();

        DangerBlock db = collision.gameObject.GetComponent<DangerBlock>();
        if (db != null && db.danger)
        {
            TakeDamage(collision.transform.position.x);
        }

        if (collision.gameObject.layer == 14) positionYOnFinalPlatform = transform.position.y;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14) // Plataforma de Nota Final
        {
            float currentSpeed = movement != null ? movement.Speed : 10f;
            collision.gameObject.transform.Translate(Vector2.down * currentSpeed * Time.deltaTime);
            transform.Translate(Vector2.down * currentSpeed * Time.deltaTime);
            onFinalPlatform = true;

            if (transform.position.y < positionYOnFinalPlatform - 10)
                GameController.instance.CalcNota();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14) onFinalPlatform = false;
    }

    IEnumerator HitedCoRoutine()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }
}