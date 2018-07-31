using UnityEngine;

public class Body : MonoBehaviour {

    public GameObject character;
    Entity entity;

	void Start ()
    {
        entity = character.GetComponentInParent<Entity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            entity.Damage(other.GetComponentInParent<Entity>(), false);
        }
        else if (other.gameObject.tag == "Magic")
        {
            Magic magic = other.gameObject.GetComponent<Magic>();

            if (magic.Ent.name != gameObject.transform.parent.name)
            {
                entity.MagicDamage(magic.Ent, magic.Type);
            }
        }
        else if (other.gameObject.tag == "Ground" && entity.IsDead)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}