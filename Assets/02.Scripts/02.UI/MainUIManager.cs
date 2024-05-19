using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [field: Header("InfoPanel")]
    [field: SerializeField] public HexaInfoPanel HexaInfoPanel { get; private set; }
    [field: SerializeField] public HexaTransitInfoPanel HexaTransitInfoPanel { get; private set; }

    private void Start()
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

        MainGameManager.Instance.UIUpdateAction += ItemCountUpdate;
    }

    private void ItemCountUpdate()
    {
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
        {
            priceElement.PriceUpdata();
        }
    }

    public void SetLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void ShowReportAnimation(bool Show)
    {
        ItemCountUpdate();
        StopAllCoroutines();
        StartCoroutine(ReportAnimation(Show));
    }

    private IEnumerator ReportAnimation(bool show = true)
    {
        RectTransform target = transform.GetChild(0).GetComponent<RectTransform>();

        transform.GetChild(0).GetChild(0).gameObject.SetActive(!show);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(show);

        float startpos = show ? 0f : 500f;
        float endPos = show ? 500f : 0;

        float animationTimer = 0;
        float animationMaxTime = .5f;

        //float c4 = (2 * Mathf.PI) / 3f;
        float x = 0;
        float t = 0;
        while (animationTimer < animationMaxTime)
        {
            animationTimer += Time.deltaTime;
            x = animationTimer / animationMaxTime;
            //t = Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
            t = 1 - Mathf.Pow(1 - x, 5);
            target.anchoredPosition = Vector2.up * Mathf.Lerp(startpos, endPos, t);
            yield return null;
        }
        target.anchoredPosition = Vector2.up * endPos;


    }
}
