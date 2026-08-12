using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public GameObject bulletEnemy;
    public Transform shotEnemySpawner;
    public float fireRate = 5;
    public float nextFire = 0;
    private Vector3 startOffset;
    private Vector3 startOffset2;

    public bool knockedFromLeft = false;

    void Start()
    {
        startOffset = offset;
        startOffset2 = offset2;
        attackDistanceY = 4;
    }

    protected override void Update()
    {
        if (isKnockedDown) {
            return; // Se estiver deitado, congela IA, mira e giro
        }

        base.Update();
        
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(target.rotation.x, target.rotation.y, transform.rotation.z),
            speed * Time.deltaTime
        );
        
        if (targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && !solved)
        {
            EnemyShot();
            anim.SetBool("shoot", true);
            anim.SetBool("walk", false);
            
            if (targetDistanceX < 0) {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 0f, 0f);
                offset.x = startOffset.x - 0.3f;
                offset2.x = startOffset2.x - 0.3f;
            } else {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 180f, 0f);
                offset.x = startOffset.x + 0.3f;
                offset2.x = startOffset2.x + 0.3f;
            }
        }
        else
        {
            Move();
            offset.x = startOffset.x;
            offset2.x = startOffset2.x;
            anim.SetBool("shoot", false);
            anim.SetBool("walk", true);
            
            if (movingRight) {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            } else {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
    }

    void EnemyShot()
    {
        if (nextFire < Time.time) {
            nextFire = Time.time + fireRate;
            Instantiate(bulletEnemy, shotEnemySpawner.position, shotEnemySpawner.rotation);
        }
    }

    // ==========================================
    // IMUNIDADE E INTELIGÊNCIA DOS TIROS
    // ==========================================
    public override void HitedFromLeft(int damage)
    {
        if (!isKnockedDown) return; // IMUNE AOS TIROS SE ESTIVER EM PÉ!

        // O tiro acertou o número que está fisicamente na ESQUERDA do boneco caído
        if (knockedFromLeft) valueRight += damage; // Caiu pra direita, logo Fundo virou Esquerda
        else valueLeft += damage; // Caiu pra esquerda, logo Topo virou Esquerda

        CheckEquationWhileDown();
        StartCoroutine(HitedCoRoutine());
    }

    public override void HitedFromRight(int damage)
    {
        if (!isKnockedDown) return; // IMUNE AOS TIROS SE ESTIVER EM PÉ!

        // O tiro acertou o número que está fisicamente na DIREITA do boneco caído
        if (knockedFromLeft) valueLeft += damage; // Caiu pra direita, logo Topo virou Direita
        else valueRight += damage; // Caiu pra esquerda, logo Fundo virou Direita

        CheckEquationWhileDown();
        StartCoroutine(HitedCoRoutine());
    }

    // ==========================================
    // RECEBEU SOCO E CAIU NO CHÃO
    // ==========================================
    public override void ReceivePunch(bool hitFromLeft)
    {
        if (solved || isKnockedDown) return;

        knockedFromLeft = hitFromLeft; 
        base.ReceivePunch(hitFromLeft); // Faz a animação física de cair da classe Enemy

        if (hitFromLeft) symbolValue = 1; // < (O 'v' caiu de lado virado pra cá)
        else symbolValue = 0; // > 

        CheckEquationWhileDown(); 
    }

    void CheckEquationWhileDown()
    {
        if (!isKnockedDown || solved) return;

        // O jogo "lê" o que está escrito no chão da esquerda para a direita
        if (knockedFromLeft) 
        {
            // Oito socou da esquerda. Visão no chão: (Fundo) < (Topo)
            if (valueRight < valueLeft) Solved();
        } 
        else 
        {
            // Oito socou da direita. Visão no chão: (Topo) > (Fundo)
            if (valueLeft > valueRight) Solved();
        }
    }
}