using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexaElementDataSO", menuName = "BlockTycoon/HexaElementDataSO")]
public class HexaElementDataSO : ScriptableObject
{

    [field: SerializeField] public HexaType HexaType { get; private set; }
    [field: SerializeField] public bool CanBuy { get; private set; }
    [field: SerializeField] public List<ItemPair> UnlockPrice { get; private set; }
    [field: SerializeField] public List<ItemPair> BuyPrice { get; private set; }

    [field: Header("Produce")]
    [field: SerializeField] public List<ProduceRecipe> ProduceRecipe { get; private set; }

    [field: SerializeField] public float ProducePerTimeBonus { get; private set; } = 1;

    [field: Header("Upgrade")]
    [field: SerializeField] public List<HexaUpgradePair> UpgradePairs { get; private set; }

    [field: Header("Render")]
    [field: SerializeField] public Sprite HexaIcon { get; private set; }
    [field: SerializeField] public Sprite HexaSubIcon1 { get; private set; }
    [field: SerializeField] public Sprite HexaSubIcon2 { get; private set; }
    [field: SerializeField] public Color TopHexaColor { get; private set; }
    [field: SerializeField] public Color BottomHexaColor { get; private set; }
    [field: SerializeField] public Color TopGaugeColor { get; private set; }
    [field: SerializeField] public Color BottomGaugeColor { get; private set; }
    [field: SerializeField] public Color BoostingColor { get; private set; }
    [field: SerializeField] public Color BoosterMaxColor { get; private set; }
    [field: SerializeField] public ParticleSystem ProcessParticle { get; private set; }

    public int GetID => MainGameDataSo.Instance.HexaDatas.FindIndex(x => x == this);
}
[System.Serializable]
public class HexaUpgradePair
{
    [field: SerializeField] public HexaUpgradeType Type { get; private set; }
    [field: SerializeField] public List<PriceValuePair> Prices { get; private set; }

    [System.Serializable]
    public struct PriceValuePair
    {
        [field: SerializeField] public int Price { get; private set; }

        [field: Tooltip("강화시 해당 값 적용. 합산 아님")]
        [field: SerializeField]
        public float Value { get; private set; }
    }
}

public enum HexaUpgradeType
{
    AddPerSec,    
}