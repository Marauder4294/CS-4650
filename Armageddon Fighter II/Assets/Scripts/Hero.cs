//using System.Collections;
//using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;

public class Hero : Entity {

    #region Hero-Specific Variable Declarations

    Vector3 cameraOffset;

    public GameObject lightning;

    float attackTimerSeconds;

    Vector2 uiMaxHealthWhiteSize;
    Vector2 uiMaxHealthRedSize;

    RectTransform uiImageHealthWhiteCurrent;
    RectTransform uiImageHealthRedCurrent;

    Vector2 uiMaxManaWhiteSize;
    Vector2 uiMaxManaBlueSize;

    RectTransform uiImageManaWhiteCurrent;
    RectTransform uiImageManaBlueCurrent;

    readonly float?[] attackAnimTimes = new float?[7];
    float? landingAnimTime;

    BoxCollider sword;
    BoxCollider shield;

    CapsuleCollider sensor;

    #endregion

    void Awake()
    {
        #region Set Unity objects

        Player = FindObjectOfType<Hero>();
        Ent = Player.GetComponent<Entity>();
        Rigid = Player.GetComponent<Rigidbody>();
        Anim = Player.GetComponent<Animator>();

        #endregion

        #region Base Attribute Setter

        Level = 1;

        Power = 15;
        Magic = 14;
        Defense = 10;
        MagicResist = 0;
        Block = 10;
        Vitality = 19;

        MaxHealth = (Vitality * 2) + (Level * 2);
        MaxMana = Magic + Level;

        Health = MaxHealth;
        Mana = MaxMana;

        #endregion

        #region Set Booleans and floats

        PositionY = Player.transform.position.y;

        JumpPower = 3f;
        JumpHeight = JumpPower;

        MovementSpeed = 0.1f;
        AttackSpeed = 1.6f;

        AttackCount = 0;
        MaxAttackNumber = 5;

        MoveTimer = -1;
        AttackTimer = -1;
        AttackLockTimer = -1;
        StunTimer = -1;
        JumpTimer = -1;
        DeathTimer = -1;

        IsDead = false;
        IsAttacking = false;
        NextAttack = false;
        IsActive = true;
        IsTouchingBoundary = false;

        KnockbackPowerHeight = 2f;
        KnockbackPowerLength = 4.5f;

        Anim.SetFloat("AttackSpeed", AttackSpeed);

        #endregion

        #region Subscribe to events

        EventManager.OnAttack += OnAttack;
        EventManager.OnJump += OnJump;
        EventManager.OnBlock += OnBlock;
        EventManager.OnLightning += OnLightning;
        EventManager.OnMove += OnMove;
        //EventManager.OnAvoid += OnAvoid;

        #endregion

        #region Set Hero-Specific variables

        attackTimerSeconds = 3;
        Image[] images = FindObjectsOfType<Image>();

        uiMaxHealthWhiteSize = images.First(a => a.name == "PlayerHealthBarOutline").rectTransform.sizeDelta;
        uiMaxHealthRedSize = images.First(a => a.name == "PlayerHealthBar").rectTransform.sizeDelta;

        uiImageHealthWhiteCurrent = images.First(a => a.name == "PlayerHealthBarOutline").rectTransform;
        uiImageHealthRedCurrent = images.First(a => a.name == "PlayerHealthBar").rectTransform;

        uiMaxManaWhiteSize = images.First(a => a.name == "PlayerManaBarOutline").rectTransform.sizeDelta;
        uiMaxManaBlueSize = images.First(a => a.name == "PlayerManaBar").rectTransform.sizeDelta;

        uiImageManaWhiteCurrent = images.First(a => a.name == "PlayerManaBarOutline").rectTransform;
        uiImageManaBlueCurrent = images.First(a => a.name == "PlayerManaBar").rectTransform;

        AnimationClip[] clip = Anim.runtimeAnimatorController.animationClips;

        ChildAnimatorStateMachine[] stateMachines = (Anim.runtimeAnimatorController as AnimatorController).layers[0].stateMachine.stateMachines;
        AnimatorStateMachine attackStateMachine =  stateMachines.First(a => a.stateMachine.name == "Attack").stateMachine;
        AnimatorStateMachine jumpStateMachine = stateMachines.First(a => a.stateMachine.name == "Jump").stateMachine;

        attackAnimTimes[1] = ((clip.First(a => a.name == "Attack_1-WindUp").length / attackStateMachine.states.First(a => a.state.name == "Attack_1-WindUp").state.speed)
            + clip.First(a => a.name == "Attack_1").length / attackStateMachine.states.First(a => a.state.name == "Attack_1").state.speed) / AttackSpeed;
        attackAnimTimes[2] = (clip.First(a => a.name == "Attack_2").length / attackStateMachine.states.First(a => a.state.name == "Attack_2").state.speed) / AttackSpeed;
        attackAnimTimes[3] = ((clip.First(a => a.name == "Attack_3-WindUp").length / attackStateMachine.states.First(a => a.state.name == "Attack_3-WindUp").state.speed)
            + clip.First(a => a.name == "Attack_3").length / attackStateMachine.states.First(a => a.state.name == "Attack_3").state.speed) / AttackSpeed;
        attackAnimTimes[4] = attackAnimTimes[3];
        attackAnimTimes[5] = ((clip.First(a => a.name == "Attack_4-WindUp").length / attackStateMachine.states.First(a => a.state.name == "Attack_4-WindUp").state.speed)
            + clip.First(a => a.name == "Attack_4").length / attackStateMachine.states.First(a => a.state.name == "Attack_4").state.speed) / AttackSpeed;
        attackAnimTimes[6] = (clip.First(a => a.name == "Jump_Attack").length / jumpStateMachine.states.First(a => a.state.name == "Jump_Attack").state.speed) / AttackSpeed;
        attackAnimTimes[0] = attackAnimTimes[1] + attackAnimTimes[2] + attackAnimTimes[3] + attackAnimTimes[4] + attackAnimTimes[5];

        landingAnimTime = clip.First(a => a.name == "Jump_Landing").length / jumpStateMachine.states.First(a => a.state.name == "Jump_Landing").state.speed;

        cameraOffset = new Vector3(Camera.main.transform.position.x - Player.transform.position.x, 
            Camera.main.transform.position.y - Player.transform.position.y, 
            Camera.main.transform.position.z - Player.transform.position.z);

        sword = Player.transform.Find("Dack/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R").GetComponent<BoxCollider>();
        shield = Player.transform.Find("Dack/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").GetComponent<BoxCollider>();

        sensor = Player.GetComponent<CapsuleCollider>();

        sword.enabled = false;

        #endregion
    }

