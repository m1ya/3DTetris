using UnityEngine;
using System.Collections;

public class CameraContollerScript : MonoBehaviour
{
    public Transform target;
    public float spinSpeed = 1f;
    public float wheelSpeed = 1f;
    float distance = 70f;

    Vector3 nowPos;
    Vector3 pos = Vector3.zero;
    public Vector2 mouse = Vector2.zero;

    // Use this for initialization
    void Start()
    {
        // Canera get Start Position from Player
        nowPos = transform.position;

        mouse.x = -0.15f;
        mouse.y = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        //拡大縮小
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0.0f)
        {
            transform.position += transform.forward * scrollWheel * wheelSpeed;
        }

        // Get MouseMove
        if (Input.GetMouseButton(0))
        {
            mouse += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * spinSpeed;
        }
        // Clamp mouseY move
        mouse.y = Mathf.Clamp(mouse.y, -0.3f + 0.5f, 0.3f + 0.5f);

        // sphere coordinates
        pos.x = distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Cos(mouse.x * Mathf.PI);
        pos.y = -distance * Mathf.Cos(mouse.y * Mathf.PI);
        pos.z = -distance * Mathf.Sin(mouse.y * Mathf.PI) * Mathf.Sin(mouse.x * Mathf.PI);
        // r and upper
        pos *= nowPos.z;

        pos.y += nowPos.y;
        //pos.x += nowPos.x; // if u need a formula,pls remove comment tag.

        transform.position = pos + target.position;
        transform.LookAt(target.position);
    }

}
