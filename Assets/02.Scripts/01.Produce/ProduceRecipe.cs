using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProduceRecipe
{
    [field: Header("Result")]
    [field : SerializeField] public int ProduceitemID { get; private set; }

    [field: SerializeField] public float ProduceTime { get; private set; }
    [field: Header("Material")]
    [field: SerializeField] public List<ItemPair> MaterailItemPairs { get; private set; }
    [field : Header("Condition")]
    [field: SerializeField] public HexaType NearHexaCondition { get; private set; }
    [field: SerializeField] public HexaType CanProduceHexa { get; private set; }
    [field: SerializeField] public int CanProduceHexaTiar { get; private set; }
}


[System.Serializable]
public class ItemPair
{
    [field: SerializeField] public int ItemID { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
}
