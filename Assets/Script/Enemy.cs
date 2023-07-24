using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float areaX = 5f;
    protected bool movingRight = true; 
    protected float minX;
    protected float maxX;
    public float speed;
    public float attackDistance;
    public int valueLeft;
    public int valueRight;
    public int operation;
    protected bool solved = false;
    protected bool onFloor = true;

    protected Animator anim;
    protected Transform target;
    private Collider2D playerCollider;
    private Collider2D bulletCollider;
    protected float targetDistance;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sprite;
    private GUIStyle guiStyle = new GUIStyle();
    public Transform leftCounterTransform;
    public Transform rightCounterTransform;
    public Vector3 offset;
    public Vector3 offset2;
    protected TextMeshProUGUI leftCounterText;
    protected TextMeshProUGUI rightCounterText;
    public Player player;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        leftCounterText = leftCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
        rightCounterText = rightCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
        minX = transform.position.x - areaX;
        maxX = transform.position.x + areaX;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        targetDistance = transform.position.x - target.position.x;
        leftCounterTransform.position = transform.position + offset;
        rightCounterTransform.position = transform.position + offset2;
        leftCounterText.text = valueLeft.ToString();
        rightCounterText.text = valueRight.ToString();
        if(transform.position.y < -20){
            gameObject.SetActive(false);
        }

    }
    
    public void Hited(int damage){

        if(operation == 0 && !solved){
            valueLeft += damage;
            if(valueLeft > valueRight){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
                // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletCollider, true);
                sprite.color = Color.green;
                solved = true;
                player.plusBullets += 10;
                GameController.instance.totalPoints++;
            } else{
                StartCoroutine(HitedCoRoutine());
            }
        } else if(operation == 1 && !solved){
            valueLeft += damage;
            if(valueLeft < valueRight){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
                // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletCollider, true);
                sprite.color = Color.green;
                solved = true;
                player.minusBullets += 10;
                GameController.instance.totalPoints++;
            }else{
                StartCoroutine(HitedCoRoutine());
            }
        }
    }

    public void Move(){
        rb2d.velocity = movingRight ? Vector2.right * speed : Vector2.left * speed;
        if (transform.position.x >= maxX) movingRight = false;
        else if (transform.position.x <= minX) movingRight = true;
    }

    IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.layer == 12){
            onFloor = true;
        }
        // if(collision.gameObject.layer == 3 || collision.gameObject.layer == 11 || collision.gameObject.layer == 10){
        //     movingRight = !movingRight;
        // }

        if(collision.gameObject.layer == 15){
            gameObject.SetActive(false);
        }
    }
    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.layer == 12){
            onFloor = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.layer == 13){
            onFloor = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.layer == 13){
            onFloor = false;
        }
    }

}
