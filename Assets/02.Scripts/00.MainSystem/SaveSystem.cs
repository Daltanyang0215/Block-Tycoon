using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem
{
    private static SaveData _save;

    public static SaveData Save
    {
        get
        {
            // 세이브가 없으면 불러오기
            if (_save == null)
                _save = LoadData();

            // 불러오기를 했는데도 없으면 빈 걸 주기
            if (_save == null)
                _save = new SaveData();
            return _save;
        }
    }

    static string path = Application.persistentDataPath + "/saves.json";

    public static void SaveData()
    {
        if (_save == null) return;
        _save.Save();
        // Set our save location and make sure we have a saves folder ready to go.

        Debug.Log("Saving ");

        FileStream fileStream = new FileStream(path, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_save));
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public static SaveData LoadData()
    {
        if (File.Exists(path))
        {
            Debug.Log("loading from save.");

            FileStream fileStream = new FileStream(path, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            return null;
        }
    }

    public static void DeleteData()
    {
        if (!File.Exists(path))
            return;

        FileInfo deleteDir = new FileInfo(path);
        deleteDir.Delete();
        _save = null;
    }
}

[System.Serializable]
public class SaveData
{
    public List<int> HasItemCode = new List<int>();
    public List<int> HasItemCount = new List<int>();
    public List<bool> UnlockList;
    public List<Vector3Int> HexaPos = new List<Vector3Int>();
    public List<int> HexaID = new List<int>();
    public List<HexaSaveData> HexaSaveDatas = new List<HexaSaveData>();
    public SaveData()
    {
        foreach (KeyValuePair<int, ItemData> item in MainGameDataSo.Instance.ItemDatas)
        {
            HasItemCode.Add(item.Key);
            HasItemCount.Add(0);
        }
        UnlockList = new List<bool>();
        for (int i = 0; i < MainGameDataSo.Instance.HexaDatas.Count; i++)
        {
            UnlockList.Add(false);
        }

        HexaPos.Add(new Vector3Int(-2, 0, 0)); HexaID.Add(0); HexaSaveDatas.Add(null);
        HexaPos.Add(new Vector3Int(-1, 2, 0)); HexaID.Add(1); HexaSaveDatas.Add(null);
        HexaPos.Add(new Vector3Int(1, 2, 0)); HexaID.Add(2); HexaSaveDatas.Add(null);
        HexaPos.Add(new Vector3Int(2, 0, 0)); HexaID.Add(3); HexaSaveDatas.Add(null);
        HexaPos.Add(new Vector3Int(1, -2, 0)); HexaID.Add(4); HexaSaveDatas.Add(null);
        HexaPos.Add(new Vector3Int(-1, -2, 0)); HexaID.Add(5); HexaSaveDatas.Add(null);
    }

    public void Save()
    {
        HasItemCode.Clear();
        HasItemCount.Clear();
        foreach (KeyValuePair<int, ItemData> item in MainGameDataSo.Instance.ItemDatas)
        {
            HasItemCode.Add(item.Key);
            HasItemCount.Add(MainGameManager.Instance.GetItemCount(item.Key));
        }
        UnlockList = MainGameManager.Instance.UnlockList;

        HexaPos.Clear();
        HexaID.Clear();
        HexaSaveDatas.Clear();
        foreach (KeyValuePair<Vector3Int, IHexaGridElement> grid in HexaGridManager.Instance.GridPositions)
        {
            HexaPos.Add(grid.Key);
            HexaID.Add(MainGameDataSo.Instance.HexaDatas.FindIndex(x => x == grid.Value.Data));

            if (!(grid.Value is IHexaGridInItem gridInItem))
            {
                HexaSaveDatas.Add(null);
                continue;
            }
            HexaSaveDatas.Add(gridInItem.SaveData());
        }
    }
}
[System.Serializable]
public class HexaSaveData
{
    public int CurRecipeIndex;
    public float FillAmount;
    public List<int> HexaMaterialItemCode = new List<int>();
    public List<int> HexaMaterialItemCount = new List<int>();
    public List<int> HexaProductItemCode = new List<int>();
    public List<int> HexaProductItemCount = new List<int>();

    public byte InVec;
    public byte OutVec;
}