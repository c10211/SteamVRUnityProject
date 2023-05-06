using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Dictionary<string, GameObject> UIObjects = new Dictionary<string, GameObject>();
    public GameManager gm;
    public LevelManager lm;

    private bool nameEntered = false;

    /*private bool diegetic = true;*/

    // Start is called before the first frame update
    void Start()
    {
        gm.playerDetails = new PlayerDetails();

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject go in tmp) UIObjects.Add(go.name, go);
    }

    // Update is called once per frame
    void Update()
    {
        if (nameEntered)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gm.ToggleDiegesis();
                changeMenuTypeUI();
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                gm.incrementMistake(true);
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                gm.incrementMistake(false);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                // Recentre player?
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Toggle timer
                if (gm.LevelRunning)
                {
                    // Toggle game timer coroutine
                    // Not implemented
                }
                else
                {
                    // Toggle main menu timer
                    if (gm.preciseMenuTimerRunning)
                    {
                        gm.StopMenuTimer();
                    }
                    else
                    {
                        gm.StartMenuTimer();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gm.LevelRunning)
                {
                    // exit that level
                    StopCoroutine(lm.gameTimer);
                    StopCoroutine(lm.preciseTimer);
                    lm.LevelStop("Esc key pressed");
                    //gm.EndGame(lm.menuTime, lm.gameTime);
                }
                else
                {
                    // Save data
                    gm.SavePlayerData("Esc key pressed");

                    // Clear for next player
                    nameEntered = false;
                    gm.playerDetails = null;
                    gm.setMenusInactive();

                    //UIObjects.GetValueOrDefault("Name Input").GetComponent<TextMeshProUGUI>().text = "";
                    //UIObjects["Name Input"].GetComponent<TextMeshProUGUI>().text = "";
                    GameObject.Find("Name Input").GetComponent<TMP_InputField>().text = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                gm.playerDetails.usedHat = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                gm.SetLevel(0);
                gm.StartGame();
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                gm.SetLevel(1);
                gm.StartGame();
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                gm.SetLevel(2);
                gm.StartGame();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gm.DiegeticActive = !gm.DiegeticActive;
                changeMenuTypeUI();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gm.QuitGame("Esc key pressed");
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            lm.SlowTime();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            lm.SpeedTime();
        }
    }

    public void SetPlayerName(string name)
    {
        if (name == null || name == "" || nameEntered)
            return;

        nameEntered = true;

        if (File.Exists(Application.persistentDataPath + "/" + name + ".txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fsIn = File.OpenRead(Application.persistentDataPath + "/" + name + ".txt");

            gm.playerDetails = (PlayerDetails)bf.Deserialize(fsIn);

            fsIn.Close();

            File.AppendAllText("D:\\Collected Data\\" + gm.playerDetails.PlayerName + ".csv", "\n");

            Debug.Log("Data loaded for " + name);
        }
        else
        {
            gm.playerDetails.PlayerName = name;

            gm.playerDetails.timer = new float[6];
            gm.playerDetails.completed = new bool[3][];
            {
                gm.playerDetails.completed[0] = new bool[2];
                gm.playerDetails.completed[1] = new bool[2];
                gm.playerDetails.completed[2] = new bool[2];
            }

            gm.playerDetails.hoursWorkedDiegetic = 0f;
            gm.playerDetails.hoursWorkedExtradiegetic = 0f;

            gm.playerDetails.moneyEarnedDiegetic = 0;
            gm.playerDetails.moneyEarnedExtradiegetic = 0;

            gm.playerDetails.mistakeCounterDiegetic = 0;
            gm.playerDetails.mistakeCounterExtradiegetic = 0;

            gm.playerDetails.usedDiegetic = false;
            gm.playerDetails.usedExtradiegetic = false;

            gm.playerDetails.usedHat = false;
            gm.playerDetails.keptToTimer = true;

            gm.playerDetails.timesPausedDiegetic = 0;
            gm.playerDetails.timesPausedExtradiegetic = 0;
            gm.playerDetails.powerupUsagesDiegetic = 0;
            gm.playerDetails.powerupUsagesExtradiegetic = 0;

            Debug.Log("New player " + name + " created");
        }

        if (!gm.LevelRunning)
        {
            Debug.Log("Starting menu timer");
            gm.setDiegeticActive(gm.DiegeticActive);
            gm.StartMenuTimer();
        }
    }

    private void changeMenuTypeUI()
    {
        string opt = gm.DiegeticActive ? "diegetic" : "extradiegetic";
        UIObjects.GetValueOrDefault("Active Menu Type").GetComponent<TextMeshProUGUI>().text = "Menu is " + opt;
    }
}