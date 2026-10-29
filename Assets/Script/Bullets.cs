using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 10;
    public int damage = 1;
    public float destroyTime = 1.5f;
    private float next = 0;
    private float rate = 0.1f;
    Vector3 initialPosition;

    private Collider2D bulletCollider;
    private bool atingiu;

    void Start()
    {
        initialPosition = transform.position;
        bulletCollider = GetComponent<Collider2D>();
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        if (atingiu) return;

        Vector2 origem = bulletCollider != null
            ? bulletCollider.bounds.center
            : (Vector2)transform.position;

        Vector2 movimento = (Vector2)transform.right * speed * Time.deltaTime;
        float distancia = movimento.magnitude;

        if (distancia <= 0f) return;

        float raio = bulletCollider != null
            ? Mathf.Max(bulletCollider.bounds.extents.x, bulletCollider.bounds.extents.y)
            : 0.02f;

        RaycastHit2D[] impactos = Physics2D.CircleCastAll(
            origem,
            raio,
            movimento.normalized,
            distancia
        );

        foreach (RaycastHit2D impacto in impactos)
        {
            Collider2D atingido = impacto.collider;

            if (atingido == null || atingido == bulletCollider)
                continue;

            if (atingido.transform.root == transform.root)
                continue;

            if (ProcessarImpacto(atingido))
                return;
        }

        transform.position += (Vector3)movimento;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessarImpacto(collision);
    }

    private bool ProcessarImpacto(Collider2D collision)
    {
        if (atingiu || collision == null || collision == bulletCollider)
            return false;

        MathBlock bloco = collision.GetComponentInParent<MathBlock>();

        if (bloco != null)
        {
            atingiu = true;
            bloco.receiveDamage(damage);
            Destroy(gameObject);
            return true;
        }

        IAtingivel alvo = collision.GetComponent<IAtingivel>();

        if (alvo != null)
        {
            atingiu = true;

            bool atingidoPelaEsquerda =
                collision.transform.position.x - initialPosition.x > 0;

            alvo.ReceberDano(damage, atingidoPelaEsquerda);
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
