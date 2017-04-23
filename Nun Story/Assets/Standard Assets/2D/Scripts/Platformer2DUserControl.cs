using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : NetworkBehaviour
    {
        
        private PlatformerCharacter2D m_Character; //intializes the PLatformerCharacter2D instance
        private status_animator_controller m_status; //initializes the statusanimatercontroller script
        private bool m_Jump; 
        private Animator m_Anim;
        private string item = "";
        private int itemNum = 0;

        private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls

        public GameObject blackHole;
        public Transform blackHoleSpawn;

        public GameObject bomb;
        public Transform bombSpawn;

        public GameObject syringe;
        public Transform syringeSpawn;

        public GameObject rockets;
        public Transform rocketsSpawn;

        public GameObject movingPlatform;

        private bool isStunned = false;
        private float stunDuration = 0f;

        private float slowMultiplier = 1;
        private float slowDuration = 0f;

        private bool inStasis = false;
        private float stasisDuration = 0f;

        private float statusUpdateTimer;
        private int statusNum = 0;

        private bool finished = false;

        private void Awake()
        {
            // Attaches each object to the releveant component
            m_Character = GetComponent<PlatformerCharacter2D>();
            m_Anim = GetComponent<Animator>();
            m_status = GetComponentInChildren<status_animator_controller>();
        }

        
        


        private void Update()
        {
            // set controlling the player only to local player
            if (!isLocalPlayer)
            {
                return;
            }


            // Set jump only if jump is not true has not been pressed, this prevents double jumping
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }


            //When item button is pressed, player will use the item they have in hand
            bool useItem = CrossPlatformInputManager.GetButton("Item");

            //If button is pressed and item is available, check which item the player has and use it
            if (useItem && !item.Equals(""))
            {
                //Sends spawn information to the server
                if (item.Equals("Item1")) {
                    CmdBlackHoleFire();
                }

                else if (item.Equals("Item2")) {
                    CmdBombFire();
                }

                else if (item.Equals("Item3")) {
                    CmdSyringeFire();
                }
                else if (item.Equals("Item4")) {
                    CmdRocketsFire();
                }
                // Resets item information, the player will have used the item at thsi point
                item = "";
                itemNum = 0;
            }


            //Player has item
            //sprite animator reads itemNum to determine what item the player is holding
            if (!item.Equals(""))
            {
                if (item.Equals("Item2"))
                {
                    itemNum = 2;
                }
                if (item.Equals("Item3"))
                {
                    itemNum = 3;
                }
            }
            //Sets animator to correspong to the correct item number
            m_Anim.SetInteger("Item", itemNum);


            //status effect timers update here
            if (isStunned)
            {
                stunDuration -= Time.deltaTime;
                if (stunDuration <= 0) { isStunned = false; print("no more stun");
                    // reset status to normal locally and on server.
                    statusNum = 0;
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }
            if (slowMultiplier != 1) {
                slowDuration -= Time.deltaTime;
                if (slowDuration <= 0) { slowMultiplier = 1; print("no more slow");
                    // reset status to normal locally and on server.
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }

            if (inStasis)
            {
                stasisDuration -= Time.deltaTime;
                if (stasisDuration <= 0)
                {
                    // reset status to normal locally and on server.
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    inStasis = false;
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }
            //if status is normal release all constraints
            else { GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; }

        }
        

        // Blackhole server command
        [Command]
        void CmdBlackHoleFire()
        {
            print("blackHole");

            GameObject newHole = Instantiate(blackHole, new Vector2(transform.position.x-10, transform.position.y+5), Quaternion.identity);
            
            print("Spawning blackhole on servers");
            // Spawn blakhole on the server
            NetworkServer.Spawn(newHole);
        }


        //Bomb server command
        [Command]
        void CmdBombFire()
        {
            print("item2");

            int facingdirection = 1;

            // spawn bomb in the direction the player is facing
            if (!m_Character.getFacingRight())
            {
                facingdirection = -1;
                print("facing left");
            }
                
            GameObject newBomb = Instantiate(bomb, new Vector2(gameObject.transform.position.x , gameObject.transform.position.y + 3), Quaternion.identity);
            newBomb.GetComponent<Rigidbody2D>().velocity = new Vector2(15f * Mathf.Cos(45f / 180f * Mathf.PI)*facingdirection , 10f * Mathf.Sin(90f / 180f * Mathf.PI));

            //ignore collision of local player
            Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<CircleCollider2D>());

            // Spawn bomb on the server
            print("Spawning bomb on servers");
            NetworkServer.Spawn(newBomb);
        }


        [Command]
        void CmdSyringeFire()
        {
            print("item3");


            int facingdirection = 1;

            // shoots syringe in the direction player is facing
            if (!m_Character.getFacingRight())
            {
                facingdirection = -1;
                print("facing left");
            }

            for (float i = 60f; i <= 120f; i += 60f / 6f)
            {
                GameObject newSyringe= Instantiate(syringe, new Vector2(transform.position.x + facingdirection*3 * Mathf.Sin((i * Mathf.PI) / 180f), transform.position.y + 3 * Mathf.Cos((i * Mathf.PI) / 180f)), Quaternion.identity);
                newSyringe.GetComponent<Rigidbody2D>().velocity = new Vector2(50 * Mathf.Sin((i * Mathf.PI) / 180f)*facingdirection, 50 * Mathf.Cos((i * Mathf.PI) / 180f));
                newSyringe.transform.Rotate(0, 0, (-i+90 )* facingdirection);
                // Spawn syringe on the server
                NetworkServer.Spawn(newSyringe);
            }

            print("Spawning syringes on servers");
            
        }


        [Command]
        void CmdRocketsFire()
        {
            print("item4");

            GameObject newRockets = Instantiate(rockets, new Vector2(transform.position.x, 70), Quaternion.identity);

            // Spawns rockets on server
            print("Spawning rockets on servers");
            NetworkServer.Spawn(newRockets);
        }


        // Server move command
        [Command]
        void CmdMove(float move, bool crouch, bool jump)
        {
            RpcMove(move, crouch, jump);
        }

        [ClientRpc]
        void RpcMove(float move, bool crouch, bool jump)
        {
            if (isLocalPlayer)
                return;
            //Plays animations for non-local players
            m_Character.Move(move, crouch, jump);
        }

        //movement is handled here
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            // Read the inputs.
            // Pass all parameters to the character control script.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //player movement is disabled when in stun or stasis
            if (!isStunned && !inStasis) {
                m_Character.Move(h*slowMultiplier, crouch, m_Jump);
                CmdMove(h, crouch, m_Jump);
            }

            //set jump to false by default
            m_Jump = false;

            //repeating status updater
            if (statusNum == 0 && ((Time.time - statusUpdateTimer) > 1.0f))
            {
                CmdsendStatus(0);
                statusUpdateTimer = Time.time;
            }
            print("Local status is: " + statusNum);

        }


        public override void OnStartLocalPlayer()
        {
            print("camera attached to robot");
            //Attaches main camera when local player is started
            Camera.main.GetComponent<CameraFollow>().setTarget(gameObject.transform);
        }


        public void setItem(string item) { this.item = item; }


        //public methods for items to apply status effect to player
        public void applyStun(float duration)
        {
            CmdsendStatus(2);
            m_status.Status(2);
            statusNum = 2;
            isStunned = true;
            stunDuration = duration;
            print("tio stun liao");
        }

        public void applySlow(float multiplier,float duration)
        {
            CmdsendStatus(1);
            m_status.Status(1);
            statusNum = 1;
            slowMultiplier = multiplier;
            slowDuration = duration;
            print("tio slow liao");
        }

        public void applyStasis(float duration)
        {
            CmdsendStatus(3);
            m_status.Status(3);
            statusNum = 3;
            inStasis = true;
            stasisDuration = duration;
            if (isLocalPlayer) { GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation; }
            print("oh no!");
        }

        public void applyfinished()
        {
            finished = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }

        public int getStatus()
        {
            return statusNum;
        }
        
        //Sends status to server
        [Command]
        void CmdsendStatus(int statusUpdate)
        {
            RpcsendStatus(statusUpdate);
            statusNum = statusUpdate;
        }

        [ClientRpc]
        void RpcsendStatus(int statusUpdate)
        {
            if (isLocalPlayer)
            {
                return;
            }
            //Sets status of non-local players
            m_status.Status(statusUpdate);
        }
    }
}