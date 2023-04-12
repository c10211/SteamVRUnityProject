using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class IngredientPile : MonoBehaviour
{
    public GameObject Ingredient;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnSingleIngredient(/*SelectEnterEventArgs grabber*/)
    {
        Debug.Log("Spawning ingredient: " + Ingredient.name);
        Vector3 pos = transform.position - new Vector3(0f, 1f, 0f);
        /*try
        {
            pos = grabber.interactorObject.transform.position;
        }
        catch
        {
            Debug.LogError("Position not read properly");
        }*/
        GameObject tmp = Instantiate(Ingredient, pos, new Quaternion());

        tmp.transform.localScale = new Vector3(9f, 9f, 4.835377f);

        //XRController cont = (XRController)grabber.interactorObject;
        //cont.AttachObject(); // This isn't working, try asking ChatGPT again, but if this and the forum post don't work just put the ingredient under the pile and use that
    }
}
