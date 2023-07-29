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
        if(symbolValue == 1){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            leftCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
            rightCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(onFloor){
            if (targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && !player.isJumping && targetDistanceY < attackDistanceY && targetDistanceY > -attackDistanceY  && !solved){
                speed = rageSpeed;
                Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                rb2d.velocity = moveDirection * speed;
                minX = transform.position.x - areaX;
                maxX = transform.position.x + areaX;
            }else{
                speed = normalSpeed;
                Move();
                if(symbolValue == 0){
                    anim.SetBool("right", movingRight ? true : false);
                    anim.SetBool("left", movingRight ? false : true);
                } else if(symbolValue == 1){
                    anim.SetBool("right", movingRight ? false : true);
                    anim.SetBool("left", movingRight ? true : false);
                }
            }
        }
    }

}
