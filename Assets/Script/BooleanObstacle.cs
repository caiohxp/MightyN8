using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanObstacle : MonoBehaviour
{
    public FloorBlock fb;
    protected SpriteRenderer sprite;
    private Collider2D playerCollider;
    private Collider2D obstacleCollider;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        obstacleCollider = GetComponent<Collider2D>();
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fb.correctAnswer){
            sprite.color = Color.green;
            gameObject.layer = 17;
        } else {
            sprite.color = Color.red;
        }
    }

}
