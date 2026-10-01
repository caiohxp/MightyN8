using System.Collections;
using UnityEngine;

public class FeedbackCorBloco : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color corOriginal;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        // Salva a cor original (branco/textura) para poder voltar depois
        corOriginal = sprite.color; 
    }

    // Chama isso se o Otto acertar a conta!
    public void FicarVerde()
    {
        sprite.color = Color.green;
    }

    // Chama isso se o Otto errar a conta!
    public void FicarVermelho()
    {
        sprite.color = Color.red;
    }
}