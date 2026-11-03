using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    public enum ModoDisparo { Soma, Subtracao, Multiplicacao, Divisao }

    [Header("Gerenciamento de Arma")]
    public ModoDisparo currentMode = ModoDisparo.Soma;

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
    [HideInInspector] public bool isPunching = false;
    private float nextPunch = 0;

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
        if (health != null && (health.isKnockedBack || health.onFinalPlatform)) return;

        TrocarOperacao();
        Shot();
        Punch();
    }

    void TrocarOperacao()
    {
        // Alterna para o próximo modo desbloqueado ao apertar a tecla E (ou Q)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
        {
            if (currentMode == ModoDisparo.Soma)
            {
                if (PlayerData.instance.unlockedSub) currentMode = ModoDisparo.Subtracao;
                else if (PlayerData.instance.unlockedMulti) currentMode = ModoDisparo.Multiplicacao;
                else if (PlayerData.instance.unlockedDiv) currentMode = ModoDisparo.Divisao;
            }
            else if (currentMode == ModoDisparo.Subtracao)
            {
                if (PlayerData.instance.unlockedMulti) currentMode = ModoDisparo.Multiplicacao;
                else if (PlayerData.instance.unlockedDiv) currentMode = ModoDisparo.Divisao;
                else currentMode = ModoDisparo.Soma; // Cicla de volta pro início
            }
            else if (currentMode == ModoDisparo.Multiplicacao)
            {
                if (PlayerData.instance.unlockedDiv) currentMode = ModoDisparo.Divisao;
                else currentMode = ModoDisparo.Soma;
            }
            else if (currentMode == ModoDisparo.Divisao)
            {
                currentMode = ModoDisparo.Soma;
            }
        }
    }

    void Shot()
    {
        if (Input.GetButtonDown("Fire1") && nextFire < Time.time && !isShooting && !isPunching)
        {
            // Consulta a munição universal diretamente do PlayerData
            if (PlayerData.instance.universalAmmo <= 0) return; 

            PlayerData.instance.universalAmmo--; // Gasta 1 da reserva
            isShooting = true;
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
            rig.velocity = new Vector2(0, rig.velocity.y); 
            anim.SetTrigger("TriggerPunch");
            nextPunch = Time.time + punchRate;
        }
    }

    // ==========================================
    // ANIMATION EVENTS
    // ==========================================
    
    // Certifique-se de usar "SpawnSelectedProjectile" na janela Animation no frame do tiro
    public void SpawnSelectedProjectile()
    {
        GameObject prefabParaAtirar = bulletPlusPrefab; // Padrão
        
        switch (currentMode)
        {
            case ModoDisparo.Soma: prefabParaAtirar = bulletPlusPrefab; break;
            case ModoDisparo.Subtracao: prefabParaAtirar = bulletMinusPrefab; break;
            case ModoDisparo.Multiplicacao: prefabParaAtirar = bulletMultPrefab; break;
            case ModoDisparo.Divisao: prefabParaAtirar = bulletDivPrefab; break;
        }

        if (prefabParaAtirar != null && shotSpawnerUp != null)
        {
            Instantiate(prefabParaAtirar, shotSpawnerUp.position, shotSpawnerUp.rotation);
        }
    }

    public void ApplyPunchDamage()
    {
        if (punchPoint == null) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPoint.position, punchRadius, enemyLayer);
        foreach (Collider2D hitCollider in hitEnemies)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy == null) continue;
            bool hitFromLeft = hitCollider.transform.position.x > transform.position.x;
            enemy.ReceivePunch(hitFromLeft);
        }
    }

    public void EndShot() { isShooting = false; }
    public void EndPunch() { isPunching = false; }
    public void CancelAttacks() { isShooting = false; isPunching = false; }

    void OnDrawGizmosSelected()
    {
        if (punchPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
    }
}