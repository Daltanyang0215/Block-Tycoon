using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoElement : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _countText;

    public void Init(int itemid)
    {
        ItemData initItem = MainGameDataSo.Instance.ItemDatas[itemid];

        _itemImage.sprite = initItem.ItemSprite;
        _slider.fillRect.GetComponent<Image>().color = initItem.ItemColor;
    }

    public void Init(Sprite sprite , Color color)
    {
        _itemImage.sprite = sprite;
        _slider.fillRect.GetComponent<Image>().color = color;
    }

    public void UpDateSlider(int curValue, int maxValue, bool showValue = true)
    {
        _slider.value = (float)curValue / maxValue;
        _countText.text = $"{curValue}/{maxValue}";
        _countText.enabled = showValue;
    }
}