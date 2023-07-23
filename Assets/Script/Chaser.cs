using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Enemy
{
    public float areaX = 5f;
    private bool movingRight = true; 
    private float minX;
    private float maxX;
    private float normalSpeed;
    private float rageSpeed;
    // Start is called before the first frame update
    void Start()
    {
        operation = 0;
        minX = transform.position.x - areaX;
        maxX = transform.position.x + areaX;
        normalSpeed = speed;
        rageSpeed = speed * 2;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(onFloor){
            if (targetDistance < attackDistance && targetDistance > attackDistance * -1 && !player.isJumping && !solved){
                speed = rageSpeed;
                Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                rb2d.velocity = moveDirection * speed;
                minX = transform.position.x - areaX;
                maxX = transform.position.x + areaX;
            }else{
                speed = normalSpeed;
                Move();
            }
        }
    }

    void Move(){
        if (movingRight){
            anim.SetBool("right", true);
            anim.SetBool("left", false);
            rb2d.velocity = Vector2.right * speed;
        } else {
            anim.SetBool("left", true);
            anim.SetBool("right", false);
            rb2d.velocity = Vector2.left * speed;
        }

        if (transform.position.x >= maxX){
            movingRight = false;
        } else if (transform.position.x <= minX){
            movingRight = true;
        }
    }
}
