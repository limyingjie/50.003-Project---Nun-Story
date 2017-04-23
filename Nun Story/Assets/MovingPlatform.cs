using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public float magnitude;
    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        //platform moves vertically in a sinusoidal motion
        transform.Translate(new Vector3(0, Mathf.Sin(Time.time-startTime), 0) * magnitude * Time.deltaTime);
    }
}
