using UnityEngine;

public enum PinType
{
    Input,
    Output
}

public class Pin : MonoBehaviour
{
    public PinType pinType;
    public int pinIndex;
    public ARCard parentCard;

    private Renderer _renderer;
    public Material defaultMaterial;
    public Material selectedMaterial;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) _renderer = GetComponentInChildren<Renderer>();
        parentCard = GetComponentInParent<ARCard>();
        if (defaultMaterial == null && _renderer != null) defaultMaterial = _renderer.material;
    }

    public void SetSelected(bool selected)
    {
        if (_renderer != null)
            _renderer.material = selected ? selectedMaterial : defaultMaterial;
    }
}