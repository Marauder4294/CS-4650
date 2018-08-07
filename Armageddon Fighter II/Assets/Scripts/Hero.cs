//using System.Collections;
//using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity {

    // TODO Create method to set level and attributes for HERO ONLY

    #region Hero-Specific Variable Declarations

    Vector3 cameraOffset;
    float cameraRotationOffSetY;

    public GameObject lightning;
    
    float stickSpeed;

    Vector2 uiMaxHealthWhiteSize;
    Vector2 uiMaxHealthRedSize;

    RectTransform uiImageHealthWhiteCurrent;
    RectTransform uiImageHealthRedCurrent;

    Vector2 uiMaxManaWhiteSize;
    Vector2 uiMaxManaBlueSize;

    RectTransform uiImageManaWhiteCurrent;
    RectTransform uiImageManaBlueCurrent;

    float? landingAnimTime;
    float? avoidAnimTime;
    readonly float?[] magicAnimTimes = new float?[4];

    float? avoidTimer;

    CapsuleCollider sensor;

    bool isMagicOne;
    bool isMagicTwo;
    bool isMagicThree;

    float? magicLockTimer;

    #endregion Hero-Specific Variable Declarations

    void Awake()
    {
        #region Common Variable Setup

        Player = FindObjectOfType<Hero>();
        Anim = Player.GetComponent<Animator>();
        Ent = Player.GetComponent<Entity>();
        Rigid = Player.GetComponent<Rigidbody>();

        PositionY = Player.transform.position.y;

        IsActive = true;
        MaxAttackNumber = 5;
        KnockbackPowerHeight = 2f;
        KnockbackPowerLength = 4.5f;

        MovementSpeed = 0.1f;
        AttackSpeed = 1.6f;
        AttackWaitTime = 3;

        SetInitialValues();

        avoidTimer = -1;

        StunLength = 0;

        magicLockTimer = -1;

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

        MagicOneCost = 2;
        MagicTwoCost = 15;
        MagicThreeCost = 25;

        #endregion Base Attribute Setter
        
        #region Subscribe to Events

        EventManager.OnAttack += OnAttack;
        EventManager.OnJump += OnJump;
        EventManager.OnBlock += OnBlock;
        EventManager.OnLightning += OnLightning;
        EventManager.OnMove += OnMove;
        EventManager.OnAvoid += OnAvoid;

        #endregion Subscribe to Events

        #endregion Common Variable Setup

        #region Set Hero-Specific Variables
        
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

        WindUpAnimTimes = new float[6];
        WindUpAnimTimes[1] = (clip.First(a => a.name == "Attack_1-WindUp").length / 3) / AttackSpeed;
        WindUpAnimTimes[2] = 0;
        WindUpAnimTimes[3] = (clip.First(a => a.name == "Attack_3-WindUp").length / 3) / AttackSpeed;
        WindUpAnimTimes[4] = WindUpAnimTimes[3];
        WindUpAnimTimes[5] = (clip.First(a => a.name == "Attack_4-WindUp").length / 3) / AttackSpeed;
        WindUpAnimTimes[0] = WindUpAnimTimes[1] + WindUpAnimTimes[2] + WindUpAnimTimes[3] + WindUpAnimTimes[4] + WindUpAnimTimes[5];

        AttackAnimTimes = new float[7];
        AttackAnimTimes[1] = (WindUpAnimTimes[1] + clip.First(a => a.name == "Attack_1").length / 1) / AttackSpeed;
        AttackAnimTimes[2] = (clip.First(a => a.name == "Attack_2").length / 1) / AttackSpeed;
        AttackAnimTimes[3] = (WindUpAnimTimes[3] + clip.First(a => a.name == "Attack_3").length / 1) / AttackSpeed;
        AttackAnimTimes[4] = AttackAnimTimes[3];
        AttackAnimTimes[5] = (WindUpAnimTimes[5] + clip.First(a => a.name == "Attack_4").length / 1) / AttackSpeed;

        AttackAnimTimes[6] = (clip.First(a => a.name == "Jump_Attack").length / 1) / AttackSpeed;
        AttackAnimTimes[0] = AttackAnimTimes[1] + AttackAnimTimes[2] + AttackAnimTimes[3] + AttackAnimTimes[4] + AttackAnimTimes[5];

        landingAnimTime = clip.First(a => a.name == "Jump_Landing").length / 5;

        magicAnimTimes[1] = (clip.First(a => a.name == "Magic_One").length / 2) / AttackSpeed;
        magicAnimTimes[0] = magicAnimTimes[1];

        avoidAnimTime = clip.First(a => a.name == "Aviod_Front").length;

        cameraOffset = new Vector3(Camera.main.transform.position.x - Player.transform.position.x, 
            Camera.main.transform.position.y - Player.transform.position.y, 
            Camera.main.transform.position.z - Player.transform.position.z);

        cameraRotationOffSetY = 29;
        

        Weapon = new BoxCollider[1];
        Weapon[0] = Player.transform.Find("Dack/root/pelvis/spine01/spine02/spine03/clavicle_R/upperarm_R/lowerarm_R/hand_R/Sword").GetComponent<BoxCollider>();
        Shield = Player.transform.Find("Dack/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").GetComponent<BoxCollider>();
        Shield.enabled = false;
        WeaponTrail = new TrailRenderer[1];
        WeaponTrail[0] = Weapon[0].transform.Find("SwordTrail").gameObject.GetComponent<TrailRenderer>();
        WeaponTrail[0].startWidth = 0.3f;
        WeaponTrail[0].endWidth = 0.0001f;
        SetWeapon(false);

        sensor = Player.GetComponent<CapsuleCollider>();

        SetMagicOne(false);
        SetMagicTwo(false);
        SetMagicThree(false);

        #endregion Set Hero-Specific Variables
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    public void OnMove(float moveX, float moveY)
    {
        if (!IsDead)
        {
            if ((moveX != 0 || moveY != 0) && MoveTimer == -1 && avoidTimer == -1 && magicLockTimer == -1 && !IsKnockedDown)
            {
                SetMoving(true);
            }
            else
            {
                SetMoving(false);
            }

            if (IsMoving)
            {
                Player.transform.LookAt(new Vector3(Player.transform.position.x + moveX, Player.transform.position.y, Player.transform.position.z + moveY));
                Player.transform.eulerAngles = new Vector3(0, Player.transform.eulerAngles.y + cameraRotationOffSetY, 0);

                if (!IsTouchingBoundary && !IsBlocking)
                {
                    if (moveX != 0 && moveY != 0)
                    {
                        stickSpeed = (Mathf.Abs(moveX) + Mathf.Abs(moveY)) / 2;
                    }
                    else if (moveX != 0)
                    {
                        stickSpeed = Mathf.Abs(moveX);
                    }
                    else if (moveY != 0)
                    {
                        stickSpeed = Mathf.Abs(moveY);
                    }

                    Player.transform.position += new Vector3(((Player.transform.forward.x * MovementSpeed) * stickSpeed), 0, ((Player.transform.forward.z * MovementSpeed) * stickSpeed));
                }
            }
            else if ((moveX != 0 || moveY != 0) && IsKnockedDown && !IsFallingBack)
            {
                KnockDownTimer = -1;
                SetKnockedDown(false);
            }

            if (!IsMoving) stickSpeed = 0;

            Anim.SetFloat("MoveSpeed", (MovementSpeed * 14) * stickSpeed);

            DecrementAttackLockTimer();
            DecrementWindUpLockTimer();
            DecrementAttackTimer();
            DecrementMagicTimer();
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

    public void OnAvoid(float avoidX, float avoidY)
    {
        if (!IsDead && avoidTimer == -1)
        {
            if ((avoidX != 0 || avoidY != 0) && magicLockTimer == -1 && !IsKnockedDown)
            {
                SetAvoiding(true);
            }
            else
            {
                SetAvoiding(false);
            }

            if (IsAvoiding && !IsTouchingBoundary && !IsBlocking)
            {
                if (IsMoving) SetMoving(false);

                Player.transform.LookAt(new Vector3(Player.transform.position.x + avoidX, Player.transform.position.y, Player.transform.position.z + avoidY));
                Player.transform.eulerAngles = new Vector3(0, Player.transform.eulerAngles.y + cameraRotationOffSetY, 0);

                avoidTimer = avoidAnimTime;

                //Anim.SetFloat("AvoidSpeed", 14);
            }

            if (!IsBlocking || IsKnockedDown)
                Camera.main.transform.position = new Vector3(Player.transform.position.x + cameraOffset.x, Camera.main.transform.position.y, Player.transform.position.z + cameraOffset.z);
        }

        DecrementAvoidTimer();
    }

    public void OnAttack()
    {
        if (AttackLockTimer == -1 && !IsKnockedDown)
        {
            if (AttackCount >= MaxAttackNumber)
            {
                AttackCount = 0;
            }

            if (!InAir)
            {
                AttackCount++;
                AttackLockTimer = AttackAnimTimes[AttackCount];
                WindUpLockTimer = WindUpAnimTimes[AttackCount];
                AttackTimer = (AttackCount == 1) ? AttackLockTimer : AttackWaitTime / AttackSpeed;
                MoveTimer = AttackLockTimer;
                JumpTimer = AttackLockTimer;
                
                if (WindUpLockTimer <= 0) SetWeapon(true);
            }
            else
            {
                AttackCount = MaxAttackNumber + 1;
                SetWeapon(true);
                AttackLockTimer = AttackAnimTimes[AttackCount];
            }

            SetAttacking(true);
            Anim.SetInteger("AttackNumber", AttackCount);
        }
        else if (AttackCount == 1 && !NextAttack)
        {
            NextAttack = true;
        }
    }

    public void OnJump()
    {
        if (!InAir && JumpTimer == -1 && !IsKnockedDown)
        {
            InAir = true;
            Anim.SetBool("Jumping", true);
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

            if (IsAttacking) SetAttacking(false);

            if (IsMoving) SetMoving(false);

            if (!IsBlocking) SetBlocking(true);

            if (Weapon[0].enabled)
            {
                SetWeapon(false);
            }
        }
        else
        {
            if (IsBlocking) SetBlocking(false);
        }
    }

    public void OnLightning()
    {
        if (magicLockTimer == -1 && Mana >= MagicOneCost)
        {
            if (IsAttacking) SetAttacking(false);
            if (IsMoving) SetMoving(false);
            if (IsBlocking) SetBlocking(false);
            magicLockTimer = magicAnimTimes[1];
            SetMagicOne(true);
        }
    }

    void UnsubscribeEvents()
    {
        EventManager.OnAttack -= OnAttack;
        EventManager.OnJump -= OnJump;
        EventManager.OnBlock -= OnBlock;
        EventManager.OnLightning -= OnLightning;
        EventManager.OnMove -= OnMove;
        EventManager.OnAvoid -= OnAvoid;
    }

    void SetMagicOne(bool isAction)
    {
        isMagicOne = isAction;
        Anim.SetBool("MagicOne", isAction);
    }

    void SetMagicTwo(bool isAction)
    {
        isMagicTwo = isAction;
        Anim.SetBool("MagicTwo", isAction);
    }

    void SetMagicThree(bool isAction)
    {
        isMagicThree = isAction;
        Anim.SetBool("MagicThree", isAction);
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter(gameObject, other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay(gameObject, other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit(gameObject, other);
    }

    void DecrementMagicTimer()
    {
        if (magicLockTimer > 0)
        {
            magicLockTimer -= Time.deltaTime;
        }
        else if (magicLockTimer > -1)
        {
            magicLockTimer = -1;

            if (isMagicOne)
            {
                SetMagicOne(false);

                GameObject projectile = Instantiate(lightning);
                projectile.transform.position = Player.transform.Find("Dack/root/pelvis/spine01/spine02/spine03/clavicle_L/upperarm_L/lowerarm_L/hand_L").transform.position;

                Transform rods = projectile.transform.Find("Rods").transform;

                rods.eulerAngles = new Vector3(rods.eulerAngles.x, rods.eulerAngles.y + 34.7f, rods.eulerAngles.z);

                projectile.transform.forward = Player.transform.forward;

                projectile.GetComponent<Magic>().Ent = Ent;
                projectile.GetComponent<Magic>().Type = "Lightning";

                TakeMana(MagicOneCost);
            }
            else if (isMagicTwo)
            {
                SetMagicTwo(false);
            }
            else if (isMagicThree)
            {
                SetMagicThree(false);
            }
        }
    }

    void DecrementAvoidTimer()
    {
        if (avoidTimer > 0)
        {
            avoidTimer -= Time.deltaTime;

            Player.transform.position += Player.transform.forward * (Time.deltaTime * 7);
        }
        else if (avoidTimer > -1)
        {
            avoidTimer = -1;

            SetAvoiding(false);
        }
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
                AttackLockTimer = AttackAnimTimes[AttackCount];
                MoveTimer = AttackLockTimer;
                AttackTimer = AttackWaitTime / AttackSpeed;
            }
            else
            {
                AttackLockTimer = -1;
                SetWeapon(false);
                SetAttacking(false);
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
        uiImageManaBlueCurrent.sizeDelta = new Vector2(uiMaxManaBlueSize.x * currentMana - ((uiMaxManaWhiteSize.x - uiMaxManaBlueSize.x) / 2), uiMaxManaBlueSize.y);
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
            Time.timeScale = 0;

            FindObjectOfType<GameManager>().GameOverToggle();
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

    protected override void TriggerEnter(GameObject fighter, Collider other)
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
                    InAir = false;
                    SetJumping(false);
                }

                if (IsAttacking) SetAttacking(false);

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

    protected override void TriggerStay(GameObject fighter, Collider other)
    {
        if (!IsDead && !IsTouchingBoundary)
        {
            if (other.gameObject.tag == "Ground" && !Rigid.isKinematic)
            {
                Rigid.isKinematic = true;
                InAir = false;
                SetJumping(false);
                Anim.SetBool("Landing", true);
            }
            else if (other.gameObject.tag == "Ground" && Mathf.Abs(fighter.transform.position.y - PositionY) > 0.1)
            {
                PositionY = (PositionY < fighter.transform.position.y) ? PositionY + 0.04f : PositionY - 0.04f;
                Camera.main.transform.position = new Vector3(fighter.transform.position.x + cameraOffset.x, PositionY + cameraOffset.y, fighter.transform.position.z + cameraOffset.z);
            }
        }
    }

    protected override void TriggerExit(GameObject fighter, Collider other)
    {
        if (!IsDead)
        {
            if (other.gameObject.tag == "Boundary")
            {
                IsTouchingBoundary = false;
            }
            else if (other.gameObject.tag == "Ground" && Rigid.isKinematic)
            {
                if (!IsKnockedDown) SetJumping(true);

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

    #endregion
}