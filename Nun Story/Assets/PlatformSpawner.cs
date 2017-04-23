using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlatformSpawner : NetworkBehaviour {

    public GameObject movingPlatform;
    public GameObject landmine;
    // Use this for initialization
    public override void OnStartServer()
    {
        print("spawning movable platform");
        float[] platform_coord = { 112.9715f, 7.5f, -22.9753f + 225.5253f, 22.78895f - 9.56f };

        for (int i = 0; i < platform_coord.Length; i += 2)
        {
            GameObject newPlatform = Instantiate(movingPlatform, new Vector3(platform_coord[i], platform_coord[i + 1] - 7f), Quaternion.identity);
            NetworkServer.Spawn(newPlatform);
        }

        print("spawning networked landmines");
        float[] landmine_coord = { -192.2653f, -22.93895f, -131.42f, -15.21f, 36.96f, 1.04f , 56.27f , 1.15f , 126.01f , 1.18f , -76.35f , -19.41f , -40.61f , -12.54f , -13.3f , 1.01f };

        for (int i = 0; i < landmine_coord.Length; i += 2)
        {
            GameObject newLandmine = Instantiate(landmine, new Vector3(225.5253f + landmine_coord[i], 22.78895f + landmine_coord[i + 1]), Quaternion.identity);
            NetworkServer.Spawn(newLandmine);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
