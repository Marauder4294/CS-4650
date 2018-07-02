using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {

    public GameObject enemy;
    Rigidbody rigidBody;
    Animator animator;
    GameObject hero;

    float movementSpeed;

    bool isActive;

	// Use this for initialization
	void Awake () {

        EventManager.OnAttack += TakeHit;
        EventManager.OnMove += Hunt;


        movementSpeed = 0.05f;
        
        rigidBody = enemy.GetComponent<Rigidbody>();
        animator = enemy.GetComponent<Animator>();
        hero = FindObjectOfType<Hero>().gameObject;

        isActive = false;
	}
	

    void TakeHit(bool isAction)
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
}
