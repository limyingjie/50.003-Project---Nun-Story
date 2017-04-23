using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
        if (otherRB != null) { otherRB.velocity = new Vector2(-otherRB.velocity.x, -otherRB.velocity.y); }
    }

}
