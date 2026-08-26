using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float Speed = 10f;
    public float pushSpeed = 4f;
    public float JumpForce = 600;
    public bool isDead = false;
    private Rigidbody2D rig;
    public bool isJumping;
    public bool doubleJump;
    bool isShooting = false;

    public GameObject bulletPlusPrefab;
    public GameObject bulletMinusPrefab;
    public float fireRate = 5;
    public float nextFire = 0;
    public float KBForce = 5;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;
    public Transform shotSpawnerUp;
    public Transform shotSpawnerDown;
    private SpriteRenderer sprite;
    private bool onFinalPlatform = false;
    private float positionYOnFinalPlatform;

    // --- Variáveis do Soco (Melee) ---
    public bool isPunching = false;
    public float punchRate = 0.5f;
    private float nextPunch = 0;
    public Transform punchPoint; // Onde o soco acerta (crie um objeto vazio na frente do personagem)
    public float punchRadius = 0.5f; // Tamanho da área de impacto do soco
    public LayerMask enemyLayer;

    public bool isPushing = false;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onFinalPlatform)
        {
            if (!Input.GetButton("Fire3"))
                Move();
            Jump();
            Shot();
            Punch();
        }
        if (PlayerData.instance.health <= 0)
        {
            GameController.instance.ShowGameOver();

            gameObject.SetActive(false);
        }
    }

    void Move()
    {
        if (!isShooting && !isPunching)
        {
            if (KBCounter <= 0)
            {
                // MÁGICA AQUI: Checa se está empurrando. Se sim, usa a velocidade lenta!
                float velocidadeAtual = isPushing ? pushSpeed : Speed;
                
                rig.velocity = new Vector2(Input.GetAxis("Horizontal") * velocidadeAtual, rig.velocity.y);
            }
            else
            {
                if (KnockFromRight)
                    rig.velocity = new Vector2(-KBForce, KBForce);
                else
                    rig.velocity = new Vector2(KBForce, KBForce);
                KBCounter -= Time.deltaTime;
            }
            
            // Controle de animação de caminhada normal
            if (Input.GetAxis("Horizontal") > 0f)
            {
                // Se estiver empurrando, não toca a animação de walk, deixa a de push assumir
                anim.SetBool("walk", !isPushing); 
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else if (Input.GetAxis("Horizontal") < 0f)
            {
                anim.SetBool("walk", !isPushing);
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if (Input.GetAxis("Horizontal") == 0f)
            {
                anim.SetBool("walk", false);
            }
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isShooting = false;
            isPunching = false;
            if (!isJumping)
            {
                rig.AddForce(Vector3.up * JumpForce, ForceMode2D.Impulse);
                doubleJump = true;
                anim.SetBool("walk", false);
                anim.SetTrigger("TriggerJump");
            }
            else
            {
                if (doubleJump)
                {
                    rig.AddForce(Vector3.up * JumpForce * 0.7f, ForceMode2D.Impulse);
                    doubleJump = false;
                }
            }
        }
    }

    void Shot()
    {
        if (Input.GetButtonDown("Fire1") && nextFire < Time.time)
        {
            isShooting = true;
            
            // Freia o personagem no eixo X instantaneamente
            rig.velocity = new Vector2(0, rig.velocity.y); 
            
            anim.SetTrigger("ShootPlus");
            // Invoke("SpawnPlusProjectile", 0.15f);
            nextFire = Time.time + fireRate;
        }

        // if (Input.GetButtonDown("Fire2") && nextFire < Time.time)
        // {
        //     isShooting = true;
            
        //     // Freia o personagem no eixo X instantaneamente
        //     rig.velocity = new Vector2(0, rig.velocity.y); 
            
        //     anim.SetTrigger("ShootMinus");
        //     Invoke("SpawnMinusProjectile", 0.15f);
        //     Invoke("EndShot", fireRate);
        //     nextFire = Time.time + fireRate;
        // }
    }

    void EndShot()
    {
        isShooting = false;
    }

    void Punch()
    {
        if (Input.GetButtonDown("Fire2") && nextPunch < Time.time && !isPunching && !isShooting)
        {
            isPunching = true;

            // Para o movimento horizontal
            rig.velocity = new Vector2(0, rig.velocity.y);

            // Toca animação
            anim.SetTrigger("TriggerPunch");

            nextPunch = Time.time + punchRate;
        }
    }

    public void ApplyPunchDamage()
    {
        if (punchPoint == null)
            return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            punchPoint.position,
            punchRadius,
            enemyLayer
        );

        foreach (Collider2D hitCollider in hitEnemies)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            Debug.Log("Oito derrubou: " + enemy.gameObject.name);

            // Descobre de que lado o inimigo está
            bool enemyIsOnRight = hitCollider.transform.position.x > transform.position.x;
            bool hitFromLeft = enemyIsOnRight;

            // MÁGICA DO POLIMORFISMO: 
            // O Player manda apenas 1 argumento (o lado do soco).
            // Todos os inimigos vão cair, e o Shooter automaticamente vai virar a inequação!
            enemy.ReceivePunch(hitFromLeft);
        }
    }

    // Chame este método por Animation Event no ÚLTIMO FRAME da animação
    public void EndPunch()
    {
        isPunching = false;
    }

    void KnockBack(float playerPosition, float collisionPosition)
    {
        KBCounter = KBTotalTime;
        if (playerPosition <= collisionPosition)
            KnockFromRight = true;
        else
            KnockFromRight = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    // Enquanto estiver socando, Oito não recebe dano por contato
    if (!isPunching &&
        (collision.gameObject.layer == 9 || collision.gameObject.layer == 8))
    {
        PlayerData.instance.health--;

        KnockBack(
            transform.position.x,
            collision.transform.position.x
        );

        StartCoroutine(HitedCoRoutine());
    }

    if (collision.gameObject.layer == 8)
    {
        collision.gameObject.SetActive(false);
    }

    if (collision.gameObject.layer == 15)
    {
        PlayerData.instance.health = 0;
    }
}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FloorBlock fb = collision.gameObject.GetComponent<FloorBlock>();
        if (fb != null)
        {
            fb.Trampled();
        }
        DangerBlock db = collision.gameObject.GetComponent<DangerBlock>();
        if (db != null)
        {
            if (db.danger)
            {
                KnockBack(transform.position.x, collision.transform.position.x);
                PlayerData.instance.health--;
                StartCoroutine(HitedCoRoutine());
            }
        }
        if (collision.gameObject.layer == 13)
        {
            isJumping = false;
            anim.SetBool("jump", false);
        }

        if (collision.gameObject.layer == 14)
        {
            positionYOnFinalPlatform = transform.position.y;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14)
        {
            collision.gameObject.transform.Translate(Vector2.down * Speed * Time.deltaTime);
            transform.Translate(Vector2.down * Speed * Time.deltaTime);
            onFinalPlatform = true;
            if (transform.position.y < positionYOnFinalPlatform - 10)
            {
                GameController.instance.CalcNota();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            isJumping = true;
            anim.SetBool("jump", true);
        }
        if (collision.gameObject.layer == 14)
        {
            onFinalPlatform = false;
        }
    }

    IEnumerator HitedCoRoutine()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    private void SpawnPlusProjectile()
    {
        Instantiate(bulletPlusPrefab, shotSpawnerUp.position, shotSpawnerUp.rotation);
    }

    private void SpawnMinusProjectile()
    {
        Instantiate(bulletMinusPrefab, shotSpawnerDown.position, shotSpawnerDown.rotation);
    }

    void OnDrawGizmosSelected()
    {
        if (punchPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
    }

    public void SetPushing(bool empurrando)
    {
        // Só empurra se estiver tentando andar na direção do bloco
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
