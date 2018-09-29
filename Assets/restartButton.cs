using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restartButton : MonoBehaviour {

    private Scene scene;

	// Use this for initialization
	void Start () {
        scene = SceneManager.GetActiveScene();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Test");
        if (other.tag == "Player")
        {
            Debug.Log("Restart Pressed");
            SceneManager.LoadScene(scene.name);

        }

    }

}

