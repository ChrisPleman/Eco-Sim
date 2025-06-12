using UnityEngine;

// At some point this should be abstracted to something like "Living Thing"
[RequireComponent(typeof(Trees))]
public class Grow : MonoBehaviour
{
    [Range(0f, 1f)]
    public float GrowthSpeed;
    [Tooltip("Energy units per second it costs to grow.")]
    public float GrowthCost;
    private Trees Tree;
    private float Scale;

    void Awake()
    {

    }

    void Start()
    {
        Tree = gameObject.GetComponent<Trees>();
    }

    void Update()
    {
        if (Tree.Energy > 0f)
        {
            // Scale = DecayingGrowth(Tree.MaxHeight);
            transform.localScale = new Vector3(Scale, Scale, Scale);
            Tree.Energy -= Time.deltaTime * GrowthCost;
        }
    }

    private void SetHeight(float scale, float cap)
    {
        Vector3 scaleVector = UniformVector3(Mathf.Min(scale, cap));
        transform.localScale = scaleVector;
    }

    // private float DecayingGrowth(float scaler, float peak)
    // {
    //     return scaler / Mathf.Pow()
    // }

    private Vector3 UniformVector3(float val)
    {
        return new Vector3(val, val, val);
    }
}
