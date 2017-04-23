using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class Projectile : NetworkBehaviour
{

    private Rigidbody2D rb;
    public float velocity;
    public float direction;
    private float fuseTime;
    private float startTime;
    public GameObject fragment;

    // Use this for initialization    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        fuseTime = 1.5f;
        startTime = Time.time;
        //initial velocity of projectile will be specified from the Platformer2DUserControl
    }


    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        //the bomb explodes if 1.5 seconds has passed since it was thrown
        gameObject.transform.Rotate(0,0,-120*Time.deltaTime);
        if (Time.time - startTime >= fuseTime) {
            explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //bomb also explodes if it comes into contact with another object
        explode();
    }

    public void setStartTime(float startTime) { this.startTime = startTime; }

    private void explode() {

        //this bomb explodes into 8 fragments, which fly in all directions
        for(float i = 0; i < 360; i+=360f/8f)
        {
            GameObject newfragment = Instantiate(fragment, new Vector2(transform.position.x + 2*Mathf.Sin((i*Mathf.PI)/180f), transform.position.y + 2 * Mathf.Cos((i * Mathf.PI) / 180f)), Quaternion.identity);
            newfragment.GetComponent<Rigidbody2D>().velocity = new Vector2(50 * Mathf.Sin((i * Mathf.PI) / 180f), 50 * Mathf.Cos((i * Mathf.PI) / 180f));
            NetworkServer.Spawn(newfragment);
        }

        //interrupts movement of players within 5 units radius and applies a 0.4 second stun to them
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
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
