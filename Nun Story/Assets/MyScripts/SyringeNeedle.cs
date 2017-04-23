using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class SyringeNeedle : NetworkBehaviour
{

	public float slowEffect;
	public float slowDuration;

    private float startTime;
	public float projectileDuration = 4f;

    private Rigidbody2D rb;

	// Use this for initialization
	void Start ()
	{
		rb = gameObject.GetComponent<Rigidbody2D> ();
		//rb.velocity = new Vector2 (50, 0);
		this.transform.Rotate (0, 0, 0);
        startTime = Time.time;
    }
	
	// Update is called once per frame
    [ServerCallback]
	void Update ()
	{
        //this object is destroyed after 4 seconds if it hasnt touched anything else yet
		if (Time.time - startTime >= projectileDuration && gameObject != null) {
            NetworkServer.Destroy(gameObject);
		}
		//projectileDuration -= Time.deltaTime;
	}

	private void OnCollisionEnter2D (Collision2D collision)
	{
        //if this hits a player, it slows their movement speed by 50% for 3 seconds.
        GameObject other = collision.gameObject;

		if (collision.gameObject.CompareTag ("Player")) {
            print("hit player");

            Platformer2DUserControl robot = collision.gameObject.GetComponent<Platformer2DUserControl>();
            if (robot != null) {
                robot.applySlow(0.5f, 3f);
            }
        }
        NetworkServer.Destroy(gameObject);
	}


}
