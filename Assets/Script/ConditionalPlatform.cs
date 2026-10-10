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

    // VARIÁVEL PRIVADA: O valor base é 8, mas muda dinamicamente se uma pedra/caixa pisar aqui
    private int valorObjetoEmCima = 8; 

    [Header("Valor Dinâmico (Variável)")]
    public bool valorMudaSozinho = false;
    public int variacaoPorSegundo = 1;
    public bool limitarValor = false;
    public int limiteInferior = 0;
    public int limiteSuperior = 10;
    private float cronometro = 0f;
    private bool objetoEstaEmCima = false; // Renomeado para abranger tanto o Otto quanto Caixas

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
        if (valorMudaSozinho && !jaEstaQuebrando)
        {
            cronometro += Time.deltaTime;
            
            if (cronometro >= 1f)
            {
                cronometro = 0f;
                valorDaPlataforma += variacaoPorSegundo;

                if(limitarValor)
                {
                    if(valorDaPlataforma <= limiteInferior) variacaoPorSegundo *= -1;
                    if(valorDaPlataforma >= limiteSuperior) variacaoPorSegundo *= -1;
                }

                AtualizarPlataforma(objetoEstaEmCima);
            }
        }
    }

    public void AtualizarPlataforma(bool temObjetoPisando)
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

        // A MÁGICA ACONTECE AQUI: A plataforma agora julga o "valorObjetoEmCima" em vez de um número fixo
        bool expressaoCorreta = MathEvaluator.Validar(valorObjetoEmCima, simbolo, valorDaPlataforma);

        switch (tipoPlataforma)
        {
            case TipoPlataforma.Normal:
                ComportamentoNormal(expressaoCorreta, temObjetoPisando);
                break;
            case TipoPlataforma.Intangivel:
                ComportamentoIntangivel(expressaoCorreta, temObjetoPisando);
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
        ContactPoint2D contato = collision.GetContact(0);
        bool pisouPorCima = contato.normal.y < -0.5f;

        if (pisouPorCima)
        {
            // 1. É o Otto pisando?
            if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
            {
                objetoEstaEmCima = true; 
                valorObjetoEmCima = 8; // Força a identidade do Otto
                
                if (tipoPlataforma == TipoPlataforma.Normal || tipoPlataforma == TipoPlataforma.Intangivel)
                {
                    coll.enabled = true; 
                    AtualizarPlataforma(true); 
                }
            }
            // 2. É uma Pedra/Caixa pisando? (PREPARADO PARA O SEU NOVO SISTEMA)
            else 
            {
                MathBlock caixaMatematica = collision.gameObject.GetComponent<MathBlock>();
                if (caixaMatematica != null)
                {
                    objetoEstaEmCima = true;
                    // A plataforma assume o valor do bloco de pedra que caiu nela!
                    valorObjetoEmCima = caixaMatematica.valorDoBloco; 
                    
                    if (tipoPlataforma == TipoPlataforma.Normal || tipoPlataforma == TipoPlataforma.Intangivel)
                    {
                        coll.enabled = true; 
                        AtualizarPlataforma(true); 
                    }
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Se o Otto ou uma Caixa saiu de cima da plataforma
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<MathBlock>() != null)
        {
            objetoEstaEmCima = false; 
            valorObjetoEmCima = 8; // Retorna o valor padrão para a plataforma voltar ao estado normal de "espera"
            AtualizarPlataforma(false);
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
        AtualizarPlataforma(objetoEstaEmCima); 
    }
}