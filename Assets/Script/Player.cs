using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject camera;
    public float Speed = 10;
    public float JumpForce = 600;
    public bool isDead = false;
    private Rigidbody2D rig;
    public bool isJumping;
    public bool doubleJump;

    public GameObject bulletPlusPrefab;
    public GameObject bulletMinusPrefab;
    public int health = 3;
    public int plusBullets;
    public int minusBullets;
    public float fireRate = 50;
    public float nextFire = 0;
    public Transform shotSpawnerUp;
    public Transform shotSpawnerDown;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Shot();
        if(health <= 0){
            gameObject.SetActive(false);
        }
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * Speed;
        if(transform.position.y < -20){
            health = 0;
        }
        camera.transform.position = new Vector3(transform.position.x, transform.position.y+2, transform.position.z - 1);
        if(Input.GetAxis("Horizontal") > 0f){
            // anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f,0f,0f);
        } else if(Input.GetAxis("Horizontal") < 0f){
            // anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f,180f,0f);
        } else if(Input.GetAxis("Horizontal") == 0f){
            // anim.SetBool("walk", false);
        }
    }
    void Jump(){
        if(Input.GetButtonDown("Jump")){
            if(!isJumping){
                rig.AddForce(new Vector2(0f,JumpForce), ForceMode2D.Impulse);
                doubleJump = true;
                // anim.SetBool("jump", true);
            } else {
                if(doubleJump){
                    rig.AddForce(new Vector2(0f,JumpForce/2), ForceMode2D.Impulse);
                    doubleJump = false;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.layer == 3){
            isJumping = false;
            UnityEngine.Debug.Log("enter");
            // anim.SetBool("jump", false);
        }
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 8){
            health--;
        }
        if(collision.gameObject.layer == 8){
            collision.gameObject.SetActive(false);
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.layer == 3){
            isJumping = true;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnityEngine.Debug.Log("test");
        FloorBlock fb = collision.gameObject.GetComponent<FloorBlock>();
        if(fb != null){
            fb.Trampled();
        }
    }

    void Shot(){
        if(plusBullets > 0){
            if(Input.GetButton("Fire1") && nextFire < Time.time){
                plusBullets--;
                nextFire = Time.time + fireRate;
                GameObject tempBullet = Instantiate(bulletPlusPrefab, shotSpawnerUp.position, shotSpawnerUp.rotation);
            }
        }
        if(minusBullets > 0){
            if(Input.GetButton("Fire2") && nextFire < Time.time){
                minusBullets--;
                nextFire = Time.time + fireRate;
                GameObject tempBullet = Instantiate(bulletMinusPrefab, shotSpawnerDown.position, shotSpawnerDown.rotation);
            }
        }
    }
}
