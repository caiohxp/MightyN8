using System.Collections;
using UnityEngine;

public class PlataformaQuiz : MonoBehaviour
{
    [Header("Lógica do Quiz")]
    public bool respostaCorreta;

    [Header("Configurações do Elevador")]
    public Transform pontoProximoAndar; 
    public float velocidadeSubida = 3f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;
    private bool jaPisou = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!jaPisou && (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player")))
        {
            ContactPoint2D contato = collision.GetContact(0);
            bool pisouPorCima = contato.normal.y < -0.5f;

            if (pisouPorCima)
            {
                jaPisou = true; 
                
                // MÁGICA 1: Gruda o Otto na plataforma para ele não deslizar nem tremer
                collision.transform.SetParent(transform);

                if (respostaCorreta)
                {
                    // Resposta certa: Verde e sobe lisinho
                    sprite.color = Color.green;
                    StartCoroutine(SubirPlataforma());
                }
                else
                {
                    // Resposta errada: Vermelho, intangível e cai
                    sprite.color = Color.red;
                    StartCoroutine(SubirPlataforma());
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Desgruda o Otto quando ele pular fora da plataforma
        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    IEnumerator SubirPlataforma()
    {
        if (pontoProximoAndar != null)
        {
            while (Vector2.Distance(transform.position, pontoProximoAndar.position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, pontoProximoAndar.position, velocidadeSubida * Time.deltaTime);
                yield return null;
            }
        }
    }
}