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

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IAtingivel alvo = collision.gameObject.GetComponent<IAtingivel>();
        if (alvo != null)
        {
            bool atingidoPelaEsquerda = (collision.transform.position.x - initialPosition.x) > 0;
            alvo.ReceberDano(damage, atingidoPelaEsquerda);
            Destroy(gameObject);
        }
    }
}
