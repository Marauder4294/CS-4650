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
            string entityTag = gameObject.transform.parent.name;

            if (magic.Ent.name != entityTag)
            {
                entity.MagicDamage(magic.Ent, magic.Type);
            }
        }
    }
}
