using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public GameObject bulletEnemy;
    public Transform shotEnemySpawner;
    public float fireRate = 5;
    public float nextFire = 0;
    private Vector3 startOffset;
    private Vector3 startOffset2;
    void Start()
    {
        startOffset = offset;
        startOffset2 = offset2;
        attackDistanceY = 4;
    }

    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target.rotation.x, target.rotation.y, transform.rotation.z), speed * Time.deltaTime);
        if(targetDistanceX < attackDistanceX && targetDistanceX > -attackDistanceX && !solved){
            EnemyShot();
            anim.SetBool("shoot", true);
            anim.SetBool("walk", false);
            if(targetDistanceX < 0){
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 0f, 0f);
                leftCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                rightCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                offset.x = System.Convert.ToSingle(startOffset.x - 0.3);
                offset2.x = System.Convert.ToSingle(startOffset2.x - 0.3);
            } else {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                shotEnemySpawner.eulerAngles = new Vector3(0f, 180f, 0f);
                leftCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                rightCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                offset.x = System.Convert.ToSingle(startOffset.x + 0.3);
                offset2.x = System.Convert.ToSingle(startOffset2.x + 0.3);
            }
        } else {
            Move();
            offset.x = startOffset.x;
            offset2.x = startOffset2.x;
            anim.SetBool("shoot", false);
            anim.SetBool("walk", true);
            if(movingRight){
                transform.eulerAngles = new Vector3(0f,180f,0f);
                leftCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                rightCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
            } else{
                transform.eulerAngles = new Vector3(0f,0f,0f);
                leftCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
                rightCounterTransform.eulerAngles = new Vector3(0f, 0f,0f);
            }
        }
    }

    void EnemyShot(){
        if(nextFire < Time.time){
            nextFire = Time.time + fireRate;
            GameObject tempBullet = Instantiate(bulletEnemy, shotEnemySpawner.position, shotEnemySpawner.rotation);
        }
    }
}
