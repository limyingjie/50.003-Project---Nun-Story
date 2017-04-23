using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteHole : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		Collider2D[] inRange = Physics2D.OverlapCircleAll (transform.position, 20);
		foreach (Collider2D cd in inRange) {
			GameObject other = cd.gameObject;
			Rigidbody2D rb = other.GetComponent<Rigidbody2D> ();
			if (rb != null) {
				rb.AddForce (new Vector2 (50 / (other.transform.position.x - transform.position.x),
					50 / (other.transform.position.y - transform.position.y)));
			}
		}
	}
}
