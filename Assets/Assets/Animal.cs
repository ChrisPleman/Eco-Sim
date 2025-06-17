using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Animal : MonoBehaviour
{
    [Header("Vision")]
    public float VisionRadius;
    public float AngleOfView;
    public LayerMask ResourceLayerMask;
    public List<GameObject> VisibleResources;
    private SphereCollider VisionSphere;

    [Header("Resources")]
    public GameObject Food;
    public GameObject Water;

    // * Checklist to track todos
    // todo: General movement
    // todo: Targeted movement (e.g., towards food)

    void Start()
    {
        VisionSphere = gameObject.GetComponent<SphereCollider>();
        VisionSphere.radius = VisionRadius;
    }


    protected virtual void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something has entered the vision SphereCollider");
        // Really it should be any resource
        if ((ResourceLayerMask.value & (1 << collider.gameObject.layer)) > 0)
        {
            Vector3 resourceVector = collider.transform.position - transform.position;
            bool resourceIsInFront = Vector3.Dot(transform.forward, resourceVector) > 0;
            bool resourceIsInAOV = Mathf.Abs(Vector3.Angle(transform.forward, resourceVector)) <= AngleOfView;
            // If the animal can see the resource, but doesn't already remember where it is
            if (resourceIsInFront & resourceIsInAOV & VisibleResources.Contains(collider.gameObject) == false)
            {
                VisibleResources.Add(collider.gameObject);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider collider)
    {
        if (VisibleResources.Contains(collider.gameObject))
        {
            // Nothing for right now, as I might want to implement a memory mechanic
        }
    }
}