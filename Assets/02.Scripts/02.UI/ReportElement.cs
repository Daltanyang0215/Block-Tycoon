using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ReportElement : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private LocalizeStringEvent _itemName;
    [SerializeField] private TMP_Text _itemCount;
    [SerializeField] private TMP_Text _itemPrice;

    public void Init(Sprite sprite, string name, string table = "Item")
    {
        _image.sprite = sprite;
        _itemName.StringReference.SetReference(table, name);
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

    public void UpdateItemCount(int count)
    {
        _itemCount.text = count.ToString();
    }
    public void UpdateItemPrice(int price)
    {
        _itemPrice.enabled = price > 0;
        _itemPrice.text = price.ToString();
    }
}
