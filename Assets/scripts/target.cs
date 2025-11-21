using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject hitEffectPrefab;  // Effet de particules � assigner

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Faire appara�tre l'effet de particules � la position de la cible
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // D�truire la cible
            Destroy(gameObject);
        }
    }

}
