using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class Target : MonoBehaviour, IPooledObject
{
    [Header("Addressables")]
    public string hitEffectKey = "HitEffectFX"; 
    private string currentLabel;

    void Start()
    {
        // Définition de la plateforme pour les Addressables
        #if UNITY_ANDROID
            currentLabel = "Quest"; 
        #else
            currentLabel = "PCVR";
        #endif
    }

    public void OnObjectSpawn()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        // Vérifie que l'objet qui touche est bien une balle
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            
            // 1. Jouer l'effet visuel (particules)
            TriggerHitEffect(hitPosition);

            // 2. "Détruire" la balle (la renvoyer au pool)
            collision.gameObject.SetActive(false);

            // 3. "Détruire" la cible (la renvoyer au pool)
            // C'est cette ligne qui libère la place pour le TargetSpawner
            gameObject.SetActive(false);
        }
    }

    private void TriggerHitEffect(Vector3 hitPosition)
    {
        // Chargement et instanciation de l'effet d'impact
        Addressables.LoadAssetsAsync<GameObject>(new List<object> { hitEffectKey, currentLabel }, 
            null, Addressables.MergeMode.Intersection).Completed += (op) => 
            {
                if(op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)
                {
                    SpawnFX(op.Result[0], hitPosition);
                }
            };
    }

    private void SpawnFX(GameObject fxPrefab, Vector3 position)
    {
        if (fxPrefab == null) return;

        GameObject hitInstance = Instantiate(fxPrefab, position, Quaternion.identity);
        
        ParticleSystem ps = hitInstance.GetComponent<ParticleSystem>();
        float duration = (ps != null) ? ps.main.duration : 1.0f;
        Destroy(hitInstance, duration + 0.1f); 
    }
}