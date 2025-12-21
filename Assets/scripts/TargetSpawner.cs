using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    public static TargetSpawner Instance { get; private set; }

    [Header("Configuration de la Zone")]
    public Collider spawnZone;

    [Header("Paramètres de Jeu")]
    public float checkInterval = 2f; 
    public int maxActiveTargets = 3;

    private int currentActiveTargets = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    public void RegisterTargetDespawn()
    {
        currentActiveTargets--;
        if (currentActiveTargets < 0) currentActiveTargets = 0;
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (currentActiveTargets < maxActiveTargets)
            {
                SpawnTarget();
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    void SpawnTarget()
    {
        if (spawnZone == null) return;

        Vector3 randomPosition = GetRandomPointInZone();
        GameObject target = ObjectPoolManager.Instance.SpawnFromPool("Target", randomPosition, Quaternion.identity);
        
        if (target != null)
        {
            currentActiveTargets++;
        }
    }

    Vector3 GetRandomPointInZone()
    {
        Bounds bounds = spawnZone.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }
}