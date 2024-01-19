using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelaMenu : MonoBehaviour
{
    public HUD camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<HUD>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = camera.transform.position;
    }
}
