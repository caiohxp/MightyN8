using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ConditionalPlatform : MonoBehaviour
{
    public enum SimboloMatematico { MaiorQue, MenorQue, Igual }
    
    [Header("Matemática da Plataforma")]
    public SimboloMatematico simbolo;
    public int valorDaPlataforma; 
    public int valorDoPlayer = 8; 

    [Header("Comportamento de Destruição")]
    public bool seDestroiComErro = true; // Se for falso, ela só fica invisível como antes
    public float tempoAteDestruir = 0.5f; // Quanto tempo o Oito consegue ficar em pé antes dela quebrar
    private bool jaEstaQuebrando = false;

    [Header("Visual")]
    public TextMeshPro textoExpressao; 
    public float transparenciaFantasma = 0.4f; 

    private Collider2D coll;
    private SpriteRenderer sprite;
    private Animator anim; // Novo componente para tocar a animação!

    void Start()
    {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Pega o Animator que você vai colocar nela
        
        AtualizarPlataforma(false); // Roda sem verificar colisão no Start
    }

    // Chamamos a função normal, mas agora passamos um aviso se o Oito pisou nela
    public void AtualizarPlataforma(bool ottoPisou)
    {
        if (jaEstaQuebrando) return; // Se já começou a quebrar, ignora a matemática!

        if (textoExpressao != null)
        {
            string stringSimbolo = "";
            if (simbolo == SimboloMatematico.MaiorQue) stringSimbolo = ">";
            else if (simbolo == SimboloMatematico.MenorQue) stringSimbolo = "<";
            else if (simbolo == SimboloMatematico.Igual) stringSimbolo = "=";

            textoExpressao.text = stringSimbolo + " " + valorDaPlataforma.ToString();
        }

        bool expressaoCorreta = false;

        switch (simbolo)
        {
            case SimboloMatematico.MaiorQue: expressaoCorreta = valorDoPlayer > valorDaPlataforma; break;
            case SimboloMatematico.MenorQue: expressaoCorreta = valorDoPlayer < valorDaPlataforma; break;
            case SimboloMatematico.Igual: expressaoCorreta = valorDoPlayer == valorDaPlataforma; break;
        }

        if (expressaoCorreta)
        {
            // Matemática certa! Sempre fica sólida e normal.
            coll.enabled = true; 
            if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 1f); 
        }
        else
        {
            // Matemática errada!
            if (seDestroiComErro)
            {
                // É um bloco de destruição!
                if (ottoPisou)
                {
                    // Otto caiu na armadilha, inicia a destruição!
                    StartCoroutine(RotinaDeDestruicao());
                }
                else
                {
                    // Otto ainda não pisou. Fica SÓLIDO para enganar o jogador!
                    coll.enabled = true; 
                    if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 1f); 
                }
            }
            else
            {
                // É um bloco clássico fantasma. Fica intangível desde o começo.
                coll.enabled = false; 
                if (sprite != null) sprite.color = new Color(1f, 1f, 1f, transparenciaFantasma); 
            }
        }
    }

    // A mágica de destruir e desmoronar
    IEnumerator RotinaDeDestruicao()
    {
        jaEstaQuebrando = true;
        if (anim != null) anim.SetTrigger("Quebrar");
        yield return null;
    }

    public void DesativarColisor()
    {
        coll.enabled = false; // O chão fica intangível e o Otto cai instantaneamente
    }

    // Chame este método no FRAME 9 (último frame) da animação
    public void FinalizarDestruicao()
    {
        gameObject.SetActive(false); // O objeto da plataforma some da tela de vez
    }

    // Quando o Otto encosta na plataforma (Pisou!)
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Algo bateu na plataforma: " + collision.gameObject.name);
        // Se quem bateu for o Player (layer 9 ou tag "Player")
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("O Otto pisou! Mandando o sinal de quebrar!");
            // Força ela a ligar a colisão rapidamente para ele encostar e logo manda a checagem
            coll.enabled = true; 
            AtualizarPlataforma(true); // O 'true' avisa que o Otto pisou!
        }
    }

    public void ReceberTiro(int danoDaBala)
    {
        if (jaEstaQuebrando) return;
        valorDaPlataforma += danoDaBala;
        AtualizarPlataforma(false); 
    }
}