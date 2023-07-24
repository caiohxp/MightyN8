using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField] private string nomeDoLevelDoJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelCreditos;


   public void jogar(){
        SceneManager.LoadScene(nomeDoLevelDoJogo);
   }

   public void abrirOpcoes(){
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
   }

   public void fecharOpcoes(){
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
   }

   public void abrirCreditos(){
        painelMenuInicial.SetActive(false);
        painelCreditos.SetActive(true);
   }

   public void fecharCreditos(){
        painelCreditos.SetActive(false);
        painelMenuInicial.SetActive(true);
   }

   public void sairJogo(){
        Debug.Log("Sair do Jogo");
        Application.Quit();
   }
}
