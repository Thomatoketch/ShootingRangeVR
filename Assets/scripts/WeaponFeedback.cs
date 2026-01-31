using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit; // Pas nécessaire pour cette partie du TP

public class WeaponFeedback : MonoBehaviour
{
    [Header("Configuration TP")]
    public Transform player;           // Glisse ton Soldat ici
    public float hoverThreshold = 2.5f; // Distance pour activer le Hover

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propBlock;
    
    // IDs des propriétés Shader
    private static readonly int HoverPropID = Shader.PropertyToID("_Hover");
    private static readonly int GrabPropID = Shader.PropertyToID("_Grab");

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (player == null) return;

        // 1. DÉTECTION "GRAB" (Est-ce que l'arme est équipée ?) 
        // On vérifie si l'arme est devenue enfant du joueur (attachée au Socket main)
        // ou on peut vérifier une variable booléenne si tu en as une ailleurs.
        bool isEquipped = transform.root == player.root;

        if (isEquipped)
        {
            // État GRAB : On allume _Grab, on éteint _Hover
            UpdateShader(0f, 1f);
        }
        else
        {
            // 2. DÉTECTION "HOVER" (Distance) 
            // Si l'arme est au sol, on regarde la distance avec le joueur
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= hoverThreshold)
            {
                // État HOVER : On allume _Hover
                UpdateShader(1f, 0f);
            }
            else
            {
                // État NEUTRE : Tout éteint
                UpdateShader(0f, 0f);
            }
        }
    }

    private void UpdateShader(float hoverValue, float grabValue)
    {
        // On remplit le bloc de propriétés (plus performant que material.SetFloat) 
        _propBlock.SetFloat(HoverPropID, hoverValue);
        _propBlock.SetFloat(GrabPropID, grabValue);

        foreach (var r in _renderers)
        {
            r.SetPropertyBlock(_propBlock);
        }
    }
}