using UnityEngine;

public class ItemArsenal : MonoBehaviour
{
    public enum TipoItem { Municao, UnlockSoma, UnlockSubtracao, UnlockMultiplicacao, UnlockDivisao }
    public TipoItem tipoDoItem;
    public int quantidadeMunicao = 5;
    
    // Trava que impede o Otto de "pegar o item no ar"
    private bool prontaParaColetar = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Se bateu em qualquer coisa que NÃO seja o Player (ex: Chão, Parede), o item destranca
        if (collision.gameObject.layer != 9 && !collision.gameObject.CompareTag("Player"))
        {
            prontaParaColetar = true;
        }

        // 2. Se o item já bateu no chão (está pronto) e quem encostar for o Otto, faz a coleta
        if (prontaParaColetar)
        {
            if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
            {
                ColetarItem();
            }
        }
    }

    // Isola a lógica dos bônus matemáticos para manter o código limpo
    private void ColetarItem()
    {
        if (PlayerData.instance != null)
        {
            switch (tipoDoItem)
            {
                case TipoItem.Municao:
                    PlayerData.instance.universalAmmo += quantidadeMunicao;
                    break;
                case TipoItem.UnlockSoma:
                    PlayerData.instance.unlockedSum = true;
                    break;
                case TipoItem.UnlockSubtracao:
                    PlayerData.instance.unlockedSub = true;
                    break;
                case TipoItem.UnlockMultiplicacao:
                    PlayerData.instance.unlockedMulti = true;
                    break;
                case TipoItem.UnlockDivisao:
                    PlayerData.instance.unlockedDiv = true;
                    break;
            }
        }

        // Destrói o bônus do cenário após a coleta
        Destroy(gameObject);
    }
}