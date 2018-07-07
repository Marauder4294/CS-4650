//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Entity
{
    public GameObject zombie;

    BoxCollider leftHand;
    BoxCollider rightHand;

    readonly int?[] attackAnimTimes = new int?[1];

    void Awake ()
    {
        EventManager.OnMove += Hunt;

        MovementSpeed = 0.05f;

        Player = FindObjectOfType<Hero>();
        Rigid = zombie.GetComponent<Rigidbody>();
        Anim = zombie.GetComponent<Animator>();
        Ent = zombie.GetComponent<Entity>();

        PositionY = zombie.transform.position.y;

        IsActive = false;
        IsMoving = false;
        IsAttacking = false;
        IsKnockedDown = false;
        IsFallingBack = false;
        NextAttack = false;
        AttackCount = 0;

        MoveTimer = 0;
        AttackTimer = 0;
        StunTimer = 0;
        FallBackTimer = 0;

        #region Base Attribute Setter

        Power = 5;
        Magic = 0;
        Defense = 5;
        MagicResist = 0;
        Block = 10;
        Vitality = 50;

        MaxHealth = Vitality;
        Health = MaxHealth;

        #endregion

        MaxAttackNumber = 2;

        KnockbackPowerHeight = 2f;
        KnockbackPowerLength = 4.5f;

        leftHand = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").GetComponent<BoxCollider>();
        rightHand = zombie.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R").GetComponent<BoxCollider>();

        leftHand.enabled = false;
        rightHand.enabled = false;

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        attackAnimTimes[0] = (int)(clip.First(a => a.name == "Attack").length * 24);
    }

    void Hunt(float moveX, float moveY)
    {
        if (IsActive && IsMoving && !IsAttacking && MoveTimer == 0 && !IsKnockedDown)
        {
            Anim.SetBool("Moving", true);
            zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
            zombie.transform.position += zombie.transform.forward * MovementSpeed;
        }
        else if (IsAttacking)
        {
            if (Mathf.Abs(Player.transform.position.x - zombie.transform.position.x) <= 2f && Mathf.Abs(Player.transform.position.z - zombie.transform.position.z) <= 2f)
            {
                zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                Anim.SetBool("Attacking", (StunTimer == 0) ? true : false);
                MoveTimer = (MoveTimer > 0) ? --MoveTimer : 60;
            }
            else
            {
                IsAttacking = false;
                IsMoving = true;
                Anim.SetBool("Attacking", false);
                Anim.SetBool("Moving", false);
            }
        }

        DecrementAttackLockTimer();
        DecrementAttackTimer();
        DecrementJumpTimer();
        DecrementKnockDownTimer();
        DecrementMoveTimer();
        DecrementStunTimer();
        Fallback();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && zombie.GetComponent<SphereCollider>().enabled)
        {
            zombie.GetComponent<SphereCollider>().enabled = false;
            IsActive = true;
            IsMoving = true;
        }
        else if (other.gameObject.tag == "Player" && !IsFallingBack && AttackLockTimer == 0)
        {
            leftHand.enabled = true;
            rightHand.enabled = true;

            AttackLockTimer = attackAnimTimes[0] ?? 0;

            AttackTimer = AttackLockTimer + 24;

            IsMoving = false;
            IsAttacking = (StunTimer == 0) ? true : false;
            zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
            Anim.SetBool("Moving", false);
            Anim.SetBool("Attacking", IsAttacking);
            MoveTimer = 60;

            AttackCount = (AttackCount < 2) ? ++AttackCount : 0; 
        }
        else if (other.gameObject.tag == "HeroSword" && !GetComponent<SphereCollider>().enabled && !IsKnockedDown)
        {
            Damage(Player.Ent, false);
            Stun();

            if (Player.Ent.AttackCount >= Player.MaxAttackNumber)
                KnockBack();
        }
        else if (other.gameObject.tag == "Ground")
        {
            InAir = false;
            Rigid.isKinematic = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
        {
            Rigid.isKinematic = true;
        }
        else if (other.gameObject.tag == "Player" && !IsFallingBack && AttackLockTimer == 0)
        {
            leftHand.enabled = true;
            rightHand.enabled = true;

            AttackLockTimer = attackAnimTimes[0] ?? 0;

            AttackTimer = AttackLockTimer + 24;

            IsMoving = false;
            IsAttacking = (StunTimer == 0) ? true : false;
            zombie.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
            Anim.SetBool("Moving", false);
            Anim.SetBool("Attacking", IsAttacking);
            MoveTimer = 60;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
        {
            Rigid.isKinematic = false;
        }
    }

    #region Entity Method Overrides

    protected override void DecrementAttackTimer()
    {
        if (AttackTimer != 1)
        {
            --AttackTimer;
        }
        else if (AttackTimer == 1)
        {
            leftHand.enabled = false;
            rightHand.enabled = false;
            AttackTimer = 0;
            AttackLockTimer = 0;
            AttackCount = 0;
            Anim.SetInteger("AttackNumber", AttackCount);
        }
    }

    #endregion
}
