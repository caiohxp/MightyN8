using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    // O modo de disparo agora vive só em PlayerData.instance.currentMode
    // (a troca com E/Q também é feita lá).

    [Header("Prefabs de Munição")]
    public GameObject bulletPlusPrefab;
    public GameObject bulletMinusPrefab;
    public GameObject bulletMultPrefab;
    public GameObject bulletDivPrefab;

    public float fireRate = 0.5f;
    public Transform shotSpawnerUp;
    [HideInInspector] public bool isShooting = false;
    private float nextFire = 0;

    [Header("Soco (Controle de Multidão)")]
    public float punchRate = 0.5f;
    public Transform punchPoint;
    public float punchRadius = 0.5f;
    public LayerMask enemyLayer;
    [Tooltip("Layer só das formas geométricas (crie uma layer 'Formas' e coloque nos prefabs). O padrão 'Everything' funciona, mas é menos eficiente.")]
    public LayerMask geometricLayer = ~0;
    [HideInInspector] public bool isPunching = false;
    private float nextPunch = 0;

    [Header("Segurança Anti-Travamento")]
    [Tooltip("Tempo máximo (pouco maior que a animação de tiro/soco) para destravar caso o Animation Event não dispare")]
    public float maxAttackLockTime = 0.6f;
    private float attackUnlockTimer = 0f;

    private Rigidbody2D rig;
    private Animator anim;
    private PlayerHealth health;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (health != null && (health.isKnockedBack || health.onFinalPlatform))
        {
            // Se tomar knockback no meio de um tiro/soco, garante que não volta travado depois
            if (isShooting || isPunching) CancelAttacks();
            return;
        }

        // Trava de segurança: destrava automaticamente se a animação for interrompida por um relevo/queda
        if (isShooting || isPunching)
        {
            attackUnlockTimer -= Time.deltaTime;
            if (attackUnlockTimer <= 0f)
            {
                CancelAttacks();
            }
        }

        Shot();
        Punch();
    }

    void Shot()
    {
        if (Input.GetButtonDown("Fire1") && nextFire < Time.time && !isShooting && !isPunching)
        {
            if (PlayerData.instance.universalAmmo <= 0) return;

            PlayerData.instance.universalAmmo--;
            isShooting = true;
            attackUnlockTimer = maxAttackLockTime;
            rig.velocity = new Vector2(0, rig.velocity.y);
            anim.SetTrigger("ShootPlus");
            nextFire = Time.time + fireRate;
        }
    }

    void Punch()
    {
        if (Input.GetButtonDown("Fire2") && nextPunch < Time.time && !isPunching && !isShooting)
        {
            isPunching = true;
            attackUnlockTimer = maxAttackLockTime;
            rig.velocity = new Vector2(0, rig.velocity.y);
            anim.SetTrigger("TriggerPunch");
            nextPunch = Time.time + punchRate;
        }
    }

    // ==========================================
    // ANIMATION EVENTS
    // ==========================================

    // Use "SpawnSelectedProjectile" na janela Animation no frame do tiro
    public void SpawnSelectedProjectile()
    {
        GameObject prefabParaAtirar = bulletPlusPrefab;

        switch (PlayerData.instance.currentMode)
        {
            case PlayerData.MathOperation.Sum: prefabParaAtirar = bulletPlusPrefab; break;
            case PlayerData.MathOperation.Sub: prefabParaAtirar = bulletMinusPrefab; break;
            case PlayerData.MathOperation.Multi: prefabParaAtirar = bulletMultPrefab; break;
            case PlayerData.MathOperation.Div: prefabParaAtirar = bulletDivPrefab; break;
        }

        if (prefabParaAtirar != null && shotSpawnerUp != null)
        {
            Instantiate(prefabParaAtirar, shotSpawnerUp.position, shotSpawnerUp.rotation);
        }
    }

    public void ApplyPunchDamage()
    {
        if (punchPoint == null) return;

        // Inimigos
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPoint.position, punchRadius, enemyLayer);
        foreach (Collider2D hitCollider in hitEnemies)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy == null) continue;
            bool hitFromLeft = hitCollider.transform.position.x > transform.position.x;
            enemy.ReceivePunch(hitFromLeft);
        }

        // Formas geométricas: um soco acerta só a forma mais próxima,
        // para não contar acerto e erro no mesmo golpe
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(punchPoint.position, punchRadius, geometricLayer);
        GeometricTarget maisProxima = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider2D hitCollider in hitObjects)
        {
            GeometricTarget alvo = hitCollider.GetComponent<GeometricTarget>();
            if (alvo == null) continue;

            float distancia = ((Vector2)hitCollider.transform.position - (Vector2)punchPoint.position).sqrMagnitude;
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProxima = alvo;
            }
        }

        if (maisProxima != null) maisProxima.ReceberSoco(health);
    }

    public void EndShot()
    {
        isShooting = false;
        attackUnlockTimer = 0f;
    }

    public void EndPunch()
    {
        isPunching = false;
        attackUnlockTimer = 0f;
    }

    public void CancelAttacks()
    {
        isShooting = false;
        isPunching = false;
        attackUnlockTimer = 0f;

        if (anim != null)
        {
            anim.ResetTrigger("ShootPlus");
            anim.ResetTrigger("TriggerPunch");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (punchPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
    }
}