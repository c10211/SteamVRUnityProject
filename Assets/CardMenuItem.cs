using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMenuItem : MenuItem
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 0.4f)
            MoveToStartPostition();
    }

    public void MoveToStartPostition()
    {
        transform.position = startPosition;
    }

    public void CheckPosition()
    {
        StartCoroutine(StartGameIfInCorrectPosition());
    }

    private IEnumerator StartGameIfInCorrectPosition()
    {
        yield return new WaitForSeconds(0.1f);
        try
        {
            if (transform.position == GameObject.Find("Card Attach").transform.position)
            {
                gameObject.GetComponent<AudioSource>().Play();
                gameManager.StartGame();
            }
        } catch { }
    }
}
