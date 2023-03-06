using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public GameObject audioSource;
    public Transform[] locations;
    public AudioClip[] sounds;

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = audioSource.GetComponent<AudioSource>();

        StartCoroutine(PlaySound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PlaySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 8f));
            audioSource.transform.position = locations[Mathf.RoundToInt(Random.Range(0, locations.Length))].position;
            audio.clip = sounds[Mathf.RoundToInt(Random.Range(0, sounds.Length))];

            audio.Play();

            Debug.Log("Played sound");
        }
    }
}
