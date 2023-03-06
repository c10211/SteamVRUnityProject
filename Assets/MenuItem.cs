using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public GameObject beckoningLight, exitDoor, handle1, handle2, gameManager;

    public bool toConfirm = false;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        handle2.SetActive(false);
        if (gameObject.name == "Card Reader")
            StartCoroutine(CheckForCard());
    }

    private IEnumerator CheckForCard()
    {
        yield return null;
        while (true)
        {
            //if (gameObject.transform.Find("Card").parent == gameObject)
            if (gameObject.transform.FindChild("Card"))
            {
                // Start game
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCardLight(bool onOff)
    {
        Debug.LogError("WHAT THE ACTUAL FUCK");
        beckoningLight.SetActive(onOff);
    }

    public void GoingOnce()
    {
        StartCoroutine(MoveTmp());
    }

    private IEnumerator MoveTmp()
    {
        Debug.Log("Here we go, " + toConfirm);
        if (!toConfirm)
        {
            gameObject.transform.localPosition += new Vector3(-0.3f, 0f, 0f);
            toConfirm = true;
            handle1.SetActive(false);
            handle2.SetActive(true);
            yield return new WaitForSeconds(5f);
            gameObject.transform.position = startPosition;
            toConfirm = false;
            handle1.SetActive(true);
            handle2.SetActive(false);
            handle1.transform.localPosition = new Vector3(-0.074f, 0.909f, 0.271f);
        }
        else
        {
            Application.Quit();
        }
    }
}
