using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Blocks : MonoBehaviour
{
    public int varValue;
    public int symbolValue;
    public TextMeshProUGUI valueText;
    protected SpriteRenderer varSprite;
    public Transform valueTransform;
    // Start is called before the first frame update
    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        varSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        valueText.text = varValue.ToString();
    }
}
