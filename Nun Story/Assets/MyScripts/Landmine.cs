using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class Landmine : NetworkBehaviour {
    public float blastForce;
    private bool exploded = false;
    private float startTime; //the time at which the landmine started to explode
    private Animator m_Anim;

    // Use this for initialization
    private void Awake()
    {
        m_Anim = GetComponent<Animator>(); //get a reference to the game object's sprite animator
    }

    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        m_Anim.SetBool("Exploded", exploded); 
        if (((Time.time - startTime) > 0.75f) && exploded)
        {
            //the landmine object is destroyed after the exploding animation plays for 0.75 seconds
            print("telling network to destroy landmine");
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }


    [ClientRpc]
    void RpcSendExploded(bool explodedstatus)
    {
        exploded = explodedstatus;
        startTime = Time.time;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //explode if a player comes within range
        if (collision.gameObject.CompareTag("Player") && !exploded) {
            exploded = true;
            RpcSendExploded(exploded);
            print("sending exploded status to server");
            startTime = Time.time;

            //knocks back all rigidbodies and applies a 0.5 second stun to players within the blast radius of 20 units
            Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, 20);
            foreach (Collider2D cd in inRange)
            {
                GameObject other = cd.gameObject;
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (other.transform.position - transform.position).normalized;
                    rb.AddForce(new Vector2(direction.x,direction.y+0.2f) * blastForce);
                    Platformer2DUserControl robot = other.GetComponent<Platformer2DUserControl>();
                    if (robot != null) { robot.applyStun(0.5f); };

                }
            }



        }

    }
}
