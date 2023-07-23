using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloorBlock : MonoBehaviour
{
    public int varValue;
    public int expressionValue;
    public int symbolValue;
    public bool correctAnswer = false;
    public bool wrongAnswer = false;
    public Transform valueTransform;
    public TextMeshProUGUI valueText;
    protected SpriteRenderer varSprite;
    Player player;

    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        varSprite = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        valueText.text = varValue.ToString();
    }

    public void Trampled(){

        if(symbolValue == 0){
            if(varValue < expressionValue){
                varSprite.color = Color.green;
                correctAnswer = true;
            } else if(!wrongAnswer){
                varSprite.color = Color.red;
                wrongAnswer = true;
                player.health--;
            }
        } else if(symbolValue == 1){
            if(varValue > expressionValue){
                varSprite.color = Color.green;
                correctAnswer = true;
            } else if(!wrongAnswer){
                varSprite.color = Color.red;
                wrongAnswer = true;
                player.health--;
            }
        } else if(symbolValue == 2){
            if(varValue == expressionValue){
                varSprite.color = Color.green;
                correctAnswer = true;
            } else if(!wrongAnswer){
                varSprite.color = Color.red;
                wrongAnswer = true;
                player.health--;
            }
        }
    }
}
