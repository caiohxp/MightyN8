using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Enemy
{
    [Header("Bomber - Detalhes Explosivos")]
    public float decrementRate = 1f; 
    private float nextDecrement;
    
    [Tooltip("O raio da explosão que vai atingir o jogador e blocos")]
    public float explosionRadius = 3f;

    private float normalSpeed;
    private float rageSpeed;
    private bool isExploding = false; // Trava para impedir movimento ou contagem extra durante a morte

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        normalSpeed = speed;
        rageSpeed = speed * 3;
    }

    void Start()
    {
        // O símbolo do Bomber será visual e mecânico como 2 (Igual)
        symbolValue = 2; // Você precisará criar uma lógica específica para "Igualdade" no Enemy base se for 2, ou ignorar se ele só explode.
        nextDecrement = Time.time + decrementRate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        // Se já está morto ou explodindo, corta o processamento e o movimento
        if (solved || isExploding || isKnockedDown) return; 

        Move();

        float anguloY = movingRight ? 0f : 180f;
        transform.eulerAngles = new Vector3(0f, anguloY, transform.eulerAngles.z);
        
        // Mantém a animação simples (apenas as pernas andando, como você gerou no Spritesheet)
        if(anim != null) anim.SetBool("walk", speed > 0);

        // Lógica de Gatilho e Velocidade da Dinamite (Entrar no "Target" = Acelerar e Acender o Pavio)
        if(targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && targetDistanceY < attackDistanceY && targetDistanceY > -attackDistanceY)
        {
            speed = rageSpeed;
            TimerToExplode(); // Só conta quando o Otto entra na zona e "acende" o Bomber
        } 
        else 
        {
            speed = normalSpeed;
        }
    }

    void TimerToExplode()
    {
        if(nextDecrement < Time.time)
        {
            valueLeft--; // Decrementa a contagem

            // Você pode adicionar um textMesh no prefab para exibir o 'valueLeft' na testa da dinamite aqui

            if(valueLeft <= 0)
            {
                TriggerExplosion();
            }
            
            nextDecrement = Time.time + decrementRate;
        }
    }

    // Rotina da Explosão em Área
    void TriggerExplosion()
    {
        if (isExploding) return;
        isExploding = true;
        speed = 0;
        rb2d.velocity = Vector2.zero; // Para o inimigo instantaneamente

        if (anim != null) anim.SetTrigger("Death"); // Toca a animação da explosão da dinamite

        // Detecta tudo ao redor no raio da explosão
        Collider2D[] objetosAtingidos = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in objetosAtingidos)
        {
            // Se a explosão bater no jogador
            if (hit.gameObject.CompareTag("Player") || hit.gameObject.layer == 9)
            {
                if (PlayerData.instance != null) PlayerData.instance.health -= 1; // Ajuste o dano da bomba no jogador
            }
            
            // Você pode adicionar lógicas aqui para a explosão empurrar caixas, por exemplo
        }

        // Se o seu Bomber for resolvido no tempo exato, você pode não querer soltar a chave se ele estourou sozinho.
        // Se a explosão for considerada uma "morte natural", chame a destruição.
    }

    // Chamado pelo Animation Event no fim da explosão da dinamite
    public void OnDeathAnimationFinish()
    {
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}