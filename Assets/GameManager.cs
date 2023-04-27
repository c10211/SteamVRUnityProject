using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    
    public bool DiegeticActive = true, LevelRunning = false, gamePaused = false;
    public int levelSelect = 0;
    private bool timerGoing = false;
    private int whichTimer;
    private float tmpTimer = 0;

    public enum timerToUpdate
    {
        DIEGETIC_MAIN,
        DIEGETIC_GAME,
        DIEGETIC_GAME_MENU,
        EXTRADIEGETIC_MAIN,
        EXTRADIEGETIC_GAME,
        EXTRADIEGETIC_GAME_MENU
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
    private Coroutine preciseMenuTimer;
    public bool preciseMenuTimerRunning = false;
    private IEnumerator PreciseTimer()
    {
        while (true)
        {
            if (DiegeticActive)
            {
                playerDetails.timer[(int)timerToUpdate.DIEGETIC_MAIN] += Time.unscaledDeltaTime;
                yield return null;
            }
            else
            {
                playerDetails.timer[(int)timerToUpdate.EXTRADIEGETIC_MAIN] += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    public void StartMenuTimer()
    {
        preciseMenuTimer = StartCoroutine(PreciseTimer());
        preciseMenuTimerRunning = true;
    }

    public void StopMenuTimer()
    {
        StopCoroutine(preciseMenuTimer);
        preciseMenuTimerRunning = false;
    }

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

            /*GameObject card = GameObject.Find("Card");
            GameObject cardReader = GameObject.Find("Card Reader");
            
            cardReader.GetComponent<XRSocketInteractor>().enabled = false;
            card.GetComponent<CardMenuItem>().MoveToStartPostition();
            cardReader.GetComponent<XRSocketInteractor>().enabled = true;*/
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

        StopMenuTimer();
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
        StopMenuTimer();

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
        ChangeOverheadLight(true);

        // # Start level subsystem
        // Load all game objects into scene (extra/diegetic)
        StartCoroutine(levelManager.LevelStart(levelSelect));

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_IN);

        // Start level timer


        whichTimer = DiegeticActive ? ((int)timerToUpdate.DIEGETIC_GAME) : ((int)timerToUpdate.EXTRADIEGETIC_GAME);
        timerGoing = true;
        LevelRunning = true;
    }

    public void EndGame(float timeInMenu, float timeInGame, string reason)
    {
        // Save details
        if (DiegeticActive)
        {
            playerDetails.timer[(int)timerToUpdate.DIEGETIC_GAME] += timeInGame;
            playerDetails.timer[(int)timerToUpdate.DIEGETIC_GAME_MENU] += timeInMenu;
        }
        else
        {
            playerDetails.timer[(int)timerToUpdate.EXTRADIEGETIC_GAME] += timeInGame;
            playerDetails.timer[(int)timerToUpdate.EXTRADIEGETIC_GAME_MENU] += timeInMenu;
        }

        StartCoroutine(FadeoutAndUnloadGame(reason));
    }

    IEnumerator FadeoutAndUnloadGame(string reason)
    {
        // Stop timer

        // Save player details
        SavePlayerData(reason);

        // Update menu details

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_OUT);

        // Teleport player
        GameObject.Find("XR Rig").transform.position = new Vector3(XRRigLocations[0].localPosition.x, GameObject.Find("XR Rig").transform.position.y, GameObject.Find("XR Rig").transform.position.z);

        // Change active light
        ChangeOverheadLight(false);

        // Load extra/diegetic menu
        setDiegeticActive(DiegeticActive);

        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_IN);

        // Start menu timer
        StartMenuTimer();

        LevelRunning = false;
    }

    public void QuitGame(string reason)
    {
        // Write all playerDetails to a file
        try
        {
            SavePlayerData(reason);
        } catch { /* No player logged in? */ }

#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ChangeOverheadLight(bool toFromGame)
    {
        PointLightGame.SetActive(toFromGame);
        PointLightMenu.SetActive(!toFromGame);
    }
    #endregion

    #region Saving Data
    public void SavePlayerData(string reason)
    {
        Debug.Log("Attempting to save");

        BinaryFormatter bf = new BinaryFormatter();
        //FileStream fsOut = File.OpenWrite(Application.persistentDataPath + "/" + name + ".txt");
        FileStream fsOut = File.OpenWrite("D:\\Collected Data\\" + playerDetails.PlayerName + ".txt");

        bf.Serialize(fsOut, playerDetails);

        fsOut.Close();

        string csvData = playerDetails.toCsv(reason);

        try
        {
            File.AppendAllText("D:\\Collected Data\\" + playerDetails.PlayerName + ".csv", csvData);
            Debug.Log("Player data written to D:\\Collected Data\\" + playerDetails.PlayerName + ".csv");
        } catch (Exception e) { Debug.Log(e.Message); }
    }
    #endregion

    #region Inputs
    // Note that keyboard inputs are controlled by UIController,
    // component on `XR Rig` > `OVR Mirror` > `Display UI`

    // Index controller inputs are handled by OnButtonPress,
    // component on `XR Rig` > `Camera Offset`
    #endregion
}

