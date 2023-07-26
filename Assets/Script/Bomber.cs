using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : Enemy
{
    public float nextDecrement;
    public float decrementRate;

    private float normalSpeed;
    private float rageSpeed;


    // Start is called before the first frame update
    void Start()
    {
        normalSpeed = speed;
        rageSpeed = speed * 3;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Move();
        anim.SetBool("right", movingRight ? false : true);
        anim.SetBool("left", movingRight ? true : false);
        if(targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && targetDistanceY < attackDistanceY && targetDistanceY > -attackDistanceY && !solved){
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

}
