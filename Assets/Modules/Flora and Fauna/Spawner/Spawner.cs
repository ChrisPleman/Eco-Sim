using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawning Criteria")]
    public float SpawnAfterSeconds;
    public GameObject ObjectToSpawn;
    public Vector3 SpawnAt;

    // * Private Variables
    private int TotalToSpawn;
    private int TotalSpawned;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TotalToSpawn = 1;
        TotalSpawned = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= SpawnAfterSeconds && TotalSpawned < TotalToSpawn)
        {
            Instantiate(ObjectToSpawn, SpawnAt, Quaternion.identity);
            TotalSpawned++;
        }
    }
}
