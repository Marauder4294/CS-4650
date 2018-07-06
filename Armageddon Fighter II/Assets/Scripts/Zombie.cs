//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Entity
{
    Hero hero;
    public GameObject enemy;

    BoxCollider leftHand;
    BoxCollider rightHand;

    readonly int?[] attackAnimTimes = new int?[1];

    void Awake ()
    {
        EventManager.OnMove += Hunt;

        MovementSpeed = 0.05f;

        hero = FindObjectOfType<Hero>();
        Rigid = enemy.GetComponent<Rigidbody>();
        Anim = enemy.GetComponent<Animator>();
        Ent = enemy.GetComponent<Entity>();

        PositionY = enemy.transform.position.y;

        IsActive = false;
        IsMoving = false;
        IsAttacking = false;
        IsKnockedDown = false;
        IsFallingBack = false;
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

        KnockbackPowerHeight = 2f;
        KnockbackPowerLength = 4.5f;

        leftHand = enemy.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").GetComponent<BoxCollider>();
        rightHand = enemy.transform.Find("Zombie/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R").GetComponent<BoxCollider>();

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
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, PositionY, hero.transform.position.z));
            enemy.transform.position += enemy.transform.forward * MovementSpeed;
        }
        else if (IsAttacking)
        {
            if (Mathf.Abs(hero.transform.position.x - enemy.transform.position.x) <= 2f && Mathf.Abs(hero.transform.position.z - enemy.transform.position.z) <= 2f)
            {
                enemy.transform.LookAt(new Vector3(hero.transform.position.x, PositionY, hero.transform.position.z));
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

        if (IsFallingBack)
        {
            if (IsGoingUp)
            {
                if (enemy.transform.position.y >= KnockbackPowerHeight)
                {
                    IsGoingUp = false;
                    movement.y = 0;
                }
            }

            if (FallBackTimer > 0)
            {
                --FallBackTimer;
                enemy.transform.position -= new Vector3(enemy.transform.forward.x * movement.x, movement.y, enemy.transform.forward.z * movement.z);
            }
            else
            {
                IsFallingBack = false;
            }
        }

        if (StunTimer > 0)
            --StunTimer;

        if (AttackTimer != 1)
        {
            --AttackTimer;
        }
        else if (AttackTimer == 1)
        {
            AttackTimer = 0;
            IsAttacking = false;
            Anim.SetBool("Attacking", IsAttacking);
            leftHand.enabled = false;
            rightHand.enabled = false;
        }

        if (AttackLockTimer > 0)
            --AttackLockTimer;

        if (MoveTimer > 0)
            --MoveTimer;

        if (KnockDownTimer > 1)
        {
            --KnockDownTimer;
        }
        else if (KnockDownTimer == 1)
        {
            KnockDownTimer = 0;
            Anim.SetBool("KnockedDown", false);
            MoveTimer = 30;
            IsKnockedDown = false;
            IsMoving = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && enemy.GetComponent<SphereCollider>().enabled)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            IsActive = true;
            IsMoving = true;
        }
        else if (other.gameObject.tag == "Player" && !IsFallingBack && AttackLockTimer == 0)
        {
            leftHand.enabled = true;
            rightHand.enabled = true;

            AttackTimer = attackAnimTimes[0] ?? 0;

            AttackLockTimer = AttackTimer + 24;

            IsMoving = false;
            IsAttacking = true;
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, PositionY, hero.transform.position.z));
            Anim.SetBool("Moving", false);
            Anim.SetBool("Attacking", (StunTimer == 0) ? true : false);
            MoveTimer = 60;
        }
        else if (other.gameObject.tag == "HeroSword" && !GetComponent<SphereCollider>().enabled && !IsKnockedDown)
        {
            Damage(hero.Ent, false);
            Stun();

            if (hero.Ent.AttackCount >= hero.MaxAttackNumber)
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
        {
            Rigid.isKinematic = false;
        }
    }

    void Stun()
    {
        Anim.SetBool("Attacking", false);
        StunTimer = 45;
    }

    void KnockBack()
    {
        IsKnockedDown = true;
        IsFallingBack = true;
        IsGoingUp = true;
        Anim.SetBool("KnockedDown", true);
        Anim.SetBool("Attacking", false);
        Anim.SetBool("Moving", false);

        movement = new Vector3(0.15f, -0.15f, 0.15f);

        if (KnockbackPowerLength >= KnockbackPowerHeight)
        {
            FallBackTimer = (int)(KnockbackPowerLength / movement.x);
        }
        else
        {
            FallBackTimer = (int)(KnockbackPowerHeight / movement.y);
        }

        enemy.transform.position -= new Vector3(enemy.transform.forward.x * movement.x, movement.y, enemy.transform.forward.z * movement.z);

        KnockDownTimer = 200;
    }
}
