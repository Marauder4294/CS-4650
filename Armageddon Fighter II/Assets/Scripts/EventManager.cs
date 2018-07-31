//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class EventManager {

    #region Event Declarations - declare delegates and events here

    public delegate void HeroEvent();
    public static event HeroEvent OnAttack;
    public static event HeroEvent OnJump;
    public static event HeroEvent OnLightning;

    public delegate void HeroBoolEvent(bool isAction);
    public static event HeroBoolEvent OnBlock;

    public delegate void HeroMovementEvent(float moveX, float moveY);
    public static event HeroMovementEvent OnMove;
    public static event HeroMovementEvent OnAvoid;

    #endregion

    #region Trigger Methods - these events notify all listeners

    public static void AttackInitiated()
    {
        if (OnAttack != null) OnAttack();
    }

    public static void JumpInitiated()
    {
        if (OnJump != null) OnJump();
    }

    public static void LightningInitiated()
    {
        if (OnLightning != null) OnLightning();
    }

    public static void BlockInitiated(bool isAction)
    {
        if (OnBlock != null) OnBlock(isAction);
    }

    public static void MoveInitiated(float moveX, float moveY)
    {
        if (OnMove != null) OnMove(moveX, moveY);
    }

    public static void AvoidInitiated(float avoidX, float avoidY)
    {
        if (OnAvoid != null) OnAvoid(avoidX, avoidY);
    }

    #endregion
}