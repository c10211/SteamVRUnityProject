using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TeleportFromFloor()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (gameObject.transform.position.y < -1.7f)
                gameObject.transform.position = startPosition + new Vector3(0f, 0.1f, 0f);
        }
    }
}
