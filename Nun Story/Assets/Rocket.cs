using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.Networking;

public class Rocket : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -50);
        //rocket flies downwards at 50 units/second
	}

    // Update is called once per frame
    [ServerCallback]
    void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //when rocket hits something, it explodes,
        //interrupting the movement of players within a 5 unit radius and stunning them for 0.4 seconds
        Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, 5);
        foreach (Collider2D cd in inRange)
        {
            GameObject other = cd.gameObject;             
            Platformer2DUserControl robot = other.GetComponent<Platformer2DUserControl>();
            if (robot != null) {
                robot.applyStun(0.4f);
                other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            }
        }
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
