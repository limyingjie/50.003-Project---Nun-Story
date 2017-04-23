using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class FragmentBehaviour : NetworkBehaviour {
    public GameObject fragment;
    private float startTime;
    private float fuseTime;
	// Use this for initialization
	void Start () {
        startTime = Time.time;
        fuseTime = 2f;
	}
	
	// Update is called once per frame
    [ServerCallback]
	void Update () {
        //detonate the fragment if 2 seconds is up
        if (Time.time - startTime >= fuseTime)
        {
            explode();
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //detonate the fragment if it collides with another object
        explode();
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

    private void explode() {
        //interrupts the movement of players within a 5 unit radius, and applies a 0.4 second stun to them.
        Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, 5);
        foreach (Collider2D cd in inRange)
        {
            GameObject other = cd.gameObject;
            Platformer2DUserControl robot = other.GetComponent<Platformer2DUserControl>();
            if (robot != null)
            {
                robot.applyStun(0.4f);
                other.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
    }
}
