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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // O bloco começa fisicamente travado (ancorado)
        rb.isKinematic = true; 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Se quem encostou foi o jogador...
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {
            // O Otto SEMPRE tenta empurrar visualmente quando encosta, independente da matemática
            collision.gameObject.GetComponent<Player>().SetPushing(true);

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
            }
            else
            {
                // Matemática falsa! O bloco continua travado como uma parede
                rb.isKinematic = true;
                rb.velocity = Vector2.zero; // Evita qualquer deslize
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Quando o Otto solta a pedra e se afasta, o bloco trava e a animação de força para
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            collision.gameObject.GetComponent<Player>().SetPushing(false);
        }
    }

    bool ValidarExpressao(int valor1, TipoSinal sinal, int valor2)
    {
        if (sinal == TipoSinal.Igual) return valor1 == valor2;
        if (sinal == TipoSinal.Maior) return valor1 > valor2;
        if (sinal == TipoSinal.Menor) return valor1 < valor2;
        return false;
    }
}