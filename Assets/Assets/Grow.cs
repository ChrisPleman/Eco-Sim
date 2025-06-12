using UnityEngine;

// At some point this should be abstracted to something like "Living Thing"
[RequireComponent(typeof(Trees))]
public class Grow : MonoBehaviour
{
    [Header("Various Traits")]
    [Range(0f, 1f)]
    // todo: Probably should be inheritable traits as well
    public float GrowthSpeed;
    public float FastestInitialGrowth;
    public float GrowthDecaySpeed;
    [Header("Cost of Growing")]
    // todo: These two should likely be inheritable traits
    public float InitialGrowthCost;
    public float GrowthCostFactor;
    [Tooltip("Energy units per second it costs to grow.")]
    // todo: More than just energy in the future
    // todo: This should increase as the object grows
    public float GrowthCost;



    // Private Variables //

    private Trees Tree;
    private float Scale;


    void Awake()
    {
        // There is some initial cost
        UpdateGrowthCost();
    }

    void Start()
    {
        // todo: At some point this should be abstracted to any GrowableObject
        Tree = gameObject.GetComponent<Trees>();
    }

    void Update()
    {
        // We need at least as much as it costs to grow, well, to grow
        if (Tree.Energy > GrowthCost)
        {
            Scale += DecayingGrowth();
            SetHeight(Scale, Tree.MaxHeight);
            Tree.Energy -= Time.deltaTime * GrowthCost;
            UpdateGrowthCost();
        }
    }

    // todo: In this end this will cost several resources: energy, water, other(?)
    private void UpdateGrowthCost()
    {
        GrowthCost = InitialGrowthCost * GrowthCostFactor * transform.localScale.x;
    }

    private void SetHeight(float scale, float cap)
    {
        Vector3 scaleVector = UniformVector3(Mathf.Min(scale, cap));
        transform.localScale = scaleVector;
    }

    private float DecayingGrowth()
    {
        // decaySpeed should be between 0 and 1, where the closer to zero you get, the slower it takes to decay
        // fastestInitialGrowth is how fast we want the object to increase in scale at the onset
        // ! Right now, we are just passing in single deltaTimes, when this should be a cumulative of time growing
        return FastestInitialGrowth / Mathf.Pow(1 + GrowthDecaySpeed, Time.deltaTime);
    }

    private Vector3 UniformVector3(float val)
    {
        return new Vector3(val, val, val);
    }
}
