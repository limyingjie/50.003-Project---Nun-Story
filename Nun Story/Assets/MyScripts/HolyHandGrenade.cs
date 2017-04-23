using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyHandGrenade : MonoBehaviour {

    public float blastRadius;
    public float blastStrength;
    public AudioClip clip;
    public AudioClip explosion;

    private float fuseTime = 3f;
    private AudioSource source;


    private bool isExploding = false;
    private float hallelujahTimer;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        hallelujahTimer = 2f;
		
	}
	
	// Update is called once per frame
	void Update () {
        fuseTime -= Time.deltaTime;
        if (fuseTime <= 0) { exploding(); }
	}

    private void exploding() {
        if (!isExploding) {
            isExploding = true;
            source.PlayOneShot(clip,0.3F);
        }
        hallelujahTimer -= Time.deltaTime;
        if(hallelujahTimer <= 0) { explode(); }
    }

    private void explode() {
        Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        AudioSource.PlayClipAtPoint(explosion, new Vector3(transform.position.x, transform.position.y,0), 1F);
        foreach (Collider2D cd in inRange)
        {
            GameObject other = cd.gameObject;
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(new Vector2(0, blastStrength));
                TestControl otherScript = other.GetComponent<TestControl>();
                if (otherScript != null) { otherScript.applyStun(3f); }
            }
        }
        Destroy(gameObject);
    }
}
