using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DestructBlock : MonoBehaviour
{
    public int blockValue;
    public int expressionValue;
    public int operation;
    public Transform valueTransform;
    public TextMeshProUGUI valueText;
    protected SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        valueText = valueTransform.GetComponentInChildren<TextMeshProUGUI>();
        sprite = GetComponent<SpriteRenderer>();
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

                gameObject.SetActive(false);
            } else{
                StartCoroutine(HitedCoRoutine());
            }
        } else if(operation == 1){
            blockValue += damage;
            if(blockValue < expressionValue){
                // Instantiate(deathAnimation, transform.position, transform.rotation);

                gameObject.SetActive(false);
            } else{
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
