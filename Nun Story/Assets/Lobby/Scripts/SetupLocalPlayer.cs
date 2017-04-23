using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets._2D;

public class SetupLocalPlayer : NetworkBehaviour {

    [SyncVar]
    public string pname = "player";
    [SyncVar]
    public Color playerColor = Color.white;

    private PlatformerCharacter2D m_Character;
    private bool isFacingRight = true;

    // Use this for initialization
    void Start()
    {
        m_Character = gameObject.GetComponentInParent<PlatformerCharacter2D>();
        if (isLocalPlayer)
        {
            GetComponentInChildren<TextMesh>().text = pname;
            GetComponentInChildren<TextMesh>().color = playerColor;
            print("start" + pname);
            CmdChangeName(pname,playerColor);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (m_Character.getFacingRight() != isFacingRight) { Flip(); }
    }
    
    void OnGUI()
    {
        //if (isLocalPlayer)
        //{
        //    pname = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), pname);
        //    if (GUI.Button(new Rect(130, Screen.height - 40, 80, 30), "Change"))
        //    {
        //        GetComponentInChildren<TextMesh>().text = pname;
        //        GetComponentInChildren<TextMesh>().color = playerColor;
        //        CmdChangeName(pname);
        //    }
        //}
    }

    [Command]
    public void CmdChangeName(string newName, Color newColor) {
        GetComponentInChildren<TextMesh>().text = newName;
        GetComponentInChildren<TextMesh>().color = newColor;
        print("cmd" + newName);
        RpcChangeName(newName,newColor);
    }

    [ClientRpc]
    public void RpcChangeName(string newName,Color newColor){
        if (isLocalPlayer)
        {
            return;
        }
        GetComponentInChildren<TextMesh>().text = newName;
        GetComponentInChildren<TextMesh>().color = newColor;
        print("rpc" + newName);
    }


    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 theScale = GetComponentInChildren<TextMesh>().gameObject.transform.localScale;
        theScale.x *= -1;
        GetComponentInChildren<TextMesh>().gameObject.transform.localScale = theScale;
    }
}
