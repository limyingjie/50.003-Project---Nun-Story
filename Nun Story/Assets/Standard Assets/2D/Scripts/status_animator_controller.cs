using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class status_animator_controller : MonoBehaviour {

    private Animator m_Anim;
    private int localStatus;
    // Use this for initialization

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
    }

    public void Status(int status)
    {
        if (status != localStatus)
        {
            print("Received Status code is: " + status);
        }
        m_Anim.SetInteger("Status", status);
        localStatus = status;
    }

}
