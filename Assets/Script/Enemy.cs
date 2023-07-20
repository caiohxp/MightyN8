using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float attackDistance;
    public GameObject deathAnimation;
    public int valueLeft;
    public int valueRight;
    public int operation;
    protected bool solved = false;

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
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        leftCounterText = leftCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
        rightCounterText = rightCounterTransform.GetComponentInChildren<TextMeshProUGUI>();
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
            }else{
                StartCoroutine(HitedCoRoutine());
            }
        }
    }

    IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

}
