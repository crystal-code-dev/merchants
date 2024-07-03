using UnityEngine;

public class HorseSpawner : MonoBehaviour {
    // Public variables
    public GameObject horsePrefab;
    public Transform[] spawnPoints;
    public float minSpeed = 2f, maxSpeed = 60f, spawnInterval = 0.2f, spawnDistance = 30f, spawnTimer = 0f;

    // Built-in methods
    private void Update() {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval) {
            SpawnHorse();
            spawnTimer = 0f;
        }
    }

    // Private custom methods
    private void SpawnHorse() {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newHorse = Instantiate(horsePrefab, spawnPoint.position, Quaternion.identity);
        HorseIA horseIA = newHorse.GetComponent<HorseIA>();

        horseIA.speed = Random.Range(minSpeed, maxSpeed);
    }
}
