using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SandwichBuilder : MonoBehaviour
{
    public GameObject Indicator, Plate;
    public List<GameObject> SandwichIngredients;

    private bool indicatorState = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((SandwichIngredients.Count == 0) != indicatorState)
        {
            Indicator.GetComponentInChildren<Renderer>().enabled = !indicatorState;
            indicatorState = !indicatorState;
        }
    }

    public void FirstItemPlaced(SelectEnterEventArgs arg0)
    {
        GameObject sandwichBase = arg0.interactableObject.transform.gameObject;

        CheckNextIngredient(sandwichBase);

        Debug.Log("First item has been placed, and it is " + sandwichBase.name);

        CheckWholeSandwich();
    }

    private void CheckNextIngredient(GameObject ing)
    {
        SandwichIngredients.Add(ing);
        if (ing.GetComponent<Ingredient>().nextIngredient != null)
        {
            //ing.GetComponent<Ingredient>().TopOfStack = false;
            CheckNextIngredient(ing.GetComponent<Ingredient>().nextIngredient);
        }
        else
        {
            //ing.GetComponent<Ingredient>().TopOfStack = true;
        }
    }

    public void CheckNextIngredient(SelectEnterEventArgs arg0)
    {
        CheckNextIngredient(arg0.interactableObject.transform.gameObject);
    }

    public void CheckWholeSandwich()
    {
        string listOfIngredients = "";
        foreach (GameObject go in SandwichIngredients)
        {
            listOfIngredients += go.GetComponent<Ingredient>().IngredientName;
        }

        bool good = GameObject.Find("Level Manager").GetComponent<LevelManager>().checkIngredients(listOfIngredients);

        if (good) StartCoroutine(SendSandwichToAether());
    }

    private IEnumerator SendSandwichToAether()
    {
        Vector3 v = SandwichIngredients[0].transform.position;
        float z = 0f;
        while (SandwichIngredients[0].transform.position != Plate.transform.position)
        {
            SandwichIngredients[0].transform.position = Vector3.Lerp(v, Plate.transform.position, z);
            z += 0.1f;
            yield return null;
        }
    }

    public void SandwichRemoved()
    {
        SandwichIngredients.Clear();
        Debug.Log("Whole sandwich removed");
    }

    public void IngredientAdded(GameObject ing)
    {
        SandwichIngredients.Add(ing);
    }
}
