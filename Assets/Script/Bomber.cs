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
        Move();
        if(symbolValue == 0){
            anim.SetBool("right", movingRight ? false : true);
            anim.SetBool("left", movingRight ? true : false);
        } else if(symbolValue == 1){
            anim.SetBool("right", movingRight ? true : false);
            anim.SetBool("left", movingRight ? false : true);
        }
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
