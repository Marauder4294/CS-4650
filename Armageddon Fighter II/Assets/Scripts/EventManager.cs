//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class EventManager {

    #region Event Declarations - declare delegates and events here

    public delegate void HeroBoolEvent(bool isAction);
    public static event HeroBoolEvent OnAttack;
    public static event HeroBoolEvent OnJump;
    public static event HeroBoolEvent OnBlock;
    public static event HeroBoolEvent OnLightning;

    public delegate void HeroMovementEvent(float moveX, float moveY);
    public static event HeroMovementEvent OnMove;
    public static event HeroMovementEvent OnAvoid;

    #endregion

    #region Trigger Methods - these events notify all listeners

    public static void AttackInitiated(bool isAction)
    {
        if (OnAttack != null)
            OnAttack(isAction);
    }

    public static void JumpInitiated(bool isAction)
    {
        if (OnJump != null)
            OnJump(isAction);
    }

    public static void BlockInitiated(bool isAction)
    {
        if (OnBlock != null)
            OnBlock(isAction);
    }

    public static void LightningInitiated(bool isAction)
    {
        if (OnLightning != null)
            OnLightning(isAction);
    }

    public static void MoveInitiated(float moveX, float moveY)
    {
        if (OnMove != null)
            OnMove(moveX, moveY);
    }

    public static void AvoidInitiated(float avoidX, float avoidY)
    {
        if (OnAvoid != null)
            OnAvoid(avoidX, avoidY);
    }

    #endregion
}
