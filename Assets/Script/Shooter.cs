using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public GameObject bulletEnemy;
    public Transform shotEnemySpawner;

    public bool knockedFromLeft = false;

    // TRAVA DE TIRO: Impede que ele reinicie a animação 60 vezes por segundo
    private bool estaAtirando = false; 

    void Start()
    {
        attackDistanceY = 4;
    }

    protected override void Update()
    {
        base.Update();
        if (isKnockedDown) {
            return; // Se estiver deitado, congela IA, mira e giro
        }

        if (target == null) return;
        
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(target.rotation.x, target.rotation.y, transform.rotation.z),
            speed * Time.deltaTime
        );
        
        // Se já está no meio da animação de tiro, não tenta atirar de novo!
        if (estaAtirando) return;

        if (targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && !solved)
        {
            // O INIMIGO DECIDIU ATIRAR:
            if (targetDistanceX < 0) {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 0f, 0f);
            } else {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 180f, 0f);
            }

            // ATIVA A TRAVA!
            estaAtirando = true;
            
            // Dispara a animação UMA ÚNICA VEZ
            anim.SetBool("walk", false);
            anim.SetTrigger("shoot"); 
        }
        else
        {
            Move();
            anim.SetBool("walk", true);
            
            if (movingRight) {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            } else {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
    }

    // ==========================================
    // A MÁGICA DA ANIMAÇÃO (CHAMADOS PELO ANIMATOR)
    // ==========================================
    
    // Coloque este evento no FRAME 5 (Momento do Coice)
    public void EnemyShot()
    {
        Instantiate(bulletEnemy, shotEnemySpawner.position, shotEnemySpawner.rotation);
    }

    // CRUCIAL: Coloque este evento no ÚLTIMO FRAME da animação (Frame 9)
    public void FinalizarTiro()
    {
        // Libera a trava. O inimigo agora pode pensar e mirar de novo.
        estaAtirando = false;
    }

    // ==========================================
    // IMUNIDADE E QUEDAS (O SEU CÓDIGO INTACTO)
    // ==========================================
    public override void HitedFromLeft(int damage)
    {
        if (!isKnockedDown) return; 

        if (knockedFromLeft) valueRight += damage; 
        else valueLeft += damage; 

        CheckEquationWhileDown();
        StartCoroutine(HitedCoRoutine());
    }

    public override void HitedFromRight(int damage)
    {
        if (!isKnockedDown) return; 

        if (knockedFromLeft) valueLeft += damage; 
        else valueRight += damage; 

        CheckEquationWhileDown();
        StartCoroutine(HitedCoRoutine());
    }

    public override void ReceivePunch(bool hitFromLeft)
    {
        if (solved || isKnockedDown) return;

        knockedFromLeft = hitFromLeft; 
        base.ReceivePunch(hitFromLeft); 

        if (hitFromLeft) symbolValue = 1; 
        else symbolValue = 0; 

        CheckEquationWhileDown(); 
    }

    void CheckEquationWhileDown()
    {
        if (!isKnockedDown || solved) return;

        if (knockedFromLeft) 
        {
            if (valueRight < valueLeft) Solved();
        } 
        else 
        {
            if (valueLeft > valueRight) Solved();
        }
    }
}