using UnityEngine;

public class LevelKey : MonoBehaviour
{
    [HideInInspector] 
    public Gate portaoAlvo;

    private bool prontaParaColetar = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Se a chave bateu em qualquer coisa que NÃO seja o Player (ex: o chão ou uma parede)
        if (collision.gameObject.layer != 9 && !collision.gameObject.CompareTag("Player"))
        {
            prontaParaColetar = true; // Destranca a chave para ser pega
        }

        // Se o Otto encostar nela E ela já estiver pronta (no chão)
        if (prontaParaColetar)
        {
            if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
            {
                if (portaoAlvo != null)
                {
                    portaoAlvo.AbrirPortao();
                }
                Destroy(gameObject);
            }
        }
    }
}