using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public PlayerDetails playerDetails;

    public GameObject[] DisplayUI;

    private GameObject[] DiegeticMenu;
    private GameObject[] ExtradiegeticMenu;

    private GameObject[] DiegeticGameItems, ExtradiegeticGameItems, IngredientPiles;

    /*public List<GameObject> SandwichBeingBuilt;*/

    public GameObject[] LevelLights, LevelHighlights;
    public Transform[] XRRigLocations;
    public GameObject LeftHandRay, RightHandRay;
    public Fadeout FadeoutSphere;

    public LevelManager levelManager;

    public GameObject PointLightGame, PointLightMenu;
    
    public bool DiegeticActive = true, LevelRunning = false;
    public int levelSelect = 1;
    private bool timerGoing = false;
    private int whichTimer;
    private float tmpTimer = 0;

    private enum timerToUpdate
    {
        DIEGETIC_MAIN,
        DIEGETIC_GAME,
        EXTRADIEGETIC_MAIN,
        EXTRADIEGETIC_GAME
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DiegeticMenu = GameObject.FindGameObjectsWithTag("Diegetic");
        ExtradiegeticMenu = GameObject.FindGameObjectsWithTag("Extradiegetic");
        setMenusInactive();

        IngredientPiles = GameObject.FindGameObjectsWithTag("Ingredient Pile");

        GameObject.Find("XR Rig").transform.position = XRRigLocations[0].localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetails != null & timerGoing)
        {
            /*playerDetails.timer[whichTimer] += Time.deltaTime;*/
            tmpTimer += Time.unscaledDeltaTime;
        }
    }

    #region Player details
    public void incrementMistake(bool plusMinus)
    {
        int num = 1;
        if (!plusMinus) num = -1;

        if (DiegeticActive)
        {
            playerDetails.mistakeCounterDiegetic += num;
        }
        else
        {
            playerDetails.mistakeCounterExtradiegetic += num;
        }
    }
    #endregion

    #region Menus
    public void ToggleDiegesis()
    {
        Debug.Log("Diegesis toggled");
        setDiegeticActive(!DiegeticActive);
    }

    public void setDiegeticActive(bool active)
    {
        Debug.Log("Diegesis set to " + active);
        DiegeticActive = active;
        foreach (GameObject g in ExtradiegeticMenu)
        {
            g.SetActive(!active);
        }
        foreach (GameObject g in DiegeticMenu)
        {
            g.SetActive(active);
        }

        if (active)
        {
            LeftHandRay.GetComponent<ToggleRay>().DeactivateRay();
            RightHandRay.GetComponent<ToggleRay>().DeactivateRay();

            GameObject monitor = GameObject.Find("Monitor");
            try
            {
                float tmp = playerDetails.timer[(int)timerToUpdate.DIEGETIC_GAME] + playerDetails.timer[(int) timerToUpdate.EXTRADIEGETIC_GAME];
                if (tmp != 0f)
                {
                    monitor.GetComponent<MonitorMenuItem>().LoadScreen(1);
                    monitor.GetComponent<MonitorMenuItem>().SetStats(playerDetails.PlayerName, tmp, 123f);
                }
                else
                {
                    monitor.GetComponent<MonitorMenuItem>().LoadScreen(0);
                }
            }
            catch
            {
                monitor.GetComponent<MonitorMenuItem>().LoadScreen(0);
            }

            GameObject card = GameObject.Find("Card");
            if (card.transform.position == GameObject.Find("Card Attach").transform.position)
            {
                card.GetComponent<CardMenuItem>().MoveToStartPostition();
            }
        }
        else
        {
            LeftHandRay.GetComponent<ToggleRay>().ActivateRay();
            RightHandRay.GetComponent<ToggleRay>().ActivateRay();

            //GameObject.Find("Change Menu").GetComponent<ToggleInterface>().Toggle();
        }
        SetLights();
    }

    public bool isDiegeticActive()
    {
        return DiegeticActive;
    }

    public void setMenusInactive()
    {
        Debug.Log("Menus deactivated");
        foreach (GameObject g in ExtradiegeticMenu)
        {
            //g.GetComponent<MenuItem>().gameManager = this;
            g.SetActive(false);
        }
        foreach (GameObject g in DiegeticMenu)
        {
            //g.GetComponent<MenuItem>().gameManager = this;
            g.SetActive(false);
        }
    }

    public void SetLevel(int x)
    {
        Debug.Log("Level set to " + x);
        levelSelect = x;
    }

    void SetLights()
    {
        Debug.Log("Lights set to " + levelSelect);
        GameObject[] tmp;
        if (DiegeticActive)
            tmp = LevelLights;
        else
            tmp = LevelHighlights;

        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].GetComponent<LevelSelectDisplay>().SetOn(i == levelSelect);
        }
    }
    #endregion

    #region Game
    public void StartGame()
    {
        Debug.Log("Game started");
        StartCoroutine(FadeoutAndLoadGame());
    }

    IEnumerator FadeoutAndLoadGame()
    {
        Debug.Log("Fading out");
        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_OUT);

        if (!DiegeticActive)
        {
            foreach (GameObject g in ExtradiegeticMenu)
            {
                g.SetActive(false);
                yield return null;
            }

            // Disable pointers and use direct interactors
            LeftHandRay.GetComponent<ToggleRay>().DeactivateRay();
            RightHandRay.GetComponent<ToggleRay>().DeactivateRay();
        }
        else
        {
            foreach (GameObject g in DiegeticMenu)
            {
                g.SetActive(false);
                yield return null;
            }
        }

        // Teleport player to correct position
        GameObject.Find("XR Rig").transform.position = new Vector3(XRRigLocations[1].localPosition.x, GameObject.Find("XR Rig").transform.position.y, GameObject.Find("XR Rig").transform.position.z);

        // Turn middle light off and left light on
        ChangeLight(true);


        // # Start level subsystem

        // Load all game objects into scene (extra/diegetic)
        StartCoroutine(levelManager.LevelStart(levelSelect));

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_IN);

        // Start level timer


        whichTimer = DiegeticActive ? ((int)timerToUpdate.DIEGETIC_GAME) : ((int)timerToUpdate.EXTRADIEGETIC_GAME);
        timerGoing = true;
        LevelRunning = true;
    }

    public void EndGame()
    {
        StartCoroutine(FadeoutAndUnloadGame());
    }

    IEnumerator FadeoutAndUnloadGame()
    {
        // Stop timer

        // End level subsystem?

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_OUT);

        // Unload all game things

        // Teleport player
        GameObject.Find("XR Rig").transform.position = new Vector3(XRRigLocations[0].localPosition.x, GameObject.Find("XR Rig").transform.position.y, GameObject.Find("XR Rig").transform.position.z);

        // Change active light
        ChangeLight(false);

        // Load extra/diegetic menu

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_IN);
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ChangeLight(bool toFromGame)
    {
        PointLightGame.SetActive(toFromGame);
        PointLightMenu.SetActive(!toFromGame);
    }
    #endregion
}

[Serializable]
public class PlayerDetails
{
    public string PlayerName;
    public float hoursWorkedDiegetic, hoursWorkedExtradiegetic;
    public int moneyEarnedDiegetic, moneyEarnedExtradiegetic;

    /*public int timeSpentInMainMenuDiegetic = 0, timeSpentInMainMenuExtradiegetic = 1,
        timeSpentInGameMenuDiegetic = 2, timeSpentInGameMenuExtradiegetic = 3;*/
    public float[] timer;
    public bool usedDiegetic, usedExtradiegetic, usedHat;
    public int mistakeCounterDiegetic, mistakeCounterExtradiegetic;
}