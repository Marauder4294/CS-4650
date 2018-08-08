//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Entity
{
    public GameObject zombie;
    public AudioClip[] soundClip;

    void Start ()
    {
        #region Common Variable Setup

        Player = FindObjectOfType<Hero>();
        Anim = zombie.GetComponent<Animator>();
        Ent = zombie.GetComponent<Entity>();
        Rigid = zombie.GetComponent<Rigidbody>();

        PositionY = zombie.transform.position.y;

        IsActive = false;
        MaxAttackNumber = 2;
        KnockbackPowerHeight = 2f;
        KnockbackPowerLength = 4.5f;

        MovementSpeed = 0.05f;
        AttackSpeed = 1;
        StunLength = 2;
        AttackWaitTime = 2;

        ExperienceEndowment = 5;

        SetInitialValues(soundClip);

        #region Base Attribute Setter

        Level = 1;

        Power = 15;
        Magic = 0;
        Defense = 5;
        MagicResist = 0;
        Block = 0;
        Vitality = 50;

        MaxHealth = Vitality;
        Health = MaxHealth;

        #endregion Base Attribute Setter

        #region Subscribe to Events

        EventManager.OnMove += Hunt;

        #endregion Subscribe to Events

        #endregion Common Variable Setup

        #region Set Zombie-Specific Variables

        Weapon = new BoxCollider[2];
        Weapon[0] = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").GetComponent<BoxCollider>();
        Weapon[1] = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R").GetComponent<BoxCollider>();
        WeaponTrail = new TrailRenderer[2];
        WeaponTrail[0] = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/WeaponTrail").GetComponent<TrailRenderer>();
        WeaponTrail[0].startWidth = 0.3f;
        WeaponTrail[0].endWidth = 0.0001f;
        WeaponTrail[1] = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L/WeaponTrail").GetComponent<TrailRenderer>();
        WeaponTrail[1].startWidth = 0.3f;
        WeaponTrail[1].endWidth = 0.0001f;

        SetWeapon(false);

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        AttackAnimTimes = new float[1];
        AttackAnimTimes[0] = clip.First(a => a.name == "Attack").length / AttackSpeed;
        WindUpAnimTimes = new float[1];
        WindUpAnimTimes[0] = AttackAnimTimes[0] / 2;

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
                if (!IsMoving) SetMoving(true);

                zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                zombie.transform.position += zombie.transform.forward * MovementSpeed;
            }

            DecrementAttackLockTimer();
            DecrementWindUpLockTimer();
            DecrementAttackTimer();
            DecrementJumpTimer();
            DecrementMoveTimer();
            DecrementStunTimer();
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
        TriggerEnter(zombie, other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay(zombie, other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit(zombie, other);
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
            SetWeapon(false);
            AttackTimer = -1;
            AttackLockTimer = -1;
            AttackCount = 0;
            Anim.SetInteger("AttackNumber", AttackCount);
        }
    }

    #endregion
}