using UnityEngine;

public class EnemyShooterBullet : MonoBehaviour
{
    [Header("Configurações do Tiro")]
    public float speed = 10f;
    public float maxDistance = 20f; // A variável de distância máxima que você pediu
    public int damage = 1;

    private Vector2 spawnPosition;

    void Start()
    {
        // Salva a coordenada exata de onde a bala nasceu no momento do tiro
        spawnPosition = transform.position;
    }

    void Update()
    {
        // 1. Move a bala para frente continuamente
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 2. Calcula a distância entre o ponto de origem e a posição atual
        float distanciaPercorrida = Vector2.Distance(spawnPosition, transform.position);

        // 3. Se passou do limite, a bala se dissipa no ar
        if (distanciaPercorrida >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    // Como marcamos "Is Trigger" na Unity, usamos OnTriggerEnter em vez de OnCollision
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Se a bala encostar em qualquer coisa que NÃO seja o Player, ela ignora e continua atravessando
        if (hitInfo.gameObject.layer == 9 || hitInfo.CompareTag("Player"))
        {
            Debug.Log("O tiro acertou o Otto!");
            
            // Aqui você chamará a função de dar dano no Otto. Exemplo:
            // hitInfo.GetComponent<PlayerHealth>().TomarDano(damage);

            // A bala se destrói ao acertar o alvo
            Destroy(gameObject);
        }
    }
}