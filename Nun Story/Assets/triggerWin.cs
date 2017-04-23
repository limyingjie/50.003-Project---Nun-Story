using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;

[RequireComponent(typeof(Platformer2DUserControl))]
public class triggerWin : MonoBehaviour {

    private double position = 0;
    public Canvas myCanvas;
    public Canvas controls;
    public Text fooText;
    private Text t;
    private Platformer2DUserControl m_Character;
    // Use this for initialization
    void Start () {
        myCanvas.enabled = false;
        //Transform child = transform.Find("Winmessage");
        //m_Character = GetComponent<Platformer2DUserControl>();
        //Text t = child.GetComponent<Text>();
        t = fooText.GetComponent<Text>();
    }

    // Update is called once per frame
 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            myCanvas.enabled = true;
            position += 0.5;
            t.text = "You made it!!";
            other.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        }
        

    }

}
