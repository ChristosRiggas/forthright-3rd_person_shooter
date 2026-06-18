using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    public Camera mainCamera;
    Ray ray;
    RaycastHit hitInfo;

    //public GameObject player;

    // Update is called once per frame
    void Update()
    {
        //float cameraDistanceOffset = Vector3.Distance(mainCamera.transform.position, player.transform.position);

        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        //Physics.Raycast(ray, out hitInfo);
        //transform.position = hitInfo.point;

        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = ray.origin + ray.direction * 1000.0f;
        }

        //Physics.Raycast(mainCamera.transform.position + mainCamera.transform.forward * cameraDistanceOffset, mainCamera.transform.forward, out hitInfo);

        ////Debug.DrawLine(mainCamera.transform.position + mainCamera.transform.forward * cameraDistanceOffset, hitInfo.point, Color.blue);

        //transform.position = hitInfo.point;
    }
}
