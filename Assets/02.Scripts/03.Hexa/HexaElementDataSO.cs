using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexaElementDataSO", menuName = "BlockTycoon/HexaElementDataSO")]
public class HexaElementDataSO : ScriptableObject
{

    [field: SerializeField] public HexaType HexaType { get; private set; }

    [field: Header("Produce")]
    [field: SerializeField] public List<ProduceRecipe> ProduceRecipe { get; private set; }

    // TODO : ���߿� �����÷��� �����ϴ� �ɷ� ��ġ�� ���ϼ� �ִ��� �׽�Ʈ �� �ʿ� ����
    [field: Header("Render")]
    [field: SerializeField] public Sprite HexaIcon { get; private set; }
    [field: SerializeField] public Color TopHexaColor { get; private set; }
    [field: SerializeField] public Color BottomHexaColor { get; private set; }
    [field: SerializeField] public Color TopGaugeColor { get; private set; }
    [field: SerializeField] public Color BottomGaugeColor { get; private set; }

}
