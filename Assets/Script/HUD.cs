using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    private float startTime;
    public float totalTime = 120f;

    private TextMeshProUGUI textHealth;
    public Transform healthTransform;
    private TextMeshProUGUI textPlusBullets;
    public Transform plusBulletsTransform;
    private TextMeshProUGUI textMinusBullets;
    public Transform minusBulletsTransform;
    private TextMeshProUGUI textTimer;
    public Transform timerTransform;
    private TextMeshProUGUI textScore;
    public Transform scoreTransform;
    // Start is called before the first frame update
    void Start()
    {
        textHealth = healthTransform.GetComponentInChildren<TextMeshProUGUI>();
        textPlusBullets = plusBulletsTransform.GetComponentInChildren<TextMeshProUGUI>();
        textMinusBullets = minusBulletsTransform.GetComponentInChildren<TextMeshProUGUI>();
        textTimer = timerTransform.GetComponentInChildren<TextMeshProUGUI>();
        textScore = scoreTransform.GetComponentInChildren<TextMeshProUGUI>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        textScore.text = "Pontos: " + GameController.instance.totalPoints.ToString();
        textHealth.text = "X " + Player.instance.health.ToString();
        textPlusBullets.text = "X " + Player.instance.plusBullets.ToString();
        textMinusBullets.text = "X " + Player.instance.minusBullets.ToString();
    }

    void Timer(){
        float timeRemaining = totalTime - (Time.time - startTime);

        timeRemaining = Mathf.Max(timeRemaining, 0f);
        // Formata o tempo em minutos e segundos
        string minutes = ((int)timeRemaining / 60).ToString();
        string seconds = ((int)timeRemaining % 60).ToString();

        // Atualiza o texto do timer na interface
        textTimer.text = minutes + ":" + seconds;
    }
}
