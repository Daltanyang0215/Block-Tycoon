
using System.Collections.Generic;

public interface IHexaGridInItem
{
    public ItemData CurProductItem { get; }
    public Dictionary<int, int> MaterialItemCount { get; }
    public int ProductItemCount { get; set; }
    public bool CanGetMaterial(int index);
    public HexaSaveData SaveData();
}
