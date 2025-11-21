using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject target;  // Assigner le prefab de la cible
    public float spawnInterval = 3f; // Temps entre chaque spawn
    public float spawnRange = 10f;   // Zone de spawn des cibles

    void Start()
    {
        // Démarrer la routine de spawn
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (true)
        {
            // Attendre avant de spawner la prochaine cible
            yield return new WaitForSeconds(spawnInterval);

            // Position aléatoire pour la cible
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-spawnRange, spawnRange), UnityEngine.Random.Range(1f, 5f), UnityEngine.Random.Range(-spawnRange, spawnRange));

            // Faire apparaître une nouvelle cible
            Instantiate(target, randomPosition, Quaternion.identity);
        }
    }
}
