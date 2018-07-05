//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    #region Attributes

    public Animator animator { get; set; }
    public Entity entity { get; set; }
    public Rigidbody rigidBody { get; set; }

    public int power { get; set; }
    public int magic { get; set; }
    public int defense { get; set; }
    public int block { get; set; }
    public int magicResist { get; set; }
    public int vitality { get; set; }

    public int maxHealth { get; set; }
    public int health { get; set; }

    public int maxMana { get; set; }
    public int mana { get; set; }

    public int attackCount { get; set; }
    public int maxAttackNumber { get; set; }

    public Vector3 movement;

    public int moveTimer { get; set; }
    public int attackTimer { get; set; }
    public int attackLockTimer { get; set; }
    public int stunTimer { get; set; }
    public int knockDownTimer { get; set; }
    public int fallBackTimer { get; set; }

    public bool isActive { get; set; }
    public bool inAir { get; set; }
    public bool isGoingUp { get; set; }
    public bool isAttacking { get; set; }
    public bool isMoving { get; set; }
    public bool isAvoiding { get; set; }
    public bool isJumping { get; set; }
    public bool isBlocking { get; set; }
    public bool isFallingBack { get; set; }
    public bool isKnockedDown { get; set; }

    public float positionY { get; set; }

    public float knockbackPowerLength { get; set; }
    public float knockbackPowerHeight { get; set; }

    public float attackSpeed { get; set; }
    public float movementSpeed { get; set; }

    public float jumpPower { get; set; }
    public float jumpHeight { get; set; }
    public int jumpTimer { get; set; }

    #endregion

    public void OnDamage(Entity entity, int damage, bool isMagic, int attackNumber, int maxAttackNumber)
    {
        if (!isMagic)
        {
            entity.health = entity.health - ((damage < entity.defense) ? (damage - entity.defense) : 1);
        }
        else
        {
            entity.health = (entity.magicResist != 0) ? ((int)(entity.health - damage * ((100 - (float)entity.magicResist) / 100))) : (entity.health - damage);
        }
    }
}
