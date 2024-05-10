
public interface IHexaGridElement
{
    public HexaElementDataSO Data { get; }

    public void Init(HexaElementDataSO data);
    public void SetNearHexa(IHexaGridElement[] hexas);
    public void RemoveNearHexa(IHexaGridElement hexa);
    public void HexaUpdate();
}