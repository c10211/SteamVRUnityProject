using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class LevelManager : MonoBehaviour
{
    GameManager gameManager;
    private Order[][] ordersByLevel = new Order[3][];
    float defaultTimeScale, slowTimeScale, defaultFixedDeltaTime, currentTimeScale;
    [SerializeField]
    private GameObject[] IngredientPiles, DiegeticGameItems, ExtradiegeticGameItems;
    public GameObject[] Confettis;
    int lev, ord;
    private bool PowerupPrimed, PowerupActive, GamePaused, inTime;

    public TextMeshPro DigitalClock1, DigitalClock2, DigitalClockColon;

    [Header("Diegetic pause menu detection")]
    [SerializeField]
    private Transform playerHead, grayscaleXTrigger;

    [Header("Pause Menu Powerup Button")]
    public Image PowerupButton;
    public Material ActivePowerupButtonMaterial, InactivePowerupButtonMaterial;
    public float PowerupTimeInSeconds = 15f;

    public Coroutine gameTimer, preciseTimer, pauseChecker;

    [Header("Diegetic Screen")]
    public TextMeshProUGUI MenuText;

    PowerupScreen screen;

    private Coroutine menuTimeKeeper;
    public float menuTime = 0f, gameTime = 0f;

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

        try
        {
            foreach (GameObject o in DiegeticGameItems)
            {
                o.SetActive(false);
            }
        }
        catch { }

        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(false);

        ord = 0;
        currentTimeScale = 1f;

        // Set sandwich list
        SetSandwichLists();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LevelStart(int lev)
    {
        DigitalClock1.color = Color.white;
        DigitalClock2.color = Color.white;
        DigitalClockColon.color = Color.white;
        Debug.Log("Starting Level");
        this.lev = lev;

        foreach (GameObject o in IngredientPiles)
            o.SetActive(true);

        if (gameManager.DiegeticActive)
        {
            foreach (GameObject o in DiegeticGameItems)
                o.SetActive(true);

            GameObject.Find("Card (1)").transform.position = GameObject.Find("Card Attach (1)").transform.position;
            GameObject.Find("Card (1)").transform.rotation = GameObject.Find("Card Attach (1)").transform.rotation;
        }

        // Extradiegetic menu activated when needed

        ord = 0;

        yield return new WaitForSeconds(1.5f);

        pauseChecker = StartCoroutine(CheckForDiegeticPause());

        Debug.Log("Starting Timer");
        gameTimer = StartCoroutine(LevelTimer(3f));

        inTime = true;
        menuTime = 0f; gameTime = 0f;
        preciseTimer = StartCoroutine(PreciseTimer());

        MenuText.text = ordersByLevel[lev][ord].ingredients;
    }

    IEnumerator LevelCompleted()
    {
        // Launch confetti
        foreach (GameObject go in Confettis) go.GetComponent<ParticleSystem>().Play();

        gameManager.playerDetails.completed[lev][gameManager.DiegeticActive ? 0 : 1] = true;

        StopCoroutine(gameTimer);
        StopCoroutine(preciseTimer);
        StopCoroutine(pauseChecker);

        yield return new WaitForSecondsRealtime(3f);

        LevelStop("Finished level ");
    }

    public void LevelStop(string reason)
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        currentTimeScale = 1f;
        DigitalClock1.text = "03";
        DigitalClock2.text = "00";

        foreach (GameObject o in IngredientPiles)
            o.SetActive(false);

        foreach (GameObject o in DiegeticGameItems)
            o.SetActive(false);

        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(false);

        // Exit to Menu
        gameManager.EndGame(menuTime, gameTime, reason + lev);

        menuTime = 0f; gameTime = 0f;
    }

    public bool checkIngredients(string current)
    {
        //Debug.Log("Checking Ingredients");
        Debug.Log("Level = " + lev + ", order number = " + ord + "\n" +
            "Requirements = " + ordersByLevel[lev][ord].ingredients + "\n" +
            "Current = " + current);

        bool corr = current == ordersByLevel[lev][ord].ingredients;

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
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log("Level complete!");
            StartCoroutine(LevelCompleted());
        }
    }

    public void SlowTimePowerup()
    {
        if (!PowerupActive)
        {
            Debug.Log("Slow powerup activated");
            if (GamePaused)
                PowerupPrimed = true;
            else
                StartCoroutine(Powerup());
            currentTimeScale = slowTimeScale;
            if (gameManager.DiegeticActive)
                screen.PrimePowerup();
        }
    }

    private IEnumerator Powerup()
    {
        PowerupPrimed = false;
        PowerupActive = true;
        if (gameManager.DiegeticActive)
        {
            screen.StartPowerupTimer(PowerupTimeInSeconds);
            gameManager.playerDetails.powerupUsagesDiegetic++;
        }
        else
        {
            gameManager.playerDetails.powerupUsagesExtradiegetic++;
        }

        SlowTime();
        yield return new WaitForSecondsRealtime(PowerupTimeInSeconds);
        SpeedTime();
        if (gameManager.DiegeticActive)
            screen.StopPowerupTimer();
        PowerupActive = false;
    }

    public void FreezeTime()
    {
        Debug.Log("Freezing time");
        GamePaused = true;
        StartCoroutine(FreezeUnfreezeTimer());
    }

    public void UnfreezeTime()
    {
        GamePaused = false;
    }

    private IEnumerator FreezeUnfreezeTimer()
    {
        Time.timeScale = 0.01f;

        yield return new WaitUntil(() => !GamePaused);
        Debug.Log("Setting Timescale to " + currentTimeScale);

        Time.timeScale = currentTimeScale;
    }

    public void SlowTime()
    {
        Debug.Log("Slowing down timescale from " + Time.timeScale);
        if(!GamePaused)
            Time.timeScale = slowTimeScale;
        currentTimeScale = slowTimeScale;

        Time.fixedDeltaTime = currentTimeScale * 0.02f;
    }

    public void SpeedTime()
    {
        Debug.Log("Returning to normal timescale from " + Time.timeScale);

        if (!GamePaused)
            Time.timeScale = defaultTimeScale;
        currentTimeScale = defaultTimeScale;

        Time.fixedDeltaTime = currentTimeScale * 0.02f;
    }

    private void SetIngredientsInteractable(bool yesOrNo)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Ingredient");

        foreach (GameObject go in gos)
        {
            go.GetComponent<Ingredient>().MakeInteractable(yesOrNo);
        }
    }

    IEnumerator CheckForDiegeticPause()
    {
        yield return null;
        while (gameManager.LevelRunning && gameManager.DiegeticActive)
        {
            if (playerHead.position.x < grayscaleXTrigger.position.x)
            {
                // Freeze time
                FreezeTime();

                // Make ingredients uninteractable
                SetIngredientsInteractable(false);

                // Count
                gameManager.playerDetails.timesPausedDiegetic++;

                GamePaused = true;

                yield return new WaitUntil(() => playerHead.position.x > grayscaleXTrigger.position.x);

                UnfreezeTime();

                if (PowerupPrimed)
                    StartCoroutine(Powerup());

                SetIngredientsInteractable(true);

                GamePaused = false;
            }
            yield return null;
        }
    }

    public void PauseButtonPressed()
    {
        if (gameManager.LevelRunning && !gameManager.DiegeticActive)
        {
            if (GamePaused)
            {
                GamePaused = false;

                ExtradiegeticUnpauseMenu();

                PauseExited();
            }
            else
            {
                GamePaused = true;

                // Count
                gameManager.playerDetails.timesPausedExtradiegetic++;

                ExtradiegeticPauseMenu();
            }
        }
    }

    public void PauseExited()
    {
        if (PowerupPrimed)
        {
            StartCoroutine(Powerup());

            if (gameManager.DiegeticActive)
                screen.StartPowerupTimer(PowerupTimeInSeconds);
        }
    }

    public void ExtradiegeticPauseMenu()
    {
        // Freeze time
        FreezeTime();

        // Pause timer
            // Done, timer freezes if GamePaused == true

        // Activate menu
        // Grayscale
        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(true);

        // Make ingredients non-interactable
        foreach (GameObject go in IngredientPiles)
        {
            try
            {
                GetComponentInChildren<Ingredient>().MakeInteractable(false);
            }
            catch
            {
                // Cutting board or something, just ignore
            }
        }

        // Activate pointer hands
        gameManager. LeftHandRay.GetComponent<ToggleRay>().ActivateRay();
        gameManager.RightHandRay.GetComponent<ToggleRay>().ActivateRay();
    }

    public void ExtradiegeticUnpauseMenu()
    {
        // Unfreeze time
        UnfreezeTime();

        // Unpause timer
            // Done, timer unfreezes if GamePaused == false

        // Pause menu timer
        //StopCoroutine(menuTimeKeeper);

        // Deactivate menu
        // Remove grayscale
        foreach (GameObject o in ExtradiegeticGameItems)
            o.SetActive(false);

        // Make ingredients interactable
        foreach (GameObject go in IngredientPiles)
        {
            try
            {
                GetComponentInChildren<Ingredient>().MakeInteractable(true);
            }
            catch
            {
                // Cutting board or something, just ignore
            }
        }

        // Deactivate pointer hands
        gameManager. LeftHandRay.GetComponent<ToggleRay>().DeactivateRay();
        gameManager.RightHandRay.GetComponent<ToggleRay>().DeactivateRay();
    }

    private IEnumerator LevelTimer(float total)
    {
        float totalSeconds = total * 60f;
        screen.SetTimeElapsed(total + ":00");
        yield return new WaitForSeconds(1);
        while (totalSeconds > 0f)
        {
            if (!GamePaused)
            {
                totalSeconds -= 1;
                int curMin = Mathf.FloorToInt(totalSeconds / 60);
                int curSec = Mathf.FloorToInt(totalSeconds % 60);
                string min = curMin > 9 ? curMin.ToString() : ("0" + curMin);
                string sec = curSec > 9 ? curSec.ToString() : ("0" + curSec);
                screen.SetTimeElapsed(min + ":" + sec);
                {
                    if (curSec % 2 == 1)
                        DigitalClockColon.text = "";
                    else
                        DigitalClockColon.text = ":";
                    DigitalClock1.text = min;
                    DigitalClock2.text = sec;
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitUntil(() => !GamePaused);
            }
        }
        DigitalClock1.color = Color.red;
        DigitalClock2.color = Color.red;
        DigitalClockColon.color = Color.red;
        inTime = false;
        gameManager.playerDetails.keptToTimer = false;

        DigitalClock1.gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2f);
        DigitalClock1.gameObject.GetComponent<AudioSource>().Stop();
    }

    /*private IEnumerator PauseMenuTimer()
    {
        while (true)
        {
            menuTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }*/

    private IEnumerator PreciseTimer()
    {
        while (true)
        {
            if (GamePaused)
            {
                menuTime += Time.unscaledDeltaTime;
                yield return null;
            }
            else
            {
                gameTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    public int GetLevelNumber()
    {
        return lev;
    }

    private void SetSandwichLists()
    {
        ordersByLevel[0] = new Order[] {
            new Order("Bottom Bun, Lettuce, Cheese, Tomato, Ham, Top Bun"),
            new Order("Bottom Bun, Egg, Onion, Ham, Pickle, Top Bun"),
            new Order("Bottom Bun, Tomato, Ham, Cheese, Onion, Top Bun"),
            new Order("Bottom Bun, Cheese, Ham, Onion, Pickle, Top Bun")
        };
        /*ordersByLevel[1] = new Order[]
        {
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Onion, Pickle, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Tomato, Onion, Ham, Cheese, Tomato, Onion, Top Bun"),
            new Order("Bottom Bun, Egg, Cheese, Tomato, Onion, Egg, Cheese, Tomato, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Cheese, Pickle, Tomato, Top Bun")
        };*/
        ordersByLevel[1] = new Order[]
        {
            new Order("Bottom Bun, Cheese, Lettuce, Ham, Egg, Onion, Tomato, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Lettuce, Tomato, Onion, Pickle, Top Bun"),
            new Order("Bottom Bun, Lettuce, Tomato, Ham, Cheese, Onion, Pickle, Top Bun"),
            new Order("Bottom Bun, Egg, Tomato, Cheese, Lettuce, Ham, Onion, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Egg, Onion, Tomato, Lettuce, Top Bun"),
            new Order("Bottom Bun, Cheese, Ham, Tomato, Onion, Lettuce, Pickle, Top Bun")/*,
            new Order("Bottom Bun, Lettuce, Tomato, Ham, Cheese, Onion, Egg, Pickle, Top Bun")*/
        };
        /*ordersByLevel[2] = new Order[]
        {
            new Order("Bottom Bun, Ham, Onion, Cheese, Lettuce, Tomato, Ham, Onion, Pickle, Cheese, Lettuce, Tomato, Ham, Cheese, Onion, Tomato, Lettuce, Top Bun"),
            new Order("Bottom Bun, Cheese, Lettuce, Tomato, Egg, Cheese, Onion, Tomato, Lettuce, Egg, Onion, Cheese, Lettuce, Tomato, Egg, Onion, Tomato, Top Bun"),
            new Order("Bottom Bun, Ham, Tomato, Cheese, Lettuce, Ham, Onion, Cheese, Tomato, Onion, Ham, Lettuce, Cheese, Onion, Tomato, Lettuce, Ham, Top Bun"),
            new Order("Bottom Bun, Cheese, Tomato, Egg, Onion, Cheese, Tomato, Lettuce, Egg, Cheese, Onion, Tomato, Lettuce, Egg, Tomato, Onion, Lettuce, Top Bun"),
            new Order("Bottom Bun, Lettuce, Ham, Cheese, Tomato, Lettuce, Ham, Onion, Cheese, Lettuce, Tomato, Ham, Onion, Pickle, Cheese, Tomato, Onion, Top Bun")
        };*/
        ordersByLevel[2] = new Order[]
        {
            new Order("Bottom Bun, Lettuce, Ham, Cheese, Tomato, Pickle, Onion, Ham, Cheese, Top Bun"),
            new Order("Bottom Bun, Egg, Tomato, Onion, Ham, Cheese, Pickle, Ham, Lettuce, Top Bun"),
            new Order("Bottom Bun, Cheese, Lettuce, Onion, Egg, Tomato, Cheese, Ham, Lettuce, Pickle, Top Bun"),
            new Order("Bottom Bun, Tomato, Lettuce, Ham, Cheese, Onion, Egg, Cheese, Ham, Pickle, Top Bun"),
            new Order("Bottom Bun, Ham, Cheese, Tomato, Egg, Lettuce, Ham, Onion, Pickle, Cheese, Top Bun"),
            new Order("Bottom Bun, Onion, Ham, Lettuce, Cheese, Tomato, Egg, Ham, Cheese, Pickle, Top Bun"),
            new Order("Bottom Bun, Pickle, Lettuce, Cheese, Ham, Tomato, Ham, Cheese, Onion, Egg, Top Bun"),
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
