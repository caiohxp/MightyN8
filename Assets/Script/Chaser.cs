using UnityEngine;

public class Chaser : Enemy
{
    private float normalSpeed;
    private float rageSpeed;

    protected override void Awake()
    {
        base.Awake(); 
        normalSpeed = speed;
        rageSpeed = speed * 2;
    }

    protected override void Update()
    {
        base.Update(); 
        if (isKnockedDown) return;

        if (onFloor)
        {
            bool alvoPertoX = Mathf.Abs(targetDistanceX) < attackDistanceX;
            bool alvoPertoY = Mathf.Abs(targetDistanceY) < attackDistanceY;

            if (alvoPertoX && alvoPertoY && !solved)
            {
                // MODO PERSEGUIÇÃO
                speed = rageSpeed;
                
                Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                
                rb2d.velocity = moveDirection * speed;

                minX = transform.position.x - areaX;
                maxX = transform.position.x + areaX;
                
                movingRight = moveDirection.x > 0;
            }
            else
            {
                // MODO PATRULHA
                speed = normalSpeed;
                Move();
            }

            // 1. O corpo vira fisicamente para onde está andando. 
            // Isso garante o movimento natural e espelha as posições dos números.
            transform.eulerAngles = movingRight ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 180f, 0f);

            // 2. A animação fica travada no símbolo correspondente, ignorando a direção da caminhada.
            if (symbolValue == 0)
            {
                anim.SetBool("right", true);
                anim.SetBool("left", false);
            }
            else if (symbolValue == 1)
            {
                anim.SetBool("left", true);
                anim.SetBool("right", false);
            }
        }
    }
}