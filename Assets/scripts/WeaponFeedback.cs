using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponFeedback : MonoBehaviour
{
    private Renderer[] _renderers;
    private MaterialPropertyBlock _propBlock;
    
    // Cette variable sert de verrou de sécurité
    private bool _isGrabbed = false; 

    private static readonly int HoverPropID = Shader.PropertyToID("_Hover");
    private static readonly int GrabPropID = Shader.PropertyToID("_Grab");

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void OnHoverEnter()
    {
        // SI on tient déjà l'objet, on ignore le Hover (le Grab est prioritaire)
        if (_isGrabbed) return;
        
        UpdateShader(1f, 0f);
    }

    public void OnHoverExit()
    {
        // SI on tient l'objet, on interdit au Hover Exit d'éteindre la lumière
        if (_isGrabbed) return;

        UpdateShader(0f, 0f);
    }

    public void OnGrabEnter()
    {
        _isGrabbed = true; // On verrouille
        UpdateShader(0f, 1f);
    }

    public void OnGrabExit()
    {
        _isGrabbed = false; // On déverrouille
        // Quand on relâche, on remet à 0 (ou à Hover si tu préfères)
        UpdateShader(0f, 0f);
    }

    private void UpdateShader(float hoverValue, float grabValue)
    {
        _propBlock.SetFloat(HoverPropID, hoverValue);
        _propBlock.SetFloat(GrabPropID, grabValue);

        foreach (var r in _renderers)
        {
            r.SetPropertyBlock(_propBlock);
        }
    }
}