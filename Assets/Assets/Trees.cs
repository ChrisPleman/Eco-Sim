using UnityEngine;

[RequireComponent(typeof(Grow))]
public class Trees : MonoBehaviour
{
    [Header("Health")]
    public float Health = 100f;
    [Header("Hydration")]
    public float Hydration = 100f;
    [Header("Energy")]
    // Makes sense that this shouldn't be capped at a 100, as larger trees should have more energy
    public float Energy;
    // This should eventually be dependent on size, tree types, random genes, health/disease(?)
    public float EnergyProduction;

    [Header("TBD")]
    public float MaxHeight;

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
    }



    private void PhotoSynthesis()
    {
        Energy += Time.deltaTime * EnergyProduction;
    }
}
