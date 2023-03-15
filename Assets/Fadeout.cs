using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadeout : MonoBehaviour
{
    Color col;

    public float time = 0.5f;

    public readonly bool FADE_DIRECTION_OUT = true;
    public readonly bool FADE_DIRECTION_IN = false;

    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<MeshRenderer>().material.color;
        Debug.Log("Started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(true));
        Debug.Log("Fading out");
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(false));
        Debug.Log("Fading in");
    }

    public void FadeOutThenIn()
    {
        StartCoroutine(FadeOutIn());
    }

    private IEnumerator FadeOutIn()
    {
        yield return Fade(FADE_DIRECTION_OUT);

        yield return new WaitForSeconds(0.5f);

        yield return Fade(FADE_DIRECTION_IN);
    }

    public IEnumerator Fade(bool direction)
    {
        float start = 0f;
        float end = 1f;
        if (!direction)
        {
            start = 1f;
            end = 0f;
        }
        float t = 0f;

        while (gameObject.GetComponent<MeshRenderer>().material.color.a != end)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, Mathf.Lerp(start, end, t * (1 / time)));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Fade ready");
        yield return null;
    }
}
