using System.Collections;
using UnityEngine;

public class ExitDoor : MenuItem
{
    public GameObject exitDoor, handle1, handle2;
    public bool toConfirm = false;
    Vector3 handlePos;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        startPosition = gameObject.transform.position;
        handle2.SetActive(false);
        handlePos = handle1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            handle1.transform.position = handlePos;
            handle2.SetActive(false);
            handle1.transform.localPosition = new Vector3(-0.074f, 0.909f, 0.271f);
            GetComponent<AudioSource>().Play();
        }
        else
        {
            string reason;
            if (gameManager.LevelRunning)
            {
                reason = "Exit door from level " + gameManager.levelManager.GetLevelNumber();
            }
            else
            {
                reason = "Exit door from main menu";
            }
            gameManager.GetComponent<GameManager>().QuitGame(reason);
        }
    }

    /*public void ExitLevelGoingOnce()
    {
        StartCoroutine(ExitLevelMoveTmp());
    }
    /*private IEnumerator ExitLevelMoveTmp()
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
            handle1.transform.position = handlePos;
            handle2.SetActive(false);
            handle1.transform.localPosition = new Vector3(-0.074f, 0.909f, 0.271f);
            GetComponent<AudioSource>().Play();
        }
        else
        {
            LevelManager lm = GameObject.Find("Level Manager").GetComponent<LevelManager>();
            gameManager.EndGame(lm.menuTime, lm.gameTime);
        }
    }*/
}
