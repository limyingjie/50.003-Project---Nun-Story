using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets._2D
{

    public class ItemPickup : MonoBehaviour
    {
        private float respawnTime = 3.0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //player will pick up an item when they touch the pickup
            //once picked up, the pickup will respawn after 3 seconds
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject player = collision.gameObject;
                getItem(player);
                StartCoroutine(respawn(gameObject, respawnTime));
            }

        }

        private void getItem(GameObject player)
        {
            /* ITEM DROP TABLE
             * ITEM_CODE    ITEM_NAME           DROPRATE
             * Item1        Black Hole          25%
             * Item2        Holy Hand Grenade   25%
             * Item3        Buns                25%         //Note: Bun is referred to as SyringeNeedle in the code
             * Item4        Rocket Barrage      25%
             */
            float roll = Random.value;
            string item = "";
            if (roll <= 0.25) { item = "Item1"; }//item1
            else if (roll <= 0.5) { item = "Item2"; }//item2
            else if (roll <= 0.75) { item = "Item3"; }//item3
            else if (roll <= 1) { item = "Item4"; }//item4
            print("Got "+item);

            Platformer2DUserControl playerCtrl = player.GetComponent<Platformer2DUserControl>();
            playerCtrl.setItem(item); //update the item variable on the other script
        }

        IEnumerator respawn(GameObject gameObject, float respawnTime)
        {
            //the sprite renderer and collider is disabled when the pickup is respawning
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(respawnTime);
            gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.GetComponent<Collider2D>().enabled = true;


        }
    }
}