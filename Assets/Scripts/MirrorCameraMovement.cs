using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCameraMovement : MonoBehaviour
{

    public Transform PlayerCamera, MirrorLocation;
    public float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Mirror position = " + MirrorLocation.position.ToString()); // Mirror is always -2.88 1.42 -1.18

        float deltaX = MirrorLocation.position.x - PlayerCamera.position.x;
        float playerY = PlayerCamera.position.y;
        float deltaZ = MirrorLocation.position.z - PlayerCamera.position.z;

        float globalX = MirrorLocation.position.x + deltaX;
        float globalZ = MirrorLocation.position.z - deltaZ;

        Vector3 newLocation = new Vector3(globalX, playerY, globalZ);
        
        gameObject.transform.position = newLocation;
        gameObject.transform.LookAt(MirrorLocation);

        float distance = Vector3.Distance(MirrorLocation.position, newLocation);
        float height = 0.8f; // More or less
        float angleR = Mathf.Atan2(height, distance);

        gameObject.GetComponent<Camera>().fieldOfView = Mathf.Rad2Deg * angleR;
    }
}
