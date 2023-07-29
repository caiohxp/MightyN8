using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound : MonoBehaviour
{
    [SerializeField] private static sound backgroundMusic;

    void Awake(){
        if(backgroundMusic == null){
            backgroundMusic = this;
            // DontDestroyOnLoad(backgroundMusic);
        }
        else{
            Destroy(gameObject);
        }
    }
   

   

}
