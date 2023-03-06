using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicControl : MonoBehaviour
{
    public GameObject controller;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(controlSun());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator controlSun()
    {
        while (true)
        {
            gameObject.transform.rotation = controller.transform.rotation;
            yield return null;
        }
    }
}
