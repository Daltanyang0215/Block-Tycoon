using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MainUIManager : MonoBehaviour
{
    #region Singleton
    private static MainUIManager _instance;
    public static MainUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("MainUIManager").GetComponent<MainUIManager>();
            }
            return _instance;
        }
    }
    #endregion
    [field: Header("ItemList")]
    [SerializeField] private Transform _elementParent;
    [SerializeField] private ReportElement _elementPrefeb;
    private Dictionary<int, ReportElement> _reportElements;

    [field: Header("HexaPrice")]
    [SerializeField] private Transform _hexaElementParent;
    [SerializeField] private HexaPriceElement _hexaPricePrefab;
    private List<HexaPriceElement> _hexaPrices;

    [field: Header("HexaUpgrade")]
    [SerializeField] private Transform _hexaUpgradeParent;
    [SerializeField] private HexaUpgradeElement _hexaUpgradePrefab;
    private List<HexaUpgradeElement> _hexaUpgrades;

    [field: Header("Mission")]
    [SerializeField] private Transform _missionElementParent;
    [SerializeField] private UIMissionElement _missionElenetPrefab;
    private List<UIMissionElement> _missionElenets;

    [field: Header("InfoPanel")]
    [field: SerializeField] public HexaInfoPanel HexaInfoPanel { get; private set; }
    [field: SerializeField] public HexaTransitInfoPanel HexaTransitInfoPanel { get; private set; }

    private bool _isShow;

    public void StartInit()
    {
        _reportElements = new Dictionary<int, ReportElement>();
        foreach (ItemData data in MainGameDataSo.Instance.ItemDatas.Values)
        {
            ReportElement item = Instantiate(_elementPrefeb, _elementParent);
            item.Init(data.ItemSprite, data.ItemName);
            item.SetColor(data.ItemColor);
            _reportElements.Add(data.ItemID, item);
            item.gameObject.SetActive(false);
        }

        _hexaPrices = new List<HexaPriceElement>();
        foreach (HexaElementDataSO hexa in MainGameDataSo.Instance.HexaDatas)
        {
            if (hexa.CanBuy)
            {
                HexaPriceElement add = Instantiate(_hexaPricePrefab, _hexaElementParent);
                add.Init(hexa);
                _hexaPrices.Add(add);
            }
        }

        _hexaUpgrades = new List<HexaUpgradeElement>();
        foreach (HexaElementDataSO hexa in MainGameDataSo.Instance.HexaDatas)
        {
            if (hexa.UpgradePairs.Count == 0) continue;
            HexaUpgradeElement add = Instantiate(_hexaUpgradePrefab, _hexaUpgradeParent);
            add.Init(hexa);
            _hexaUpgrades.Add(add);
        }

        _missionElenets = new List<UIMissionElement>();
        foreach (MissionDataSO mission in MainGameDataSo.Instance.MissionDatas.Values)
        {
            if (MainGameManager.Instance.MissionComplite[mission.MissionID]) continue;
            UIMissionElement element = Instantiate(_missionElenetPrefab, _missionElementParent);
            element.Init(mission);
            _missionElenets.Add(element);
        }

        MainGameManager.Instance.UIUpdateAction += ItemCountUpdate;


    }

    private void ItemCountUpdate()
    {
        if (!_isShow) return;

        foreach (ItemData data in MainGameDataSo.Instance.ItemDatas.Values)
        {
            int itemcount = MainGameManager.Instance.GetItemCount(data.ItemID);
            if (itemcount == 0)
            {
                _reportElements[data.ItemID].gameObject.SetActive(false);
            }
            else
            {
                _reportElements[data.ItemID].gameObject.SetActive(true);
                _reportElements[data.ItemID].UpdateItemCount(itemcount);
            }
        }
        foreach (HexaPriceElement priceElement in _hexaPrices)
            priceElement.PriceUpdata();

        foreach (HexaUpgradeElement element in _hexaUpgrades)
            element.UpgradeUpdata();

        foreach (UIMissionElement missionElement in _missionElenets)
            missionElement.MissionUpdata();
    }

    public void SetLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public string GetLocalString(string table, string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, key, LocalizationSettings.SelectedLocale);
    }

    public void ShowReportAnimation(bool Show)
    {
        _isShow = Show;
        ItemCountUpdate();
        StopAllCoroutines();
        StartCoroutine(ReportAnimation(Show));
    }

    private IEnumerator ReportAnimation(bool show = true)
    {
        RectTransform target = transform.GetChild(0).GetComponent<RectTransform>();

        transform.GetChild(0).GetChild(0).gameObject.SetActive(!show);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(show);

        float startpos = show ? 50f : target.rect.size.y * target.localScale.y;
        float endPos = show ? target.rect.size.y * target.localScale.y : 50;

        float animationTimer = 0;
        float animationMaxTime = .5f;

        float x = 0;
        float t = 0;
        while (animationTimer < animationMaxTime)
        {
            animationTimer += Time.deltaTime;
            x = animationTimer / animationMaxTime;
            t = 1 - Mathf.Pow(1 - x, 5);
            target.anchoredPosition = Vector2.up * Mathf.Lerp(startpos, endPos, t) + Vector2.left * 120;
            yield return null;
        }
        target.anchoredPosition = Vector2.up * endPos + Vector2.left * 120;


    }
}
