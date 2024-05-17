using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class HexaTransitInfoPanel : MonoBehaviour
{
    private HexaGridTransit _curHexaElement;

    [SerializeField] private LocalizeStringEvent _hexaName;
    [SerializeField] private TMP_Dropdown _recipeDropdown;
    [SerializeField] private Transform _inputArrow;
    [SerializeField] private Transform _outputArrow;
    [SerializeField] private Transform _inputMainArrow;
    [SerializeField] private Transform _outputMainArrow;

    private byte inputVec = 0;
    private byte outputVec = 3;

    public void ShowPanel(HexaGridTransit curElenemt)
    {

        transform.GetComponentInParent<Canvas>().enabled = true;

        transform.position = Camera.main.WorldToScreenPoint(curElenemt.transform.position);

        _curHexaElement = curElenemt;

        _hexaName.StringReference.SetReference("Hexa", _curHexaElement.Data.name);

        _recipeDropdown.ClearOptions();

        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }

        List<string> list = new List<string>();
        for (int i = 0; i < _curHexaElement.Data.ProduceRecipe.Count; i++)
        {
            ProduceRecipe recipe = _curHexaElement.Data.ProduceRecipe[i];
            list.Add(recipe.RecipeName);
        }

        if (list.Count > 0)
        {
            _recipeDropdown.AddOptions(list);
            _recipeDropdown.value = list.FindIndex(x => x == _curHexaElement.CurRecipe.RecipeName);
            UpdateRecipe();
        }
        inputVec = curElenemt.InputVec;
        outputVec = curElenemt.OutputVec;
        SetInputArrow();
        SetOutputArrow();
    }

    public void HidePanel()
    {
        transform.GetComponentInParent<Canvas>().enabled = false;
    }

    public void UpdateInputVec(bool cw)
    {
        if (cw)
        {
            inputVec++;
            if (inputVec > 5)
                inputVec = 0;
        }
        else
        {
            inputVec--;
            if (inputVec > 250)
                inputVec = 5;
        }
        _curHexaElement.SetVec(inputVec, outputVec);
        SetInputArrow();
    }

    private void SetInputArrow()
    {
        _inputArrow.eulerAngles = Vector3.back * (inputVec * 60 + 180);
        _inputMainArrow.eulerAngles = Vector3.back * (inputVec * 60);
    }

    public void UpdateOutputVec(bool cw)
    {
        if (cw)
        {
            outputVec++;
            if (outputVec > 5)
                outputVec = 0;
        }
        else
        {
            outputVec--;
            if (outputVec > 250)
                outputVec = 5;
        }
        _curHexaElement.SetVec(inputVec, outputVec);
        SetOutputArrow();
    }

    private void SetOutputArrow()
    {
        _outputArrow.eulerAngles = Vector3.back * (outputVec * 60 + 180);
        _outputMainArrow.eulerAngles = Vector3.back * (outputVec * 60 + 180);
    }

    public void UpdateRecipe(int index = -1)
    {
        if (index >= 0)
        {
            _curHexaElement.SetReciepe(index);
        }
    }
    public void DestroySelectHexa()
    {
        HidePanel();
        HexaGridManager.Instance.DestoryHexaGrid(_curHexaElement.gameObject);
    }
}
