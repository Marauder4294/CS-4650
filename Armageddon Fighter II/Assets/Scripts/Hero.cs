//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity {

    #region Variable Declarations

    //GameManager gameManager;
    //InputManager inputManager;

    Animator animator;
    GameObject hero;
    Rigidbody rigidBody;
    Camera camera;
    float cameraOffsetX;
    float cameraOffsetZ;

    AnimationClip currentClip;

    float horizontal;
    float vertical;

    float movementSpeed;
    float attackSpeed;
    
    bool nextAttack;
    bool isMoving;
    bool isAvoiding;
    bool isJumping;
    bool isBlocking;
    bool isGoingUp;
    Vector3 movement;

    float jumpPower;
    float jumpHeight;
    int moveTimer;
    int jumptimer;

    #endregion

    void Awake() {

        //gameManager = FindObjectOfType<GameManager>();
        //inputManager = gameManager.inputManager;

        hero = GameObject.FindGameObjectWithTag("Player");
        rigidBody = hero.GetComponent<Rigidbody>();

        #region Base Attribute Setter **Placeholder until saves are added

        power = 15;
        magic = 5;
        defense = 10;
        magicResist = 0;
        block = 10;
        vitality = 20;

        #endregion

        animator = GetComponent<Animator>();

        camera = FindObjectOfType<Camera>();
        cameraOffsetX = camera.transform.position.x - hero.transform.position.x;
        cameraOffsetZ = camera.transform.position.z - hero.transform.position.z;

        jumpPower = 3f;
        jumpHeight = jumpPower;

        movementSpeed = 0.1f;
        attackSpeed = 1.2f;

        attackCount = 0;
        maxAttackNumber = 5;

        moveTimer = 0;
        attackTimer = 0;
        jumptimer = 0;

        animator.SetFloat("AttackSpeed", attackSpeed);

        EventManager.OnAttack += OnAttack;
        EventManager.OnJump += OnJump;
        EventManager.OnBlock += OnBlock;
        EventManager.OnMove += OnMove;
        //EventManager.OnAvoid += OnAvoid;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Terrain" && inAir)
        {
            animator.SetBool("Landing", true);
            animator.SetBool("Jumping", false);
            inAir = false;
            rigidBody.isKinematic = true;
            jumptimer = 30;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            animator.SetBool("Landing", false);
            animator.SetBool("Jumping", true);
            if (inAir == false)
                inAir = true;
            rigidBody.isKinematic = false;
        }
    }

    public void OnAttack(bool isAction)
    {
        if (isAction == true)
        {
            if (attackCount >= 5)
                attackCount = 0;

            animator.SetBool("Attacking", true);

            attackCount = (inAir == false) ? ++attackCount : 5;

            animator.SetInteger("AttackNumber", attackCount);

            moveTimer = (inAir == false) ? 45 : 0;
            attackTimer = 100;
        }
        else
        {
            animator.SetBool("Attacking", false);
        }

        if (attackTimer != 1)
        {
            --attackTimer;
        }
        else
        {
            attackTimer = 0;
            attackCount = 0;
            animator.SetInteger("AttackNumber", attackCount);
        }
    }

    public void OnJump(bool isAction)
    {
        attackCount = (isAction == true) ? 0 : attackCount;

        if (isAction == true && inAir == false && jumptimer == 0)
        {
            inAir = true;
            animator.SetBool("Jumping", isAction);
            animator.SetBool("Landing", false);
            rigidBody.isKinematic = false;

            isGoingUp = true;

            jumpHeight = hero.transform.position.y + jumpPower;

            movement.y = 0.15f;

            hero.transform.position += movement;
        }
        else if (inAir == true && isGoingUp == true)
        {
            if (hero.transform.position.y > jumpHeight)
            {
                isGoingUp = false;
                movement.y = 0;
            }
            else
            {
                hero.transform.position += movement;
            }
        }

        if (jumptimer > 0)
            --jumptimer;
    }

    public void OnBlock(bool isAction)
    {
        attackCount = (isAction == true) ? 0 : attackCount;

        isBlocking = isAction;

        animator.SetBool("Blocking", isBlocking);

        if (isAction == true)
            isMoving = false;

        animator.SetBool("Moving", isMoving);
    }

    public void OnMove(float moveX, float moveY)
    {
        isMoving = ((moveX != 0 || moveY != 0) && moveTimer == 0) ? true : false;

        if (isMoving == true)
        {
            if (isBlocking == false)
                hero.transform.LookAt(hero.transform.position += new Vector3(-moveY * movementSpeed, 0, -moveX * movementSpeed));

            hero.transform.forward += new Vector3(-moveY, 0, -moveX);

            if (isBlocking == false)
                camera.transform.position = new Vector3(hero.transform.position.x + cameraOffsetX, camera.transform.position.y, hero.transform.position.z + cameraOffsetZ);
        }

        animator.SetBool("Moving", isMoving);
        animator.SetFloat("MoveSpeed", (movementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);

        if (moveTimer > 0)
            --moveTimer;
    }

    //public void OnAvoid(float avoidX, float avoidY)
    //{
    //    isMoving = ((avoidX != 0 || avoidY != 0) && moveTimer == 0) ? true : false;

    //    if (isMoving == true)
    //    {
    //        if (isBlocking == false)
    //            hero.transform.LookAt(hero.transform.position += new Vector3(-avoidY * 3, 0, -avoidX * 3));

    //        hero.transform.forward += new Vector3(-avoidY, 0, -avoidX);

    //        if (isBlocking == false)
    //            camera.transform.position = new Vector3(hero.transform.position.x + cameraOffsetX, camera.transform.position.y, hero.transform.position.z + cameraOffsetZ);
    //    }

    //    animator.SetBool("Avoiding", isAvoiding);
    //    //animator.SetFloat("AvoidSpeed", (movementSpeed * 14f) * ((System.Math.Abs(avoidX) + System.Math.Abs(avoidY))) / 2);
    //}
}
