using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position += Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.eulerAngles += new Vector3(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.eulerAngles += new Vector3(90, 0, 0);
        }
    }
}
