using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProduceRecipe
{
    [field: Header("Material")]
    [field: SerializeField] public HexaType NearHexaCondition { get; private set; }
    [field: SerializeField] public List<ItemPair> MaterailItemPairs { get; private set; }

    [field: Tooltip("-1 : ���� ���� \n0 : ���� ����\n?? : 1ȸ ���꿡 �ʿ��� �ð�")]
    [field: SerializeField] public float ProduceTime { get; private set; }
    [field: Tooltip("1ȸ Ŭ���� ä������ �Ҵ緮 / 1 => MAX")]
    [field: SerializeField] public float ProduceClick { get; private set; }

    [field: Header("Result")]
    [field: SerializeField] public List<ItemPair> ProduceItemPairs { get; private set; }

}


[System.Serializable]
public class ItemPair
{
    [field: SerializeField] public ItemType ProduceItemType { get; private set; }
    [field: SerializeField] public int ProduceAmount { get; private set; }
}
