using UnityEngine;

public class GeometricTarget : MonoBehaviour
{
    [Header("Identificação")]
    [Tooltip("Define qual é esta forma (0 = Círculo, 1 = Quadrado, 2 = Triângulo)")]
    public int idDaForma;

    [Header("Comportamento Base")]
    public float tempoDeVida = 6f; // Destrói se o Otto ignorar

    // Máquina que atirou esta forma
    [HideInInspector] public GeometricMachine maquinaPai;

    private bool foiSocada = false;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    // Chamada pelo PlayerCombat quando o soco acerta a forma
    public void ReceberSoco(PlayerHealth jogador)
    {
        if (foiSocada) return; // evita contar o mesmo soco duas vezes
        foiSocada = true;

        if (maquinaPai != null)
        {
            maquinaPai.RegistrarAcerto(idDaForma, jogador, transform.position.x);
        }

        // Efeito visual de quebra aqui

        Destroy(gameObject);
    }
}