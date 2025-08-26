using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [Header("Ground Settings")]
    public GameObject [] groundPrefabs;   // different ground tiles (for variety)
    public int numberOfTiles = 5;        // how many tiles to keep alive
    public float tileLength = 30f;       // z-size of your ground tile prefab
    public float laneDistance = 3f;      // distance between lanes (must match PlayerController)

    [Header("Player Reference")]
    public Transform player;

    [Header("Obstacle Settings")]
    public GameObject minePrefab;        // assign your Mine prefab
    public int obstaclesPerTile = 2;     // how many mines per tile

    private float zSpawn = 0f;
    private Queue<GameObject> activeTiles = new Queue<GameObject>();

    void Start()
    {
        // Spawn initial ground tiles
        for (int i = 0; i < numberOfTiles; i++)
        {
            if (i == 0)
                SpawnTile(0, false); // first tile = safe
            else
                SpawnTile(Random.Range(0, groundPrefabs.Length), true);
        }
    }

    void Update()
    {
        // Spawn new tiles as player moves forward
        if (player.position.z - 35f > zSpawn - (numberOfTiles * tileLength))
        {
            SpawnTile(Random.Range(0, groundPrefabs.Length), true);
            DeleteTile();
        }
    }

    void SpawnTile(int prefabIndex, bool spawnObstacles)
    {
        GameObject tile = Instantiate(
            groundPrefabs [prefabIndex],
            Vector3.forward * zSpawn,
            Quaternion.identity,
            transform
        );

        activeTiles.Enqueue(tile);

        // Optionally spawn obstacles
        if (spawnObstacles && minePrefab != null)
        {
            for (int i = 0; i < obstaclesPerTile; i++)
            {
                int lane = Random.Range(-1, 2); // -1 = left, 0 = middle, 1 = right
                float xPos = lane * laneDistance;
                float zPos = zSpawn + Random.Range(5f, tileLength - 5f);

                Vector3 spawnPos = new Vector3(xPos, 0.5f, zPos); // adjust Y if mine sits too high/low
                Instantiate(minePrefab, spawnPos, Quaternion.identity, tile.transform);
            }
        }

        zSpawn += tileLength;
    }

    void DeleteTile()
    {
        GameObject oldTile = activeTiles.Dequeue();
        Destroy(oldTile);
    }
}
