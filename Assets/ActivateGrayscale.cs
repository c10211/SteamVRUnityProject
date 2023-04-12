using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ActivateGrayscale : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by " + other.name);

        // Pause timer

        // --Pause gravity-- should be paused by time

        // Start timer for diegetic menu

        // Increment counter
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
