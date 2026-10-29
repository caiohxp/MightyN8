using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConditionalPlatform : MonoBehaviour
{
    public enum TipoPlataforma { Normal, Intangivel, Magnetica }

    [Header("Configuração Geral")]
    public TipoPlataforma tipoPlataforma;
    public MathEvaluator.Simbolo simbolo;
    public int valorDaPlataforma;

    [Header("Valor Dinâmico (Variável)")]
    public bool valorMudaSozinho = false;
    public int variacaoPorSegundo = 1;
    public bool limitarValor = false;
    public int limiteInferior = 0;
    public int limiteSuperior = 10;
    private float cronometro = 0f;

    [Header("Visual Geral")]
    public TextMeshPro textoExpressao;
    public float transparenciaFantasma = 0.4f;

    [Header("Apenas: Plataforma Normal")]
    public bool seDestroiComAcerto = true;
    public float tempoAteDestruir = 0.5f;

    [Header("Apenas: Plataforma Magnética")]
    public LayerMask playerLayer;
    public float magnetForceMultiplier = 0.9f;

    private Collider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool jaEstaQuebrando = false;
    private int valorDoPlayer = 8; // O valor fixo do Oito

    void Start()
    {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        AtualizarVisuais();
    }

    void Update()
    {
        if (jaEstaQuebrando) return;

        // A MÁGICA ACONTECE AQUI: A plataforma radariza a área e julga objetos de forma independente!
        if (tipoPlataforma == TipoPlataforma.Intangivel)
        {
            EscanearObjetosParaIntangibilidade();
        }

        if (valorMudaSozinho)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= 1f)
            {
                cronometro = 0f;
                valorDaPlataforma += variacaoPorSegundo;

                if (limitarValor)
                {
                    if (valorDaPlataforma <= limiteInferior) variacaoPorSegundo *= -1;
                    if (valorDaPlataforma >= limiteSuperior) variacaoPorSegundo *= -1;
                }
                AtualizarVisuais();
            }
        }
    }

    void AtualizarVisuais()
    {
        if (textoExpressao != null)
        {
            string stringSimbolo = "";
            if (simbolo == MathEvaluator.Simbolo.Maior) stringSimbolo = ">";
            else if (simbolo == MathEvaluator.Simbolo.Menor) stringSimbolo = "<";
            else if (simbolo == MathEvaluator.Simbolo.Igual) stringSimbolo = "=";

            textoExpressao.text = valorDaPlataforma.ToString();
        }
    }

    // --- O SISTEMA MULTI-OBJETOS ---
    void EscanearObjetosParaIntangibilidade()
    {
        // Cria uma caixa de detecção levemente maior que a plataforma (para detectar frações de segundo antes de tocar)
        Vector2 tamanhoBusca = coll.bounds.size;
        tamanhoBusca.y += 0.5f; 
        tamanhoBusca.x += 0.2f;

        Collider2D[] objetosProximos = Physics2D.OverlapBoxAll(coll.bounds.center, tamanhoBusca, 0f);
        bool temAlguemPisandoCerto = false;

        foreach (Collider2D hit in objetosProximos)
        {
            bool isPlayer = hit.gameObject.layer == 9 || hit.gameObject.CompareTag("Player");
            MathBlock caixa = hit.gameObject.GetComponentInParent<MathBlock>();

            if (isPlayer || caixa != null)
            {
                int valorDoObjeto = isPlayer ? valorDoPlayer : caixa.valorDoBloco;
                bool expressaoCorreta = MathEvaluator.Validar(valorDoObjeto, simbolo, valorDaPlataforma);

                // Força a Unity a ignorar colisões APENAS com quem está matematicamente incorreto
                Physics2D.IgnoreCollision(coll, hit, !expressaoCorreta);

                // Se houver pelo menos um objeto com a conta certa perto, a plataforma fica visível (sólida)
                if (expressaoCorreta) temAlguemPisandoCerto = true;
            }
        }

        // Controle Visual: Se a caixa certa estiver nela, a plataforma fica opaca. Se só o Otto (errado) estiver, fica fantasma.
        if (sprite != null)
        {
            sprite.color = temAlguemPisandoCerto ? Color.white : new Color(1f, 1f, 1f, transparenciaFantasma);
        }
    }

    // --- LÓGICA DA PLATAFORMA NORMAL (Que afeta todos de uma vez) ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (jaEstaQuebrando || tipoPlataforma != TipoPlataforma.Normal) return;

        ContactPoint2D contato = collision.GetContact(0);
        bool pisouPorCima = contato.normal.y < -0.5f;

        if (pisouPorCima)
        {
            bool isPlayer = collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player");
            MathBlock caixa = collision.gameObject.GetComponentInParent<MathBlock>();

            if (isPlayer || caixa != null)
            {
                int valorDoObjeto = isPlayer ? valorDoPlayer : caixa.valorDoBloco;
                bool expressaoCorreta = MathEvaluator.Validar(valorDoObjeto, simbolo, valorDaPlataforma);

                if (expressaoCorreta)
                {
                    if (sprite != null) sprite.color = Color.white;
                    if (seDestroiComAcerto) StartCoroutine(RotinaDeDestruicao());
                }
                else
                {
                    // A plataforma normal ainda pune o erro quebrando ou sumindo globalmente
                    coll.enabled = false;
                    if (sprite != null) sprite.color = new Color(1f, 1f, 1f, transparenciaFantasma);
                }
            }
        }
    }

    // --- LÓGICA DO ÍMÃ MAGNÉTICO ---
    void OnCollisionStay2D(Collision2D collision)
    {
        if (tipoPlataforma == TipoPlataforma.Magnetica)
        {
            bool isPlayer = collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player");
            if (isPlayer)
            {
                bool expressaoCorreta = MathEvaluator.Validar(valorDoPlayer, simbolo, valorDaPlataforma);
                if (!expressaoCorreta)
                {
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null && playerRb.velocity.y > 0.1f)
                    {
                        // Anula o pulo
                        playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * (1f - magnetForceMultiplier));
                    }
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
        AtualizarVisuais();
    }
}