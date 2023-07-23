using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestructBlock : MonoBehaviour
{
    public int varValue;
    public int expressionValue;
    public int symbolValue;
    public Transform constBlock;
    public Transform symbolBlock;
    public Transform valueTransform;
    public TextMeshProUGUI valueText;
    private Collider2D playerCollider;
    protected SpriteRenderer constSprite;
    protected SpriteRenderer symbolSprite;
    protected SpriteRenderer varSprite;
    // Start is called before the first frame update
    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        playerCollider = FindObjectOfType<Player>().GetComponent<Collider2D>();
        constSprite = constBlock.GetComponent<SpriteRenderer>();
        symbolSprite = symbolBlock.GetComponent<SpriteRenderer>();
        varSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        valueText.text = varValue.ToString();
    }

    public void Hited(int damage){

        if(symbolValue == 0){
            varValue += damage;
            if(varValue > expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                constSprite.color = Color.green;
                symbolSprite.color = Color.green;
                varSprite.color = Color.green;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
            } else{
                StartCoroutine(HitedCoRoutine());
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
            }
        } else if(symbolValue == 1){
            varValue += damage;
            if(varValue < expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                constSprite.color = Color.green;
                symbolSprite.color = Color.green;
                varSprite.color = Color.green;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, true);
            } else{
                StartCoroutine(HitedCoRoutine());
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider, false);
            }
        }
    }

    IEnumerator HitedCoRoutine(){
        varSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        constSprite.color = Color.white;
        symbolSprite.color = Color.white;
        varSprite.color = Color.white;
    }
}
