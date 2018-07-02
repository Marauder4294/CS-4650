using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {

    public GameObject enemy;
    Rigidbody rigidBody;
    Animator animator;
    GameObject hero;

    /// <summary>
    /// Maximum health the enemy can hold
    /// </summary>
    public float maxHealth;
    /// <summary>
    /// Currnet amount of health the enemy has
    /// </summary>
    public float currentHealth;
    /// <summary>
    /// Defense against magic.
    /// Needs to be in a percentage like 0.5
    /// </summary>
    public float magicDefense;
    /// <summary>
    /// Enemy has blocking active.
    /// Needs to be in a percentage
    /// </summary>
    public float blockDefense;
    /// <summary>
    /// Holds the bool value whether the enemy is blocking
    /// </summary>
    private bool isBlocking = false;
    /// <summary>
    /// Total amount of defense from any attacks.
    /// Needs to be in a percentage like 0.5
    /// </summary>
    private float totalDefense = 0.0f;

    float movementSpeed;

    bool isActive;

	// Use this for initialization
	void Awake () {

        EventManager.OnAttack += IsHit;
        EventManager.OnMove += Hunt;


        movementSpeed = 0.05f;
        
        rigidBody = enemy.GetComponent<Rigidbody>();
        animator = enemy.GetComponent<Animator>();
        hero = FindObjectOfType<Hero>().gameObject;

        isActive = false;
	}

    void IsHit(bool isAction)
    {

    }

    void Hunt(float moveX, float moveY)
    {
        if (isActive)
        {
            animator.SetBool("Moving", true);

            enemy.transform.LookAt(new Vector3(hero.transform.position.x - 0.5f, hero.transform.position.y, hero.transform.position.z - 0.5f));
            enemy.transform.position += enemy.transform.forward * movementSpeed;

            //animator.SetFloat("MoveSpeed", (movementSpeed * 14f) * ((System.Math.Abs(moveX) + System.Math.Abs(moveY))) / 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "HeroSword")
        {
            //TODO: 10 needs to be replaced by the damage being done by the sword
            TakeDamage(10, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && enemy.GetComponent<SphereCollider>().enabled)
        {
            enemy.GetComponent<SphereCollider>().enabled = false;
            isActive = true;
        }
        else if (other.gameObject.name.Contains("Hero"))
        {
            if (enemy.transform.position == new Vector3(hero.transform.position.x - 0.5f, hero.transform.position.y, hero.transform.position.z - 0.5f))
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Attacking", true);
            }
            else
            {
                enemy.transform.LookAt(new Vector3(hero.transform.position.x - 0.5f, hero.transform.position.y, hero.transform.position.z - 0.5f));
                animator.SetBool("Moving", true);
                animator.SetBool("Attacking", false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("Attacking", false);
            animator.SetBool("Moving", true);
        }
    }
    /// <summary>
    /// Function called when the zombie dies
    /// </summary>
    private void OnDeath()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// Function to determine how much damage is done to the zombie when it is hit
    /// </summary>
    /// <param name="damage">The amount of damage done by the attack</param>
    /// <param name="isMagic">Whether the attack is a magic attack</param>
    private void TakeDamage(float damage, bool isMagic)
    {
        // Starts defense at 0
        // Checks if the enemy is blocking and adds there defense to total defense
        // Checks if attack is magic, then adds it's magic defense to total defense
        totalDefense = 0;
        if (isBlocking)
        {
            totalDefense += blockDefense;
        }
        if (isMagic)
        {
            totalDefense += magicDefense;
        }

        // Determine damage by multiplying the damage by the totalDefense by the enemy.
        // Then subtracts that amount from the damage to determine how much damage will be done
        damage = damage - (damage * totalDefense);

        // Verifies there is still damge to be done to the enemy
        if (damage > 0)
        {
            // if the current health is greater than the damage being done, damage subtracted from the health.
            if (currentHealth > damage)
            {
                currentHealth -= damage;
            }
            else
            {
                OnDeath();
            }
        }
    }
}
