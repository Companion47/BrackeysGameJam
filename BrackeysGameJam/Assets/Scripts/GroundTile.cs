using UnityEngine;

public class GroundTile : MonoBehaviour
{
    private GroundSpawner spawner;

    [Header("Obstacle Settings")]
    public GameObject minePrefab;   // assign your Mine prefab here
    public int obstaclesPerTile = 2; // how many to spawn per tile (adjust)

    void Start()
    {
        spawner = GameObject.FindObjectOfType<GroundSpawner>();
        SpawnObstacles();
    }

    public void SpawnObstacles()
    {
        for (int i = 0; i < obstaclesPerTile; i++)
        {
            // Pick random lane: -1 = left, 0 = middle, 1 = right
            int lane = Random.Range(-1, 2);

            // Random Z position within this tile
            float zPos = transform.position.z + Random.Range(5f, spawner.tileLength - 5f);

            // X position depends on lane width (same as PlayerController's laneDistance)
            float xPos = lane * spawner.laneDistance;

            Vector3 spawnPos = new Vector3(xPos, 0.5f, zPos); // 0.5f = adjust mine height
            Instantiate(minePrefab, spawnPos, Quaternion.identity, transform);
        }
    }
}
