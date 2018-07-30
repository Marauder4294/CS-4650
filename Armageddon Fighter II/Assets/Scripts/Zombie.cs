//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Entity
{
    public GameObject zombie;

    void Awake ()
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

        SetInitialValues();

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
            //else if (IsAttacking && AttackTimer == -1)
            //{
            //    if (Mathf.Abs(Player.transform.position.x - zombie.transform.position.x) <= 2f && Mathf.Abs(Player.transform.position.z - zombie.transform.position.z) <= 2f)
            //    {
            //        zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
            //        Anim.SetBool("Attacking", (StunTimer == 0) ? true : false);
            //        MoveTimer = (MoveTimer > 0) ? -Time.deltaTime : 3;
            //    }
            //    else
            //    {
            //        SetAttacking(false);
            //        SetMoving(false);
            //    }
            //}

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
        if (!IsDead && !Player.IsDead)
        {
            if (other.gameObject.tag == "Player" && zombie.GetComponent<SphereCollider>().enabled)
            {
                zombie.GetComponent<SphereCollider>().enabled = false;
                zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                IsActive = true;
                SetMoving(true);
            }
            else if (other.gameObject.tag == "Boundary" && IsActive)
            {
                zombie.GetComponent<SphereCollider>().enabled = true;
                IsActive = false;

                SetMoving(false);

                if (IsAttacking) SetAttacking(false);
            }
            else if (other.gameObject.tag == "Player" && !IsKnockedDown && AttackLockTimer == -1 && StunTimer == -1)
            {
                AttackLockTimer = AttackAnimTimes[0];
                WindUpLockTimer = WindUpAnimTimes[0];

                AttackTimer = AttackLockTimer + 2;
                MoveTimer = AttackTimer;

                zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                SetMoving(false);
                SetAttacking(true);

                AttackCount = (AttackCount < MaxAttackNumber) ? ++AttackCount : 0;
            }
            else if (other.gameObject.tag == "Ground")
            {
                InAir = false;
                Rigid.isKinematic = true;
            }
            else if (other.gameObject.tag == "DeathBoundary")
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag == "Ground")
            {
                InAir = false;
                Rigid.isKinematic = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsActive && !IsDead && !Player.IsDead)
        {
            if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
            }
            else if (other.gameObject.tag == "Player" && !IsKnockedDown && AttackTimer == -1 && StunTimer == -1)
            {
                WindUpLockTimer = WindUpAnimTimes[0];
                AttackLockTimer = AttackAnimTimes[0];

                AttackTimer = AttackLockTimer + 1;

                SetMoving(false);

                if (StunTimer == -1)
                {
                    if (!IsAttacking) SetAttacking(true);
                }
                else
                {
                    if (IsAttacking) SetAttacking(false);
                }

                zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                MoveTimer = 3;

                AttackCount = (AttackCount < 2) ? ++AttackCount : 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsDead)
        {
            if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
            {
                Rigid.isKinematic = false;
            }
            else if (other.gameObject.tag == "Player" && !IsKnockedDown)
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
