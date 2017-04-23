using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour {
    private int position;
	// Use this for initialization
	void Start () {
        position = 0;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            position += 1;
            print("chanign scene");
            SceneManager.LoadScene("End Scene" + position);
        }
    }
}
