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
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        DangerBlock db = collision.gameObject.GetComponent<DangerBlock>();
        DestructBlock block = collision.gameObject.GetComponent<DestructBlock>();
        if (enemy != null)
        {
            Vector3 impactDirection = collision.transform.position - initialPosition;
            UnityEngine.Debug.Log(impactDirection);

            if (impactDirection.x < 0 && !collision.CompareTag("Shooter"))
            {
                // Acertou pela direita
                enemy.HitedFromRight(damage);
            }
            else
            {
                // Acertou pela esquerda
                enemy.HitedFromLeft(damage);
            }
        } else if(block != null){
            UnityEngine.Debug.Log("X");
            block.Hited(damage);
        } else if(db != null && next < Time.time){
            next = rate + Time.time;
            db.Hited(damage);
        }
        UnityEngine.Debug.Log(gameObject);
        Destroy(gameObject);
    }
}
