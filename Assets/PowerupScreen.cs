using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupScreen : MonoBehaviour
{
    public GameObject PowerupText, PowerupPrimed, PowerupTimer;
    public Image TimerBar;
    public TextMeshProUGUI LevelTimer;
    public string LevelTime;

    // Start is called before the first frame update
    void Start()
    {
        StopPowerupTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimeElapsed(string ElapsedTime)
    {
        LevelTimer.text = ElapsedTime + "/" + LevelTime;
    }

    public void PrimePowerup()
    {
        PowerupText.SetActive(false);
        PowerupPrimed.SetActive(true);
    }

    public void StartPowerupTimer(float powerupTime)
    {
        Debug.Log("Changing screen to timer");
        PowerupText.SetActive(false);
        PowerupPrimed.SetActive(false);
        PowerupTimer.SetActive(true);

        StartCoroutine(PowerupTimerBarShrink(powerupTime));
    }

    IEnumerator PowerupTimerBarShrink(float powerupTime)
    {
        Debug.Log("Animating timer");
        float curTime = powerupTime;
        while (curTime > 0f)
        {
            yield return null;
            curTime -= Time.unscaledDeltaTime;

            TimerBar.transform.localScale = new Vector3(Mathf.Lerp(1, 0, (curTime / powerupTime)), 1f, 1f);
        }
        yield return null;

        StopPowerupTimer();
    }

    public void StopPowerupTimer()
    {
        PowerupText.SetActive(true);
        PowerupTimer.SetActive(false);
    }
}
