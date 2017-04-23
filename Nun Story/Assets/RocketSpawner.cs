using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketSpawner : NetworkBehaviour {
    public GameObject rocket;

    private float coolDown = 0.2f;
    private float coolDownTimer = 0;
    private float startTime;
    private float duration = 6f;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
    [ServerCallback]
	void Update () {
        //the rocket spawner will spawn rockets every 0.2 seconds for a duration of 6 seconds
        if (Time.time >= coolDownTimer) {
            RocketFire();
            coolDownTimer = Time.time + coolDown;
        }
        if (Time.time - startTime >= duration) { Destroy(gameObject); NetworkServer.Destroy(gameObject); }
	}

    
    void RocketFire() {
        //rockets will spawn randomly between a horizontal range of -30 to 10 units from where the rocket barrage was initiated
        float spread = Random.Range(-30, 10);
        GameObject rocketObject = Instantiate(rocket, new Vector3(transform.position.x + spread, transform.position.y), Quaternion.identity);
        rocketObject.transform.Rotate(0, 0, -120);
        NetworkServer.Spawn(rocketObject);
    }
}
