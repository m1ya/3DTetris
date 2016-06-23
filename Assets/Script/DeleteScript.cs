﻿using UnityEngine;
using System.Collections;

public class DeleteScript : MonoBehaviour {

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Block")
        {
            Vector3 Pos = transform.position;
            GameManagerScript.field[(int)Pos.x, (int)Pos.y, (int)Pos.z] = 0;
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
