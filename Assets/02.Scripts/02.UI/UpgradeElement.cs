using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour
{
    private int _hexaid, _pairindex;
    private HexaUpgradePair _pair;

    [SerializeField] private LocalizeStringEvent _upgradeName;
    [SerializeField] private TMP_Text _upgradePrice;
    [SerializeField] private Transform _maxUpgradeImage;

    [SerializeField] private Button _upgradeButton;

    public void Init(int hexaid, int pairindex, HexaUpgradePair pair)
    {
        _hexaid = hexaid;
        _pairindex = pairindex;
        _pair = pair;

        _upgradeName.StringReference.SetReference("Upgrade", pair.Type.ToString());
        ElementUpdate();
    }

    public void ElementUpdate()
    {
        int pairLevel = MainGameManager.Instance.Upgrades[_hexaid][_pairindex];

        if (pairLevel >= _pair.Prices.Count)
        {
            (_upgradeName.StringReference["Value"] as IntVariable).Value = (int)_pair.Prices[^1].Value;
            _maxUpgradeImage.gameObject.SetActive(true);
            return;
        }
        _maxUpgradeImage.gameObject.SetActive(false);

        _upgradePrice.text = _pair.Prices[pairLevel].Price.ToString();
        (_upgradeName.StringReference["Value"] as IntVariable).Value = (int)_pair.Prices[pairLevel].Value;

        _upgradeButton.interactable = _pair.Prices[pairLevel].Price <= MainGameManager.Instance.HasMoney;

        _upgradeButton.onClick.RemoveAllListeners();
        _upgradeButton.onClick.AddListener(() =>
        {
            MainGameManager.Instance.AddMoney(-_pair.Prices[pairLevel].Price);
            MainGameManager.Instance.Upgrades[_hexaid][_pairindex]++;
            HexaGridManager.Instance.HexaUpgradeUpdate();
            ElementUpdate();
        });
    }


}
