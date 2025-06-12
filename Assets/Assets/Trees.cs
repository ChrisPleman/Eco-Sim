using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Grow))]
public class Trees : MonoBehaviour, IGenetic
{
    [Header("Health")]
    public float Health = 100f;
    [Header("Hydration")]
    public float Hydration = 100f;
    [Tooltip("Drinking")]
    public float HydrationProduction;
    public float WaterAvailable;
    [Header("Energy")]
    // Makes sense that this shouldn't be capped at a 100, as larger trees should have more energy
    public float Energy;
    // This should eventually be dependent on size, tree types, random genes, health/disease(?)
    public float EnergyProduction;

    [Header("TBD")]
    // todo: This should be controlled by a gene --> So maybe this itself could be a gene type
    public float MaxHeight;
    [Tooltip("In meters")]
    // todo: Make sure that this changes as the plant grows
    public float RootRadius;

    public List<Gene> Genes;

    // [Header("Functionality & Abilities")]
    // [SerializeField] Grow Grow;

    void Awake()
    {
        // Technically we'll probably need to start with some energy & a small amount of energy production
        Energy = 10f;
        EnergyProduction = .1f;
    }

    void Start()
    {

    }

    void Update()
    {
        PhotoSynthesis();
        // todo: I'll need a way to make sure that drinking is dependent on proximity to water
        Drink();
    }

    private void PhotoSynthesis()
    {
        Energy += Time.deltaTime * EnergyProduction;
    }

    private void SenseWater()
    {
        // Find some way to update the available water
        // Water available can be a fraction of the root radius
        float rootArea = Mathf.PI * Mathf.Pow(RootRadius, 2);

        // todo: steps ordered in terms of complexity
        // ? 1. When plant is first planted, senses around at a max radious
        // ? 2. At a consistent periodic rate, senses around itself
        // ? 3. Everytime, or some fraction of the time, the tree grows, it senses around itself
        // ? 4. This should be able to determine if there is something blocking it (rock, mountain, other?)
        // ? 5. If there are overlapping roots, it will need to compete (i.e., need to figure out how to dole out resources)
    }

    private void Drink()
    {
        Hydration += Time.deltaTime * HydrationProduction;
    }

    public void InheritGenes()
    {
        throw new System.NotImplementedException();
    }
}
