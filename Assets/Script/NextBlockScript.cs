using UnityEngine;
using System.Collections;

public class NextBlockScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManagerScript.gameoverflag == false)
        {
            transform.eulerAngles += new Vector3(0, 0.2f, 0);
        }
	}
}
