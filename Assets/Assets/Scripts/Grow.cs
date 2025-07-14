using UnityEngine;

// At some point this should be abstracted to something like "Living Thing"
[RequireComponent(typeof(Trees))]
public class Grow : MonoBehaviour
{
    [Header("Various Traits")]
    [Range(0f, 1f)]
    // todo: Probably should be inheritable traits as well
    public float GrowthSpeed;
    // ! Deprecating these at the moment
    // public float FastestInitialGrowth;
    // public float GrowthDecaySpeed;
    [Header("Cost of Growing")]
    // todo: These two should likely be inheritable traits
    // ! Deprecating this for now
    // public float InitialGrowthCost;
    public float GrowthCostFactor;
    [Tooltip("Energy units per second it costs to grow.")]
    // todo: More than just energy in the future
    // todo: This should increase as the object grows
    public float GrowthCost;

    // We need to account for trees that begin growing after time zero
    private float TimeBorn;
    // We need a way to control the 'timeAlive' variable to account for periods of lack of growth
    // Essentially the scaling jumps because it's using Time.time, when it should pause when not growing
    private float TimeGrowing;




    // Private Variables //

    private Trees Tree;
    private float Scale;


    void Awake()
    {
        // There is some initial cost
        UpdateGrowthCost();
        TimeBorn = Time.time;
        TimeGrowing = 0f;
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
            SetHeight(Tree.MaxHeight, GrowthSpeed);
            Tree.Energy -= Time.deltaTime * GrowthCost;
            UpdateGrowthCost();
        }
    }

    // todo: In this end this will cost several resources: energy, water, other(?)
    private void UpdateGrowthCost()
    {
        GrowthCost = transform.localScale.x;
    }

    private void SetHeight(float cap, float speed)
    {
        TimeGrowing += Time.deltaTime;
        float currScale = SigmoidGrowth(cap, speed, TimeGrowing);
        Vector3 scaleVector = UniformVector3(currScale);
        transform.localScale = scaleVector;
    }

    private float SigmoidGrowth(float cap, float speed, float timeAlive)
    {
        // The growth will follow the sigmoid function, but will be capped at a certain height and speed

        // alpha ensures that the initial scale will always be 1 regardless of the height cap passed
        float alpha = cap - 1;
        return cap / (1 + alpha * Mathf.Exp(-speed * timeAlive));
    }

    private Vector3 UniformVector3(float val)
    {
        return new Vector3(val, val, val);
    }
}
