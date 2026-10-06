using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Estilingue 'Y'")]
    public GameObject bulletPlusPrefab;
    public GameObject bulletMinusPrefab;
    public float fireRate = 0.5f;
    public Transform shotSpawnerUp;
    public Transform shotSpawnerDown;
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

        Shot();
        Punch();
    }

    void Shot()
    {
        if (Input.GetButtonDown("Fire1") && nextFire < Time.time)
        {
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

    // Acionado pelas Animações (Animation Events)
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
    private void SpawnPlusProjectile() { Instantiate(bulletPlusPrefab, shotSpawnerUp.position, shotSpawnerUp.rotation); }
    private void SpawnMinusProjectile() { Instantiate(bulletMinusPrefab, shotSpawnerDown.position, shotSpawnerDown.rotation); }

    // Utilidade externa para outros scripts pararem os ataques
    public void CancelAttacks()
    {
        isShooting = false;
        isPunching = false;
    }

    void OnDrawGizmosSelected()
    {
        if (punchPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
    }
}