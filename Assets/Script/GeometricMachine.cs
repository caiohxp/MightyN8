using System.Collections.Generic;
using UnityEngine;

public class GeometricMachine : MonoBehaviour
{
    [Header("As 3 Formas (Prefabs)")]
    public GameObject prefabCirculo; // ID 0
    public GameObject prefabQuadrado; // ID 1
    public GameObject prefabTriangulo; // ID 2

    [Header("Configuração de Tiro")]
    public Transform pontoDeTiro;
    public float forcaDoTiro = 5f;
    public float tempoEntreTiros = 1.5f;
    
    [Header("A Sequência do Outdoor")]
    [Tooltip("Coloque aqui os IDs na ordem que o outdoor mostra (ex: 0, 2, 1 para Círculo, Triângulo, Quadrado)")]
    public List<int> sequenciaCorreta; 

    // O progresso atual do Otto
    private int indiceAtualDaSequencia = 0;
    
    private float cronometroDeTiro;
    private bool puzzleConcluido = false;

    void Update()
    {
        if (puzzleConcluido) return;

        cronometroDeTiro += Time.deltaTime;
        if (cronometroDeTiro >= tempoEntreTiros)
        {
            cronometroDeTiro = 0f;
            AtirarFormaAleatoria();
        }
    }

    void AtirarFormaAleatoria()
    {
        // Escolhe uma forma aleatoriamente
        int idSorteado = Random.Range(0, 3);
        GameObject prefabEscolhido = null;

        if (idSorteado == 0) prefabEscolhido = prefabCirculo;
        else if (idSorteado == 1) prefabEscolhido = prefabQuadrado;
        else if (idSorteado == 2) prefabEscolhido = prefabTriangulo;

        // Instancia a forma e atira
        if (prefabEscolhido != null)
        {
            GameObject novaForma = Instantiate(prefabEscolhido, pontoDeTiro.position, Quaternion.identity);
            
            // Avisa a forma quem é o mestre dela
            GeometricTarget scriptForma = novaForma.GetComponent<GeometricTarget>();
            if (scriptForma != null) scriptForma.maquinaPai = this;

            // Joga para a frente da máquina (esquerda)
            Rigidbody2D rb = novaForma.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(-forcaDoTiro, rb.velocity.y);
            }
        }
    }

    // Esta função é chamada pelas formas quando o Otto soca elas
    public void RegistrarAcerto(int idDaFormaAcertada)
    {
        if (puzzleConcluido) return;

        // Verifica se a forma que ele socou bate com o que ele precisava bater na sequência
        if (idDaFormaAcertada == sequenciaCorreta[indiceAtualDaSequencia])
        {
            Debug.Log("Socou a forma CERTA!");
            indiceAtualDaSequencia++; // Avança na sequência do Outdoor

            // Se ele completou todas da sequência
            if (indiceAtualDaSequencia >= sequenciaCorreta.Count)
            {
                PuzzleResolvido();
            }
        }
        else
        {
            Debug.Log("Socou a ERRADA! Sequência zerada.");
            // Punição: Se socar a errada, o progresso zera
            indiceAtualDaSequencia = 0;
            // Opcional: Tocar um som de erro
        }
    }

    void PuzzleResolvido()
    {
        puzzleConcluido = true;
        Debug.Log("PUZZLE DA MÁQUINA CONCLUÍDO!");
        
        // O que acontece agora?
        // 1. A máquina para de atirar (porque puzzleConcluido = true)
        // 2. Você pode abrir um portão:
        // portao.AbrirPortao();
        // 3. Destruir a máquina ou mudar o sprite dela para "Quebrada/Desativada".
    }
}