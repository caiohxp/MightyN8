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

        // 1. O SEGREDO PARA O TEXTO NÃO FICAR ESPELHADO
        // Força a rotação da UI a ficar sempre travada de frente para a câmera (0,0,0)
        leftCounterTransform.rotation = Quaternion.identity;
        rightCounterTransform.rotation = Quaternion.identity;

        // 2. O AJUSTE DE OFFSETS
        // Verifica se o corpo do inimigo girou para a esquerda
        bool viradoParaEsquerda = Mathf.Abs(enemyLogic.transform.eulerAngles.y - 180f) < 10f;
        
        // Aplica o ajuste de distância (aqueles 0.3f do seu Shooter antigo)
        float ajusteX = viradoParaEsquerda ? -0.3f : 0f;

        // Posiciona os textos mantendo a distância do offset configurado
        leftCounterTransform.position = enemyLogic.transform.position + new Vector3(offsetLeft.x + ajusteX, offsetLeft.y, 0);
        rightCounterTransform.position = enemyLogic.transform.position + new Vector3(offsetRight.x + ajusteX, offsetRight.y, 0);

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