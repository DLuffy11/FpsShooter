using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private bool _isSelected;
    [SerializeField] private Sprite _imageSprite;
    [SerializeField] private GameObject _isSelectedGameObject;
    private Image _image;

    private void Awake()
    {
        _image = gameObject.transform.GetChild(0).GetComponent<Image>();
        _isSelectedGameObject = gameObject.transform.GetChild(1).gameObject;
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        _isSelectedGameObject.SetActive(isSelected);
    }

    public void SetImage(Sprite image)
    {
        _imageSprite = image;
        _image.sprite = _imageSprite;
    }
}
