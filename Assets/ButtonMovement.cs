using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMovement : MenuItem
{
    public Collider sensor;
    private IEnumerator tracker;
    public ButtonSensor bs;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y <= -0.65)
        {
            StartCoroutine(ButtonPressed());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered or something send help");
    }

    void OnTriggerExit(Collider item)
    {
        tracker = TrackCollider(item);
        StartCoroutine(tracker);
    }

    private void OnTriggerStay(Collider other)
    {
        transform.localPosition += new Vector3(0f, -0.05f, 0f);
    }

    IEnumerator TrackCollider(Collider item)
    {
        yield return null;
        while (true)
        {
            if (!bs.isTriggered)
            {
                transform.localPosition += new Vector3(0f, 0.05f, 0f);
            }
            else
            {
                transform.position = startPosition;
                StopCoroutine(tracker);
            }
            yield return null;
        }
    }

    IEnumerator ButtonPressed()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);

        transform.position = startPosition;

        gameManager.setDiegeticActive(false);
    }
}
