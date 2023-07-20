using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestructBlock : MonoBehaviour
{
    public int blockValue;
    public int expressionValue;
    public int operation;
    public Transform constBlock;
    public Transform operationBlock;
    public Transform valueTransform;
    public TextMeshProUGUI valueText;
    private Collider2D playerCollider;
    protected SpriteRenderer constSprite;
    protected SpriteRenderer opSprite;
    protected SpriteRenderer varSprite;
    // Start is called before the first frame update
    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        constSprite = constBlock.GetComponent<SpriteRenderer>();
        opSprite = operationBlock.GetComponent<SpriteRenderer>();
        varSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        valueText.text = blockValue.ToString();
    }

    public void Hited(int damage){

        if(operation == 0){
            blockValue += damage;
            if(blockValue > expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                constSprite.color = Color.green;
                opSprite.color = Color.green;
                varSprite.color = Color.green;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
            } else{
                StartCoroutine(HitedCoRoutine());
                constSprite.color = Color.white;
                opSprite.color = Color.white;
                varSprite.color = Color.white;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
            }
        } else if(operation == 1){
            blockValue += damage;
            if(blockValue < expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                constSprite.color = Color.green;
                opSprite.color = Color.green;
                varSprite.color = Color.green;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
            } else{
                StartCoroutine(HitedCoRoutine());
                constSprite.color = Color.white;
                opSprite.color = Color.white;
                varSprite.color = Color.white;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
            }
        }
    }

    IEnumerator HitedCoRoutine(){
        varSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        varSprite.color = Color.white;
    }
}
