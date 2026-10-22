using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject pilarFront;
    private Animator anim;
    private Collider2D coll;
    private bool isOpen = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        if (pilarFront != null) 
        {
            pilarFront.SetActive(false);
        }
    }

    public void AbrirPortao()
    {
        if (isOpen) return;
        
        isOpen = true;
        
        // Toca a animação de abrir (se tiver um Animator)
        if (anim != null) anim.SetTrigger("open");

        // Transforma a parede sólida em um "fantasma" para o Otto atravessar
        coll.isTrigger = true; 
    }

    public void FinalizarAbertura()
    {
        if (pilarFront != null)
        {
            pilarFront.SetActive(true);
        }
    }
}