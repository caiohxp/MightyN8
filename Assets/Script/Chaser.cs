using UnityEngine;

public class Chaser : Enemy
{
    private float normalSpeed;
    private float rageSpeed;

    protected override void Awake()
    {
        base.Awake(); // Crucial: Puxa o rb2d, anim e o target da classe Enemy
        normalSpeed = speed;
        rageSpeed = speed * 2;
    }

    void Start()
    {
        if(symbolValue == 0){
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        } else if(symbolValue == 1){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    protected override void Update()
    {
        base.Update(); // Crucial: Calcula targetDistanceX e targetDistanceY]
        if (isKnockedDown) return;

        if (onFloor)
        {
            // Checa se o alvo está dentro da caixa de ataque (ignorando se está pulando para simplificar e evitar bugs)
            bool alvoPertoX = Mathf.Abs(targetDistanceX) < attackDistanceX;
            bool alvoPertoY = Mathf.Abs(targetDistanceY) < attackDistanceY;

            if (alvoPertoX && alvoPertoY && !solved)
            {
                // MODO PERSEGUIÇÃO
                speed = rageSpeed;
                
                Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                
                rb2d.velocity = moveDirection * speed;

                // Atualiza a âncora da patrulha para onde ele perseguiu
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

            // Controle da Animação baseado no sinal
            if (symbolValue == 0)
            {
                anim.SetBool("right", movingRight);
                anim.SetBool("left", !movingRight);
            }
            else if (symbolValue == 1)
            {
                anim.SetBool("right", !movingRight);
                anim.SetBool("left", movingRight);
            }
        }
    }
}