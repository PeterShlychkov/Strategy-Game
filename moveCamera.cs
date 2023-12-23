using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public GameObject cam;

    private float xspeed;
    private float zspeed;

    public float cameraSpeed = 0.05f;

    private Vector3 cameraMovement;

    void Start()
    {
        cam.transform.position = new Vector3(400, 320, 400);
    }

    void Update()
    {
       cameraMovement = new Vector3(xspeed, 0, zspeed);
       cam.transform.Translate(cameraMovement);
        if (Input.GetKey(KeyCode.W))
        {
            zspeed = cameraSpeed;
        } 
        else if (Input.GetKey(KeyCode.S))
        {
            zspeed = cameraSpeed * -1;
        } 
        else
        {
            zspeed = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            xspeed = cameraSpeed;
        } 
        else if (Input.GetKey(KeyCode.A))
        {
            xspeed = cameraSpeed * -1;
        } 
        else
        {
            xspeed = 0;
        }
    }
}
