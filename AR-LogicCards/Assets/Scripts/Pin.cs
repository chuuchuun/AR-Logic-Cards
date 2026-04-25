using UnityEngine;

public enum PinType
{
    Input,
    Output
}

public class Pin : MonoBehaviour
{
    [Header("Pin Settings")]
    public PinType pinType;
    public int pinIndex;
    public ARCard parentCard;

    public Material defaultMaterial;
    public Material selectedMaterial;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();
        if (defaultMaterial == null) defaultMaterial = _renderer?.material;
        parentCard = GetComponentInParent<ARCard>();
    }

    public void SetSelected(bool selected)
    {
        Debug.Log($"SetSelected called on {name}, selected={selected}");
        if (_renderer == null) Debug.LogError("Renderer is null on " + name);
        if (selectedMaterial == null) Debug.LogError("selectedMaterial is null on " + name);
        if (_renderer != null && selectedMaterial != null)
        {
            _renderer.material = selected ? selectedMaterial : defaultMaterial;
            Debug.Log("Material changed");
        }
    }
}