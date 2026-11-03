using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private float startTime;
    public float totalTime = 120f;

    [Header("UI Components")]
    public Transform healthTransform;
    private TextMeshProUGUI textHealth;
    public Transform scoreTransform;
    private TextMeshProUGUI textScore;
    public Transform timerTransform;
    private TextMeshProUGUI textTimer;

    [Header("Dynamic Arsenal")]
    public Transform municaoTransform;
    private TextMeshProUGUI textAmmo;

    [Header("Weapon Wheel Core")]
    public Transform centralEffectTransform;
    private TextMeshProUGUI textCentralEffect;

    [Header("Selection Wheel (Locked Bases)")]
    public GameObject blockSum;
    public GameObject blockSub;
    public GameObject blockMulti;
    public GameObject blockDiv;

    [Header("Selection Wheel (Active/Glowing Images)")]
    public GameObject activeSum;
    public GameObject activeSub;
    public GameObject activeMulti;
    public GameObject activeDiv;

    [Header("Selection Wheel (Invisible Buttons)")]
    public GameObject btnSum;
    public GameObject btnSub;
    public GameObject btnMulti;
    public GameObject btnDiv;

    void Start()
    {
        if (healthTransform)
            textHealth = healthTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (scoreTransform)
            textScore = scoreTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (timerTransform)
            textTimer = timerTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (municaoTransform)
            textAmmo = municaoTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (centralEffectTransform)
            textCentralEffect = centralEffectTransform.GetComponentInChildren<TextMeshProUGUI>();

        startTime = Time.time;
    }

    void Update()
    {
        UpdateTimer();
        UpdateStandardText();
        UpdateSelectionWheel();
    }

    void UpdateStandardText()
    {
        if (PlayerData.instance == null)
            return;

        if (textScore) textScore.text = PlayerData.instance.totalPoints.ToString();
        if (textHealth) textHealth.text = PlayerData.instance.health.ToString();
        if (textAmmo)
            textAmmo.text = "● " + PlayerData.instance.universalAmmo + "/30";
    }

    void UpdateSelectionWheel()
    {
        if (PlayerData.instance == null) return;

        // Tranca/Destranca a base cinza e o botão interagível
        if (blockSum) blockSum.SetActive(!PlayerData.instance.unlockedSum);
        if (btnSum) btnSum.SetActive(PlayerData.instance.unlockedSum);

        if (blockSub) blockSub.SetActive(!PlayerData.instance.unlockedSub);
        if (btnSub) btnSub.SetActive(PlayerData.instance.unlockedSub);

        if (blockMulti) blockMulti.SetActive(!PlayerData.instance.unlockedMulti);
        if (btnMulti) btnMulti.SetActive(PlayerData.instance.unlockedMulti);

        if (blockDiv) blockDiv.SetActive(!PlayerData.instance.unlockedDiv);
        if (btnDiv) btnDiv.SetActive(PlayerData.instance.unlockedDiv);

        // Desliga os destaques (brilhos/escala maior) de todos
        if (activeSum) activeSum.SetActive(false);
        if (activeSub) activeSub.SetActive(false);
        if (activeMulti) activeMulti.SetActive(false);
        if (activeDiv) activeDiv.SetActive(false);

        string efeitoReal = "";
        Color corDoEfeito = Color.white;
        int mult = PlayerData.instance.currentMultiplier;

        // Ativa o destaque da arma certa e cria o texto combinado (ex: "+2", "x3") com a cor do quadrante
        switch (PlayerData.instance.currentMode)
        {
            case PlayerData.MathOperation.Sum:
                if (activeSum) activeSum.SetActive(true);
                efeitoReal = "+" + mult;
                corDoEfeito = Color.green;
                break;
            case PlayerData.MathOperation.Sub:
                if (activeSub) activeSub.SetActive(true);
                efeitoReal = "-" + mult;
                corDoEfeito = Color.red;
                break;
            case PlayerData.MathOperation.Multi:
                if (activeMulti) activeMulti.SetActive(true);
                efeitoReal = "x" + mult;
                corDoEfeito = Color.blue;
                break;
            case PlayerData.MathOperation.Div:
                if (activeDiv) activeDiv.SetActive(true);
                efeitoReal = "÷" + mult;
                corDoEfeito = Color.yellow;
                break;
        }

        // Aplica o "efeito real" no miolo da roda (Centro da roda)
        if (textCentralEffect != null)
        {
            textCentralEffect.text = efeitoReal;
            textCentralEffect.color = corDoEfeito;
        }
    }

    void UpdateTimer()
    {
        float timeRemaining = totalTime - (Time.time - startTime);
        timeRemaining = Mathf.Max(timeRemaining, 0f);
        
        string minutes = ((int)timeRemaining / 60).ToString();
        string seconds = ((int)timeRemaining % 60).ToString("00");

        if (textTimer) 
        {
            textTimer.text = minutes + ":" + seconds;
            
            // Fica vermelho e pulsa nos últimos 10 segundos
            if (timeRemaining <= 10f)
            {
                textTimer.color = Color.red;
                // Efeito simples de pulsar (aumenta e diminui a escala levemente usando o Seno do tempo)
                float scale = 1f + Mathf.Sin(Time.time * 10f) * 0.1f;
                timerTransform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                textTimer.color = Color.white;
                timerTransform.localScale = Vector3.one;
            }
        }
        if(timeRemaining <= 0 && PlayerData.instance != null) PlayerData.instance.health = 0;
    }
}
