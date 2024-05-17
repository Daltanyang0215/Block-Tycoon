using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexaElementDataSO", menuName = "BlockTycoon/HexaElementDataSO")]
public class HexaElementDataSO : ScriptableObject
{

    [field: SerializeField] public HexaType HexaType { get; private set; }
    [field:SerializeField]public bool CanBuy {  get; private set; }
    [field:SerializeField]public List<ItemPair> BuyPrice { get; private set; }  

    [field: Header("Produce")]
    [field: SerializeField] public List<ProduceRecipe> ProduceRecipe { get; private set; }

    // TODO : 나중에 메인컬러만 변경하는 걸로 배치를 줄일수 있는지 테스트 할 필요 있음
    [field: Header("Render")]
    [field: SerializeField] public Sprite HexaIcon { get; private set; }
    [field: SerializeField] public Sprite HexaSubIcon1 { get; private set; }
    [field: SerializeField] public Sprite HexaSubIcon2 { get; private set; }
    [field: SerializeField] public Color TopHexaColor { get; private set; }
    [field: SerializeField] public Color BottomHexaColor { get; private set; }
    [field: SerializeField] public Color TopGaugeColor { get; private set; }
    [field: SerializeField] public Color BottomGaugeColor { get; private set; }

}
