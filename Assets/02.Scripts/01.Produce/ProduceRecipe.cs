using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProduceRecipe
{
    [field: Header("Material")]
    [field: SerializeField] public HexaType NearHexaCondition { get; private set; }
    [field: SerializeField] public List<ItemPair> MaterailItemPairs { get; private set; }

    [field: Tooltip("-1 : 무한 생산 \n0 : 생산 안함\n?? : 1회 생산에 필요한 시간")]
    [field: SerializeField] public float ProduceTime { get; private set; }
    [field: Tooltip("1회 클릭을 채워지는 할당량 / 1 => MAX")]
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
