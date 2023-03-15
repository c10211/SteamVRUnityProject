using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MenuItem
{
    public int selectedLevel;
    private IEnumerator Lights;
    public GameObject[] LightBulbs;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Lights = SelectLight();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LogHover()
    {
        Debug.Log("Lever is being hovered over");
    }

    public void StartLightSelect()
    {
        Debug.Log("Starting lightshow");
        StartCoroutine(Lights);
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
        transform.localPosition = Vector3.zero;
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
