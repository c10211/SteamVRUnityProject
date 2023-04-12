using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Order[][] ordersByLevel = new Order[3][];
    float defaultTimeScale, slowTimeScale, defaultFixedDeltaTime, currentTimeScale;
    [SerializeField]
    private GameObject[] IngredientPiles, DiegeticGameItems, ExtradiegeticGameItems;
    int lev, ord;
    private bool PowerupPrimed, PowerupActive, GamePaused;
    GameManager gameManager;

    Coroutine gameTimer;


    public float PowerupTimeInSeconds = 30f;
    PowerupScreen screen;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        defaultTimeScale = Time.timeScale;
        slowTimeScale = 0.1f;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        screen = GameObject.Find("Powerup Monitor").GetComponent<PowerupScreen>();
        screen.LevelTime = "5:00";

        IngredientPiles = GameObject.FindGameObjectsWithTag("Ingredient Pile");
        DiegeticGameItems = GameObject.FindGameObjectsWithTag("In-Game Diegetic");
        ExtradiegeticGameItems = GameObject.FindGameObjectsWithTag("In-Game Extradiegetic");

        foreach (GameObject o in IngredientPiles)
            o.SetActive(false);

        foreach (GameObject o in DiegeticGameItems)
            o.SetActive(false);

        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LevelStart(int lev)
    {
        Debug.Log("Starting Level");
        this.lev = lev;

        foreach (GameObject o in IngredientPiles)
            o.SetActive(true);

        foreach (GameObject o in DiegeticGameItems)
            o.SetActive(gameManager.DiegeticActive);

        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(!gameManager.DiegeticActive);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("Starting Timer");
        gameTimer = StartCoroutine(LevelTimer(10f));
    }

    public bool checkIngredients(string current)
    {
        Debug.Log("Checking Ingredients");
        bool corr = current == ordersByLevel[lev][ord].ingredients;

        // Start separate coroutine
        if (corr)
        {
            // Increment order counter
            ord++;

            // Check if more orders exist
            bool more = ord > ordersByLevel.Length - 1;

            // Fireworks?

            // End game?
            if (!more)
            {
                gameManager.EndGame();
            }
        }

        return corr;
    }

    public void SlowTimePowerup()
    {
        if (!PowerupActive)
        {
            PowerupPrimed = true;
            screen.PrimePowerup();
        }
    }

    public void GrayscaleZoneExited()
    {
        if (PowerupPrimed)
        {
            StartCoroutine(Powerup());

            screen.StartPowerupTimer(PowerupTimeInSeconds);
        }
    }

    private IEnumerator Powerup()
    {
        PowerupPrimed = false;
        PowerupActive = true;
        SlowTime();
        yield return new WaitForSecondsRealtime(PowerupTimeInSeconds);
        SpeedTime();
        PowerupActive = false;
    }

    public void FreezeTime(Collider other)
    {
        if (other.gameObject.name == "Main Camera")
        {
            FreezeTime();
        }
    }

    public void FreezeTime()
    {
        Debug.Log("Freezing time");
        GamePaused = true;
        StartCoroutine(FreezeUnfreezeTimer());
    }

    public void UnfreezeTime(Collider other)
    {
        if (other.gameObject.name == "Main Camera")
        {
            UnfreezeTime();
        }
    }

    public void UnfreezeTime()
    {
        Debug.Log("Unfreezing time");
        GamePaused = false;
    }

    private IEnumerator FreezeUnfreezeTimer()
    {
        Time.timeScale = 0;

        yield return new WaitUntil(() => !GamePaused);

        Time.timeScale = currentTimeScale;
    }

    public void SlowTime()
    {
        Debug.Log("Slowing down timescale from " + Time.timeScale);
        Time.timeScale = slowTimeScale;

        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
    }

    public void SpeedTime()
    {
        Debug.Log("Returning to normal timescale from " + Time.timeScale);

        Time.timeScale = defaultTimeScale;

        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
    }

    public void ExtradiegeticPauseMenu()
    {
        // Freeze time

        // Grayscale

        // Pause timer

        // Start menu timer

        // Activate menu

        // Make ingredients non-interactable
    }

    public void ExtradiegeticUnpauseMenu()
    {
        // Unfreeze time

        // Remove grayscale

        // Unpause timer

        // Pause menu timer

        // Deactivate menu

        // Make ingredients interactable
    }

    // Requirements:
    // -------------
    // Timer

    // List of orders

    // Check if current sandwich matches order

    // Orders:
    // List of ingredient names, matches GameObject names
    // Time to be added
    // Difficulty level? In array split by difficulty?

    // Time powerup isActive

    // Slow gravity on other objects

    private IEnumerator LevelTimer(float total)
    {
        Debug.Log("Level timer coroutine");
        float totalSeconds = total * 60f;
        screen.SetTimeElapsed(total + ":00");
        yield return new WaitForSeconds(1);
        while (totalSeconds > 0f)
        {
            totalSeconds -= 1;
            int curMin = Mathf.FloorToInt(totalSeconds / 60);
            int curSec = Mathf.FloorToInt(totalSeconds % 60);
            string min = (curMin > 9 ? (curMin.ToString()) : ("0" + curMin));
            string sec = (curSec > 9 ? (curSec.ToString()) : ("0" + curSec));
            screen.SetTimeElapsed(min + ":" + sec);
            yield return new WaitForSeconds(1f);
        }
    }
}

public class Order
{
    public string ingredients;
    public float time;
}
