using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ingredient : MonoBehaviour
{
    public GameObject IngredientPile, GrabTransform, nextIngredient;
    public bool raw;
    public int whichMaterialCooks = -1;
    public Material rawMaterial, cookedMaterial, burntMaterial;
    public float cookTime = 10f;

    public InteractionLayerMask ingredientLayer;

    [SerializeField]
    private Transform startPosition;
    private bool inPile = true;
    private bool topOfStack = false;
    private float elapsedCookedTime = 0f;
    private IEnumerator grilling;
    private SandwichBuilder sandwichBuilder;
    [SerializeField]
    private string ingredientName;

    private void Start()
    {
        sandwichBuilder = GameObject.Find("Cutting Board").GetComponent<SandwichBuilder>();
    }

    void Awake()
    {
        startPosition = transform;
        //grilling = OnGrill();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 0.3f)
            Destroy(gameObject);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "CookArea")
        {
            StartCoroutine(grilling);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "CookArea")
        {
            StopCoroutine(grilling);
        }
    }

    private IEnumerator OnGrill()
    {
        while (true)
        {
            yield return null;
            elapsedCookedTime += Time.deltaTime;


        }
    }*/

    public void PickedFromPile()
    {
        if (inPile)
        {
            ReplenishPile();
            RemoveObsoleteCollider();

            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            gameObject.GetComponentInChildren<XRGrabInteractable>().interactionLayers = ingredientLayer;

            try
            {
                gameObject.GetComponentInChildren<XRSocketInteractor>().socketActive = true;
            } catch { /*Bun top probably*/ }

            inPile = false;
        }
    }

    public void ReplenishPile()
    {
        GameObject next = Instantiate(gameObject, IngredientPile.transform);
        next.transform.position = startPosition.position;
        next.transform.localScale = startPosition.localScale;

        next.GetComponent<Rigidbody>().isKinematic = false;
        next.GetComponent<Rigidbody>().useGravity = true;
        next.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void RemoveObsoleteCollider()
    {
        GrabTransform.GetComponent<BoxCollider>().enabled = false;
        Destroy(GrabTransform);
    }

    public void MakeInteractable(bool status)
    {
        GetComponent<XRGrabInteractable>().enabled = status;
        GetComponentInChildren<XRGrabInteractable>().enabled = status;
    }

    public bool isInPile()
    {
        return inPile;
    }

    public Transform thisStartPosition
    {
        get => startPosition;
    }

    public string IngredientName { get => ingredientName; }
}
