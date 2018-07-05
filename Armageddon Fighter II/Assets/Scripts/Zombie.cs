//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Zombie : Entity
{
    Hero hero;
    public GameObject enemy;

	void Awake ()
    {
        EventManager.OnMove += Hunt;

        movementSpeed = 0.05f;

        hero = FindObjectOfType<Hero>();
        rigidBody = enemy.GetComponent<Rigidbody>();
        animator = enemy.GetComponent<Animator>();
        entity = enemy.GetComponent<Entity>();

        positionY = enemy.transform.position.y;

        isActive = false;
        isMoving = false;
        isAttacking = false;
        isKnockedDown = false;
        isFallingBack = false;
        attackCount = 0;

        moveTimer = 0;
        attackTimer = 0;
        stunTimer = 0;
        fallBackTimer = 0;

        #region Base Attribute Setter **Placeholder until multiple enemies are added

        power = 5;
        magic = 0;
        defense = 5;
        magicResist = 0;
        block = 10;
        vitality = 50;

        maxHealth = vitality;
        health = maxHealth;

        #endregion

        knockbackPowerHeight = 2f;
        knockbackPowerLength = 4.5f;
    }

    void Hunt(float moveX, float moveY)
    {
        if (isActive && isMoving && !isAttacking && moveTimer == 0 && !isKnockedDown)
        {
            animator.SetBool("Moving", true);
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, positionY, hero.transform.position.z));
            enemy.transform.position += enemy.transform.forward * movementSpeed;
        }
        else if (isAttacking)
        {
            if (Mathf.Abs(hero.transform.position.x - enemy.transform.position.x) <= 2f && Mathf.Abs(hero.transform.position.z - enemy.transform.position.z) <= 2f)
            {
                enemy.transform.LookAt(new Vector3(hero.transform.position.x, positionY, hero.transform.position.z));
                animator.SetBool("Attacking", (stunTimer == 0 && attackTimer == 0) ? true : false);
                moveTimer = (moveTimer > 0) ? --moveTimer : 60;
            }
            else
            {
                isAttacking = false;
                isMoving = true;
                animator.SetBool("Attacking", false);
                animator.SetBool("Moving", false);
            }
        }

        if (isFallingBack)
        {
            if (isGoingUp)
            {
                if (enemy.transform.position.y >= knockbackPowerHeight)
                {
                    isGoingUp = false;
                    movement.y = 0;
                }
            }

            if (fallBackTimer > 0)
            {
                --fallBackTimer;
                enemy.transform.position -= new Vector3(enemy.transform.forward.x * movement.x, movement.y, enemy.transform.forward.z * movement.z);
            }
            else
            {
                isFallingBack = false;
            }
        }

        if (stunTimer > 0)
            --stunTimer;

        if (attackTimer > 0)
            --attackTimer;

        if (moveTimer > 0)
            --moveTimer;

        if (knockDownTimer > 1)
        {
            --knockDownTimer;
        }
        else if (knockDownTimer == 1)
        {
            knockDownTimer = 0;
            animator.SetBool("KnockedDown", false);
            moveTimer = 30;
            isKnockedDown = false;
            isMoving = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && enemy.GetComponent<SphereCollider>().enabled)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            isActive = true;
            isMoving = true;
        }
        else if (other.gameObject.tag == "Player" && !isFallingBack)
        {
            isMoving = false;
            isAttacking = true;
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, positionY, hero.transform.position.z));
            animator.SetBool("Moving", false);
            animator.SetBool("Attacking", (stunTimer == 0) ? true : false);
            moveTimer = 60;
        }
        else if (other.gameObject.tag == "HeroSword" && !GetComponent<SphereCollider>().enabled && !isKnockedDown)
        {
            OnDamage(entity, hero.power, false, hero.attackCount, hero.maxAttackNumber);
            OnHit();

            if (hero.attackCount >= hero.maxAttackNumber)
                KnockBack();
        }
        else if (other.gameObject.tag == "Ground")
        {
            inAir = false;
            rigidBody.isKinematic = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground" && !rigidBody.isKinematic)
        {
            rigidBody.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && rigidBody.isKinematic)
        {
            rigidBody.isKinematic = false;
        }
    }

    void OnHit()
    {
        animator.SetBool("Attacking", false);
        stunTimer = 45;
    }

    void KnockBack()
    {
        isKnockedDown = true;
        isFallingBack = true;
        isGoingUp = true;
        animator.SetBool("KnockedDown", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Moving", false);

        movement = new Vector3(0.15f, -0.15f, 0.15f);

        if (knockbackPowerLength >= knockbackPowerHeight)
        {
            fallBackTimer = (int)(knockbackPowerLength / movement.x);
        }
        else
        {
            fallBackTimer = (int)(knockbackPowerHeight / movement.y);
        }

        enemy.transform.position -= new Vector3(enemy.transform.forward.x * movement.x, movement.y, enemy.transform.forward.z * movement.z);

        knockDownTimer = 200;
    }
}
