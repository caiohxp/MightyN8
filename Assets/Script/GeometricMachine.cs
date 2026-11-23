using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeometricMachine : MonoBehaviour
{
    public const int MaxFiguras = 16; // 4 formas x 4 cores

    [System.Serializable]
    public class FiguraDaMaquina
    {
        [Tooltip("Só para organizar no Inspector (ex: Círculo Vermelho). Aparece como título do elemento.")]
        public string nome;

        [Tooltip("Prefab da figura que a máquina atira (precisa de GeometricTarget, Rigidbody2D e Collider2D)")]
        public GameObject prefab;

        [Tooltip("Imagem que aparece no outdoor ANTES de acertar (silhueta ou texto)")]
        public Sprite silhueta;

        [Tooltip("Imagem da figura que substitui a silhueta quando o Otto acerta")]
        public Sprite formaRevelada;
    }

    [Header("Figuras desta máquina (máximo 16)")]
    [Tooltip("Coloque só as figuras que ESTA máquina vai soltar. O ID de cada figura é a posição dela nesta lista (Element 0 = ID 0, Element 1 = ID 1...).")]
    public List<FiguraDaMaquina> figuras = new List<FiguraDaMaquina>();

    [Header("Configuração de Tiro")]
    public Transform pontoDeTiro;
    public float forcaDoTiro = 5f;
    [Range(0f, 1f)]
    [Tooltip("Chance de, em cada tiro, sair a figura que o outdoor está pedindo agora. 0 = totalmente aleatório. Ajuda quando a máquina tem muitas figuras.")]
    public float chanceDaFiguraEsperada = 0f;

    [Header("Máquina (Animação)")]
    [Tooltip("Se vazio, pega o Animator do próprio objeto. O Animation Event 'AtirarFormaAleatoria' precisa estar na animação da máquina.")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    [Tooltip("Opcional: sprite da máquina depois que o puzzle for resolvido")]
    public Sprite spriteDesativada;

    [Header("A Sequência do Outdoor")]
    [Tooltip("IDs (posição na lista 'Figuras') na ordem que o Otto precisa acertar. Ex: 3, 0, 3, 1")]
    public List<int> sequenciaCorreta;

    [Tooltip("Um SpriteRenderer por posição da sequência, na mesma ordem (slot 0 = primeira figura). Têm que ser objetos da cena, não prefabs.")]
    public List<SpriteRenderer> slotsDoOutdoor;

    [Header("Quando o puzzle for resolvido")]
    [Tooltip("Ligue aqui a abertura do portão, por exemplo")]
    public UnityEvent aoConcluir;

    private int indiceAtualDaSequencia = 0;
    private bool puzzleConcluido = false;
    private bool configuracaoValida = false;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        configuracaoValida = ValidarConfiguracao();
        if (configuracaoValida) MostrarSilhuetasNoOutdoor();
    }

    bool ValidarConfiguracao()
    {
        if (figuras == null || figuras.Count == 0)
        {
            Debug.LogError("GeometricMachine: a lista 'figuras' está vazia. Adicione as figuras que esta máquina vai soltar.", this);
            return false;
        }

        if (figuras.Count > MaxFiguras)
        {
            Debug.LogError("GeometricMachine: no máximo " + MaxFiguras + " figuras por máquina (há " + figuras.Count + ").", this);
            return false;
        }

        if (sequenciaCorreta == null || sequenciaCorreta.Count == 0)
        {
            Debug.LogError("GeometricMachine: 'sequenciaCorreta' está vazia. Preencha no Inspector.", this);
            return false;
        }

        foreach (int id in sequenciaCorreta)
        {
            if (id < 0 || id >= figuras.Count)
            {
                Debug.LogError("GeometricMachine: a sequência tem o ID " + id + ", mas a lista 'figuras' só vai de 0 a " + (figuras.Count - 1) + ".", this);
                return false;
            }
        }

        for (int i = 0; i < figuras.Count; i++)
        {
            if (figuras[i].prefab == null)
            {
                Debug.LogError("GeometricMachine: a figura " + i + " (" + figuras[i].nome + ") está sem prefab.", this);
                return false;
            }

            if (figuras[i].prefab.GetComponent<GeometricTarget>() == null)
            {
                Debug.LogError("GeometricMachine: o prefab da figura " + i + " (" + figuras[i].nome + ") não tem o script GeometricTarget.", this);
                return false;
            }
        }

        if (slotsDoOutdoor == null || slotsDoOutdoor.Count < sequenciaCorreta.Count)
        {
            Debug.LogWarning("GeometricMachine: há menos slots no outdoor do que figuras na sequência.", this);
        }

        // Slots precisam ser objetos da cena (Hierarchy). Se for um prefab do Project,
        // o script trocaria o sprite do próprio prefab.
        if (slotsDoOutdoor != null)
        {
            for (int i = 0; i < slotsDoOutdoor.Count; i++)
            {
                SpriteRenderer slot = slotsDoOutdoor[i];
                if (slot != null && !slot.gameObject.scene.IsValid())
                {
                    Debug.LogError("GeometricMachine: o slot " + i + " do outdoor aponta para um PREFAB do Project (" + slot.name + "). Arraste um objeto da Hierarchy.", this);
                    return false;
                }
            }
        }

        return true;
    }

    void MostrarSilhuetasNoOutdoor()
    {
        for (int i = 0; i < sequenciaCorreta.Count; i++)
        {
            if (i >= slotsDoOutdoor.Count || slotsDoOutdoor[i] == null) continue;
            slotsDoOutdoor[i].sprite = figuras[sequenciaCorreta[i]].silhueta;
        }
    }

    // ==========================================
    // ANIMATION EVENT
    // Coloque "AtirarFormaAleatoria" no frame da animação da máquina em que ela dispara.
    // Cada vez que a animação toca, uma figura sai.
    // ==========================================
    public void AtirarFormaAleatoria()
    {
        if (puzzleConcluido || !configuracaoValida) return;
        if (pontoDeTiro == null) return;

        int idSorteado = SortearId();
        GameObject novaForma = Instantiate(figuras[idSorteado].prefab, pontoDeTiro.position, Quaternion.identity);

        // A máquina define o ID, então não há como o ID do prefab destoar da lista
        GeometricTarget scriptForma = novaForma.GetComponent<GeometricTarget>();
        if (scriptForma != null)
        {
            scriptForma.maquinaPai = this;
            scriptForma.idDaForma = idSorteado;
        }

        Rigidbody2D rb = novaForma.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(-forcaDoTiro, rb.velocity.y);
        }
    }

    int SortearId()
    {
        if (chanceDaFiguraEsperada > 0f && Random.value < chanceDaFiguraEsperada)
        {
            return sequenciaCorreta[indiceAtualDaSequencia];
        }

        return Random.Range(0, figuras.Count);
    }

    // Chamada pela figura quando o Otto soca ela
    public void RegistrarAcerto(int idDaFormaAcertada, PlayerHealth jogador, float posicaoXDaForma)
    {
        if (puzzleConcluido || !configuracaoValida) return;

        if (idDaFormaAcertada == sequenciaCorreta[indiceAtualDaSequencia])
        {
            RevelarSlot(indiceAtualDaSequencia);
            indiceAtualDaSequencia++;

            if (indiceAtualDaSequencia >= sequenciaCorreta.Count)
            {
                PuzzleResolvido();
            }
        }
        else
        {
            // Errou: o progresso é mantido, mas o Otto perde 1 de vida
            if (jogador != null) jogador.TakeDamage(posicaoXDaForma, true);
        }
    }

    // Troca a silhueta/texto pela imagem da figura
    void RevelarSlot(int indice)
    {
        if (indice >= slotsDoOutdoor.Count || slotsDoOutdoor[indice] == null) return;
        slotsDoOutdoor[indice].sprite = figuras[sequenciaCorreta[indice]].formaRevelada;
    }

    void PuzzleResolvido()
    {
        puzzleConcluido = true;

        // Para a animação (e com ela, os tiros)
        if (animator != null) animator.enabled = false;
        if (spriteRenderer != null && spriteDesativada != null) spriteRenderer.sprite = spriteDesativada;

        aoConcluir?.Invoke();
    }
}