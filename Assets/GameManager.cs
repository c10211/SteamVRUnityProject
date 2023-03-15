using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] DiegeticMenu;
    private GameObject[] ExtradiegeticMenu;

    private GameObject[] DiegeticGameItems;
    private GameObject[] ExtradiegeticGameItems;

    public GameObject[] LevelLights, LevelHighlights;
    public GameObject LeftHandRay, RightHandRay;
    public Fadeout FadeoutSphere;
    
    public bool DiegeticActive = true;
    public int levelSelect = 1;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DiegeticMenu = GameObject.FindGameObjectsWithTag("Diegetic");
        ExtradiegeticMenu = GameObject.FindGameObjectsWithTag("Extradiegetic");
        setDiegeticActive(DiegeticActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDiegeticActive(bool active)
    {
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
        }
        else
        {
            LeftHandRay.GetComponent<ToggleRay>().ActivateRay();
            RightHandRay.GetComponent<ToggleRay>().ActivateRay();

            GameObject.Find("Change Menu").GetComponent<ToggleInterface>().Toggle();
        }
        SetLights();
    }

    public void SetLevel(int x)
    {
        levelSelect = x;
    }

    void SetLights()
    {
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

    public bool isDiegeticActive()
    {
        return DiegeticActive;
    }

    public void StartGame()
    {
        StartCoroutine(FadeoutAndLoadGame());
    }

    IEnumerator FadeoutAndLoadGame()
    {
        yield return FadeoutSphere.Fade(FadeoutSphere.FADE_DIRECTION_OUT);

        foreach (GameObject g in ExtradiegeticMenu)
        {
            g.SetActive(false);
            yield return null;
        }
        foreach (GameObject g in DiegeticMenu)
        {
            g.SetActive(false);
            yield return null;
        }

        // Load all game objects into scene

        // Teleport player to correct position

        // Start level timer

        // Start level subsystem

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
}
