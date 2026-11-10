using UnityEngine;
using UnityEngine.SceneManagement;

public class PortaInterativa : MonoBehaviour
{
    [Header("Sprites da Porta")]
    public Sprite portaFechada;
    public Sprite portaAberta;

    [Header("Configuração de Cenário")]
    [Tooltip("Nome da Cena do Hub (Pátio da Escola) para onde a porta leva")]
    public string nomeDaCenaHub = "Hub_Escola"; 

    [Header("UI de Interação")]
    [Tooltip("Coloque aqui o GameObject do botão [ E ] que aparece acima da porta")]
    public GameObject avisoInteracao;

    private SpriteRenderer spriteRenderer;
    private bool jogadorEstaPerto = false;
    private bool portaJaEstaAberta = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = portaFechada; // Garante que comece fechada
        
        if (avisoInteracao != null)
            avisoInteracao.SetActive(false); // Garante que o aviso comece invisível
    }

    void Update()
    {
        // Se o jogador estiver perto, a porta estiver fechada, e ele apertar a tecla E
        if (jogadorEstaPerto && !portaJaEstaAberta && Input.GetKeyDown(KeyCode.E))
        {
            AbrirEAtravessar();
        }
    }

    void AbrirEAtravessar()
    {
        portaJaEstaAberta = true;
        
        // 1. Troca o visual para porta aberta
        spriteRenderer.sprite = portaAberta;
        
        // 2. Esconde o aviso [ E ]
        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);
            
        // 3. Carrega a próxima cena (O Hub World) 
        // Opcional: Você pode colocar um Invoke("TrocarDeCena", 0.5f) para dar tempo de ver a porta abrir!
        Invoke("TrocarDeCena", 0.3f); 
    }

    void TrocarDeCena()
    {
        SceneManager.LoadScene(nomeDaCenaHub);
    }

    // --- DETECÇÃO DO JOGADOR (BOX COLLIDER 2D - IS TRIGGER) ---
    void OnTriggerEnter2D(Collider2D collision)
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

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            jogadorEstaPerto = false;
            if (avisoInteracao != null) avisoInteracao.SetActive(false);
        }
    }
}