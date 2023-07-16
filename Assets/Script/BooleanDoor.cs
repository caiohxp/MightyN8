using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanDoor : MonoBehaviour
{
    public FloorBlock fb;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(fb.correctAnswer){
            gameObject.SetActive(false);
        }
    }
}
