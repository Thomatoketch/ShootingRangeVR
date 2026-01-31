using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [Header("Cibles")]
    public Transform followAnchor;
    public LayerMask obstacleMask; // Mets "Default" et "Industrial"

    [Header("Mouvement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f; // Augmenté pour réagir vite
    public float lookAheadDist = 3f;  // Distance de vision (Augmentée)
    public float droneRadius = 0.4f;  // Marge de sécurité (Augmentée)

    [Header("Scanner (Radar)")]
    // On teste ces angles dans l'ordre précis. Le premier qui passe gagne.
    private float[] searchAngles = { 0, 15, -15, 30, -30, 45, -45, 60, -60, 80, -80 };

    [Header("Visuel (Debug)")]
    public Renderer droneRenderer;
    public Color colorOK = Color.cyan;
    public Color colorBlocked = Color.red;
    private MaterialPropertyBlock propBlock;
    private int colorID;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        colorID = Shader.PropertyToID("_BaseColor"); // Ou "_Color" selon ton shader
    }

    void Update()
    {
        if (followAnchor == null) return;

        // 1. Direction vers la cible idéale
        Vector3 targetDir = (followAnchor.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, followAnchor.position);

        // Stop si arrivé
        if (dist < 0.5f) {
            UpdateColor(colorOK);
            return;
        }

        Vector3 bestDir = Vector3.zero;
        bool pathFound = false;

        // 2. SCANNING : On teste les angles un par un
        foreach (float angle in searchAngles)
        {
            // On tourne le vecteur "targetDir" selon l'angle Y
            Vector3 testDir = Quaternion.Euler(0, angle, 0) * targetDir;

            // SPHERECAST : On vérifie si ça passe
            if (!Physics.SphereCast(transform.position, droneRadius, testDir, out RaycastHit hit, lookAheadDist, obstacleMask))
            {
                bestDir = testDir;
                pathFound = true;
                
                // Debug vert : Chemin trouvé
                Debug.DrawRay(transform.position, testDir * lookAheadDist, Color.green);
                break; // On arrête de chercher, on a trouvé le meilleur chemin
            }
        }

        // 3. APPLICATION DU MOUVEMENT
        if (pathFound)
        {
            UpdateColor(colorOK);

            // Rotation rapide vers la sortie
            Quaternion targetRot = Quaternion.LookRotation(bestDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            
            // Avancer
            transform.position += bestDir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 4. FALLBACK : BLOQUÉ !
            // Si tous les rayons tapent un mur (cul-de-sac), on recule ou on pivote sur place
            UpdateColor(colorBlocked);
            
            // Debug rouge : Panique
            Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
            
            // Rotation d'urgence (tourne sur soi-même pour chercher une issue)
            transform.Rotate(0, 200 * Time.deltaTime, 0);
        }
    }

    void UpdateColor(Color c)
    {
        if (droneRenderer)
        {
            droneRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(colorID, c);
            droneRenderer.SetPropertyBlock(propBlock);
        }
    }
}