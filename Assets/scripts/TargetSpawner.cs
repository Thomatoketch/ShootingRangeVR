using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [Header("Paramètres")]
    public Transform[] spawnPoints; // Glisse tes points d'apparition ici
    public float respawnTime = 2f;  // Temps avant qu'une nouvelle cible n'apparaisse
    public int maxActiveTargets = 3; // Combien de cibles en même temps max ?

    private float timer;

    void Update()
    {
        // Compte combien de cibles sont actives dans le jeu actuellement
        // (Note: C'est une méthode simple, pour plus de perf on garderait une liste locale)
        int activeTargets = 0;
        foreach(Transform child in ObjectPoolManager.Instance.transform)
        {
            if (child.gameObject.activeInHierarchy && child.tag == "Target")
            {
                activeTargets++;
            }
        }

        // Si on a de la place pour une cible
        if (activeTargets < maxActiveTargets)
        {
            timer += Time.deltaTime;
            if (timer >= respawnTime)
            {
                SpawnTarget();
                timer = 0;
            }
        }
    }

    void SpawnTarget()
    {
        // Choisir un point au hasard
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        // Demander au PoolManager
        ObjectPoolManager.Instance.SpawnFromPool("Target", spawnPoint.position, spawnPoint.rotation);
    }
}