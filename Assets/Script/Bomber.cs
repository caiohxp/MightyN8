using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Enemy
{
    public float nextDecrement;
    public float decrementRate;

    public float areaX = 5f;
    private float minX;
    private float maxX;
    private bool movingRight = true; 
    private float normalSpeed;
    private float rageSpeed;


    // Start is called before the first frame update
    void Start()
    {
        minX = transform.position.x - areaX;
        maxX = transform.position.x + areaX;
        normalSpeed = speed;
        rageSpeed = speed * 3;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Move();
        if(targetDistance < attackDistance && !solved){
            TimerToExplode();
            speed = rageSpeed;
        } else {
            speed = normalSpeed;
        }
    }

    void TimerToExplode(){
        if(nextDecrement < Time.time){
            if(valueLeft <= 0){
                anim.SetTrigger("Death");
            }
            nextDecrement = Time.time + decrementRate;
            valueLeft--;
        }
    }

    void OnDeathAnimationFinish(){
        Destroy(gameObject, 0.5f);
    }

    void Move(){
        if (movingRight){
            anim.SetBool("right", false);
            anim.SetBool("left", true);
            rb2d.velocity = Vector2.right * speed;
        } else {
            anim.SetBool("left", false);
            anim.SetBool("right", true);
            rb2d.velocity = Vector2.left * speed;
        }

        if (transform.position.x >= maxX){
            movingRight = false;
        } else if (transform.position.x <= minX){
            movingRight = true;
        }
    }
}
