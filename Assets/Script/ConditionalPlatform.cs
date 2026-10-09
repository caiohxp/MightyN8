using System.Collections;
using UnityEngine;
using TMPro; 

public class ConditionalPlatform : MonoBehaviour
{
    public enum TipoPlataforma { Normal, Intangivel, Magnetica }

    [Header("Configuração Geral")]
    public TipoPlataforma tipoPlataforma;
    public MathEvaluator.Simbolo simbolo;
    public int valorDaPlataforma; 
    public int valorDoPlayer = 8; 

    [Header("Valor Dinâmico (Variável)")]
    [Tooltip("Marque para o valor da plataforma mudar sozinho com o tempo")]
    public bool valorMudaSozinho = false;
    [Tooltip("Quanto soma (ou subtrai se for número negativo) por segundo")]
    public int variacaoPorSegundo = 1;

    public bool limitarValor = false;
    public int limiteInferior = 0;
    public int limiteSuperior = 10;
    
    private float cronometro = 0f;
    private bool ottoEstaEmCima = false;

    [Header("Visual Geral")]
    public TextMeshPro textoExpressao; 
    public float transparenciaFantasma = 0.4f; 

    [Header("Apenas: Plataforma Normal")]
    public bool seDestroiComErro = true; 
    public float tempoAteDestruir = 0.5f; 

    [Header("Apenas: Plataforma Magnética")]
    public LayerMask playerLayer; 
    public float magnetForceMultiplier = 0.9f;

    private Collider2D coll;
    private SpriteRenderer sprite;
    private Animator anim; 
    
    private bool jaEstaQuebrando = false;
    private bool imaAtivo = false;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
        
        AtualizarPlataforma(false); 
    }

    void Update()
    {
        // Se a opção estiver ligada e a plataforma ainda estiver inteira, conta o tempo
        if (valorMudaSozinho && !jaEstaQuebrando)
        {
            cronometro += Time.deltaTime;
            
            // A cada 1 segundo exato:
            if (cronometro >= 1f)
            {
                cronometro = 0f;
                valorDaPlataforma += variacaoPorSegundo;
                
                if(limitarValor)
                {
                    if(valorDaPlataforma <= limiteInferior) variacaoPorSegundo *= -1;
                    if(valorDaPlataforma >= limiteSuperior) variacaoPorSegundo *= -1;
                }

                // Atualiza o texto e a física passando o estado real do Otto
                AtualizarPlataforma(ottoEstaEmCima);
            }
        }
    }

    public void AtualizarPlataforma(bool ottoPisou)
    {
        if (jaEstaQuebrando) return; 

        if (textoExpressao != null)
        {
            string stringSimbolo = "";
            if (simbolo == MathEvaluator.Simbolo.Maior) stringSimbolo = ">";
            else if (simbolo == MathEvaluator.Simbolo.Menor) stringSimbolo = "<";
            else if (simbolo == MathEvaluator.Simbolo.Igual) stringSimbolo = "=";
            
            textoExpressao.text = valorDaPlataforma.ToString();
        }

        bool expressaoCorreta = MathEvaluator.Validar(valorDoPlayer, simbolo, valorDaPlataforma);

        switch (tipoPlataforma)
        {
            case TipoPlataforma.Normal:
                ComportamentoNormal(expressaoCorreta, ottoPisou);
                break;
            case TipoPlataforma.Intangivel:
                ComportamentoIntangivel(expressaoCorreta, ottoPisou);
                break;
            case TipoPlataforma.Magnetica:
                ComportamentoMagnetico(expressaoCorreta);
                break;
        }
    }

    void ComportamentoNormal(bool correta, bool pisou)
    {
        if (correta)
        {
            coll.enabled = true; 
            if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 1f); 
        }
        else
        {
            if (seDestroiComErro)
            {
                if (pisou) StartCoroutine(RotinaDeDestruicao());
                else
                {
                    coll.enabled = true; 
                    if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 1f); 
                }
            }
            else
            {
                coll.enabled = false; 
                if (sprite != null) sprite.color = new Color(1f, 1f, 1f, transparenciaFantasma); 
            }
        }
    }

    void ComportamentoIntangivel(bool correta, bool pisou)
    {
        if (correta)
        {
            coll.enabled = true;
            if (sprite != null) sprite.color = new Color(1f, 1f, 1f, pisou ? 1f : transparenciaFantasma);
        }
        else
        {
            coll.enabled = false;
            if (sprite != null) sprite.color = new Color(1f, 1f, 1f, transparenciaFantasma);
        }
    }

    void ComportamentoMagnetico(bool correta)
    {
        coll.enabled = true;
        if (sprite != null) sprite.color = new Color(1f, 1f, 1f, 1f);
        
        imaAtivo = !correta;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            ottoEstaEmCima = true; // Avisa o sistema que o Otto subiu
            
            if (tipoPlataforma == TipoPlataforma.Normal || tipoPlataforma == TipoPlataforma.Intangivel)
            {
                coll.enabled = true; 
                AtualizarPlataforma(true); 
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            ottoEstaEmCima = false; // Avisa o sistema que o Otto saiu
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (tipoPlataforma == TipoPlataforma.Magnetica && imaAtivo)
        {
            if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                
                if (playerRb != null && playerRb.velocity.y > 0.1f)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * (1f - magnetForceMultiplier));
                }
            }
        }
    }

    IEnumerator RotinaDeDestruicao()
    {
        jaEstaQuebrando = true;
        if (anim != null) anim.SetTrigger("Quebrar");
        yield return null;
    }

    public void DesativarColisor()
    {
        coll.enabled = false; 
    }

    public void FinalizarDestruicao()
    {
        gameObject.SetActive(false); 
    }

    public void ReceberTiro(int danoDaBala)
    {
        if (jaEstaQuebrando) return;
        valorDaPlataforma += danoDaBala;
        AtualizarPlataforma(ottoEstaEmCima); 
    }
}