    private object FindObjectsOfTypeAll()
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    public void OnMove(float moveX, float moveY)
    {
        if (!IsDead)
        {
            IsMoving = ((moveX != 0 || moveY != 0) && MoveTimer == -1 && !IsKnockedDown) ? true : false;

            if (IsMoving)
            {
                if (!IsTouchingBoundary && !IsBlocking)
                {
                    Player.transform.LookAt(Player.transform.position += new Vector3(-moveY * MovementSpeed, 0, -moveX * MovementSpeed));
                }
                
                Player.transform.forward += new Vector3(-moveY, 0, -moveX);
            }
            else if ((moveX != 0 || moveY != 0) && IsKnockedDown && !IsFallingBack)
            {
                KnockDownTimer = -1;
                IsKnockedDown = false;
                Anim.SetBool("KnockedDown", IsKnockedDown);
            }

            Anim.SetBool("Moving", IsMoving);
            Anim.SetFloat("MoveSpeed", (MovementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);

            DecrementAttackLockTimer();
            DecrementAttackTimer();
            DecrementJumpTimer();
            DecrementMoveTimer();
            DecrementStunTimer();
            JumpUp();

            if (!IsBlocking || IsKnockedDown)
                Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffset.x, Camera.main.transform.position.y, Player.transform.position.z + cameraOffset.z);
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
        if (!IsDead && !IsTouchingBoundary)
        {
            if (other.gameObject.tag == "Boundary")
            {
                IsTouchingBoundary = true;
                Rigid.isKinematic = false;
                //sensor.isTrigger = false;
            }
            else if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Anim.SetBool("Landing", true);

                if (InAir || IsJumping)
                {
                    IsJumping = false;
                    InAir = false;
                    Anim.SetBool("Jumping", IsJumping);
                }

                if (IsAttacking)
                {
                    IsAttacking = false;
                    Anim.SetBool("Attacking", IsAttacking);
                }

                Rigid.isKinematic = true;
                JumpTimer = landingAnimTime ?? 0;
                MoveTimer = JumpTimer;
            }
            else if (other.gameObject.tag == "DeathBoundary")
            {
                Player.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().enabled = false;
                Player.transform.Find("Hair").GetComponent<SkinnedMeshRenderer>().enabled = false;
                Player.transform.Find("HalfOne").GetComponent<SkinnedMeshRenderer>().enabled = false;
                Player.transform.Find("HalfTwo").GetComponent<SkinnedMeshRenderer>().enabled = false;
                Player.transform.Find("Shield").GetComponent<SkinnedMeshRenderer>().enabled = false;
                IsKnockedDown = true;
                Death(Ent);
            }
        }
        else
        {
            if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsDead)
        {
            if (other.gameObject.tag == "Boundary")
            {
                IsTouchingBoundary = false;
            }
            else if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
            {
                if (!IsKnockedDown)
                {
                    IsJumping = true;
                    Anim.SetBool("Jumping", IsJumping);
                }

                Rigid.isKinematic = false;
                Anim.SetBool("Landing", false);
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

    private void OnTriggerStay(Collider other)
    {
        if (!IsDead && !IsTouchingBoundary)
        {
            if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
                IsJumping = false;
                InAir = false;
                Anim.SetBool("Jumping", IsJumping);
                Anim.SetBool("Landing", true);
            }
            else if (other.gameObject.tag == "Ground" && Mathf.Abs(Player.transform.position.y - PositionY) > 0.1)
            {
                PositionY = (PositionY < Player.transform.position.y) ? PositionY + 0.04f : PositionY - 0.04f;
                Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffset.x, PositionY + cameraOffset.y, Player.transform.position.z + cameraOffset.z);
            }
        }
    }

    public void OnAttack(bool isAction)
    {
        if (AttackLockTimer == -1)
        {
            if (AttackCount >= 5)
            {
                AttackCount = 0;
            }

            if (InAir == false)
            {
                AttackCount++;
                AttackLockTimer = attackAnimTimes[AttackCount] ?? 0;
                AttackTimer = (AttackCount == 1) ? AttackLockTimer : attackTimerSeconds / AttackSpeed;
                MoveTimer = AttackLockTimer;
                JumpTimer = AttackLockTimer;
                sword.enabled = true;
            }
            else
            {
                AttackCount = 5;
                AttackLockTimer = 0.25f;
                sword.enabled = true;
                AttackLockTimer = attackAnimTimes[6] ?? 0;
            }

            IsAttacking = true;
            Anim.SetBool("Attacking", IsAttacking);
            Anim.SetInteger("AttackNumber", AttackCount);
        }
        else if (AttackCount == 1 && !NextAttack)
        {
            NextAttack = true;
        }
    }

    public void OnJump(bool isAction)
    {
        if (isAction == true && InAir == false && JumpTimer == -1 && !IsKnockedDown)
        {
            InAir = true;
            Anim.SetBool("Jumping", isAction);
            Anim.SetBool("Landing", false);
            Anim.SetBool("Attacking", false);
            Rigid.isKinematic = false;

            IsGoingUp = true;

            JumpHeight = Player.transform.position.y + JumpPower;

            movement.y = 0.15f;

            Player.transform.position += movement;
        }
    }

    public void OnBlock(bool isAction)
    {
        if (isAction)
        {
            if (AttackCount > 0)
            {
                AttackCount = 0;
                Anim.SetInteger("AttackNumber", AttackCount);
            }

            if (IsAttacking)
            {
                IsAttacking = false;
                Anim.SetBool("Attacking", IsAttacking);
            }

            if (IsMoving)
            {
                IsMoving = false;
                Anim.SetBool("Moving", IsMoving);
            }

            if (!IsBlocking)
            {
                IsBlocking = true;
                Anim.SetBool("Blocking", IsBlocking);
            }

            if (sword.enabled)
            {
                sword.enabled = false;
            }
        }
        else
        {
            if (IsBlocking)
            {
                IsBlocking = false;
                Anim.SetBool("Blocking", IsBlocking);
            }
        }
    }

    public void OnLightning(bool isAction)
    {
        if (Mana >= 5)
        {
            var projectile = Instantiate(lightning);
            projectile.transform.position = new Vector3(Player.transform.position.x + (Player.transform.forward.x / 2), Player.transform.position.y + 0.5f, Player.transform.position.z + (Player.transform.forward.z / 2));
            projectile.transform.forward = Player.transform.forward;
            //projectile.transform.eulerAngles = new Vector3(projectile.transform.eulerAngles.x, 0, projectile.transform.eulerAngles.z);

            projectile.GetComponent<Magic>().Ent = Ent;
            projectile.GetComponent<Magic>().Type = "Lightning";

            TakeMana(0);
        }
    }

    //public void OnAvoid(float avoidX, float avoidY)
    //{
    //    IsMoving = ((avoidX != 0 || avoidY != 0) && MoveTimer == 0) ? true : false;

    //    if (IsMoving == true)
    //    {
    //        if (IsBlocking == false)
    //            Player.transform.LookAt(Player.transform.position += new Vector3(-avoidY * 3, 0, -avoidX * 3));

    //        Player.transform.forward += new Vector3(-avoidY, 0, -avoidX);

    //        if (IsBlocking == false)
    //            Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffsetX, Camera.main.transform.position.y, Player.transform.position.z + cameraOffsetZ);
    //    }

    //    Anim.SetBool("Avoiding", IsAvoiding);
    //    //Anim.SetFloat("AvoidSpeed", (MovementSpeed * 14f) * ((System.Math.Abs(avoidX) + System.Math.Abs(avoidY))) / 2);
    //}

    void UnsubscribeEvents()
    {
        EventManager.OnAttack -= OnAttack;
        EventManager.OnJump -= OnJump;
        EventManager.OnBlock -= OnBlock;
        EventManager.OnLightning -= OnLightning;
        EventManager.OnMove -= OnMove;
        //EventManager.OnAvoid -= OnAvoid;
    }

    #region Entity Method Overrides

    protected override void DecrementAttackLockTimer()
    {
        if (AttackLockTimer > 0)
        {
            AttackLockTimer -= Time.deltaTime;
        }
        else if (AttackLockTimer > -1)
        {
            if (AttackCount == 1 && NextAttack)
            {
                NextAttack = false;
                AttackCount++;
                Anim.SetInteger("AttackNumber", AttackCount);
                AttackLockTimer = attackAnimTimes[AttackCount] ?? 0;
                AttackTimer = attackTimerSeconds / AttackSpeed;
            }
            else
            {
                sword.enabled = false;
                IsAttacking = false;
                Anim.SetBool("Attacking", IsAttacking);
                AttackLockTimer = -1;
            }
        }
    }

    protected override void TakeHealth()
    {
        float currentHealth = (float)Health / MaxHealth;

        uiImageHealthWhiteCurrent.sizeDelta = new Vector2(uiMaxHealthWhiteSize.x * currentHealth, uiMaxHealthWhiteSize.y);
        uiImageHealthRedCurrent.sizeDelta = new Vector2(uiMaxHealthRedSize.x * currentHealth - ((uiMaxHealthWhiteSize.x - uiMaxHealthRedSize.x) / 2), uiMaxHealthRedSize.y);
    }

    protected override void TakeMana(int cost)
    {
        Mana -= cost;

        float currentMana = (float)Mana / MaxMana;

        uiImageManaWhiteCurrent.sizeDelta = new Vector2(uiMaxManaWhiteSize.x * currentMana, uiMaxManaWhiteSize.y);
        uiImageManaBlueCurrent.sizeDelta = new Vector2(uiMaxManaBlueSize.x * currentMana - ((uiMaxManaBlueSize.x - uiMaxManaBlueSize.x) / 2), uiMaxManaBlueSize.y);
    }

    protected override void DecrementDeathTimer()
    {
        if (DeathTimer > 0)
        {
            DeathTimer -= Time.deltaTime;
        }
        else if (DeathTimer > -1)
        {
            DeathTimer = -1;
        }
    }

    protected override void Fallback()
    {
        if (IsFallingBack)
        {
            if (IsGoingUp)
            {
                if (Player.transform.position.y >= KnockbackPowerHeight)
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

                if (IsDead)
                {
                    UnsubscribeEvents();
                }
            }
        }
    }

    #endregion
}
