using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloorBlock : MonoBehaviour
{
    public int blockValue;
    public int expressionValue;
    public int operation;
    public bool correctAnswer = false;
    public bool wrongAnswer = false;
    public Transform valueTransform;
    public TextMeshProUGUI valueText;
    protected SpriteRenderer sprite;
    Player player;

    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        sprite = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        valueText.text = blockValue.ToString();
    }

    public void Trampled(){

        if(operation == 0){
            if(blockValue < expressionValue){
                sprite.color = Color.green;
                correctAnswer = true;
            } else if(!wrongAnswer){
                sprite.color = Color.red;
                wrongAnswer = true;
                player.health--;
            }
        } else if(operation == 1){
            if(blockValue > expressionValue){
                sprite.color = Color.green;
                correctAnswer = true;
            } else if(!wrongAnswer){
                sprite.color = Color.red;
                wrongAnswer = true;
                player.health--;
            }
        }
    }

    IEnumerator HitedCoRoutine(){
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }
}
