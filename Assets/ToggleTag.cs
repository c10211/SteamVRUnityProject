using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DonFlame()
    {
        this.tag = "lit";
    }

    public void DoffFlame()
    {
        this.tag = "Untagged";
    }
}
