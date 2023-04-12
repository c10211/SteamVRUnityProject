using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonitorMenuItem : MonoBehaviour
{
    public GameObject[] screens;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScreen(int x)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == x);
        }
    }

    public void LoadScreen(string name)
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(screen.name == name);
        }
    }

    public void SetStats(string name, float time, float money)
    {
        GameObject.Find("Username Variable").GetComponent<TextMeshProUGUI>().text = name;
        GameObject.Find("Hours Variable").GetComponent<TextMeshProUGUI>().text = time.ToString();
        GameObject.Find("Money Variable").GetComponent<TextMeshProUGUI>().text = money.ToString();
    }
}
