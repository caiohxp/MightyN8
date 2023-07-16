using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atirador : Enemy
{
    public GameObject bulletEnemy;
    public Transform shotEnemySpawner;
    public float fireRate = 5;
    public float nextFire = 0;
    void Start()
    {
        operation = 0;
    }

    protected override void Update()
    {
        base.Update();
        UnityEngine.Debug.Log("target "+target.rotation);
        UnityEngine.Debug.Log("transform "+transform.rotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target.rotation.x, target.rotation.y, transform.rotation.z), speed * Time.deltaTime);
        if(targetDistance < attackDistance){
            EnemyShot();
        }
        if(targetDistance < 0){
            shotEnemySpawner.eulerAngles = new Vector3(0f,0f,0f);
        } else{
            shotEnemySpawner.eulerAngles = new Vector3(0f,180f,0f);
        }
    }

    void EnemyShot(){
        if(nextFire < Time.time){
            nextFire = Time.time + fireRate;
            GameObject tempBullet = Instantiate(bulletEnemy, shotEnemySpawner.position, shotEnemySpawner.rotation);
        }
    }
}
