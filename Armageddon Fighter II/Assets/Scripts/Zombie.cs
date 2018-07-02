//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Zombie : Entity {

    Entity entity;
    public GameObject enemy;
    Rigidbody rigidBody;
    Animator animator;
    Hero hero;

    float movementSpeed;

    bool isActive;
    bool canMove;

	void Awake () {

        EventManager.OnMove += Hunt;

        movementSpeed = 0.05f;
        
        rigidBody = enemy.GetComponent<Rigidbody>();
        animator = enemy.GetComponent<Animator>();
        entity = enemy.GetComponent<Entity>();
        hero = FindObjectOfType<Hero>();

        isActive = false;
        canMove = false;
        attackCount = 0;
        attackTimer = 0;
        stunTimer = 0;

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
    }

    void Hunt(float moveX, float moveY)
    {
        if (isActive && canMove == true)
        {
            animator.SetBool("Moving", true);

            enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));

            enemy.transform.position += enemy.transform.forward * movementSpeed;
        }

        if (stunTimer > 0)
            --stunTimer;

        if (attackTimer > 0)
            --attackTimer;

        if (knockDownTimer > 1)
        {
            --knockDownTimer;
        }
        else if (knockDownTimer == 1)
        {
            knockDownTimer = 0;
            animator.SetBool("KnockedDown", false);
            canMove = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && enemy.GetComponent<SphereCollider>().enabled)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            isActive = true;
            canMove = true;
        }
        else if (other.gameObject.tag == "Player")
        {
            canMove = false;
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));
            animator.SetBool("Moving", false);
            animator.SetBool("Attacking", (stunTimer == 0) ? true : false);
        }
        else if (other.gameObject.tag == "HeroSword" && this.GetComponent<SphereCollider>().enabled == false)
        {
            EventManager.DamageInitiated(entity, hero.power, false, hero.attackCount, hero.maxAttackNumber);
            OnHit();

            if (hero.attackCount >= hero.maxAttackNumber)
                KnockBack();
        }
        else if (other.gameObject.tag == "Terrain")
        {
            inAir = false;
            rigidBody.isKinematic = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemy.transform.LookAt(new Vector3(hero.transform.position.x, 0, hero.transform.position.z));
            animator.SetBool("Attacking", (stunTimer == 0 && attackTimer == 0) ? true : false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && stunTimer == 0 && knockDownTimer == 0)
        {
            canMove = true;
            animator.SetBool("Attacking", false);
            animator.SetBool("Moving", true);
        }
        else if (other.gameObject.tag == "Terrain")
        {
            if (inAir == false)
                inAir = true;
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
        animator.SetBool("KnockedDown", true);
        animator.SetBool("Attacking", false);
        animator.SetBool("Moving", false);
        enemy.transform.position -= enemy.transform.forward * 1.5f;

        knockDownTimer = 200;
    }
}
