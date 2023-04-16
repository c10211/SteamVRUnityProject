using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Order[][] ordersByLevel = new Order[3][];
    float defaultTimeScale, slowTimeScale, defaultFixedDeltaTime, currentTimeScale;
    [SerializeField]
    private GameObject[] IngredientPiles, DiegeticGameItems, ExtradiegeticGameItems;
    public GameObject[] Confettis;
    int lev, ord;
    private bool PowerupPrimed, PowerupActive, GamePaused;
    GameManager gameManager;

    Coroutine gameTimer;
    public TextMeshProUGUI MenuText;

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

        ord = 0;

        // Set sandwich list
        SetSandwichLists();
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

        ord = 0;

        yield return new WaitForSeconds(1.5f);

        Debug.Log("Starting Timer");
        gameTimer = StartCoroutine(LevelTimer(10f));

        MenuText.text = ordersByLevel[lev][ord].ingredients;
    }

    IEnumerator LevelCompleted()
    {
        // Launch confetti
        foreach (GameObject go in Confettis) go.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSecondsRealtime(3f);
        
        StopCoroutine(gameTimer);

        LevelStop();
    }

    public void LevelStop()
    {
        foreach (GameObject o in IngredientPiles)
            o.SetActive(false);

        foreach (GameObject o in DiegeticGameItems)
            o.SetActive(false);

        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(false);

        StopCoroutine(gameTimer);
    }

    public bool checkIngredients(string current)
    {
        Debug.Log("Checking Ingredients");
        Debug.Log("Level = " + lev + ", order number = " + ord + "\n" +
            "Requirements = " + ordersByLevel[lev][ord].ingredients + "\n" +
            "Current = " + current);

        bool corr = current == ordersByLevel[lev][ord].ingredients;

        // Start separate coroutine
        if (corr)
        {
            Debug.Log("Burger is GOOD!!!");

            // Check if more orders exist
            bool more = ord > ordersByLevel.Length - 1;

            // Fireworks?

            // End game?
            /*if (!more)
            {
                gameManager.EndGame();
            }*/
        }

        Debug.Log("Ingredients Checked");
        return corr;
    }

    public void NextOrder()
    {
        ord++;

        Debug.Log("Going to next order, number " + ord);

        try
        {
            if (ordersByLevel[lev][ord] != null)
            {
                Debug.Log("New ingredients = " + ordersByLevel[lev][ord].ingredients);
                MenuText.text = ordersByLevel[lev][ord].ingredients;
            }
        }
        catch
        {
            Debug.Log("Level complete!");
            StartCoroutine(LevelCompleted());
        }
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

    private void SetSandwichLists()
    {
        ordersByLevel[0] = new Order[] {
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Top Bun"),
            new Order("Bottom Bun, Egg, Cheese, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Tomato, Onion, Top Bun")
        };
        ordersByLevel[1] = new Order[]
        {
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Onion, Pickle, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Tomato, Onion, Ham, Cheese, Tomato, Onion, Top Bun"),
            new Order("Bottom Bun, Egg, Cheese, Tomato, Onion, Egg, Cheese, Tomato, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Cheese, Pickle, Tomato, Top Bun")
        };
        ordersByLevel[2] = new Order[]
        {
            new Order("Bottom Bun, Ham, Onion, Cheese, Lettuce, Tomato, Ham, Onion, Pickle, Cheese, Lettuce, Tomato, Ham, Cheese, Onion, Tomato, Lettuce, Top Bun"),
            new Order("Bottom Bun, Cheese, Lettuce, Tomato, Egg, Cheese, Onion, Tomato, Lettuce, Egg, Onion, Cheese, Lettuce, Tomato, Egg, Onion, Tomato, Top Bun"),
            new Order("Bottom Bun, Ham, Tomato, Cheese, Lettuce, Ham, Onion, Cheese, Tomato, Onion, Ham, Lettuce, Cheese, Onion, Tomato, Lettuce, Ham, Top Bun"),
            new Order("Bottom Bun, Cheese, Tomato, Egg, Onion, Cheese, Tomato, Lettuce, Egg, Cheese, Onion, Tomato, Lettuce, Egg, Tomato, Onion, Lettuce, Top Bun"),
            new Order("Bottom Bun, Lettuce, Ham, Cheese, Tomato, Lettuce, Ham, Onion, Cheese, Lettuce, Tomato, Ham, Onion, Pickle, Cheese, Tomato, Onion, Top Bun")
        };
    }
}

public class Order
{
    public string ingredients;
    public float time;

    public Order(string ing, float t)
    {
        ingredients = ing;
        time = t;
    }

    public Order(string ing)
    {
        ingredients = ing;
        time = 50f;
    }
}
