using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public int totalPoints;
    public int totalEnemys;
    public static GameController instance;
    public GameObject gameOver;
    public GameObject aprovacao;
    public Transform notaAprovacao;
    public GameObject reprovacao;
    public Transform notaReprovacao;
    private TextMeshProUGUI notaText;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowGameOver(){
        gameOver.SetActive(true);
    }

    public void ChangeScene(string lvlgame){
        DontDestroyOnLoad(player.gameObject);
        SceneManager.LoadScene(lvlgame);
    }

    public void CalcNota(){
        float nota = ((float)totalPoints/totalEnemys)*10;
        if(nota >= 7){
            aprovacao.SetActive(true);
            notaText = notaAprovacao.GetComponentInChildren<TextMeshProUGUI>();
            notaText.text = nota.ToString("0.00");
        } else{
            reprovacao.SetActive(true);
            notaText = notaReprovacao.GetComponentInChildren<TextMeshProUGUI>();
            notaText.text = nota.ToString("0.00");
        }
    }
    
}
