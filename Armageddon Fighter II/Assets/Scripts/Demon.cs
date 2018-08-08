//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Demon : Entity
{
    public GameObject demon;

    public AudioClip[] soundClip;

    void Start()
    {
        #region Common Variable Setup

        Player = FindObjectOfType<Hero>();
        Anim = demon.GetComponent<Animator>();
        Ent = demon.GetComponent<Entity>();
        Rigid = demon.GetComponent<Rigidbody>();

        PositionY = demon.transform.position.y;

        IsActive = false;
        MaxAttackNumber = 1;
        KnockbackPowerHeight = 1f;
        KnockbackPowerLength = 2.25f;

        MovementSpeed = 0.03f;
        AttackSpeed = 3.5f;
        StunLength = 2;
        AttackWaitTime = 2;

        ExperienceEndowment = 50;

        SetInitialValues(soundClip);

        #region Base Attribute Setter

        Level = 5;

        Power = 40;
        Magic = 0;
        Defense = 25;
        MagicResist = 0;
        Block = 0;
        Vitality = 100;

        MaxHealth = Vitality;
        Health = MaxHealth;

        #endregion Base Attribute Setter

        #region Subscribe to Events

        EventManager.OnMove += Hunt;

        #endregion Subscribe to Events

        #endregion Common Variable Setup

        #region Set Zombie-Specific Variables

        Weapon = new BoxCollider[1];
        Weapon[0] = demon.transform.Find("Demon/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/Mace").GetComponent<BoxCollider>();
        WeaponTrail = new TrailRenderer[1];
        WeaponTrail[0] = demon.transform.Find("Demon/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/Mace/MaceTrail").GetComponent<TrailRenderer>();
        WeaponTrail[0].startWidth = 0.3f;
        WeaponTrail[0].endWidth = 0.0001f;

        SetWeapon(false);

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        AttackAnimTimes = new float[2];
        AttackAnimTimes[0] = clip.First(a => a.name == "Attack").length / AttackSpeed;
        AttackAnimTimes[1] = clip.First(a => a.name == "Attack_Hold").length / 2;
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
                if (!IsMoving) SetMoving(true);

                demon.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                demon.transform.position += demon.transform.forward * MovementSpeed;
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
        TriggerEnter(demon, other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay(demon, other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit(demon, other);
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

    protected override void TriggerEnter(GameObject fighter, Collider other)
    {
        if (!IsDead && !Player.IsDead)
        {
            if (other.gameObject.tag == "Player" && fighter.GetComponent<SphereCollider>().enabled)
            {
                fighter.GetComponent<SphereCollider>().enabled = false;
                fighter.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                IsActive = true;
                SetMoving(true);
            }
            else if (other.gameObject.tag == "Boundary" && IsActive)
            {
                fighter.GetComponent<SphereCollider>().enabled = true;
                IsActive = false;

                SetMoving(false);

                if (IsAttacking) SetAttacking(false);
            }
            else if (other.gameObject.tag == "Player" && !IsKnockedDown && !InAir && AttackLockTimer == -1 && StunTimer == -1)
            {
                if (AttackLockTimer == -1) fighter.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));

                WindUpLockTimer = WindUpAnimTimes[0];

                AttackLockTimer = AttackAnimTimes[0] + AttackAnimTimes[1];

                AttackTimer = AttackLockTimer + 0.5f;
                MoveTimer = AttackTimer;
                
                SetMoving(false);
                SetAttacking(true);

                AttackCount = MaxAttackNumber;
                Anim.SetInteger("AttackNumber", AttackCount);

                Audio.PlayOneShot(SoundClip[0]);
            }
            else if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
                if (IsAttacking) SetAttacking(false);
            }
            else if (other.gameObject.tag == "DeathBoundary")
            {
                Player.GiveExperience(ExperienceEndowment * 2);

                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag == "Ground")
            {
                InAir = false;
                Rigid.isKinematic = true;

                Audio.PlayOneShot(SoundClip[2]);

                Player.GetComponent<Animator>().SetBool("Win", true);
            }
        }
    }

    protected override void TriggerStay(GameObject fighter, Collider other)
    {
        if (IsActive && !IsDead && !Player.IsDead)
        {
            if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
            }
            else if (other.gameObject.tag == "Player" && !IsKnockedDown && AttackTimer == -1 && StunTimer == -1)
            {
                if (AttackLockTimer == -1) fighter.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));

                WindUpLockTimer = WindUpAnimTimes[0];

                AttackLockTimer = AttackAnimTimes[0] + AttackAnimTimes[1];

                AttackTimer = AttackLockTimer + 0.5f;
                MoveTimer = AttackTimer;

                SetMoving(false);
                SetAttacking(true);

                AttackCount = MaxAttackNumber;
                Anim.SetInteger("AttackNumber", AttackCount);

                Audio.PlayOneShot(SoundClip[0]);
            }
        }
    }

    protected override void TriggerExit(GameObject fighter, Collider other)
    {
        if (!IsDead)
        {
            if (other.gameObject.tag == "Player" && !IsKnockedDown)
            {
                if (IsAttacking) SetAttacking(false);
            }
        }
        else
        {
            if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
            {
                Rigid.isKinematic = false;
            }
        }
    }

    #endregion
}