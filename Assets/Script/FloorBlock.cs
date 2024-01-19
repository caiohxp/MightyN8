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
    public Player player;

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

    void correct()
    {
        varSprite.color = Color.green;
        correctAnswer = true;
        PlayerData.instance.plusBullets += 5;
        PlayerData.instance.minusBullets += 5;
    }

    void wrong()
    {
        varSprite.color = Color.red;
        wrongAnswer = true;
        PlayerData.instance.health--;
        PlayerData.instance.plusBullets += 2;
        PlayerData.instance.minusBullets += 2;
    }

    public void Trampled()
    {

        if (symbolValue == 0)
        {
            if (varValue < expressionValue)
            {
                correct();
            }
            else if (!wrongAnswer)
            {
                wrong();
            }
        }
        else if (symbolValue == 1)
        {
            if (varValue > expressionValue)
            {
                correct();
            }
            else if (!wrongAnswer)
            {
                wrong();
            }
        }
        else if (symbolValue == 2)
        {
            if (varValue == expressionValue)
            {
                correct();
            }
            else if (!wrongAnswer)
            {
                wrong();
            }
        }
    }
}
