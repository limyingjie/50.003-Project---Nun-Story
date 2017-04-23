using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

	public float speed = 50f;
	public float jumpPower = 150f;
	public float maxSpeed = 3;
    public string inputDirection;
    public string inputJump;

	public bool grounded = true;
	private Rigidbody2D rb2d;
	private Animator anim;

	public GameObject bullet;
	private float bulletCd = 1 / 16f;
	private float bulletCdTimer = 0;
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls

	void Blarg() {
		//Tell server to set this player as not ready before loading a scene
		NetworkServer.SetClientNotReady( connectionToServer );

		//now load the scene
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("imdone", UnityEngine.SceneManagement.LoadSceneMode.Additive);
	}


	// Use this for initialization
	void Start() {
		anim = gameObject.GetComponent<Animator> ();
		rb2d = gameObject.GetComponent<Rigidbody2D> ();

		//tell server that this client should be ready now
		NetworkServer.SetClientReady( connectionToServer );
	}


	// Update is called once per frame
	void Update ()
	{

        int horizontal = 0;
        int vertical = 0;

        //if (!isLocalPlayer)
        //{
        //    return;
        //}

        //sets boolean grounded to outside variable gounded
        anim.SetBool ("grounded", grounded);
        //sets float speed to horizontal input
		anim.SetFloat ("speed", Mathf.Abs (Input.GetAxis (inputDirection)));

        //Check if we are running either in the Unity editor or in a standalone build.
        #if  UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw(inputDirection));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw(inputJump));


        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE


        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                float x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                float y = touchEnd.y - touchOrigin.y;

                //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                    horizontal = x > 0 ? 1 : -1;
                else
                    //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                    vertical = y > 0 ? 1 : -1;
            }
        }

#endif  //End of mobile platform dependendent compilation section started above with #elif
        //check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 )
        {
            //makes player face left if input direction is left
            if (horizontal < 0) {

                transform.localScale = new Vector3 (-1, 1, 1);
            }

            //makes player face right if input direction is right
		    if (horizontal > 0) {
                transform.localScale = new Vector3 (1, 1, 1);
            }
        }




        //adds force to rigid body if grounded and inputjump is pressed, nothing otherwise
		if (Input.GetButtonDown (inputJump) && grounded == true) {

			rb2d.AddForce (Vector2.up * jumpPower);
		}

		if (Input.GetKey (KeyCode.Alpha1)) {
			if (Time.time >= bulletCdTimer) {
				float spray = Random.Range (-30f, 30f) / 180 * Mathf.PI;
                GameObject newBullet = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y + 2), Quaternion.identity);
                newBullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (100 * Mathf.Sin (spray), 100 * Mathf.Cos (spray));
                Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
				bulletCdTimer = Time.time + bulletCd;
			}
		}
	}

	void FixedUpdate ()
	{
		float h = Input.GetAxis (inputDirection);

        //calculates force on player object
		rb2d.AddForce ((Vector2.right * speed) * h);

        //limits speed to maxspeed if velocity is more than maxspeed (moving right)
		if (rb2d.velocity.x > maxSpeed) {
			rb2d.velocity = new Vector2 (maxSpeed, rb2d.velocity.y);
		}

        //limits speed to maxspeed if velocity is more than maxspeed (moving left)
		if (rb2d.velocity.x < -maxSpeed) {
			rb2d.velocity = new Vector2 (-maxSpeed, rb2d.velocity.y);
		}
	}
}
