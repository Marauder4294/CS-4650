using UnityEngine;

public class Body : MonoBehaviour {

    public GameObject character;
    Entity entity;

	// Use this for initialization
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
            entity.Damage(other.GetComponentInParent<Entity>(), true);
        }
    }
}
