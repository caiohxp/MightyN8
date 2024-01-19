using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public int totalEnemys;
    private bool jogoPausado = false;
    public static GameController instance;
    public GameObject gameOver;
    public GameObject aprovacao;
    public Transform notaAprovacao;
    public GameObject reprovacao;
    public GameObject pause;
    public Transform notaReprovacao;
    private TextMeshProUGUI notaText;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Chama o método para pausar o jogo
            AlternarPausa();
        }
    }

    public void FecharJogo()
    {
        #if UNITY_EDITOR
                // No editor, apenas para de executar a cena
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // Em uma build, fecha o jogo normalmente
                Application.Quit();
        #endif
    }

    void AlternarPausa()
    {
        // Inverte o valor da variável para indicar se o jogo está pausado ou não
        jogoPausado = !jogoPausado;

        // Pausa ou despausa o jogo dependendo do valor da variável
        Time.timeScale = jogoPausado ? 0f : 1f;

        // Se quiser, pode também exibir um menu de pausa ou outras ações ao pausar o jogo
        if (jogoPausado)
        {
            pause.SetActive(true);
        }
        else
        {
            pause.SetActive(false);
            Debug.Log("Jogo despausado!");
        }
    }

    public void ShowGameOver(){
        gameOver.SetActive(true);
    }

    public void ChangeScene(string lvlgame){
        if(lvlgame.Equals("fase2")){
            PlayerData.instance.plusBullets +=10;
            PlayerData.instance.minusBullets += 20;
            PlayerData.instance.totalPoints = 0;
            DontDestroyOnLoad(PlayerData.instance.gameObject);
        }
        SceneManager.LoadScene(lvlgame);
    }

    public void CalcNota(){
        float nota = ((float)PlayerData.instance.totalPoints/totalEnemys)*10;
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
