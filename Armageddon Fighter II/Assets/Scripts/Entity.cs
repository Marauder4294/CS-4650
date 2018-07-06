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

    protected bool IsActive { get; set; }
    protected bool IsGateKeeper { get; set; }
    protected Animator Anim { get; set; }
    protected Rigidbody Rigid { get; set; }

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
    protected int MoveTimer { get; set; }
    protected bool IsMoving { get; set; }
    protected bool IsAvoiding { get; set; }

    #endregion

    #region Combat Fields

    protected bool IsAttacking { get; set; }
    protected bool IsBlocking { get; set; }
    protected int AttackTimer { get; set; }
    protected int AttackLockTimer { get; set; }
    protected int StunTimer { get; set; }

    #endregion

    #region Speed Modifiers

    protected float AttackSpeed { get; set; }
    protected float MovementSpeed { get; set; }

    #endregion

    #region Jump/Knockback Fields

    #region Jump

    protected float JumpPower { get; set; }
    protected float JumpHeight { get; set; }
    protected int JumpTimer { get; set; }
    protected bool IsJumping { get; set; }

    #endregion

    protected bool InAir { get; set; }
    protected bool IsGoingUp { get; set; }
    protected float PositionY { get; set; }

    #region Knockback

    protected float KnockbackPowerLength { get; set; }
    protected float KnockbackPowerHeight { get; set; }
    protected int KnockDownTimer { get; set; }
    protected int FallBackTimer { get; set; }
    protected bool IsFallingBack { get; set; }
    protected bool IsKnockedDown { get; set; }

    #endregion

    #endregion

    #endregion

    #region Universal Methods

    protected void Damage(Entity other, bool isMagic)
    {
        // TODO change other.Power and other.Magic to damage

        if (!isMagic)
        {
            Health = Health - ((other.Power > Defense) ? (other.Power - Defense) : 1);
        }
        else
        {
            Health = (MagicResist != 0) ? ((int)(Health - other.Magic * ((100 - (float)MagicResist) / 100))) : (Health - other.Magic);
        }
    }

    #endregion
}
