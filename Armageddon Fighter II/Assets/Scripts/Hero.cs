//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Entity {

    #region Variable Declarations

    float cameraOffsetX;
    float cameraOffsetZ;

    int?[] attackAnimTimes = new int?[7];

    #endregion

    void Awake()
    {
        entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        rigidBody = entity.GetComponent<Rigidbody>();

        #region Base Attribute Setter **Placeholder until saves are added

        power = 15;
        magic = 5;
        defense = 10;
        magicResist = 0;
        block = 10;
        vitality = 20;

        #endregion

        animator = entity.GetComponent<Animator>();
        AnimationClip[] clip = animator.runtimeAnimatorController.animationClips;

        attackAnimTimes[1] = (int)((clip.First(a => a.name == "Attack_1-WindUp").length + clip.First(a => a.name == "Attack_1").length) * 24);
        attackAnimTimes[2] = (int)(clip.First(a => a.name == "Attack_2").length * 24);
        attackAnimTimes[3] = (int)((clip.First(a => a.name == "Attack_3-WindUp").length + clip.First(a => a.name == "Attack_3").length) * 24);
        attackAnimTimes[4] = attackAnimTimes[3];
        attackAnimTimes[5] = (int)((clip.First(a => a.name == "Attack_4-WindUp").length + clip.First(a => a.name == "Attack_4").length) * 24);
        attackAnimTimes[0] = attackAnimTimes[1] + attackAnimTimes[2] + attackAnimTimes[3] + attackAnimTimes[4] + attackAnimTimes[5];

        attackAnimTimes[6] = (int)(clip.First(a => a.name == "Jump_Attack").length * 24);

        cameraOffsetX = Camera.main.transform.position.x - entity.transform.position.x;
        cameraOffsetZ = Camera.main.transform.position.z - entity.transform.position.z;

        jumpPower = 3f;
        jumpHeight = jumpPower;

        movementSpeed = 0.1f;
        attackSpeed = 1.2f;

        attackCount = 0;
        maxAttackNumber = 5;

        moveTimer = 0;
        attackTimer = 0;
        attackLockTimer = 0;
        jumpTimer = 0;

        isAttacking = false;

        animator.SetFloat("AttackSpeed", attackSpeed);

        EventManager.OnAttack += OnAttack;
        EventManager.OnJump += OnJump;
        EventManager.OnBlock += OnBlock;
        EventManager.OnMove += OnMove;
        //EventManager.OnAvoid += OnAvoid;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !rigidBody.isKinematic)
        {
            animator.SetBool("Landing", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Attacking", false);
            inAir = false;
            rigidBody.isKinematic = true;
            jumpTimer = 24;
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && rigidBody.isKinematic)
        {
            rigidBody.isKinematic = false;
            animator.SetBool("Jumping", true);
            animator.SetBool("Landing", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !rigidBody.isKinematic)
        {
            rigidBody.isKinematic = true;
            animator.SetBool("Jumping", false);
            animator.SetBool("Landing", true);
        }
    }

    public void OnAttack(bool isAction)
    {
        if (attackLockTimer == 0)
        {
            isAttacking = true;

            if (attackCount >= 5)
            {
                attackCount = 0;
            }
            else
            {
                attackTimer = 100;
            }

            animator.SetBool("Attacking", isAttacking);

            if (inAir == false)
            {
                attackCount++;
                attackLockTimer = attackAnimTimes[attackCount] ?? 0;
                moveTimer = attackLockTimer;
                jumpTimer = attackLockTimer;
            }
            else
            {
                attackCount = 5;
                attackLockTimer = 1;
            }

            animator.SetInteger("AttackNumber", attackCount);
        }
    }

    public void OnJump(bool isAction)
    {
        //attackCount = (isAction == true) ? 0 : attackCount;

        if (isAction == true && inAir == false && jumpTimer == 0)
        {
            inAir = true;
            animator.SetBool("Jumping", isAction);
            animator.SetBool("Landing", false);
            animator.SetBool("Attacking", false);
            rigidBody.isKinematic = false;

            isGoingUp = true;

            jumpHeight = entity.transform.position.y + jumpPower;

            movement.y = 0.15f;

            entity.transform.position += movement;
        }
    }

    public void OnBlock(bool isAction)
    {
        if (isAction)
        {
            if (attackCount > 0)
            {
                attackCount = 0;
                animator.SetInteger("AttackNumber", attackCount);
            }

            if (isAttacking)
            {
                isAttacking = false;
                animator.SetBool("Attacking", isAttacking);
            }

            if (isMoving)
            {
                isMoving = false;
                animator.SetBool("Moving", isMoving);
            }

            if (!isBlocking)
            {
                isBlocking = true;
                animator.SetBool("Blocking", isBlocking);
            }
        }
        else
        {
            if (isBlocking)
            {
                isBlocking = false;
                animator.SetBool("Blocking", isBlocking);
            }
        }
    }

    public void OnMove(float moveX, float moveY)
    {
        isMoving = ((moveX != 0 || moveY != 0) && moveTimer == 0) ? true : false;

        if (isMoving == true)
        {
            if (isBlocking == false)
                entity.transform.LookAt(entity.transform.position += new Vector3(-moveY * movementSpeed, 0, -moveX * movementSpeed));

            entity.transform.forward += new Vector3(-moveY, 0, -moveX);

            if (isBlocking == false)
                Camera.main.transform.position = new Vector3(entity.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, entity.transform.position.z + cameraOffsetZ);
        }

        animator.SetBool("Moving", isMoving);
        animator.SetFloat("MoveSpeed", (movementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);

        #region Timer Updates

        if (moveTimer > 0)
            --moveTimer;

        if (attackTimer != 1)
        {
            --attackTimer;
        }
        else
        {
            attackTimer = 0;
            attackLockTimer = 0;
            attackCount = 0;
            animator.SetInteger("AttackNumber", attackCount);
        }

        if (inAir == true && isGoingUp == true)
        {
            if (entity.transform.position.y > jumpHeight)
            {
                isGoingUp = false;
                movement.y = 0;
            }
            else
            {
                entity.transform.position += movement;
            }
        }

        if (jumpTimer > 0)
            --jumpTimer;

        if (attackLockTimer > 0)
        {
            if (isAttacking == true)
            {
                isAttacking = false;
                animator.SetBool("Attacking", isAttacking);
            }

            --attackLockTimer;
        }

        #endregion
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
    //            Camera.main.transform.position = new Vector3(hero.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, hero.transform.position.z + cameraOffsetZ);
    //    }

    //    animator.SetBool("Avoiding", isAvoiding);
    //    //animator.SetFloat("AvoidSpeed", (movementSpeed * 14f) * ((System.Math.Abs(avoidX) + System.Math.Abs(avoidY))) / 2);
    //}
}
