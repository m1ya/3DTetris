using UnityEngine;
using System.Collections;

public class DownScript : MonoBehaviour {

    bool flag;

    // Use this for initialization
    void Start () {
        flag = true;
    }
	
	// Update is called once per frame
	void Update () {
            Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (flag == true && other.gameObject.tag == "Block")
        {
            other.gameObject.SendMessage("Flag");
            other.gameObject.SendMessage("Position");
            flag = false;
        }
    }

}
