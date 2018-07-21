using UnityEngine;

public class Lightning : Magic {

    GameObject lightning;
    Magic magic;

    void Start()
    {
        lightning = gameObject;
        magic = GetComponent<Magic>();
    }

    void FixedUpdate ()
    {
        lightning.transform.position += transform.forward;
	}

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Body" && magic.Ent.name != other.transform.parent.name) || other.tag == "DeathBoundary" || other.tag == "Boundary" || other.tag == "Magic")
        {
            Destroy(lightning);
        }
    }
}