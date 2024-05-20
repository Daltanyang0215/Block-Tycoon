
using UnityEngine;

public interface IHexaGridElement
{
    public HexaElementDataSO Data { get; }
    public Vector2 Pos { get; }

    public void Init(HexaElementDataSO data, HexaSaveData saveData);
    public void SetNearHexa(IHexaGridElement[] hexas);
    public void RemoveNearHexa(IHexaGridElement hexa);
    public void HexaUpdate();
}
