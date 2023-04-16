using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SandwichBuilder : MonoBehaviour
{
    public GameObject Indicator, Plate;
    public List<GameObject> SandwichIngredients;

    private bool indicatorState = true;
    private LevelManager levelManager;
    private bool finishingSandwich = false;

    public AudioClip GoodSound, BadSound;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((GetComponent<XRSocketInteractor>().interactablesSelected.Count == 0) != indicatorState)
        {
            Indicator.GetComponentInChildren<Renderer>().enabled = !indicatorState;
            indicatorState = !indicatorState;
        }
    }

    /*public void FirstItemPlaced(SelectEnterEventArgs arg0)
    {
        GameObject sandwichBase = arg0.interactableObject.transform.gameObject;

        CheckNextIngredient(sandwichBase);

        Debug.Log("First item has been placed, and it is " + sandwichBase.name);

        CheckWholeSandwich();
    }*/

    private void CheckNextIngredient(GameObject ing)
    {
        try
        {
            Debug.Log(ing.GetComponent<XRSocketInteractor>().interactablesSelected.ToString());
        }
        catch
        {
            Debug.Log("No next ingredient");
        }
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

    public void BellRung()
    {
        if (!finishingSandwich)
        {
            CheckWholeSandwich();
        }
    }

    public void CheckWholeSandwich()
    {
        string listOfIngredients = "";
        if (GetComponent<XRSocketInteractor>().interactablesSelected.Count != 0)
        {
            listOfIngredients = getIngredientString(GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.gameObject);
        }
        Debug.Log("List: " + listOfIngredients);

        bool good = levelManager.checkIngredients(listOfIngredients);
        if (good)
            StartCoroutine(SendSandwichToAether(getWholeSandwich(GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.gameObject)));
    }

    private string getIngredientString(GameObject go)
    {
        string tmp = go.GetComponent<Ingredient>().IngredientName;

        try
        {
            if (go.GetComponentInChildren<XRSocketInteractor>().interactablesSelected[0] != null)
            {
                tmp += getIngredientString(go.GetComponentInChildren<XRSocketInteractor>().interactablesSelected[0].transform.gameObject);
            }
        }
        catch
        {
            Debug.Log("No more ings");
        }

        return tmp;
    }

    private List<GameObject> getWholeSandwich(GameObject go)
    {
        List<GameObject> tmp = new List<GameObject>();

        tmp.Add(go);

        try
        {
            while (go.GetComponentInChildren<XRSocketInteractor>().interactablesSelected[0] != null)
            {
                go = go.GetComponentInChildren<XRSocketInteractor>().interactablesSelected[0].transform.gameObject;
                tmp.Add(go);
            }
        }
        catch
        {
            Debug.Log("Full sandwich in list");
        }

        return tmp;
    }

    private IEnumerator SendSandwichToAether(List<GameObject> wholeSandwich)
    {
        finishingSandwich = true;

        Vector3 v = wholeSandwich[0].transform.position;
        float z = 0f;

        // Disable socket to remove its clutching grip
        GetComponent<XRSocketInteractor>().socketActive = false;

        while (wholeSandwich[0].transform.position != Plate.transform.position)
        {
            wholeSandwich[0].transform.position = Vector3.Lerp(v, Plate.transform.position, z);
            z += 0.1f;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Activate smoke effect
        Plate.transform.parent.GetComponentInChildren<ParticleSystem>().Play();

        // Play good sound
        try
        {
            Debug.Log(GoodSound.ToString());
            Plate.transform.parent.GetComponent<AudioSource>().PlayOneShot(GoodSound);
        }
        catch
        {
            Debug.Log("Something wrong with playing good audio");
        }
        
        // Delete sandwich
        foreach (GameObject go in wholeSandwich)
        {
            Destroy(go);
        }

        // Re-enable socket
        GetComponent<XRSocketInteractor>().socketActive = true;

        // Next order / finish level
        levelManager.NextOrder();

        finishingSandwich = false;
    }

    /*public void SandwichRemoved()
    {
        SandwichIngredients.Clear();
        Debug.Log("Whole sandwich removed");
    }*/

    /*public void IngredientAdded(GameObject ing)
    {
        SandwichIngredients.Add(ing);
    }*/
}
