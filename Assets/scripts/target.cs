using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject hitEffectPrefab;  // Effet de particules à assigner

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Faire apparaître l'effet de particules à la position de la cible
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // Détruire la cible
            Destroy(gameObject);
        }
    }

}
