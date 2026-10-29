using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public GameObject bulletEnemy;
    public Transform shotEnemySpawner;

    [Header("Coice do Texto no Tiro")]
    public EnemyUI enemyUI_Referencia;
    public float distanciaDoRecuo = 0.5f;

    public bool knockedFromLeft = false;

    // Travas lógicas
    private bool estaAtirando = false;
    private bool recuoTextoAplicado = false; // Controla se o texto está recuado

    void Start()
    {
        attackDistanceY = 4;
    }

    protected override void Update()
    {
        base.Update();

        // --- LEITURA AUTOMÁTICA DA ANIMAÇÃO ---
        if (anim != null && anim.gameObject.activeSelf && anim.enabled)
        {
            // Verifica na Unity se o estado de animação atual se chama exatamente "shoot"
            bool tocandoTiro = anim.GetCurrentAnimatorStateInfo(0).IsName("ShooterShot");

            if (tocandoTiro && !recuoTextoAplicado)
            {
                // A animação de tiro começou: aplica o recuo apenas uma vez
                recuoTextoAplicado = true;
                if (enemyUI_Referencia != null)
                {
                    enemyUI_Referencia.offsetLeft.x -= distanciaDoRecuo;
                    enemyUI_Referencia.offsetRight.x -= distanciaDoRecuo;
                }
            }
            else if (!tocandoTiro && recuoTextoAplicado)
            {
                // A animação de tiro acabou: devolve o texto ao normal
                recuoTextoAplicado = false;
                if (enemyUI_Referencia != null)
                {
                    enemyUI_Referencia.offsetLeft.x += distanciaDoRecuo;
                    enemyUI_Referencia.offsetRight.x += distanciaDoRecuo;
                }
            }
        }

        // Se estiver deitado, congela IA, mira e giro
        if (isKnockedDown) return;
        if (target == null) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(target.rotation.x, target.rotation.y, transform.rotation.z),
            speed * Time.deltaTime
        );

        if (estaAtirando) return;

        if (targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && !solved)
        {
            // O INIMIGO DECIDIU ATIRAR:
            if (targetDistanceX < 0)
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 180f, 0f);
            }

            estaAtirando = true;
            anim.SetBool("walk", false);
            anim.SetTrigger("shoot");
        }
        else
        {
            Move();
            anim.SetBool("walk", true);

            if (movingRight)
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
    }

    // ==========================================
    // ANIMAÇÃO DE DISPARO FÍSICO DA BALA
    // ==========================================

    // Mantenha este Animation Event no frame exato do coice para instanciar a borracha
    public void EnemyShot()
    {
        Instantiate(bulletEnemy, shotEnemySpawner.position, shotEnemySpawner.rotation);
    }

    // Mantenha este Animation Event no último frame (Frame 9) para liberar a IA para andar de novo
    public void FinalizarTiro()
    {
        estaAtirando = false;
    }

    // ==========================================
    // IMUNIDADE E QUEDAS
    // ==========================================

    // --- CORREÇÃO: SOBRESCREVE A REGRA DE DANO DO INIMIGO BASE ---
    public override void ReceberDano(int dano, bool atingidoPelaEsquerda)
    {
        // Se o Shooter estiver deitado, ele ignora a regra de 180 graus do Chaser
        // e usa puramente o lado em que a bala bateu na tela.
        if (isKnockedDown)
        {
            if (atingidoPelaEsquerda) HitedFromLeft(dano);
            else HitedFromRight(dano);
        }
        else
        {
            // Se estiver em pé, obedece o código base normal
            base.ReceberDano(dano, atingidoPelaEsquerda);
        }
    }

    public override void HitedFromLeft(int damage)
    {
        if (!isKnockedDown) return;

        if (knockedFromLeft) valueRight += damage;
        else valueLeft += damage;

        CheckEquationWhileDown();
        if (!solved)
            StartCoroutine(HitedCoRoutine());
    }

    public override void HitedFromRight(int damage)
    {
        if (!isKnockedDown) return;

        if (knockedFromLeft) valueLeft += damage;
        else valueRight += damage;

        CheckEquationWhileDown();
        if (!solved)
            StartCoroutine(HitedCoRoutine());
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

    public override void ReceivePunch(bool hitFromLeft)
    {
        if (solved || isKnockedDown) return;

        if (anim != null)
        {
            anim.enabled = true;
            anim.ResetTrigger("shoot");

            // Entra em ShooterWalk e atualiza a pose antes de a queda
            // desativar o Animator.
            anim.Play("ShooterWalk", 0, 0f);
            anim.Update(0f);
        }

        estaAtirando = false;

        if (recuoTextoAplicado)
        {
            recuoTextoAplicado = false;

            if (enemyUI_Referencia != null)
            {
                enemyUI_Referencia.offsetLeft.x += distanciaDoRecuo;
                enemyUI_Referencia.offsetRight.x += distanciaDoRecuo;
            }
        }

        knockedFromLeft = hitFromLeft;
        base.ReceivePunch(hitFromLeft);

        symbolValue = hitFromLeft ? 1 : 0;
        CheckEquationWhileDown();
    }

    // --- NOVIDADE: O COMPORTAMENTO DE CURA DO SHOOTER ---
    protected override void Solved()
    {
        if (solved) return;

        estaAtirando = false;

        if (recuoTextoAplicado)
        {
            recuoTextoAplicado = false;

            if (enemyUI_Referencia != null)
            {
                enemyUI_Referencia.offsetLeft.x += distanciaDoRecuo;
                enemyUI_Referencia.offsetRight.x += distanciaDoRecuo;
            }
        }

        if (isKnockedDown)
        {
            if (knockdownCoroutine != null)
            {
                StopCoroutine(knockdownCoroutine);
                knockdownCoroutine = null;
            }

            isKnockedDown = false;

            if (rb2d != null)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.constraints |= RigidbodyConstraints2D.FreezeRotation;
            }

            // Mantém a direção para a qual estava virado e remove a rotação da queda.
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }

        if (anim != null)
        {
            anim.enabled = true;
            anim.ResetTrigger("shoot");
            anim.SetBool("walk", true);
            anim.Play("ShooterWalk", 0, 0f);
            anim.Update(0f);
        }

        base.Solved();
    }
}