using UnityEngine;

public class MathBlock : MonoBehaviour
{
    public enum TipoSinal { Igual, Maior, Menor }

    [Header("Matemática do Bloco")]
    public int valorDoBloco; 
    public TipoSinal sinalEsquerdo;
    public TipoSinal sinalDireito;
    
    [Header("Status do Jogador")]
    public int valorDoPlayer = 8; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Substituímos o isKinematic!
        // O bloco começa com o Eixo X e a Rotação travados. A gravidade (Y) continua funcionando!
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {

            ContactPoint2D contato = collision.GetContact(0);

            bool bateuNaLateral = Mathf.Abs(contato.normal.x) > 0.5f;

            if(bateuNaLateral){

                collision.gameObject.GetComponent<Player>().SetPushing(true);

                bool ottoEstaNaEsquerda = collision.transform.position.x < transform.position.x;
                bool expressaoCorreta = false;

                if (ottoEstaNaEsquerda)
                {
                    expressaoCorreta = ValidarExpressao(valorDoPlayer, sinalEsquerdo, valorDoBloco);
                }
                else
                {
                    expressaoCorreta = ValidarExpressao(valorDoBloco, sinalDireito, valorDoPlayer);
                }

                if (expressaoCorreta)
                {
                    // Matemática verdadeira! Destrava o eixo X para ser empurrado, mas não deixa rolar (FreezeRotation)
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
                }
                else
                {
                    // Matemática falsa! Trava o Eixo X como uma parede
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    
                    // Zera apenas a velocidade horizontal, mantendo a queda (Y) intacta
                    rb.velocity = new Vector2(0, rb.velocity.y); 
                }
            } else {
                collision.gameObject.GetComponent<Player>().SetPushing(false);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
        } 
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.layer == 9)
        {
            // Quando soltar a pedra, trava o eixo X novamente
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = new Vector2(0, rb.velocity.y);
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