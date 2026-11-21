using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeometricMachine : MonoBehaviour
{
    [System.Serializable]
    public class VisualDaForma
    {
        public string nome; // só para organizar no Inspector (ex: Círculo)

        [Tooltip("Imagem que aparece no outdoor ANTES de acertar (silhueta ou texto)")]
        public Sprite silhueta;

        [Tooltip("Imagem da forma que substitui a silhueta quando o Otto acerta")]
        public Sprite formaRevelada;
    }

    [Header("As 3 Formas (Prefabs)")]
    public GameObject prefabCirculo;   // ID 0
    public GameObject prefabQuadrado;  // ID 1
    public GameObject prefabTriangulo; // ID 2

    [Header("Configuração de Tiro")]
    public Transform pontoDeTiro;
    public float forcaDoTiro = 5f;

    [Header("Máquina (Animação)")]
    [Tooltip("Se vazio, pega o Animator do próprio objeto. O Animation Event 'AtirarFormaAleatoria' precisa estar na animação da máquina.")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    [Tooltip("Opcional: sprite da máquina depois que o puzzle for resolvido")]
    public Sprite spriteDesativada;

    [Header("A Sequência do Outdoor")]
    [Tooltip("IDs na ordem que o Otto precisa acertar (ex: 0, 2, 1 = Círculo, Triângulo, Quadrado)")]
    public List<int> sequenciaCorreta;

    [Tooltip("Um SpriteRenderer por posição da sequência, na mesma ordem (slot 0 = primeira forma)")]
    public List<SpriteRenderer> slotsDoOutdoor;

    [Tooltip("Índice = ID da forma. Elemento 0 = Círculo, 1 = Quadrado, 2 = Triângulo")]
    public List<VisualDaForma> visuais;

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
        if (sequenciaCorreta == null || sequenciaCorreta.Count == 0)
        {
            Debug.LogError("GeometricMachine: 'sequenciaCorreta' está vazia. Preencha no Inspector.", this);
            return false;
        }

        if (visuais == null || visuais.Count < 3)
        {
            Debug.LogError("GeometricMachine: preencha 'visuais' com 3 elementos (0 = Círculo, 1 = Quadrado, 2 = Triângulo).", this);
            return false;
        }

        foreach (int id in sequenciaCorreta)
        {
            if (id < 0 || id >= visuais.Count)
            {
                Debug.LogError("GeometricMachine: a sequência tem um ID inválido: " + id, this);
                return false;
            }
        }

        if (slotsDoOutdoor == null || slotsDoOutdoor.Count < sequenciaCorreta.Count)
        {
            Debug.LogWarning("GeometricMachine: há menos slots no outdoor do que formas na sequência.", this);
        }

        return true;
    }

    void MostrarSilhuetasNoOutdoor()
    {
        for (int i = 0; i < sequenciaCorreta.Count; i++)
        {
            if (i >= slotsDoOutdoor.Count || slotsDoOutdoor[i] == null) continue;
            slotsDoOutdoor[i].sprite = visuais[sequenciaCorreta[i]].silhueta;
        }
    }

    // ==========================================
    // ANIMATION EVENT
    // Coloque "AtirarFormaAleatoria" no frame da animação da máquina em que ela dispara.
    // Cada vez que a animação toca, uma forma sai.
    // ==========================================
    public void AtirarFormaAleatoria()
    {
        if (puzzleConcluido || !configuracaoValida) return;

        int idSorteado = Random.Range(0, 3);
        GameObject prefabEscolhido = null;

        if (idSorteado == 0) prefabEscolhido = prefabCirculo;
        else if (idSorteado == 1) prefabEscolhido = prefabQuadrado;
        else if (idSorteado == 2) prefabEscolhido = prefabTriangulo;

        if (prefabEscolhido == null || pontoDeTiro == null) return;

        GameObject novaForma = Instantiate(prefabEscolhido, pontoDeTiro.position, Quaternion.identity);

        GeometricTarget scriptForma = novaForma.GetComponent<GeometricTarget>();
        if (scriptForma != null) scriptForma.maquinaPai = this;

        Rigidbody2D rb = novaForma.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(-forcaDoTiro, rb.linearVelocity.y);
        }
    }

    // Chamada pela forma quando o Otto soca ela
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

    // Troca a silhueta/texto pela imagem da forma
    void RevelarSlot(int indice)
    {
        if (indice >= slotsDoOutdoor.Count || slotsDoOutdoor[indice] == null) return;
        slotsDoOutdoor[indice].sprite = visuais[sequenciaCorreta[indice]].formaRevelada;
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