//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    #region Attributes

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

    public int moveTimer { get; set; }
    public int attackTimer { get; set; }
    public int stunTimer { get; set; }
    public int knockDownTimer { get; set; }

    public bool inAir { get; set; }
    public bool isAttacking { get; set; }
    public bool isKnockedDown { get; set; }
    public float positionY { get; set; }
    #endregion
}
