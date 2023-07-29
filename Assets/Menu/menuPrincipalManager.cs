using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField] private string nomeDoLevelDoJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelOpcoes2;
    [SerializeField] private GameObject painelOpcoes3;
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelHistoria;
    [SerializeField] private GameObject painelHistoria2;
    [SerializeField] private GameObject painelHistoria3;




   public void jogar(){
        SceneManager.LoadScene(nomeDoLevelDoJogo);
   }

   public void abrirOpcoes(){
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
   }

   public void abrirOpcoes2(){
        painelOpcoes.SetActive(false);
        painelOpcoes2.SetActive(true);
   }

   public void abrirOpcoes3(){
        painelOpcoes2.SetActive(false);
        painelOpcoes3.SetActive(true);
   }

     public void abrirHistoria(){
        painelMenuInicial.SetActive(false);
        painelHistoria.SetActive(true);
     }
    public void abrirHistoria2(){
        painelHistoria.SetActive(false);
        painelHistoria2.SetActive(true);
   }

   public void abrirHistoria3(){
        painelHistoria2.SetActive(false);
        painelHistoria3.SetActive(true);
   }

   public void fecharHistoria(){
        painelHistoria.SetActive(false);
        painelMenuInicial.SetActive(true);
   }

   public void fecharHistoria2(){
        painelHistoria2.SetActive(false);
        painelHistoria.SetActive(true);
   }

     public void fecharHistoria3(){
          painelHistoria3.SetActive(false);
          painelHistoria2.SetActive(true);
     }

      public void fecharOpcoes(){
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
   }

   public void fecharOpcoes2(){
        painelOpcoes2.SetActive(false);
        painelOpcoes.SetActive(true);
   }

     public void fecharOpcoes3(){
          painelOpcoes3.SetActive(false);
          painelOpcoes2.SetActive(true);
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
        #if UNITY_EDITOR
                // No editor, apenas para de executar a cena
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // Em uma build, fecha o jogo normalmente
                Application.Quit();
        #endif
   }
}
