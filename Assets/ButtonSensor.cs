using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSensor : MonoBehaviour
{
    public bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
