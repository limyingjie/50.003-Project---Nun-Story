using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : NetworkBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private status_animator_controller m_status;
        private bool m_Jump;
        private Animator m_Anim;
        private string item = "";
        private int itemNum = 0;

        public GameObject bullet;
        public Transform bulletSpawn;
        private float bulletCd = 1 / 16f;
        private float bulletCdTimer = 0;
        private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls

        public GameObject blackHole;
        public Transform blackHoleSpawn;

        public GameObject bomb;
        public Transform bombSpawn;
        private float bombCd = 2f;
        private float bombCdTimer = 0;

        public GameObject syringe;
        public Transform syringeSpawn;
        private float syringeCd = 1 / 4f;
        private float syringeCdTimer = 0;

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

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            m_Anim = GetComponent<Animator>();
            m_status = GetComponentInChildren<status_animator_controller>();
        }




        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }


            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }


            bool useItem = CrossPlatformInputManager.GetButton("Item");
            //bool useItem = Input.GetKey(KeyCode.Alpha5);


            if (useItem && !item.Equals(""))
            {
                if (item.Equals("Item1"))
                {
                    //CmdBulletFire();
                    CmdBlackHoleFire();
                }

                if (item.Equals("Item2"))
                {
                    CmdBombFire();
                }

                if (item.Equals("Item3"))
                {
                    CmdSyringeFire();
                }
                if (item.Equals("Item4"))
                {
                    CmdRocketsFire();
                }
                item = "";
                itemNum = 0;
            }


            //Player has item
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

            m_Anim.SetInteger("Item", itemNum);

            syringeCdTimer -= Time.deltaTime;

            if (isStunned)
            {
                stunDuration -= Time.deltaTime;
                if (stunDuration <= 0) { isStunned = false; print("no more stun");
                    statusNum = 0;
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }
            if (slowMultiplier != 1) {
                slowDuration -= Time.deltaTime;
                if (slowDuration <= 0) { slowMultiplier = 1; print("no more slow");
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }

            if (inStasis) {
                stasisDuration -= Time.deltaTime;
                if (stasisDuration <= 0) {
                    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    inStasis = false;
                    CmdsendStatus(0);
                    m_status.Status(0);
                }
            }

        }

        public void IgnoreCollider(Collider c)
        {
            Physics.IgnoreCollision(c, GetComponent<Collider>());
        }

        [Command]
        void CmdBulletFire()
        {
            print("item1");
            if (Time.time >= bulletCdTimer)
            {
                float spray = UnityEngine.Random.Range(-30f, 30f) / 180 * Mathf.PI;
                //GameObject newBullet = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                GameObject newBullet = Instantiate(bullet, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);

                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(100 * Mathf.Sin(spray), 100 * Mathf.Cos(spray));
                if (isLocalPlayer)
                {
                    Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                    Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());
                    Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<CircleCollider2D>());
                }
                bulletCdTimer = Time.time + bulletCd;

                // Spawn the bullet on the Clients
                NetworkServer.Spawn(newBullet);
            }
        }

        [Command]
        void CmdBlackHoleFire()
        {
            print("blackHole");

            GameObject newHole = Instantiate(blackHole, new Vector2(transform.position.x-5, transform.position.y+5), Quaternion.identity);
            
            print("Spawning blackhole on servers");
            NetworkServer.Spawn(newHole);
        }


        [Command]
        void CmdBombFire()
        {
            print("item2");
            if (Time.time >= bombCdTimer)
            //if (true)
            {
                int facingdirection = 1;

                if (!m_Character.getFacingRight())
                {
                    facingdirection = -1;
                    print("facing left");
                }
                
                GameObject newBomb = Instantiate(bomb, new Vector2(gameObject.transform.position.x , gameObject.transform.position.y + 3), Quaternion.identity);
                newBomb.GetComponent<Rigidbody2D>().velocity = new Vector2(15f * Mathf.Cos(45f / 180f * Mathf.PI)*facingdirection , 10f * Mathf.Sin(90f / 180f * Mathf.PI));
                if (true)
                {
                    Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
                    Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<BoxCollider2D>());
                    Physics2D.IgnoreCollision(newBomb.GetComponent<Collider2D>(), gameObject.GetComponent<CircleCollider2D>());
                }
                bombCdTimer = Time.time + bombCd;

                    // Spawn the bomb on the Clients
                print("Spawning bomb on servers");
                NetworkServer.Spawn(newBomb);
            }
        }




        [Command]
        void CmdSyringeFire()
        {
            print("item3");
            //if (true)
            if (Time.time>=syringeCdTimer)
            {

                int facingdirection = 1;

                if (!m_Character.getFacingRight())
                {
                    facingdirection = -1;
                    print("facing left");
                }

                GameObject newSyringe = Instantiate(syringe, new Vector2(gameObject.transform.position.x + 2.5f*facingdirection, gameObject.transform.position.y), Quaternion.identity);
                newSyringe.GetComponent<Rigidbody2D>().velocity = new Vector2(50*facingdirection,0);
                syringeCdTimer = syringeCd;
                syringeCdTimer = Time.time + syringeCd;

                print("Spawning syringes on servers");
                NetworkServer.Spawn(newSyringe);
                //RpcSyringeFire(newSyringe);
            }
        }

        [Command]
        void CmdRocketsFire()
        {
            print("item4");

            GameObject newRockets = Instantiate(rockets, new Vector2(transform.position.x, 70), Quaternion.identity);

            print("Spawning rockets on servers");
            NetworkServer.Spawn(newRockets);
        }


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
            m_Character.Move(move, crouch, jump);
        }

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
            if (!isStunned || !inStasis) {
                m_Character.Move(h*slowMultiplier, crouch, m_Jump);
                CmdMove(h, crouch, m_Jump);
            }

            m_Jump = false;

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
            Camera.main.GetComponent<CameraFollow>().setTarget(gameObject.transform);
        }

        public void setItem(string item) { this.item = item; }

        public void applyStun(float duration)
        {
            CmdsendStatus(2);
            m_status.Status(2);
            statusNum = 2;
            isStunned = true;
            stunDuration = duration;
            print("tio stun liao");
        }
        //update
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
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            print("oh no!");
        }

        public int getStatus()
        {
            return statusNum;
        }
        

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
            m_status.Status(statusUpdate);
        }
    }
}
