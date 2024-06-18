//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Localization.Components;

//public class HexaTransitInfoPanel : MonoBehaviour
//{
//    private HexaGridTransit _curHexaElement;

//    [SerializeField] private LocalizeStringEvent _hexaName;
//    [SerializeField] private ReportElement _curItemReport;
//    [SerializeField] private Transform _inputArrow;
//    [SerializeField] private Transform _outputArrow;
//    [SerializeField] private Transform _inputMainArrow;
//    [SerializeField] private Transform _outputMainArrow;

//    private byte inputVec = 0;
//    private byte outputVec = 3;

//    public void ShowPanel(HexaGridTransit curElenemt)
//    {

//        transform.GetComponentInParent<Canvas>().enabled = true;

//        Vector2 pos = Camera.main.WorldToScreenPoint(curElenemt.transform.position);
//        Vector2 viewPos = Camera.main.WorldToViewportPoint(curElenemt.transform.position);
//        if (viewPos.x > .75f)
//        {
//            pos.x -= 550 * (Screen.width / 1920);
//        }
//        if (viewPos.y > .85f)
//        {
//            pos.y -= 125 * (Screen.height / 1080);
//        }
//        if (viewPos.y < .25f)
//        {
//            pos.y += 200 * (Screen.height / 1080);
//        }
//        transform.position = pos;

//        _curHexaElement = curElenemt;
//        _curHexaElement.InfoUpData = UpdateInfo;
//        UpdateInfo();

//        _hexaName.StringReference.SetReference("Hexa", _curHexaElement.Data.name);
        
//        inputVec = curElenemt.InputVec;
//        outputVec = curElenemt.OutputVec;
//        SetInputArrow();
//        SetOutputArrow();
//    }

//    public void HidePanel()
//    {
//        transform.GetComponentInParent<Canvas>().enabled = false;
//        _curHexaElement.InfoUpData = null;
//    }

//    public void UpdateInputVec(bool cw)
//    {
//        if (cw)
//        {
//            inputVec++;
//            if (inputVec > 5)
//                inputVec = 0;
//        }
//        else
//        {
//            inputVec--;
//            if (inputVec > 250)
//                inputVec = 5;
//        }
//        _curHexaElement.SetVec(inputVec, outputVec);
//        SetInputArrow();
//    }

//    private void SetInputArrow()
//    {
//        _inputArrow.eulerAngles = Vector3.back * (inputVec * 60 + 180);
//        _inputMainArrow.eulerAngles = Vector3.back * (inputVec * 60);
//    }

//    public void UpdateOutputVec(bool cw)
//    {
//        if (cw)
//        {
//            outputVec++;
//            if (outputVec > 5)
//                outputVec = 0;
//        }
//        else
//        {
//            outputVec--;
//            if (outputVec > 250)
//                outputVec = 5;
//        }
//        _curHexaElement.SetVec(inputVec, outputVec);
//        SetOutputArrow();
//    }

//    private void SetOutputArrow()
//    {
//        _outputArrow.eulerAngles = Vector3.back * (outputVec * 60 + 180);
//        _outputMainArrow.eulerAngles = Vector3.back * (outputVec * 60 + 180);
//    }

//    public void UpdateInfo()
//    {
//        ItemData curData = MainGameDataSo.Instance.ItemDatas[_curHexaElement.CurItemid];
//        _curItemReport.Init(curData.ItemSprite, curData.ItemName);
//        _curItemReport.SetColor(curData.ItemColor);
//    }
//    public void DestroySelectHexa()
//    {
//        HidePanel();
//        HexaGridManager.Instance.DestoryHexaGrid(_curHexaElement.gameObject);
//    }
//}
