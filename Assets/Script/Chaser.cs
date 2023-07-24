using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Enemy
{
    private float normalSpeed;
    private float rageSpeed;
    // Start is called before the first frame update
    void Start()
    {
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
                anim.SetBool("right", movingRight ? true : false);
                anim.SetBool("left", movingRight ? false : true);
            }
        }
    }

}
