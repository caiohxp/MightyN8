using System.Collections;
using UnityEngine;

public class MathBlock : MonoBehaviour
{
    public enum TipoSinal { Igual, Maior, Menor }

    [Header("Matemática do Bloco")]
    public int valorDoBloco; // Ex: 8, 9, 7
    public TipoSinal sinalEsquerdo;
    public TipoSinal sinalDireito;
    
    [Header("Status do Jogador")]
    public int valorDoPlayer = 8; // O valor fixo do Oito

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color corOriginal;
    private bool isBlinking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        corOriginal = spriteRenderer.color;

        // O bloco começa fisicamente travado (ancorado)
        rb.isKinematic = true; 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Se quem encostou foi o jogador...
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {
            // Descobre de que lado o Otto está batendo
            bool ottoEstaNaEsquerda = collision.transform.position.x < transform.position.x;
            bool expressaoCorreta = false;

            if (ottoEstaNaEsquerda)
            {
                // Lógica da Esquerda: (Valor do Otto) [Sinal Esquerdo] (Valor do Bloco)
                expressaoCorreta = ValidarExpressao(valorDoPlayer, sinalEsquerdo, valorDoBloco);
            }
            else
            {
                // Lógica da Direita: (Valor do Bloco) [Sinal Direito] (Valor do Otto)
                expressaoCorreta = ValidarExpressao(valorDoBloco, sinalDireito, valorDoPlayer);
            }

            if (expressaoCorreta)
            {
                // Matemática verdadeira! O bloco destrava e obedece à física para ser empurrado
                rb.isKinematic = false; 
                
                // Avisa o Otto para tocar a animação de empurrar
                collision.gameObject.GetComponent<Player>().SetPushing(true);
            }
            else
            {
                // Matemática falsa! O bloco trava como uma parede
                TravarBloco(collision.gameObject);

                // Feedback visual de erro (Pisca vermelho)
                if (!isBlinking) StartCoroutine(PiscarErro());
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Quando o Otto se afasta, o bloco trava novamente e a animação para
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {
            TravarBloco(collision.gameObject);
        }
    }

    void TravarBloco(GameObject player)
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero; // Para qualquer deslize residual instantaneamente
        player.GetComponent<Player>().SetPushing(false); // Para a animação
    }

    bool ValidarExpressao(int valor1, TipoSinal sinal, int valor2)
    {
        if (sinal == TipoSinal.Igual) return valor1 == valor2;
        if (sinal == TipoSinal.Maior) return valor1 > valor2;
        if (sinal == TipoSinal.Menor) return valor1 < valor2;
        return false;
    }

    IEnumerator PiscarErro()
    {
        isBlinking = true;
        spriteRenderer.color = Color.red;
        // Dica: Aqui você pode adicionar um AudioSource tocando som de erro (bloqueado)
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = corOriginal;
        yield return new WaitForSeconds(0.2f);
        isBlinking = false;
    }
}