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
    protected float[] AttackAnimTimes { get; set; }
    protected float[] WindUpAnimTimes { get; set; }
    protected BoxCollider[] Weapon { get; set; }
    protected BoxCollider Shield { get; set; }
    protected TrailRenderer[] WeaponTrail { get; set; }
    protected AudioSource Audio { get; set; }
    protected AudioClip[] SoundClip { get; set; }

    #region Attributes

    protected int Level { get; set; }

    protected int Power { get; set; }
    protected int Magic { get; set; }
    protected int Defense { get; set; }
    protected int Block { get; set; }
    protected int MagicResist { get; set; }
    protected int Vitality { get; set; }

    #region Speed Modifiers

    protected float AttackSpeed { get; set; }
    protected float MovementSpeed { get; set; }

    #endregion

    protected int MaxHealth { get; set; }
    protected int Health { get; set; }

    protected int MaxMana { get; set; }
    protected int Mana { get; set; }

    protected int MagicOneCost { get; set; }
    protected int MagicTwoCost { get; set; }
    protected int MagicThreeCost { get; set; }

    #endregion

    #region Movement Fields

    protected Vector3 movement;
    protected float MoveTimer { get; set; }
    protected bool IsMoving { get; set; }
    protected bool IsAvoiding { get; set; }
    protected bool IsTouchingBoundary { get; set; }

    #endregion

    #region Combat Fields

    protected int AttackDamage { get; set; }
    protected bool IsAttacking { get; set; }
    protected bool NextAttack { get; set; }
    protected bool IsBlocking { get; set; }
    protected float AttackTimer { get; set; }
    protected float AttackLockTimer { get; set; }
    protected float AttackWaitTime { get; set; }
    protected float WindUpLockTimer { get; set; }
    protected float StunTimer { get; set; }
    protected float StunLength { get; set; }
    public bool IsDead { get; set; }
    protected float DeathTimer { get; set; }
    protected float ExperienceEndowment { get; set; }

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
                if (gameObject.tag != "Player") Player.GiveExperience(ExperienceEndowment);

                Death(other);
            }
            else
            {
                Audio.PlayOneShot(SoundClip[1]);
            }
        }
    }

    public void MagicDamage(Entity other, string type)
    {
        Health = (MagicResist != 0) ? (int)(Health - other.Magic * ((100f - MagicResist) / 100f)) : Health - other.Magic;

        Stun();
        TakeHealth();

        if (Health <= 0)
        {
            if (gameObject.tag != "Player") Player.GiveExperience(ExperienceEndowment);

            Death(other);
        }
        else
        {
            Audio.PlayOneShot(SoundClip[1]);
        }
    }

    protected void SetInitialValues(AudioClip[] sound)
    {
        JumpPower = 3f;
        JumpHeight = JumpPower;

        AttackCount = 0;
        MoveTimer = -1;
        AttackTimer = -1;
        AttackLockTimer = -1;
        StunTimer = -1;
        JumpTimer = -1;
        DeathTimer = -1;
        FallBackTimer = -1;
        KnockDownTimer = -1;
        WindUpLockTimer = -1;

        SetMoving(false);
        SetAvoiding(false);
        SetAttacking(false);
        SetBlocking(false);
        SetJumping(false);
        SetKnockedDown(false);
        SetDead(false);

        InAir = false;
        IsFallingBack = false;
        NextAttack = false;
        IsTouchingBoundary = false;

        Audio = Camera.main.GetComponent<AudioSource>();
        //SoundClip = new AudioClip[sound.Length];
        SoundClip = sound;

        Anim.SetBool("Landing", false);
        Anim.SetFloat("AttackSpeed", AttackSpeed);
        Anim.SetFloat("MoveSpeed", MovementSpeed);
        Anim.SetInteger("AttackNumber", AttackCount);
    }

    protected void Stun()
    {
        if (Weapon[0].enabled) SetWeapon(false);
        SetAttacking(false);
        SetMoving(false);
        StunTimer = StunLength;
    }

    protected virtual void Death(Entity other)
    {
        if (!IsKnockedDown)
        {
            KnockBack(other);
        }

        DeathTimer = 5;
        SetDead(true);
    }

    public void KnockBack(Entity other)
    {
        SetKnockedDown(true);
        SetAttacking(false);
        SetMoving(false);
        IsFallingBack = true;
        IsGoingUp = true;

        if (IsBlocking) SetBlocking(false);

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

    protected void SetMoving(bool IsAction)
    {
        IsMoving = IsAction;
        Anim.SetBool("Moving", IsAction);
    }

    protected void SetAttacking(bool IsAction)
    {
        IsAttacking = IsAction;
        Anim.SetBool("Attacking", IsAction);
    }

    protected void SetKnockedDown(bool IsAction)
    {
        IsKnockedDown = IsAction;
        Anim.SetBool("KnockedDown", IsAction);
    }

    protected void SetJumping(bool IsAction)
    {
        InAir = IsAction;
        Anim.SetBool("Jumping", IsAction);
    }

    protected void SetBlocking(bool IsAction)
    {
        IsBlocking = IsAction;
        Anim.SetBool("Blocking", IsAction);

        if (Shield != null) Shield.enabled = IsAction;
    }

    protected void SetAvoiding(bool IsAction)
    {
        IsAvoiding = IsAction;
        Anim.SetBool("Avoiding", IsAction);
    }

    protected void SetDead(bool IsAction)
    {
        IsDead = IsAction;
        Anim.SetBool("Dead", IsAction);
    }

    protected void SetWeapon(bool isAction)
    {
        foreach (BoxCollider weapon in Weapon)
        {
            weapon.enabled = isAction;
        }

        foreach (TrailRenderer weaponTrail in WeaponTrail)
        {
            weaponTrail.emitting = isAction;
        }
    }

    protected virtual void TakeHealth()
    {
        
    }

    protected virtual void TakeMana(int cost)
    {

    }

    protected virtual void TriggerEnter(GameObject fighter, Collider other)
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
                AttackLockTimer = AttackAnimTimes[0];
                WindUpLockTimer = WindUpAnimTimes[0];

                AttackTimer = AttackLockTimer;
                MoveTimer = AttackTimer;

                fighter.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                SetMoving(false);
                SetAttacking(true);

                AttackCount = (AttackCount < MaxAttackNumber) ? ++AttackCount : 0;
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
                if (gameObject.tag != "Player") Player.GiveExperience(ExperienceEndowment * 2);

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
            }
        }
    }

    protected virtual void TriggerStay(GameObject fighter, Collider other)
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

                AttackCount = (AttackCount < MaxAttackNumber && AttackAnimTimes.Length - 1 >= AttackCount + 1) ? ++AttackCount : 0;

                AttackLockTimer = AttackAnimTimes[AttackCount];

                AttackTimer = AttackLockTimer + AttackWaitTime;

                if (IsMoving) SetMoving(false);

                if (!IsAttacking) SetAttacking(true);

                fighter.transform.LookAt(new Vector3(Player.transform.position.x, PositionY, Player.transform.position.z));
                MoveTimer = 3;
                
                Anim.SetInteger("AttackNumber", AttackCount);

                Audio.PlayOneShot(SoundClip[0]);
            }
        }
    }

    protected virtual void TriggerExit(GameObject fighter, Collider other)
    {
        if (other.gameObject.tag == "Player" && !IsKnockedDown)
        {
            if (IsAttacking) SetAttacking(false);
        }

        if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
        {
            Rigid.isKinematic = false;
        }
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
            SetWeapon(false);
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
            SetAttacking(false);
            SetWeapon(false);
        }
    }

    protected virtual void DecrementWindUpLockTimer()
    {
        if (WindUpLockTimer > 0)
        {
            WindUpLockTimer -= Time.deltaTime;
        }
        else if (WindUpLockTimer > -1)
        {
            WindUpLockTimer = -1;
            SetWeapon(true);
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
            SetKnockedDown(false);
            MoveTimer = 1;
            SetMoving(true);
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
                    Rigid.isKinematic = false;
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