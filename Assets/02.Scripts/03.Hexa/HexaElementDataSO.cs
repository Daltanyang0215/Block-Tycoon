using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexaElementDataSO", menuName = "BlockTycoon/HexaElementDataSO")]
public class HexaElementDataSO : ScriptableObject
{
    [field: SerializeField] public List<ProducItemPair> itemPairs { get; private set; }
    [field: Tooltip("-1 : 무한 생산 \n0 : 생산 안함\n?? : 1회 생산에 필요한 시간")]
    [field: SerializeField] public float ProducTime { get; private set; }
    [field: Tooltip("1회 클릭을 채워지는 할당량 / 1 => MAX")]
    [field: SerializeField] public float ProducClick { get; private set; }

    // TODO : 나중에 메인컬러만 변경하는 걸로 배치를 줄일수 있는지 테스트 할 필요 있음
    [field: Header("Render")]
    [field: SerializeField] public Sprite HexaIcon { get; private set; }
    [field: SerializeField] public Color TopHexaColor { get; private set; }
    [field: SerializeField] public Color BottomHexaColor { get; private set; }
    [field: SerializeField] public Color TopGaugeColor { get; private set; }
    [field: SerializeField] public Color BottomGaugeColor { get; private set; }

}

[System.Serializable]
public class ProducItemPair
{
    [field: SerializeField] public ItemType ProducItemType { get; private set; }
    [field: SerializeField] public int ProducAmount { get; private set; }
}