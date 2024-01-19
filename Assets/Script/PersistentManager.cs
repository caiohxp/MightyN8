using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance { get; private set; }

    public UnityEvent onGameOver;
    public GameObject gameOverScreen;
    public GameObject aprovacao;
    public Transform notaAprovacao;
    public GameObject reprovacao;
    public GameObject pause;
    public Transform notaReprovacao;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerGameOver()
    {
        onGameOver.Invoke(); // Disparar o evento de game over
    }
}
