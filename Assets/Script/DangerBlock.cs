using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DangerBlock : MonoBehaviour
{
    public float varValue;
    public int symbolValue;
    public TextMeshProUGUI valueText;
    protected SpriteRenderer varSprite;
    public Transform valueTransform;
    public int const1Value;
    public int operationValue;
    public int const2Value;
    private int expressionValue;
    public bool danger = true;
    public Transform const1Block;
    public Transform operationBlock;
    public Transform const2Block;
    public Transform symbolBlock;
    protected SpriteRenderer const1Sprite;
    protected SpriteRenderer const2Sprite;
    protected SpriteRenderer operationSprite;
    protected SpriteRenderer symbolSprite;
    // Start is called before the first frame update
    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        varSprite = GetComponent<SpriteRenderer>();
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        const1Sprite = const1Block.GetComponent<SpriteRenderer>();
        const2Sprite = const2Block.GetComponent<SpriteRenderer>();
        operationSprite = operationBlock.GetComponent<SpriteRenderer>();
        symbolSprite = symbolBlock.GetComponent<SpriteRenderer>();
        const1Sprite.color = Color.red;
        const2Sprite.color = Color.red;
        operationSprite.color = Color.red;
        symbolSprite.color = Color.red;
        varSprite.color = Color.red;

        if(operationValue == 0){
            expressionValue = const1Value + const2Value;
        } else if(operationValue == 1){
            expressionValue = const1Value - const2Value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        valueText.text = varValue.ToString();
    }

    public void Hited(int damage){
        UnityEngine.Debug.Log(damage);
        if(symbolValue == 0){
            varValue += damage;
            if(varValue > expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                const1Sprite.color = Color.green;
                const2Sprite.color = Color.green;
                operationSprite.color = Color.green;
                symbolSprite.color = Color.green;
                varSprite.color = Color.green;
                danger = false;
                PlayerData.instance.plusBullets += (const1Value + const2Value)/2;
            } else{
                const1Sprite.color = Color.red;
                const2Sprite.color = Color.red;
                operationSprite.color = Color.red;
                symbolSprite.color = Color.red;
                varSprite.color = Color.red;
            }
        } else if(symbolValue == 1){
            varValue += damage;
            if(varValue < expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);
                const1Sprite.color = Color.green;
                const2Sprite.color = Color.green;
                operationSprite.color = Color.green;
                symbolSprite.color = Color.green;
                varSprite.color = Color.green;
                danger = false;
                PlayerData.instance.minusBullets += (const1Value + const2Value)/2;
            } else{
                const1Sprite.color = Color.red;
                const2Sprite.color = Color.red;
                operationSprite.color = Color.red;
                symbolSprite.color = Color.red;
                varSprite.color = Color.red;
            }
        }
    }

    IEnumerator HitedCoRoutine(){
        varSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        const1Sprite.color = Color.white;
        const2Sprite.color = Color.white;
        operationSprite.color = Color.white;
        symbolSprite.color = Color.white;
        varSprite.color = Color.white;
    }
}
