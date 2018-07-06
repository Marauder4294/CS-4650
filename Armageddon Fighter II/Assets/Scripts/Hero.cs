//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Entity {

    #region Hero-Specific Variable Declarations

    float cameraOffsetX;
    float cameraOffsetZ;

    readonly int?[] attackAnimTimes = new int?[6];

    #endregion

    void Awake()
    {
        #region Set Unity objects

        Ent = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        Rigid = Ent.GetComponent<Rigidbody>();
        Anim = Ent.GetComponent<Animator>();

        #endregion

        #region Base Attribute Setter

        Power = 15;
        Magic = 5;
        Defense = 10;
        MagicResist = 0;
        Block = 10;
        Vitality = 20;

        #endregion

        #region Set Booleans and floats

        JumpPower = 3f;
        JumpHeight = JumpPower;

        MovementSpeed = 0.1f;
        AttackSpeed = 1.2f;

        AttackCount = 0;
        MaxAttackNumber = 5;

        MoveTimer = 0;
        AttackTimer = 0;
        AttackLockTimer = 0;
        JumpTimer = 0;

        IsAttacking = false;

        Anim.SetFloat("AttackSpeed", AttackSpeed);

        #endregion

        #region Subscribe to events

        EventManager.OnAttack += OnAttack;
        EventManager.OnJump += OnJump;
        EventManager.OnBlock += OnBlock;
        EventManager.OnMove += OnMove;
        //EventManager.OnAvoid += OnAvoid;

        #endregion

        #region Set Hero-Specific variables 

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        attackAnimTimes[1] = (int)((clip.First(a => a.name == "Attack_1-WindUp").length + clip.First(a => a.name == "Attack_1").length) * 24);
        attackAnimTimes[2] = (int)(clip.First(a => a.name == "Attack_2").length * 24);
        attackAnimTimes[3] = (int)((clip.First(a => a.name == "Attack_3-WindUp").length + clip.First(a => a.name == "Attack_3").length) * 24);
        attackAnimTimes[4] = attackAnimTimes[3];
        attackAnimTimes[5] = (int)((clip.First(a => a.name == "Attack_4-WindUp").length + clip.First(a => a.name == "Attack_4").length) * 24);
        attackAnimTimes[0] = attackAnimTimes[1] + attackAnimTimes[2] + attackAnimTimes[3] + attackAnimTimes[4] + attackAnimTimes[5];

        cameraOffsetX = Camera.main.transform.position.x - Ent.transform.position.x;
        cameraOffsetZ = Camera.main.transform.position.z - Ent.transform.position.z;

        #endregion
    }

    public void OnMove(float moveX, float moveY)
    {
        IsMoving = ((moveX != 0 || moveY != 0) && MoveTimer == 0) ? true : false;

        if (IsMoving == true)
        {
            if (IsBlocking == false)
                Ent.transform.LookAt(Ent.transform.position += new Vector3(-moveY * MovementSpeed, 0, -moveX * MovementSpeed));

            Ent.transform.forward += new Vector3(-moveY, 0, -moveX);

            if (IsBlocking == false)
                Camera.main.transform.position = new Vector3(Ent.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, Ent.transform.position.z + cameraOffsetZ);
        }

        Anim.SetBool("Moving", IsMoving);
        Anim.SetFloat("MoveSpeed", (MovementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);

        #region Timer Updates

        if (MoveTimer > 0)
            --MoveTimer;

        if (AttackTimer != 1)
        {
            --AttackTimer;
        }
        else
        {
            AttackTimer = 0;
            AttackLockTimer = 0;
            AttackCount = 0;
            Anim.SetInteger("AttackNumber", AttackCount);
        }

        if (InAir == true && IsGoingUp == true)
        {
            if (Ent.transform.position.y > JumpHeight)
            {
                IsGoingUp = false;
                movement.y = 0;
            }
            else
            {
                Ent.transform.position += movement;
            }
        }

        if (JumpTimer > 0)
            --JumpTimer;

        if (AttackLockTimer > 0)
        {
            if (IsAttacking == true)
            {
                IsAttacking = false;
                Anim.SetBool("Attacking", IsAttacking);
            }

            --AttackLockTimer;
        }

        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
        {
            Anim.SetBool("Landing", true);
            Anim.SetBool("Jumping", false);
            Anim.SetBool("Attacking", false);
            InAir = false;
            Rigid.isKinematic = true;
            JumpTimer = 24;
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
        {
            Rigid.isKinematic = false;
            Anim.SetBool("Jumping", true);
            Anim.SetBool("Landing", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
        {
            Rigid.isKinematic = true;
            Anim.SetBool("Jumping", false);
            Anim.SetBool("Landing", true);
        }
    }

    public void OnAttack(bool isAction)
    {
        if (AttackLockTimer == 0)
        {
            IsAttacking = true;

            if (AttackCount >= 5)
            {
                AttackCount = 0;
            }
            else
            {
                AttackTimer = 100;
            }

            Anim.SetBool("Attacking", IsAttacking);

            if (InAir == false)
            {
                AttackCount++;
                AttackLockTimer = attackAnimTimes[AttackCount] ?? 0;
                MoveTimer = AttackLockTimer;
                JumpTimer = AttackLockTimer;
            }
            else
            {
                AttackCount = 5;
                AttackLockTimer = 1;
            }

            Anim.SetInteger("AttackNumber", AttackCount);
        }
    }

    public void OnJump(bool isAction)
    {
        //attackCount = (isAction == true) ? 0 : attackCount;

        if (isAction == true && InAir == false && JumpTimer == 0)
        {
            InAir = true;
            Anim.SetBool("Jumping", isAction);
            Anim.SetBool("Landing", false);
            Anim.SetBool("Attacking", false);
            Rigid.isKinematic = false;

            IsGoingUp = true;

            JumpHeight = Ent.transform.position.y + JumpPower;

            movement.y = 0.15f;

            Ent.transform.position += movement;
        }
    }

    public void OnBlock(bool isAction)
    {
        if (isAction)
        {
            if (AttackCount > 0)
            {
                AttackCount = 0;
                Anim.SetInteger("AttackNumber", AttackCount);
            }

            if (IsAttacking)
            {
                IsAttacking = false;
                Anim.SetBool("Attacking", IsAttacking);
            }

            if (IsMoving)
            {
                IsMoving = false;
                Anim.SetBool("Moving", IsMoving);
            }

            if (!IsBlocking)
            {
                IsBlocking = true;
                Anim.SetBool("Blocking", IsBlocking);
            }
        }
        else
        {
            if (IsBlocking)
            {
                IsBlocking = false;
                Anim.SetBool("Blocking", IsBlocking);
            }
        }
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
