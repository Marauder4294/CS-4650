using UnityEngine;

public class Lightning : Magic {

    Magic magic;

    void Start()
    {
        magic = GetComponent<Magic>();
    }

    void Update ()
    {
        transform.position += transform.forward;
	}

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Body" && magic.Ent.name != other.transform.parent.name) || other.tag == "DeathBoundary" || other.tag == "Boundary" || other.tag == "Magic")
        {
            Destroy(gameObject);
        }
    }
}