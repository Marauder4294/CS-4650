//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skeleton : Entity
{
    public GameObject skeleton;

    System.Random rJumpAttack;

    float? yAttackMove = 0;

    public AudioClip[] soundClip;

    void Start()
    {
        #region Common Variable Setup

        Player = FindObjectOfType<Hero>();
        Anim = skeleton.GetComponent<Animator>();
        Ent = skeleton.GetComponent<Entity>();
        Rigid = skeleton.GetComponent<Rigidbody>();

        PositionY = skeleton.transform.position.y;

        IsActive = false;
        MaxAttackNumber = 2;
        KnockbackPowerHeight = 1.5f;
        KnockbackPowerLength = 3.5f;

        MovementSpeed = 0.06f;
        AttackSpeed = 2;
        StunLength = 2;
        AttackWaitTime = 1;

        ExperienceEndowment = 10;

        rJumpAttack = new System.Random();

        SetInitialValues(soundClip);

        JumpPower = 4;
        JumpHeight = JumpPower;

        #region Base Attribute Setter

        Level = 3;

        Power = 20;
        Magic = 15;
        Defense = 10;
        MagicResist = 0;
        Block = 0;
        Vitality = 60;

        MaxHealth = Vitality;
        Health = MaxHealth;

        #endregion Base Attribute Setter

        #region Subscribe to Events

        EventManager.OnMove += Hunt;

        #endregion Subscribe to Events

        #endregion Common Variable Setup

        #region Set Zombie-Specific Variables

        Weapon = new BoxCollider[1];
        Weapon[0] = skeleton.transform.Find("Skeleton/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/Sword").GetComponent<BoxCollider>();
        WeaponTrail = new TrailRenderer[1];
        WeaponTrail[0] = skeleton.transform.Find("Skeleton/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/Sword/SwordTrail").GetComponent<TrailRenderer>();
        WeaponTrail[0].startWidth = 0.3f;
        WeaponTrail[0].endWidth = 0.0001f;

        SetWeapon(false);

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        AttackAnimTimes = new float[4];
        AttackAnimTimes[1] = clip.First(a => a.name == "Attack_1").length / AttackSpeed;
        AttackAnimTimes[2] = clip.First(a => a.name == "Attack_2").length / AttackSpeed;
        AttackAnimTimes[3] = clip.First(a => a.name == "Jump_Attack").length / AttackSpeed;
        AttackAnimTimes[0] = AttackAnimTimes[1] + AttackAnimTimes[2];

        WindUpAnimTimes = new float[1];
        WindUpAnimTimes[0] = 0;

        #endregion Set Zombie-Specific Variables
    }

    private void OnDestroy()
    {
        EventManager.OnMove -= Hunt;
    }

    void Hunt(float moveX, float moveY)
    {
        if (IsActive && !IsDead && !Player.IsDead)
        {
            if (!IsAttacking && MoveTimer == -1 && !IsKnockedDown && StunTimer == -1)
            {
                if (!IsMoving && !InAir)
                {
                    SetMoving(true);
                    SetWeapon(false);
                }
                
                if (!InAir && rJumpAttack.Next(0, 100) < 15 && false)
                {
                    SetJumping(true);
                    Anim.SetBool("Landing", false);
                    if (IsMoving) SetMoving(false);
                    if (IsAttacking) SetAttacking(false);
                    if (Weapon[0].enabled) SetWeapon(false);
                    IsGoingUp = true;
                    Rigid.isKinematic = false;

                    movement.y = 0.15f;
                    JumpHeight = skeleton.transform.position.y + JumpPower;
                }
                else if (IsMoving)
                {
                    skeleton.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                    skeleton.transform.position += skeleton.transform.forward * MovementSpeed;
                }
            }
            else if (InAir && IsAttacking && JumpTimer == -1)
            {
                skeleton.transform.position += new Vector3((movement.x * (MovementSpeed / 10)), ((movement.y / (float)yAttackMove) * (MovementSpeed / 10)), (movement.x * (MovementSpeed / 10)));
            }

            DecrementAttackLockTimer();
            DecrementWindUpLockTimer();
            DecrementAttackTimer();
            DecrementJumpTimer();
            DecrementMoveTimer();
            DecrementStunTimer();
            JumpUp();
        }
        else
        {
            DecrementDeathTimer();
        }

        Fallback();
        DecrementKnockDownTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter(skeleton, other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay(skeleton, other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit(skeleton, other);
    }

    #region Entity Method Overrides

    protected override void DecrementAttackTimer()
    {
        if (AttackTimer > 0)
        {
            AttackTimer -= Time.deltaTime;
        }
        else if (AttackTimer > -1)
        {
            AttackTimer = -1;

            if (AttackCount == MaxAttackNumber)
            {
                AttackCount = 0;
                SetWeapon(false);
                Anim.SetInteger("AttackNumber", AttackCount);
            }
        }
    }

    protected override void DecrementAttackLockTimer()
    {
        if (AttackLockTimer > 0)
        {
            AttackLockTimer -= Time.deltaTime;
        }
        else if (AttackLockTimer > -1)
        {
            if (AttackCount >= MaxAttackNumber && !InAir)
            {
                AttackLockTimer = -1;
                SetAttacking(false);
                SetWeapon(false);
            }
        }
    }

    protected override void JumpUp()
    {
        if (InAir == true && IsGoingUp == true)
        {
            if (Ent.transform.position.y >= JumpHeight)
            {
                IsGoingUp = false;
                movement.y = -JumpPower;
                movement.x = (Player.transform.position.x - skeleton.transform.position.x) / JumpPower;
                movement.z = (Player.transform.position.z - skeleton.transform.position.z) / JumpPower;

                SetAttacking(true);
                SetWeapon(true);
                AttackCount = 3;

                if (movement.x >= movement.z)
                {
                    yAttackMove = movement.x;
                }
                else
                {
                    yAttackMove = movement.z;
                }

                skeleton.transform.position += new Vector3((movement.x * (MovementSpeed / 10)), ((movement.y / (float)yAttackMove) * (MovementSpeed / 10)), (movement.x * (MovementSpeed / 10)));
            }
            else
            {
                skeleton.transform.position += movement;
                skeleton.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
            }
        }
    }

    #endregion
}