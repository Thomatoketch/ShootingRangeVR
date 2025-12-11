using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [Header("Configuration de la Zone")]
    public Collider spawnZone; // Assigne ici ton BoxCollider (Zone)

    [Header("Paramètres de Jeu")]
    public float respawnTime = 2f;
    public int maxActiveTargets = 3;

    private float timer;

    void Update()
    {
        // 1. Compter les cibles actives
        int activeTargets = 0;
        if (ObjectPoolManager.Instance != null)
        {
            foreach(Transform child in ObjectPoolManager.Instance.transform)
            {
                if (child.gameObject.activeInHierarchy && child.CompareTag("Target"))
                {
                    activeTargets++;
                }
            }
        }

        // 2. Gestion du temps et du spawn
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
        if (spawnZone == null)
        {
            Debug.LogError("Attention : Pas de Spawn Zone assignée dans le TargetSpawner !");
            return;
        }

        // On prend juste un point au hasard, sans se poser de questions
        Vector3 randomPosition = GetRandomPointInZone();

        // On fait apparaître la cible
        ObjectPoolManager.Instance.SpawnFromPool("Target", randomPosition, Quaternion.identity);
    }

    Vector3 GetRandomPointInZone()
    {
        // Récupère les limites du Collider
        Bounds bounds = spawnZone.bounds;

        // Génère une coordonnée aléatoire pour x, y et z à l'intérieur de ces limites
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }

    // Visuel pour voir la zone dans l'éditeur (optionnel)
    void OnDrawGizmos()
    {
        if (spawnZone != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Rouge transparent
            Gizmos.DrawCube(spawnZone.bounds.center, spawnZone.bounds.size);
        }
    }
}