using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public GameObject beckoningLight;
    public GameManager gameManager;

    public Vector3 startPosition;

    // Start is called before the first frame update
    protected void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        startPosition = gameObject.transform.position;
        if (gameObject.name == "Card Reader")
            StartCoroutine(CheckForCard());
    }

    private IEnumerator CheckForCard()
    {
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
