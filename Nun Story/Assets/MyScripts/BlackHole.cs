using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.Networking;

public class BlackHole : NetworkBehaviour {

    private float duration = 5f;
    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}

    // Update is called once per frame
    
    void Update () {
        //apply a force to suck in all rigidbodies within a range of 40 units
        Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, 40);
        foreach (Collider2D cd in inRange) {
            GameObject other = cd.gameObject;
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null) {
                float distance = (other.transform.position - transform.position).magnitude;
                Vector2 direction = (other.transform.position - transform.position).normalized;
                rb.AddForce(new Vector2( direction.x, direction.y)*rb.mass*-5000f/distance/distance);
            }
        }
        //destroy the black hole once its time is up
        if (Time.time - startTime >= duration) { NetworkServer.Destroy(gameObject); Destroy(gameObject); }
            
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //when a player touches the black hole,
        //apply stasis to the player with duration equal to the remaining duration of the black hole.
        Platformer2DUserControl robot = collision.gameObject.GetComponent<Platformer2DUserControl>();
        if (robot != null) {
            robot.applyStasis(duration-(Time.time-startTime));
        }
    }

}
