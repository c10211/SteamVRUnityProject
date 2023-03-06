using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAudio : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.volume = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        audioSource.Play();
    }
}
