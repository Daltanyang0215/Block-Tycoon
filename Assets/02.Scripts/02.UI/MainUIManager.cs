using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] private Transform _elementParent;
    [SerializeField] private ReportElement _elementPrefeb;
    private Dictionary<ItemType,ReportElement> _reportElements;

    [field: SerializeField] public HexaInfoPanel HexaInfoPanel { get; private set; }

    private void Start()
    {
        _reportElements = new Dictionary<ItemType,ReportElement>();
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ReportElement item = Instantiate(_elementPrefeb, _elementParent);
            item.Init(MainGameDataSo.Instance.GetItemSprite(type), type.ToString());
            _reportElements.Add(type,item);
            item.gameObject.SetActive(false);
        }
        MainGameManager.Instance.UIUpdateAction += ItemCountUpdate;
    }

    private void ItemCountUpdate()
    {
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            int itemcount = MainGameManager.Instance.GetItemCount(type);
            if (itemcount == 0)
            {
                _reportElements[type].gameObject.SetActive(false);
            }
            else
            {
                _reportElements[type].gameObject.SetActive(true);
                _reportElements[type].UpdateItemCount(itemcount);
            }
        }
    }


    public void ShowReportAnimation(bool Show)
    {
        StopAllCoroutines();
        StartCoroutine(ReportAnimation(Show));
    }

    private IEnumerator ReportAnimation(bool show = true)
    {
        RectTransform target = transform.GetChild(0).GetComponent<RectTransform>();

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
