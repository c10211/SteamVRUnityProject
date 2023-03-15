using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverHandle : MenuItem
{
    public GameObject leverThing;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        leverThing.transform.RotateAround(leverThing.transform.position, new Vector3(1f, 0f, 0f), gameObject.transform.rotation.x);
    }

    public void OnRelease()
    {
        transform.position = GameObject.Find("Cylinder.003").transform.position;
    }
}
