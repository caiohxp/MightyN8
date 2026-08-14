using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float areaX = 5f;
    protected bool movingRight = true; 
    protected float minX;
    protected float maxX;
    public float speed;
    public float attackDistanceX;
    public float attackDistanceY;
    public int valueLeft;
    public int valueRight;
    public int symbolValue;
    protected bool solved = false;
    protected bool onFloor = true;

    protected Animator anim;
    protected Transform target;
    private Collider2D playerCollider;
    private Collider2D bulletCollider;
    protected float targetDistanceX;
    protected float targetDistanceY;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sprite;
    public Transform leftCounterTransform;
    public Transform rightCounterTransform;
    public Vector3 offset;
    public Vector3 offset2;
    protected TextMeshProUGUI leftCounterText;
    protected TextMeshProUGUI rightCounterText;
    public Player player;
    public float deathLimitY = -20f; 

    // --- SISTEMA UNIVERSAL DE SOCO ---
    public bool isKnockedDown = false;
    public float knockdownTime = 3f;
    public float knockbackForce = 5f;
    protected Coroutine knockdownCoroutine;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        leftCounterText = leftCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
        rightCounterText = rightCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
        minX = transform.position.x - areaX;
        maxX = transform.position.x + areaX;
    }

    protected virtual void Update()
    {
        // Textos acompanham a rotação física do boneco ao cair
        leftCounterTransform.position = transform.position + (transform.rotation * offset);
        rightCounterTransform.position = transform.position + (transform.rotation * offset2);
        
        leftCounterText.text = valueLeft.ToString();
        rightCounterText.text = valueRight.ToString();

        // Se estiver derrubado, congela a perseguição
        if (isKnockedDown) return;

        targetDistanceX = transform.position.x - target.position.x;
        targetDistanceY = transform.position.y - target.position.y;
        
        if(transform.position.y < deathLimitY){
            gameObject.SetActive(false);
        }
    }

    protected void Solved(){
        gameObject.layer = 17;
        sprite.color = Color.green;
        solved = true;
        PlayerData.instance.health++;
        PlayerData.instance.totalPoints++;
    }
    
    // Transformado em VIRTUAL para o Shooter poder bloquear se estiver em pé
    public virtual void HitedFromLeft(int damage){
        if(symbolValue == 0 && !solved){
            valueLeft += damage;
            if(valueLeft > valueRight) Solved();
            else StartCoroutine(HitedCoRoutine());
        } else if(symbolValue == 1 && !solved){
            valueLeft += damage;
            if(valueLeft < valueRight) Solved();
            else StartCoroutine(HitedCoRoutine());
        }
    }

    public virtual void HitedFromRight(int damage){
        if(symbolValue == 0 && !solved){
            valueRight += damage;
            if(valueLeft > valueRight) Solved();
            else StartCoroutine(HitedCoRoutine());
        } else if(symbolValue == 1 && !solved){
            valueRight += damage;
            if(valueLeft < valueRight) Solved();
            else StartCoroutine(HitedCoRoutine());
        }
    }

    public virtual void Move(){
        if (isKnockedDown) return;
        rb2d.velocity = movingRight ? new Vector2(speed, rb2d.velocity.y) : new Vector2(-speed, rb2d.velocity.y);
        if (transform.position.x >= maxX) movingRight = false;
        else if (transform.position.x <= minX) movingRight = true;
    }

    // --- MÉTODOS DO SOCO (Cair e Levantar) ---
    public virtual void ReceivePunch(bool hitFromLeft)
    {
        if (solved || isKnockedDown) return;
        KnockDown(hitFromLeft);
    }

    protected virtual void KnockDown(bool hitFromLeft)
    {
        isKnockedDown = true;
        anim.SetBool("walk", false);
        
        // Desliga o Animator para ele não brigar com a rotação de 90 graus
        anim.enabled = false; 

        float direction = hitFromLeft ? 1f : -1f;
        
        // Zera o X atual para não acumular, mas mantém a gravidade (Y)
        rb2d.velocity = new Vector2(0, rb2d.velocity.y); 
        
        // Dá o empurrão do soco (com um pulinho no Y para dar impacto)
        rb2d.AddForce(new Vector2(direction * knockbackForce, 3f), ForceMode2D.Impulse);

        // --- CORREÇÃO DO ÂNGULO DE QUEDA ---
        float currentY = transform.eulerAngles.y;
        
        // Verifica se o inimigo está virado de costas (Y próximo de 180)
        bool isFacingLeft = Mathf.Abs(currentY - 180f) < 10f; 
        
        // Ângulo padrão: Soco da esquerda cai pra direita (-90) / Soco da direita cai pra esquerda (90)
        float fallAngle = hitFromLeft ? -90f : 90f;
        
        // Se o boneco estiver espelhado no mundo, invertemos a matemática da queda!
        if (isFacingLeft) 
        {
            fallAngle = -fallAngle;
        }

        // Gira o boneco fisicamente corrigido
        transform.eulerAngles = new Vector3(0f, currentY, fallAngle);
        // -----------------------------------

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
        anim.enabled = true; // Religa a animação
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f); // Levanta o boneco
        anim.SetBool("walk", true);
    }

    protected IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.layer == 12) onFloor = true;
        if(collision.gameObject.layer == 15) gameObject.SetActive(false);
    }
    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.layer == 12) onFloor = false;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.layer == 13) onFloor = true;
        if(collision.gameObject.layer == 16) movingRight = !movingRight;
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.layer == 13) onFloor = false;
    }
}