[Serializable]
public class PlayerDetails
{
    public string PlayerName;
    public float hoursWorkedDiegetic, hoursWorkedExtradiegetic;
    public int   moneyEarnedDiegetic, moneyEarnedExtradiegetic;

    public float[] timer;
    public bool usedDiegetic, usedExtradiegetic, usedHat, keptToTimer;
    public int mistakeCounterDiegetic, mistakeCounterExtradiegetic;
    public int timesPausedDiegetic, timesPausedExtradiegetic, powerupUsagesDiegetic, powerupUsagesExtradiegetic;

    public bool[][] completed; // [level][diegetic]

    public string toCsv(string reason)
    {
        string tmp = "Name," +
            "Diegetic Level 1," +
            "Extradiegetic Level 1," +
            "Diegetic Level 2," +
            "Extradiegetic Level 2," +
            "Diegetic Level 3," +
            "Extradiegetic Level 3," +
            "Diegetic Main Menu," +
            "Extradiegetic Main Menu," +
            "Diegetic Pause Menu," +
            "Extradiegetic Pause Menu," +
            "Diegetic Game Time," +
            "Extradiegetic Game Time," +
            "Diegetic Paused Times," +
            "Extradiegetic Paused Times," +
            "Diegetic Powerup Used," +
            "Extradiegetic Powerup Used," +
            "Used Hat," +
            "Reason" +
            "\n" + PlayerName + "," +
            completed[0][0] + "," +
            completed[0][1] + "," +
            completed[1][0] + "," +
            completed[1][1] + "," +
            completed[2][0] + "," +
            completed[2][1] + "," +
            timer[(int)GameManager.timerToUpdate.DIEGETIC_MAIN] + "," + 
            timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_MAIN] + "," + 
            timer[(int)GameManager.timerToUpdate.DIEGETIC_GAME_MENU] + "," + 
            timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_GAME_MENU] + "," +
            timer[(int)GameManager.timerToUpdate.DIEGETIC_GAME] + "," +
            timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_GAME] + "," +
            timesPausedDiegetic + "," +
            timesPausedExtradiegetic + "," +
            powerupUsagesDiegetic + "," +
            powerupUsagesExtradiegetic + "," +
            usedHat + "," +
            reason + "\n";

        return tmp;
    }

    public string toString()
    {
        //GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        string tmp = "Player Name: " + PlayerName + "\n" +
            "Hours Worked:\n\tDiegetic: " + hoursWorkedDiegetic + "\tExtradiegetic: " + hoursWorkedExtradiegetic +
            //"Money Earned:\n\tDiegetic: " + moneyEarnedDiegetic + "\tExtradiegetic: " + moneyEarnedExtradiegetic +
            "\nTime Spent in Main Menu:\n\tDiegetic: " + timer[(int)GameManager.timerToUpdate.DIEGETIC_MAIN] + "\tExtradiegetic: " + timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_MAIN] +
            "\nTime Spent in Pause Menu:\n\tDiegetic: " + timer[(int)GameManager.timerToUpdate.DIEGETIC_GAME_MENU] + "\tExtradiegetic: " + timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_GAME_MENU] +
            "\nTime Spent in Game::\n\tDiegetic: " + timer[(int)GameManager.timerToUpdate.DIEGETIC_GAME] + "\tExtradiegetic: " + timer[(int)GameManager.timerToUpdate.EXTRADIEGETIC_GAME] +
            "\nThey " + (usedHat ? "used" : "did not use") + " the hat.\n\n\n";

        return tmp;
    }
}