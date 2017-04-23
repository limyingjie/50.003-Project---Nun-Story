using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControl : MonoBehaviour
{

	private Rigidbody2D rb;
    private AudioSource source;
    public AudioClip voiceClip;
    public AudioClip shootClip;

	public GameObject bullet;
	private float bulletCd = 1 / 12f;
	private float bulletCdTimer = 0;
    private float channelCd = 5f;
    private float channelCdTimer=0;
    private float channelDuration = 3f;
    private float channelTimer = 0;

	public GameObject bomb;
	private float bombCd = 2f;
	private float bombCdTimer = 0;

    public GameObject syringe;
    private float syringeCd = 1 / 4f;
    private float syringeCdTimer = 0;

    private bool isChanelling = false;
    private bool isStunned=false;
    private float stunDuration = 0f;


	// Use this for initialization
	void Start ()
	{
		rb = gameObject.GetComponent<Rigidbody2D> ();
        source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update ()
	{
        Vector2 velocity = rb.velocity;
        if (!isChanelling)
        {
            if (Input.GetKey(KeyCode.A))
            {
                velocity = new Vector2(-10, rb.velocity.y);
            }
            if (Input.GetKey(KeyCode.D))
            {
                velocity = new Vector2(10, rb.velocity.y);
            }
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(new Vector2(rb.velocity.x, 50 - rb.velocity.y * 2));
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (Time.time >= bulletCdTimer)
                {
                    float spray = Random.Range(-30f, 30f) / 180 * Mathf.PI;
                    GameObject newBullet = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y + 0), Quaternion.identity);
                    newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(100 * Mathf.Sin(spray), 100 * Mathf.Cos(spray));
                    Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    bulletCdTimer = Time.time + bulletCd;
                }
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                if (Time.time >= bombCdTimer)
                {
                    GameObject newBomb = Network.Instantiate(bomb, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity,0) as GameObject;
                    Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    bombCdTimer = Time.time + bombCd;
                }
            }

            if (Input.GetKey(KeyCode.Alpha3))
            {
                if (syringeCdTimer <= 0)
                {
                    GameObject newSyringe = Instantiate(syringe, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                    Physics2D.IgnoreCollision(newSyringe.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    syringeCdTimer = syringeCd;
                }
            }
            syringeCdTimer -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Alpha4))
            {
                if (channelCdTimer <= 0)
                {
                    source.PlayOneShot(voiceClip, 1F);
                    isChanelling = true;
                    channelTimer = channelDuration;
                    channelCdTimer = channelCd;
                }
            }
        }
        if (isChanelling)
        {
            if (channelTimer >= 0)
            {
                channelTimer -= Time.deltaTime;
                if (Time.time >= bulletCdTimer)
                {
                    float spray = Random.Range(120f, 150f) / 180 * Mathf.PI;
                    GameObject newBullet = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y + 0), Quaternion.identity);
                    newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(100 * Mathf.Sin(spray), 100 * Mathf.Cos(spray));
                    Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    source.PlayOneShot(shootClip, 0.1F);
                    bulletCdTimer = Time.time + bulletCd;
                }
            }
            else
            {
                isChanelling = false;
            }
        }
        channelCdTimer -= Time.deltaTime;

        if (isStunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0) { isStunned = false; }
        }
        


        if (isChanelling) rb.velocity = new Vector2(0, 0);
        else if (!isStunned) { rb.velocity = velocity; }

    }

    public void applyStun(float duration) {
        isStunned = true;
        stunDuration = duration;
    }

}
