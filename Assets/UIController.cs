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

    public TextMeshProUGUI tmp;

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
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gm.LevelRunning)
                {
                    // exit that level
                    gm.EndGame();
                }
                else
                {
                    // save data and clear for next player
                    nameEntered = false;
                    gm.playerDetails = null;
                    gm.setMenusInactive();

                    UIObjects.GetValueOrDefault("Name Input").GetComponent<TextMeshProUGUI>().text = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                gm.playerDetails.usedHat = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                changeMenuTypeUI();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gm.QuitGame();
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
        if (name == null || name == "")
            return;

        nameEntered = true;

        if (File.Exists(Application.persistentDataPath + "/" + name + ".txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fsIn = File.OpenRead(Application.persistentDataPath + "/" + name + ".txt");

            gm.playerDetails = (PlayerDetails)bf.Deserialize(fsIn);

            fsIn.Close();
        }
        else
        {
            gm.playerDetails.PlayerName = name;
        }

        gm.setDiegeticActive(gm.DiegeticActive);
    }

    private void changeMenuTypeUI()
    {
        gm.DiegeticActive  = !gm.DiegeticActive;
        String opt = gm.DiegeticActive ? "diegetic" : "extradiegetic";
        UIObjects.GetValueOrDefault("Active Menu Type").GetComponent<TextMeshProUGUI>().text = "Menu is " + opt;
    }
}