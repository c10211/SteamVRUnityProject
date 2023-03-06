using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockControl : MonoBehaviour
{
    public GameObject hh, mh, sh;
    public float hour;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetTime());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SetTime()
    {
        while (true)
        {
            DateTime tmp = DateTime.Now;
            setHands(tmp.Hour, tmp.Minute, tmp.Second);
            yield return new WaitForSeconds(1f);
        }
    }

    void setHands(int h, int m, int s)
    {
        hour = h + (m / 60f);
        
        hh.transform.localEulerAngles = new Vector3(hour * (360f / 12f), 0, 0);
        mh.transform.localEulerAngles = new Vector3(m * (360 / 60), 0, 0);
        sh.transform.localEulerAngles = new Vector3(s * (360 / 60), 0, 0);
    }
}
