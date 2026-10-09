using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IAtingivel
{
    [Header("Configurações Base")]
    public float speed = 3f;
    public float areaX = 5f;
    public float deathLimitY = -20f;
    public float attackDistanceX = 5f;
    public float attackDistanceY = 4f;

    [Header("Matemática")]
    public int valueLeft;
    public int valueRight;
    public int symbolValue; // 0 = Maior, 1 = Menor

    [Header("Física e Soco")]
    public float knockbackForce = 5f;
    public float knockdownTime = 3f;
    
    [HideInInspector] public bool isKnockedDown = false;
    [HideInInspector] public bool solved = false;
    
    protected bool movingRight = true;
    protected bool onFloor = true;
    protected float minX, maxX;
    protected float targetDistanceX, targetDistanceY;

    protected Rigidbody2D rb2d;
    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Transform target;
    protected Coroutine knockdownCoroutine;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        
        Player p = FindObjectOfType<Player>();
        if (p != null) target = p.transform;
        
        minX = transform.position.x - areaX;
        maxX = transform.position.x + areaX;
    }

    protected virtual void Update()
    {
        if (isKnockedDown) return;

        if (target != null)
        {
            targetDistanceX = transform.position.x - target.position.x;
            targetDistanceY = transform.position.y - target.position.y;
        }

        if (transform.position.y < deathLimitY)
        {
            gameObject.SetActive(false);
        }
    }

    // --- INTERFACE UNIVERSAL DE DANO ---
    public virtual void ReceberDano(int dano, bool atingidoPelaEsquerda)
    {
        if (atingidoPelaEsquerda) HitedFromLeft(dano);
        else HitedFromRight(dano);
    }

    public virtual void HitedFromLeft(int damage)
    {
        if (solved) return;
        valueLeft += damage;
        CheckResolution();
    }

    public virtual void HitedFromRight(int damage)
    {
        if (solved) return;
        valueRight += damage;
        CheckResolution();
    }

    protected void CheckResolution()
    {
        if (symbolValue == 0 && valueLeft > valueRight) Solved();
        else if (symbolValue == 1 && valueLeft < valueRight) Solved();
        else StartCoroutine(HitedCoRoutine());
    }

    protected void Solved()
    {
        gameObject.layer = 17; // EnemySolved
        sprite.color = Color.green;
        solved = true;
        PlayerData.instance.health++;
        PlayerData.instance.totalPoints++;
    }

    public virtual void Move()
    {
        if (isKnockedDown) return;
        rb2d.velocity = movingRight ? new Vector2(speed, rb2d.velocity.y) : new Vector2(-speed, rb2d.velocity.y);
        
        if (transform.position.x >= maxX) movingRight = false;
        else if (transform.position.x <= minX) movingRight = true;
    }

    // --- SISTEMA DE SOCO FÍSICO ---
    public virtual void ReceivePunch(bool hitFromLeft)
    {
        if (solved || isKnockedDown) return;
        KnockDown(hitFromLeft);
    }

    protected virtual void KnockDown(bool hitFromLeft)
    {
        isKnockedDown = true;
        anim.SetBool("walk", false);
        anim.enabled = false; 

        float direction = hitFromLeft ? 1f : -1f;
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        rb2d.AddForce(new Vector2(direction * knockbackForce, 3f), ForceMode2D.Impulse);

        float currentY = transform.eulerAngles.y;
        bool isFacingLeft = Mathf.Abs(currentY - 180f) < 10f;
        float fallAngle = hitFromLeft ? -90f : 90f;
        
        if (isFacingLeft) fallAngle = -fallAngle;
        transform.eulerAngles = new Vector3(0f, currentY, fallAngle);

        if (knockdownCoroutine != null) StopCoroutine(knockdownCoroutine);
        knockdownCoroutine = StartCoroutine(KnockdownRoutine());
    }

    protected virtual IEnumerator KnockdownRoutine()
    {
        yield return new WaitForSeconds(knockdownTime);
        if (solved) yield break;
        GetBackUp();
    }

    protected virtual void GetBackUp()
    {
        isKnockedDown = false;
        anim.enabled = true;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        anim.SetBool("walk", true);
    }

    protected IEnumerator HitedCoRoutine()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    // --- COLISÕES BÁSICAS ---
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 12) onFloor = true;
        if(collision.gameObject.layer == 15) gameObject.SetActive(false);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 12) onFloor = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 13) onFloor = true;
        if(collision.gameObject.layer == 16) movingRight = !movingRight;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 13) onFloor = false;
    }
}