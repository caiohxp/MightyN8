using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float fireRate = 5;
    public float nextFire = 0;
    public float KBForce = 5;
    public float KBCounter;
    public float KBTotalTime;
    public bool KnockFromRight;
    public Transform shotSpawnerUp;
    public Transform shotSpawnerDown;
    private bool lookingCamera = false;
    private SpriteRenderer sprite;
    private bool onFinalPlatform = false;
    private float positionYOnFinalPlatform;

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
        if(!onFinalPlatform){
            if(!Input.GetButton("Fire3"))
                Move();
            MoveCamera();
            Jump();
            Shot();
        }
        if(PlayerData.instance.health <= 0){
            GameController.instance.ShowGameOver();
            
            gameObject.SetActive(false);
        }

    }

    void MoveCamera(){
        if(!lookingCamera){
            camera.transform.position = new Vector3(transform.position.x, transform.position.y+4, transform.position.z - 1);
        }
        if(Input.GetButton("Fire3")){
            lookingCamera = true;
            if(Input.GetButton("Vertical") && Input.GetButton("Fire3") && Input.GetButton("Horizontal")){
                if(Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") > 0) {
                    camera.transform.position = new Vector3(transform.position.x + 10, transform.position.y+8, transform.position.z - 1);
                } else if(Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") < 0){
                    camera.transform.position = new Vector3(transform.position.x - 10, transform.position.y+8, transform.position.z - 1);
                } else if(Input.GetAxis("Vertical") < 0 && Input.GetAxis("Horizontal") < 0){
                    camera.transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z - 1);
                } else if(Input.GetAxis("Vertical") < 0 && Input.GetAxis("Horizontal") > 0){
                    camera.transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z - 1);
                }
            } else if(Input.GetButton("Vertical") && Input.GetButton("Fire3")){
                if(Input.GetAxis("Vertical") > 0){
                    camera.transform.position = new Vector3(transform.position.x, transform.position.y+8, transform.position.z - 1);
                } else{
                    camera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                }
            } else if(Input.GetButton("Horizontal") && Input.GetButton("Fire3")){
                if(Input.GetAxis("Horizontal") > 0){
                    camera.transform.position = new Vector3(transform.position.x + 10, transform.position.y+4, transform.position.z - 1);
                } else{
                    camera.transform.position = new Vector3(transform.position.x - 10, transform.position.y+4, transform.position.z - 1);
                }
            }
        }
        else{
            lookingCamera = false;
        }
    }

    void Move()
    {
        if(KBCounter <= 0) rig.velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, rig.velocity.y);
        else{
            if(KnockFromRight) rig.velocity = new Vector2(-KBForce, KBForce);
            else rig.velocity = new Vector2(KBForce, KBForce);
            KBCounter -= Time.deltaTime;
        }
        // Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        // transform.position += movement * Time.deltaTime * Speed;
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

    void Shot(){
        if(PlayerData.instance.plusBullets > 0){
            if(Input.GetButton("Fire1") && nextFire < Time.time){
                anim.SetTrigger("ShootPlus");
                Invoke("SpawnPlusProjectile", 0.15f);
                PlayerData.instance.plusBullets--;
                nextFire = Time.time + fireRate;
            }
        }
        if(PlayerData.instance.minusBullets > 0){
            if(Input.GetButton("Fire2") && nextFire < Time.time){
                anim.SetTrigger("ShootMinus");
                Invoke("SpawnMinusProjectile", 0.15f);
                PlayerData.instance.minusBullets--;
                nextFire = Time.time + fireRate;
            }
        }
    }

    void KnockBack(float playerPosition, float collisionPosition){
        KBCounter = KBTotalTime;
        if(playerPosition <= collisionPosition) KnockFromRight = true;
        else KnockFromRight = false;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 8){
            PlayerData.instance.health--;
            KnockBack(transform.position.x, collision.transform.position.x);
            StartCoroutine(HitedCoRoutine());
        }
        if(collision.gameObject.layer == 8){
            collision.gameObject.SetActive(false);
        }

        if(collision.gameObject.layer == 15){
            PlayerData.instance.health = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FloorBlock fb = collision.gameObject.GetComponent<FloorBlock>();
        if(fb != null){
            fb.Trampled();
        }
        DangerBlock db = collision.gameObject.GetComponent<DangerBlock>();
        if(db != null){
            if(db.danger){
                KnockBack(transform.position.x, collision.transform.position.x);
                PlayerData.instance.health--;
                StartCoroutine(HitedCoRoutine());
            }
        }
        if(collision.gameObject.layer == 13){
            isJumping = false;
            anim.SetBool("jump", false);
        }

        if(collision.gameObject.layer == 14){
            positionYOnFinalPlatform = transform.position.y;
        }

    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.layer == 14){
            collision.gameObject.transform.Translate(Vector2.down * Speed * Time.deltaTime);
            transform.Translate(Vector2.down * Speed * Time.deltaTime);
            onFinalPlatform = true;
            if(transform.position.y < positionYOnFinalPlatform - 10){
                GameController.instance.CalcNota();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.layer == 13){
            isJumping = true;
            anim.SetBool("jump", true);
        }
        if(collision.gameObject.layer == 14){
            onFinalPlatform = false;
        }
        
    }
    

    IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    private void SpawnPlusProjectile(){
        Instantiate(bulletPlusPrefab, shotSpawnerUp.position, shotSpawnerUp.rotation);
    }

    private void SpawnMinusProjectile(){
        Instantiate(bulletMinusPrefab, shotSpawnerDown.position, shotSpawnerDown.rotation);
    }

    
}
