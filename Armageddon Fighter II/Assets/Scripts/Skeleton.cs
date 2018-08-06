//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skeleton : Entity
{
    // TODO Add JumpAttack to Skeleton

    public GameObject skeleton;

    void Awake()
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

        SetInitialValues();

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

        AttackAnimTimes = new float[3];
        AttackAnimTimes[1] = clip.First(a => a.name == "Attack_1").length / AttackSpeed;
        AttackAnimTimes[2] = clip.First(a => a.name == "Attack_2").length / AttackSpeed;
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
            if (IsActive && !IsAttacking && MoveTimer == -1 && !IsKnockedDown && StunTimer == -1)
            {
                if (!IsMoving)
                {
                    SetMoving(true);
                    SetWeapon(false);
                }

                skeleton.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                skeleton.transform.position += skeleton.transform.forward * MovementSpeed;
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
            if (AttackCount == MaxAttackNumber)
            {
                AttackLockTimer = -1;
                SetAttacking(false);
                SetWeapon(false);
            }
        }
    }

    #endregion
}