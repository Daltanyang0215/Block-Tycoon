using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoElement : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _countText;

    public void Init(ItemType type)
    {
        _itemImage.sprite = MainGameDataSo.Instance.GetItemSprite(type);
        _slider.fillRect.GetComponent<Image>().color = MainGameDataSo.Instance.GetItemColor(type);
    }

    public void UpDateSlider(int curValue, int maxValue)
    {
        _slider.value = curValue / (float)maxValue;
        _countText.text = $"{curValue}/{maxValue}";
    }
}