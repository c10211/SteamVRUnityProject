using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MenuItem
{
    public int selectedLevel;
    private Coroutine Lights;
    public GameObject[] LightBulbs;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        //Lights = SelectLight();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Awake()
    {
        foreach (GameObject light in LightBulbs)
            light.GetComponent<LevelSelectDisplay>().SetOn(false);

        setActiveLight(gameManager.levelSelect);
    }

    public void LogHover()
    {
        Debug.Log("Lever is being hovered over");
    }

    public void StartLightSelect()
    {
        Debug.Log("Starting lightshow");
        Lights = StartCoroutine(SelectLight());
    }

    public void OnRelease()
    {
        Debug.Log("Ending lightshow");

        float tmp = gameObject.transform.localRotation.x;

        if (tmp <= -12.5)
            gameObject.transform.Rotate(-25f, 0f, 0f);
        else if (tmp >= 12.5)
            gameObject.transform.Rotate(25f, 0f, 0f);
        else
            gameObject.transform.Rotate(0f, 0f, 0f);

        StopCoroutine(Lights);
        StartCoroutine(SetPosition(selectedLevel));
    }

    IEnumerator SelectLight()
    {
        yield return null;
        while (true)
        {
            float tmp = gameObject.transform.localRotation.x;

            if (tmp <= -0.12f)
                setActiveLight(2);
            else if (tmp >= 0.12f)
                setActiveLight(0);
            else
                setActiveLight(1);

            yield return null;
        }
    }

    IEnumerator SetPosition(int position)
    {
        yield return null;
        Quaternion[] positions = { new Quaternion(0.216439605f, 0f, 0f, 0.976296067f), new Quaternion(0f, 0f, 0f, 1f), new Quaternion(-0.216439515f, 0f, 0f, 0.976296067f) };
        transform.localPosition = Vector3.zero;
        transform.localRotation = positions[position];
        yield return null;
    }

    private void setActiveLight(int toSet)
    {
        if (toSet != selectedLevel)
        {
            LightBulbs[selectedLevel].GetComponent<LevelSelectDisplay>().SetOn(false);
            LightBulbs[toSet].GetComponent<LevelSelectDisplay>().SetOn(true);
            selectedLevel = toSet;
            gameObject.GetComponent<AudioSource>().Play();
            gameManager.SetLevel(toSet);
        }
    }
}
