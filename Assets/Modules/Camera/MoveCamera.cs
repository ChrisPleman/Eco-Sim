using UnityEngine;


public class MoveCamera : MonoBehaviour
{
    [Header("Movement Speed")]
    public float MoveSpeed;
    public float UpSpeed;

    // * Private variables

    void Awake()
    {
        
    }

    void Update()
    {
        // Steer
        transform.LookAt(Input.mousePosition);

        // Move forward
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * transform.forward);
        }

        // Move left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-MoveSpeed * Time.deltaTime * transform.right);
        }

        // Move right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * transform.right);
        }

        // Move back
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-MoveSpeed * Time.deltaTime * transform.forward);
        }

        // Move up
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Time.deltaTime * UpSpeed * transform.up);
        }

        // Move down
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.Translate(-UpSpeed * Time.deltaTime * transform.up);
        }
    }
}
