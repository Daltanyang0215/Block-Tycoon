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

    public void Init(Sprite sprite, string name)
    {
        _image.sprite = sprite;
        _itemName.StringReference.SetReference("Item",name);
    }

    public void UpdateItemCount(int count)
    {
        _itemCount.text = count.ToString();
    }
}
