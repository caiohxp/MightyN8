using UnityEngine;

public class FloatingBlock : MonoBehaviour
{
    [Header("Lógica do Bloco")]
    public bool correctAnswer = true;
    private bool jaFoiAberto = false;

    [Header("O que tem dentro?")]
    public GameObject keyPrefab;
    public Transform pontoDeSpawn; 
    
    [Tooltip("Arraste O PORTÃO ESPECÍFICO da cena que esta chave vai abrir")]
    public Gate portaoQueVaiAbrir; 

    [Header("Visual")]
    public Sprite blocoAbertoSprite;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (jaFoiAberto) return;

        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contato = collision.GetContact(0);
            bool bateuPorBaixo = contato.normal.y > 0.5f;

            if (bateuPorBaixo)
            {
                if (correctAnswer)
                {
                    AbrirBloco();
                }
            }
        }
    }

    void AbrirBloco()
    {
        jaFoiAberto = true;
        
        if (blocoAbertoSprite != null) spriteRenderer.sprite = blocoAbertoSprite;

        if (keyPrefab != null && pontoDeSpawn != null)
        {
            // Cria a chave no mundo e guarda ela numa variável temporária
            GameObject novaChave = Instantiate(keyPrefab, pontoDeSpawn.position, Quaternion.identity);
            
            // Puxa o script da chave recém-criada e ensina qual portão ela deve abrir
            LevelKey scriptDaChave = novaChave.GetComponent<LevelKey>();
            if (scriptDaChave != null)
            {
                scriptDaChave.portaoAlvo = this.portaoQueVaiAbrir;
            }
        }
    }
}