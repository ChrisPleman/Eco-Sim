using UnityEngine;
using UnityEngine.Animations;

public class Sun : MonoBehaviour
{
    // * Orbit
    [Header("Orbit")]
    // public float Radius;
    public float DegreesPerSecond;
    [HideInInspector]
    public float Intensity;
    [HideInInspector]
    public float Temperature;


    [Header("Debugging")]
    public bool PrintData;
    public bool ShowForwardVector;


    // * Private Variables
    // private Vector3 origin;

    // * Debugging
    public GameObject forwardVectorVisualizer;
    private GameObject fwv;
    private Vector3 currForward;
    private Vector3 prevForward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // // * Origin
        // origin = Vector3.zero;

        // * Debugging
        if (ShowForwardVector)
        {
            fwv = Instantiate(forwardVectorVisualizer, transform.position, Quaternion.identity);
            fwv.transform.up = transform.forward;
        }


        currForward = transform.forward;
        prevForward = currForward;

        // * Emission Data
        Temperature = gameObject.GetComponent<Light>().colorTemperature;
        Intensity = gameObject.GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(DegreesPerSecond * Time.deltaTime, 0f, 0f));


        // Stay focused on the origin of the map
        // transform.LookAt(origin);

        // Debug any changes in the forward vector
        currForward = transform.forward;
        if (PrintData && currForward != prevForward)
        {
            fwv.transform.up = currForward;
            Debug.Log($"I am the sun, and my forward vector is {currForward}");
            prevForward = currForward;
        }

        
    }
}
