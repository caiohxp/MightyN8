using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject camera;
    public float Speed = 10f;
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
    private bool lookingCamera = false;
    private SpriteRenderer sprite;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
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
        // rig.velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, rig.velocity.y);
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * Speed;
        if(transform.position.y < -20){
            health = 0;
        }
        if(!lookingCamera){
            camera.transform.position = new Vector3(transform.position.x, transform.position.y+4, transform.position.z - 1);
        }
        if(Input.GetButtonDown("Vertical")){
            lookingCamera = true;
            camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        } else if(Input.GetButtonUp("Vertical")){
            lookingCamera = false;
        }
        if(Input.GetAxis("Horizontal") > 0f){
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f,0f,0f);
        } else if(Input.GetAxis("Horizontal") < 0f){
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f,180f,0f);
        } else if(Input.GetAxis("Horizontal") == 0f){
            anim.SetBool("walk", false);
        }
    }
    void Jump(){
        if(Input.GetButtonDown("Jump")){
            if(!isJumping){
                rig.AddForce(Vector3.up * JumpForce, ForceMode2D.Impulse);
                doubleJump = true;
                anim.SetBool("walk", false);
                anim.SetTrigger("TriggerJump");
            } else {
                if(doubleJump){
                    rig.AddForce(Vector3.up * JumpForce * 0.7f, ForceMode2D.Impulse);
                    doubleJump = false;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.layer == 3){
            UnityEngine.Debug.Log("encostou");
            isJumping = false;
            // anim.SetTrigger("TriggerJump");
        }
        // if (collision.gameObject.layer == 11){
        //     UnityEngine.Debug.Log("plat.y "+ collision.contacts[0].point.y);
        //     UnityEngine.Debug.Log("player.y "+ transform.position.y);
        //     if (collision.contacts[0].point.y < transform.position.y){
        //         Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        //     }
        // }
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 8){
            // health--;
            StartCoroutine(HitedCoRoutine());
        }
        if(collision.gameObject.layer == 8){
            collision.gameObject.SetActive(false);
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.layer == 3){
            isJumping = true;
            UnityEngine.Debug.Log("pulou");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FloorBlock fb = collision.gameObject.GetComponent<FloorBlock>();
        if(fb != null){
            fb.Trampled();
        }
    }

    IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    private void SpawnProjectile(){
        Instantiate(bulletPlusPrefab, shotSpawnerUp.position, shotSpawnerUp.rotation);
    }

    void Shot(){
        if(plusBullets > 0){
            if(Input.GetButton("Fire1") && nextFire < Time.time){
                anim.SetTrigger("ShootPlus");
                Invoke("SpawnProjectile", 0.15f);
                plusBullets--;
                nextFire = Time.time + fireRate;
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
