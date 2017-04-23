using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
	private float startTime;
	private float fuseTime;


	// Use this for initialization
	void Start ()
	{
		startTime = Time.time;
		fuseTime = 2f;
	}
	
	// Update is called once per frame
    [ServerCallback]
	void Update ()
	{
		if (Time.time - startTime >= fuseTime) {
			NetworkServer.Destroy (gameObject);        }

	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NetworkServer.Destroy(gameObject);
    }
}
