using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPReticleAnimation : MonoBehaviour
{
    public float maxSize = 1.3f, minSize = 0.8f, sizeScale = 0.004f, rotationSpeed = 0.3f;

    private Coroutine GO;

    void Awake()
    {
        GO = StartCoroutine(Pulse());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Stop()
    {
        StopCoroutine(GO);
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            yield return Grow(gameObject.transform.localScale.x);
            yield return Shrink(gameObject.transform.localScale.x);
        }
    }

    private IEnumerator Grow(float size)
    {
        while (size < maxSize)
        {
            gameObject.transform.localScale += new Vector3(sizeScale, 0f, sizeScale);
            size += sizeScale;
            yield return null;
        }
    }

    private IEnumerator Shrink(float size)
    {
        while (size > minSize)
        {
            gameObject.transform.localScale -= new Vector3(sizeScale, 0f, sizeScale);
            size -= sizeScale;
            yield return null;
        }
    }
}
