using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HelpScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void Back()
    {
        SceneManager.LoadScene("Start");
    }
}
