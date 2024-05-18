
using System.Collections.Generic;

public interface IHexaGridInItem
{
    public ProduceRecipe CurRecipe { get; }
    public Dictionary<int, int> MaterialItemCount { get; }
    public Dictionary<int, int> ProductItemCount { get; }
    public bool CanGetMaterial(int index);
    public HexaSaveData SaveData();
}
