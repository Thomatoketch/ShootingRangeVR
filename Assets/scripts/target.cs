using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic; // Nécessaire pour les listes

public class Target : MonoBehaviour, IPooledObject
{
    [Header("Addressables")]
    public string hitEffectKey = "HitEffectFX"; 
    private string currentLabel;

    void Start()
    {
        // CORRECTION ICI : Il faut utiliser "Android" pour correspondre à ton groupe Addressable
        #if UNITY_ANDROID
            currentLabel = "Quest"; 
        #else
            currentLabel = "PCVR";
        #endif
    }

    public void OnObjectSpawn()
    {
        // Réinitialisation (optionnelle pour l'instant)
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            
            TriggerHitEffect(hitPosition);

            // Désactivation pour le Pooling
            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void TriggerHitEffect(Vector3 hitPosition)
    {
        // Note: Utilisation de la liste pour l'intersection (Key + Label)
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