using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WindCell : MonoBehaviour
{
    [Header("Attributes")]
    public Vector3 WindVector;


    // * Private Variables
    private BoxCollider BoxCollider;


    void Start()
    {
        BoxCollider = gameObject.GetComponent<BoxCollider>();
        // Set the BoxCollider to be a 1x1xHeightOfArrow cell
        // BoxCollider.center.y
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Another object ({other.gameObject}) has entered this cell at {transform.position}");
        Debug.Log($"Applying force at the following direction: {WindVector}");

        Rigidbody otherObjectRB = other.gameObject.GetComponent<Rigidbody>();

        // ! For whaterver reason, it seems like the ball that's dropped into a cell only ever moves one way. Unsure as to why that is atm. 
        otherObjectRB.AddForce(WindVector * Random.Range(.1f, 1f), ForceMode.Impulse);
    }
}
