using UnityEngine;

public class Lightning : Magic {

    Magic magic;
    AudioSource audioSource;
    public AudioClip[] soundClip;


    void Start()
    {
        magic = GetComponent<Magic>();
        audioSource = Camera.main.GetComponent<AudioSource>();

        audioSource.PlayOneShot(soundClip[0]);
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

            audioSource.PlayOneShot(soundClip[1]);
        }
    }
}