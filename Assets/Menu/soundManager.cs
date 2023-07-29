using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class soundManager : MonoBehaviour
{
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;
    public bool muted = false;

    void Start()
    {
        soundOnIcon.enabled = true;
        soundOffIcon.enabled = false;
        AudioListener.pause = false;  
    }

    public void OnButtonPress(){

        if(AudioListener.pause == false){
            muted = true;
            AudioListener.pause = true;

        }else{
            muted = false;
            AudioListener.pause = false;
        }

        UpdateButtonIcon();
    }

    private void UpdateButtonIcon(){
      
       if(muted){
            soundOnIcon.enabled = false;
            soundOffIcon.enabled = true;
       }else{
            soundOnIcon.enabled = true;
            soundOffIcon.enabled = false;
       }
       
    }

  
}
