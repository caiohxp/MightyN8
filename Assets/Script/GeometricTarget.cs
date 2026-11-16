using UnityEngine;

public class GeometricTarget : MonoBehaviour
{
    [Header("Identificação")]
    [Tooltip("Define qual é esta forma (ex: 0 = Círculo, 1 = Quadrado, 2 = Triângulo)")]
    public int idDaForma;
    
    [Header("Comportamento Base")]
    public float tempoDeVida = 6f; // Destrói se o Otto ignorar

    // Referência ao mestre que atirou para reportar quando levar o soco
    [HideInInspector] public GeometricMachine maquinaPai;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    // Usaremos a função pública para quando o Otto der o "Soco Físico"
    public void ReceberSoco()
    {
        // Se a máquina que atirou ainda existir, avisa que esta forma levou o soco
        if (maquinaPai != null)
        {
            maquinaPai.RegistrarAcerto(idDaForma);
        }
        
        // Efeito visual de quebra aqui
        
        Destroy(gameObject); // Some da tela ao levar soco
    }
}