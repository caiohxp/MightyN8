using UnityEngine;
using Cinemachine;

public class PortaInicialSala : MonoBehaviour
{
    [Header("Sprites da Porta")]
    public Sprite portaFechada;
    public Sprite portaAberta;

    [Header("UI de Interação")]
    [Tooltip("Coloque aqui o GameObject do botão [ E ] que aparece acima da porta")]
    public GameObject avisoInteracao;

    [Header("Controle de Cenário (Canvas/Imagens)")]
    [Tooltip("A imagem preta que esconde o corredor")]
    public GameObject imagemPretaCorredor;
    [Tooltip("A imagem/visual da sala de aula que deve ser ativada")]
    public GameObject imagemSala;

    [Header("Câmeras (Cinemachine)")]
    [Tooltip("A câmera que fica parada mostrando a sala inteira")]
    public CinemachineVirtualCamera cameraFixaSala;
    [Tooltip("A câmera que segue o Otto normalmente pelo jogo")]
    public CinemachineVirtualCamera cameraFollowOtto;

    private SpriteRenderer spriteRenderer;
    private bool jogadorEstaPerto = false;
    private bool portaJaEstaAberta = false;
    private Collider2D colisorFisico; // Variável para o Collider bloqueador

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = portaFechada; 
        
        // Pega o BoxCollider2D da porta (que deve ter o Is Trigger DESMARCADO)
        colisorFisico = GetComponent<Collider2D>();
        
        if (avisoInteracao != null) avisoInteracao.SetActive(false);
        
        if (imagemPretaCorredor != null) imagemPretaCorredor.SetActive(true); 
        if (imagemSala != null) imagemSala.SetActive(false); 
        
        if (cameraFixaSala != null) cameraFixaSala.Priority = 11;
        if (cameraFollowOtto != null) cameraFollowOtto.Priority = 10;
    }

    void Update()
    {
        // Se o jogador esbarrar na parede da porta, ela estiver fechada e apertar E
        if (jogadorEstaPerto && !portaJaEstaAberta && Input.GetKeyDown(KeyCode.E))
        {
            AbrirERevelarCenario();
        }
    }

    void AbrirERevelarCenario()
    {
        portaJaEstaAberta = true;
        
        spriteRenderer.sprite = portaAberta;
        
        // Desliga a barreira física para o Otto passar!
        if (colisorFisico != null) colisorFisico.enabled = false;
        
        if (avisoInteracao != null) avisoInteracao.SetActive(false);

        if (imagemPretaCorredor != null) imagemPretaCorredor.SetActive(false);
        if (imagemSala != null) imagemSala.SetActive(true);

        if (cameraFixaSala != null) cameraFixaSala.Priority = 9;
        if (cameraFollowOtto != null) cameraFollowOtto.Priority = 11;
    }

    // --- DETECÇÃO DO JOGADOR (OnCollision em vez de OnTrigger) ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            if (!portaJaEstaAberta)
            {
                jogadorEstaPerto = true;
                if (avisoInteracao != null) avisoInteracao.SetActive(true);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            jogadorEstaPerto = false;
            if (avisoInteracao != null) avisoInteracao.SetActive(false);
        }
    }
}