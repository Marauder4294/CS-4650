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

        Player = FindObjectOfType<Hero>();
        Ent = Player.GetComponent<Entity>();
        Rigid = Player.GetComponent<Rigidbody>();
        Anim = Player.GetComponent<Animator>();

        #endregion

        #region Base Attribute Setter

        Power = 15;
        Magic = 5;
        Defense = 10;
        MagicResist = 0;
        Block = 10;
        Vitality = 20;

        MaxHealth = Vitality * 2;
        MaxMana = Magic;

        Health = MaxHealth;
        Mana = MaxMana;

        #endregion

        #region Set Booleans and floats

        JumpPower = 3f;
        JumpHeight = JumpPower;

        MovementSpeed = 0.1f;
        AttackSpeed = 1.6f;

        AttackCount = 0;
        MaxAttackNumber = 5;

        MoveTimer = 0;
        AttackTimer = 0;
        AttackLockTimer = 0;
        JumpTimer = 0;

        IsAttacking = false;
        NextAttack = false;

        KnockbackPowerHeight = 1.5f;
        KnockbackPowerLength = 2f;

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

        attackAnimTimes[1] = (int)(((clip.First(a => a.name == "Attack_1-WindUp").length + clip.First(a => a.name == "Attack_1").length) * (24f * 1.5f)) / AttackSpeed);
        attackAnimTimes[2] = (int)((clip.First(a => a.name == "Attack_2").length * 24f) / AttackSpeed);
        attackAnimTimes[3] = (int)(((clip.First(a => a.name == "Attack_3-WindUp").length + clip.First(a => a.name == "Attack_3").length) * (24f * 1.5f)) / AttackSpeed);
        attackAnimTimes[4] = attackAnimTimes[3];
        attackAnimTimes[5] = (int)(((clip.First(a => a.name == "Attack_4-WindUp").length + clip.First(a => a.name == "Attack_4").length) * (24f * 1.5f)) / AttackSpeed);
        attackAnimTimes[0] = attackAnimTimes[1] + attackAnimTimes[2] + attackAnimTimes[3] + attackAnimTimes[4] + attackAnimTimes[5];

        cameraOffsetX = Camera.main.transform.position.x - Player.transform.position.x;
        cameraOffsetZ = Camera.main.transform.position.z - Player.transform.position.z;

        #endregion
    }

    public void OnMove(float moveX, float moveY)
    {
        IsMoving = ((moveX != 0 || moveY != 0) && MoveTimer == 0) ? true : false;

        if (IsMoving == true)
        {
            if (IsBlocking == false)
                Player.transform.LookAt(Player.transform.position += new Vector3(-moveY * MovementSpeed, 0, -moveX * MovementSpeed));

            Player.transform.forward += new Vector3(-moveY, 0, -moveX);

            if (IsBlocking == false)
                Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, Player.transform.position.z + cameraOffsetZ);
        }

        Anim.SetBool("Moving", IsMoving);
        Anim.SetFloat("MoveSpeed", (MovementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);

        DecrementAttackLockTimer();
        DecrementAttackTimer();
        DecrementJumpTimer();
        DecrementKnockDownTimer();
        DecrementMoveTimer();
        DecrementStunTimer();
        Fallback();
        JumpUp();

        #region Hero-specific Updates



        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
        {
            Anim.SetBool("Landing", true);
            Anim.SetBool("Jumping", false);
            IsAttacking = false;
            Anim.SetBool("Attacking", IsAttacking);
            InAir = false;
            Rigid.isKinematic = true;
            JumpTimer = 24;
        }
        else if (other.gameObject.tag == "EnemyWeapon" && !IsKnockedDown)
        {
            Damage(other.gameObject.GetComponentInParent<Entity>(), false);
            Stun();
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
            if (AttackCount >= 5)
            {
                AttackCount = 0;
            }

            if (InAir == false)
            {
                AttackCount++;
                AttackLockTimer = attackAnimTimes[AttackCount] ?? 0;
                AttackTimer = (AttackCount == 1) ? AttackLockTimer : (int)(100 / AttackSpeed);
                MoveTimer = AttackLockTimer;
                JumpTimer = AttackLockTimer;
            }
            else
            {
                AttackCount = 5;
                AttackLockTimer = 1;
            }

            IsAttacking = true;
            Anim.SetBool("Attacking", IsAttacking);
            Anim.SetInteger("AttackNumber", AttackCount);
        }
        else if (AttackCount == 1 && !NextAttack)
        {
            NextAttack = true;
        }
    }

    public void OnJump(bool isAction)
    {
        if (isAction == true && InAir == false && JumpTimer == 0)
        {
            InAir = true;
            Anim.SetBool("Jumping", isAction);
            Anim.SetBool("Landing", false);
            Anim.SetBool("Attacking", false);
            Rigid.isKinematic = false;

            IsGoingUp = true;

            JumpHeight = Player.transform.position.y + JumpPower;

            movement.y = 0.15f;

            Player.transform.position += movement;
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
    //    IsMoving = ((avoidX != 0 || avoidY != 0) && MoveTimer == 0) ? true : false;

    //    if (IsMoving == true)
    //    {
    //        if (IsBlocking == false)
    //            Player.transform.LookAt(Player.transform.position += new Vector3(-avoidY * 3, 0, -avoidX * 3));

    //        Player.transform.forward += new Vector3(-avoidY, 0, -avoidX);

    //        if (IsBlocking == false)
    //            Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, Player.transform.position.z + cameraOffsetZ);
    //    }

    //    Anim.SetBool("Avoiding", IsAvoiding);
    //    //Anim.SetFloat("AvoidSpeed", (MovementSpeed * 14f) * ((System.Math.Abs(avoidX) + System.Math.Abs(avoidY))) / 2);
    //}

    #region Entity Method Overrides

    protected override void DecrementAttackLockTimer()
    {
        if (AttackLockTimer > 1)
        {
            --AttackLockTimer;
        }
        else if (AttackLockTimer == 1)
        {
            if (AttackCount == 1 && NextAttack)
            {
                NextAttack = false;
                AttackCount++;
                Anim.SetInteger("AttackNumber", AttackCount);
                AttackLockTimer = attackAnimTimes[AttackCount] ?? 0;
                AttackTimer = (int)(100 / AttackSpeed);
            }
            else
            {
                IsAttacking = false;
                Anim.SetBool("Attacking", IsAttacking);
                AttackLockTimer = 0;
            }
        }
    }

    #endregion
}
