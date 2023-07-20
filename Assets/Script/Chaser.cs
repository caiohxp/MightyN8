using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        operation = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (targetDistance < attackDistance && !solved){
            Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            rb2d.velocity = moveDirection * speed;
        }
        else{
            rb2d.velocity = Vector2.zero;
        }
    }
}
