using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

    bool flag = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (flag == true)
        {
            //Blockの落下処理
            transform.position += Vector3.down * Time.deltaTime;

            //Blockの操作処理
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Block" || other.gameObject.tag == "Wall")
        {
            //下方向にRayを飛ばす
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            //Rayが距離0.5で何かにぶつかったら
            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                flag = false;
            }
        }
    }

}
