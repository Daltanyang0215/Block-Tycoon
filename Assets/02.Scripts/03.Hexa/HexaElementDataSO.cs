using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexaElementDataSO", menuName = "BlockTycoon/HexaElementDataSO")]
public class HexaElementDataSO : ScriptableObject
{
    [field: SerializeField] public List<ProducItemPair> itemPairs { get; private set; }
    [field: Tooltip("-1 : ���� ���� \n0 : ���� ����\n?? : 1ȸ ���꿡 �ʿ��� �ð�")]
    [field: SerializeField] public float ProducTime { get; private set; }
    [field: Tooltip("1ȸ Ŭ���� ä������ �Ҵ緮 / 1 => MAX")]
    [field: SerializeField] public float ProducClick { get; private set; }

    // TODO : ���߿� �����÷��� �����ϴ� �ɷ� ��ġ�� ���ϼ� �ִ��� �׽�Ʈ �� �ʿ� ����
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