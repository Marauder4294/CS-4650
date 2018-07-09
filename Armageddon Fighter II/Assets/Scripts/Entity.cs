//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    #region Public Fields

    public Entity Ent { get; set; }
    public int AttackCount { get; set; }
    public int MaxAttackNumber { get; set; }

    #endregion

    #region Protected Fields

    protected Hero Player { get; set; }
    protected bool IsActive { get; set; }
    protected bool IsGateKeeper { get; set; }
    protected Animator Anim { get; set; }
    protected Rigidbody Rigid { get; set; }
    protected int Level { get; set; }

    #region Attributes

    protected int Power { get; set; }
    protected int Magic { get; set; }
    protected int Defense { get; set; }
    protected int Block { get; set; }
    protected int MagicResist { get; set; }
    protected int Vitality { get; set; }

    protected int MaxHealth { get; set; }
    protected int Health { get; set; }

    protected int MaxMana { get; set; }
    protected int Mana { get; set; }

    #endregion

    #region Movement Fields

    protected Vector3 movement;
    protected float MoveTimer { get; set; }
    protected bool IsMoving { get; set; }
    protected bool IsAvoiding { get; set; }

    #endregion

    #region Combat Fields

    protected bool IsAttacking { get; set; }
    protected bool NextAttack { get; set; }
    protected bool IsBlocking { get; set; }
    protected float AttackTimer { get; set; }
    protected float AttackLockTimer { get; set; }
    protected float StunTimer { get; set; }
    public bool IsDead { get; set; }
    protected float DeathTimer { get; set; }

    #endregion

    #region Speed Modifiers

    protected float AttackSpeed { get; set; }
    protected float MovementSpeed { get; set; }

    #endregion

    #region Jump/Knockback Fields

    #region Jump

    protected float JumpPower { get; set; }
    protected float JumpHeight { get; set; }
    protected float JumpTimer { get; set; }
    protected bool IsJumping { get; set; }

    #endregion

    protected bool InAir { get; set; }
    protected bool IsGoingUp { get; set; }
    protected float PositionY { get; set; }

    #region Knockback

    protected float KnockbackPowerLength { get; set; }
    protected float KnockbackPowerHeight { get; set; }
    protected float KnockDownTimer { get; set; }
    protected float FallBackTimer { get; set; }
    protected bool IsFallingBack { get; set; }
    protected bool IsKnockedDown { get; set; }

    #endregion

    #endregion

    #endregion

    #region Universal Methods

    public void Damage(Entity other, bool isMagic)
    {
        // TODO change other.Power and other.Magic to damage

        if (other.AttackCount >= other.MaxAttackNumber)
        {
            KnockBack(other);
        }

        if (!IsBlocking)
        {
            if (!isMagic)
            {
                Health = Health - ((other.Power > Defense) ? (other.Power - Defense) : 1);
            }
            else
            {
                Health = (MagicResist != 0) ? (int)(Health - other.Magic * ((100f - MagicResist) / 100f)) : Health - other.Magic;
            }

            Stun();
            TakeHealth();

            if (Health <= 0)
            {
                Death(other);
            }
        }
    }

    protected void Stun()
    {
        Anim.SetBool("Attacking", false);
        StunTimer = 2;
    }

    protected virtual void Death(Entity other)
    {
        if (!IsKnockedDown)
        {
            KnockBack(other);
        }

        DeathTimer = 5;
        IsDead = true;
        Anim.SetBool("Dead", IsDead);
    }

    public void KnockBack(Entity other)
    {
        IsKnockedDown = true;
        IsFallingBack = true;
        IsAttacking = false;
        IsMoving = false;
        IsGoingUp = true;
        Anim.SetBool("KnockedDown", IsKnockedDown);
        Anim.SetBool("Attacking", IsAttacking);
        Anim.SetBool("Moving", IsMoving);

        if (IsBlocking)
        {
            IsBlocking = false;
            Anim.SetBool("Blocking", IsBlocking);
        }

        movement = new Vector3(0.15f, 0.15f, 0.15f);

        if (KnockbackPowerLength >= KnockbackPowerHeight)
        {
            FallBackTimer = KnockbackPowerLength * movement.x;
        }
        else
        {
            FallBackTimer = KnockbackPowerHeight * movement.y;
        }

        transform.LookAt(new Vector3(other.transform.position.x, PositionY, other.transform.position.z));
        transform.position += new Vector3(-transform.forward.x * movement.x, movement.y, -transform.forward.z * movement.z);

        KnockDownTimer = 5;
    }

    protected virtual void TakeHealth()
    {
        
    }

    protected virtual void DecrementStunTimer()
    {
        if (StunTimer > 0)
        {
            StunTimer -= Time.deltaTime;
        }
        else if (StunTimer > -1)
        {
            StunTimer = -1;
        }
    }

    protected virtual void DecrementAttackTimer()
    {
        if (AttackTimer > 0)
        {
            AttackTimer -= Time.deltaTime;
        }
        else if (AttackTimer > -1)
        {
            AttackTimer = -1;
            AttackLockTimer = -1;
            AttackCount = 0;
            Anim.SetInteger("AttackNumber", AttackCount);
        }
    }

    protected virtual void DecrementAttackLockTimer()
    {
        if (AttackLockTimer > 0)
        {
            AttackLockTimer -= Time.deltaTime;
        }
        else if (AttackLockTimer > -1)
        {
            AttackLockTimer = -1;
            IsAttacking = false;
            Anim.SetBool("Attacking", IsAttacking);
        }
    }

    protected virtual void DecrementMoveTimer()
    {
        if (MoveTimer > 0)
        {
            MoveTimer -= Time.deltaTime;
        }
        else if (MoveTimer > -1)
        {
            MoveTimer = -1;
        }
    }

    protected virtual void DecrementKnockDownTimer()
    {
        if (KnockDownTimer > 0)
        {
            KnockDownTimer -= Time.deltaTime;
        }
        else if (KnockDownTimer > -1)
        {
            KnockDownTimer = -1;
            IsKnockedDown = false;
            Anim.SetBool("KnockedDown", IsKnockedDown);
            MoveTimer = 1;
            IsMoving = true;
        }
    }

    protected virtual void DecrementJumpTimer()
    {
        if (JumpTimer > 0)
        {
            JumpTimer -= Time.deltaTime;
        }
        else if (JumpTimer > -1)
        {
            JumpTimer = -1;
        }
    }

    protected virtual void JumpUp()
    {
        if (InAir == true && IsGoingUp == true)
        {
            if (Ent.transform.position.y >= JumpHeight)
            {
                IsGoingUp = false;
                movement.y = 0;
            }
            else
            {
                Ent.transform.position += movement;
            }
        }
    }

    protected virtual void Fallback()
    {
        if (IsFallingBack)
        {
            if (IsGoingUp)
            {
                if (Ent.transform.position.y >= KnockbackPowerHeight)
                {
                    IsGoingUp = false;
                    movement.y = 0;
                }
            }

            if (FallBackTimer > 0)
            {
                FallBackTimer -= Time.deltaTime;
                transform.position += new Vector3(-transform.forward.x * movement.x, movement.y, -transform.forward.z * movement.z);
            }
            else if (FallBackTimer > -1)
            {
                IsFallingBack = false;
                FallBackTimer = -1;
                movement.x = 0;
                movement.z = 0;
            }
        }
    }

    protected virtual void DecrementDeathTimer()
    {
        if (DeathTimer > 0)
        {
            DeathTimer -= Time.deltaTime;
        }
        else if (DeathTimer > -1)
        {
            DeathTimer = -1;
            Destroy(Ent.gameObject);
        }
    }

    #endregion
}
