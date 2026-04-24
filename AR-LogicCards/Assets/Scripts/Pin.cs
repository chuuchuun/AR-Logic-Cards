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
        if (_renderer != null && selectedMaterial != null && defaultMaterial != null)
            _renderer.material = selected ? selectedMaterial : defaultMaterial;
    }
}