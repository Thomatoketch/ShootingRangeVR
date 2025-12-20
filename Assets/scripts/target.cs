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
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            
            TriggerHitEffect(hitPosition);

            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.AddScore(10); 
            }

            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void TriggerHitEffect(Vector3 hitPosition)
    {
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
        Instantiate(fxPrefab, position, Quaternion.identity);
    }
}