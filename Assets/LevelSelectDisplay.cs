using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectDisplay : MonoBehaviour
{
    public GameObject[] lightTubes;
    public GameObject NumberOn, NumberOff;
    public Material LightOn, LightOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOn(bool orOff)
    {
        foreach(GameObject light in lightTubes)
        {
            light.GetComponent<MeshRenderer>().material = orOff ? LightOn : LightOff;
        }
        NumberOn.SetActive(orOff);
        NumberOff.SetActive(!orOff);
    }
}
