using UnityEngine;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("Cérebro do Inimigo")]
    public Enemy enemyLogic;

    [Header("Textos UI")]
    public TextMeshProUGUI leftCounterText;
    public TextMeshProUGUI rightCounterText;

    [Header("Posicionamento Físico")]
    public Transform leftCounterTransform;
    public Transform rightCounterTransform;
    public Vector3 offsetLeft;
    public Vector3 offsetRight;

    // Cache de performance para evitar ToString() desnecessário
    private int lastLeftValue = -999;
    private int lastRightValue = -999;

    void Start()
    {
        if (enemyLogic == null)
            enemyLogic = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        if (enemyLogic == null) return;

        // 1. O SEGREDO ATUALIZADO: Trava o Y para não espelhar, mas COPIA o Z para deitar com o Shooter
        float anguloZ = enemyLogic.transform.eulerAngles.z;
        leftCounterTransform.rotation = Quaternion.Euler(0f, 0f, anguloZ);
        rightCounterTransform.rotation = Quaternion.Euler(0f, 0f, anguloZ);

        // 2. O AJUSTE DE OFFSETS E ÓRBITA
        bool viradoParaEsquerda = Mathf.Abs(enemyLogic.transform.eulerAngles.y - 180f) < 10f;
        float ajusteX = viradoParaEsquerda ? -0.3f : 0f;

        Vector3 offsetEsquerdoFinal = new Vector3(offsetLeft.x + ajusteX, offsetLeft.y, 0);
        Vector3 offsetDireitoFinal = new Vector3(offsetRight.x + ajusteX, offsetRight.y, 0);

        // Multiplicar pela rotação faz os textos acompanharem o giro do corpo perfeitamente
        leftCounterTransform.position = enemyLogic.transform.position + (enemyLogic.transform.rotation * offsetEsquerdoFinal);
        rightCounterTransform.position = enemyLogic.transform.position + (enemyLogic.transform.rotation * offsetDireitoFinal);

        // 3. ATUALIZAÇÃO OTIMIZADA DOS NÚMEROS
        if (enemyLogic.valueLeft != lastLeftValue)
        {
            lastLeftValue = enemyLogic.valueLeft;
            leftCounterText.text = lastLeftValue.ToString();
        }

        if (enemyLogic.valueRight != lastRightValue)
        {
            lastRightValue = enemyLogic.valueRight;
            rightCounterText.text = lastRightValue.ToString();
        }
    }
}