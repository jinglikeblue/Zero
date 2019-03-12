using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基于UGUI ScrollView组件的高性能列表组件。对性能有高要求的列表组件可以考虑使用该组件
/// </summary>
public class ZList : MonoBehaviour
{
    public enum ELayoutType
    {
        /// <summary>
        /// 垂直列表
        /// </summary>
        VERTICAL,
        /// <summary>
        /// 横向列表
        /// </summary>
        HORIZONTAL,        
    }

    /// <summary>
    /// 布局方式
    /// </summary>
    public ELayoutType layoutType = ELayoutType.VERTICAL;

    /// <summary>
    /// 列表项的间隔
    /// </summary>
    public float itemGap = 0;

    /// <summary>
    /// 滚动组件
    /// </summary>
    ScrollRect _scrollRect;

    /// <summary>
    /// 内存组件
    /// </summary>
    RectTransform _content;

    /// <summary>
    /// 列表项的Prefab
    /// </summary>
    GameObject _itemPrefab;

    /// <summary>
    /// 列表项的大小
    /// </summary>
    Vector2 _itemSize;

    /// <summary>
    /// 生成的列表项示例列表
    /// </summary>
    List<GameObject> _items = new List<GameObject>();    

    /// <summary>
    /// 列表视口
    /// </summary>
    Rect _viewport;

    /// <summary>
    /// 视口可见的最大列表项数量
    /// </summary>
    int _itemMaxCount;

    /// <summary>
    /// 列表的数据
    /// </summary>
    object[] _datas;

    /// <summary>
    /// 列表项更新的回调
    /// </summary>
    Action<int, object, GameObject> _onItemUpdate;

    /// <summary>
    /// 列表项占用的空间
    /// </summary>
    float _itemSpace = 0;

    /// <summary>
    /// item的缓存池
    /// </summary>
    Queue<GameObject> _itemPool = new Queue<GameObject>();

    /// <summary>
    /// 正在显示的列表项
    /// </summary>
    Dictionary<int, GameObject> _showedDic = new Dictionary<int, GameObject>();

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        //_scrollRect.viewport;
        if (null == _scrollRect)
        {
            throw new Exception("can't find component ScrollRect");
        }
        //var vp = transform.Find("Viewport");
        _viewport = _scrollRect.GetComponent<RectTransform>().rect;

        _content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        _itemPrefab = _content.GetChild(0).gameObject;
        var rt = _itemPrefab.GetComponent<RectTransform>();
        _itemSize = rt.sizeDelta;
        var topLeft = new Vector2(0, 1);
        rt.pivot = topLeft;
        rt.anchorMin = topLeft;
        rt.anchorMax = topLeft;
        _itemPrefab.SetActive(false);
        SetItemGap(itemGap);
    }

    public void SetItemGap(float gap)
    {
        itemGap = gap;
        SetData(_datas, _onItemUpdate);
    }

    private void OnEnable()
    {
        _scrollRect.onValueChanged.AddListener(OnScroll);
    }

    public void SetData(object[] data, Action<int, object, GameObject> onItemUpdate)
    {
        Clear();

        switch (layoutType)
        {
            case ELayoutType.VERTICAL:
                _itemSpace = _itemSize.y + itemGap;
                _itemMaxCount = Mathf.CeilToInt(_viewport.height / _itemSpace) + 1;
                break;
            case ELayoutType.HORIZONTAL:
                _itemSpace = _itemSize.x + itemGap;
                _itemMaxCount = Mathf.CeilToInt(_viewport.width / _itemSpace) + 1;
                break;
        }      

        _datas = data;
        _onItemUpdate = onItemUpdate;

        if(null == _datas || _datas.Length == 0)
        {
            return;
        }

        int count = _datas.Length;
        SetContentSize((_itemSpace * count) - itemGap);


        RefreshUI();
    }

    /// <summary>
    /// 显示指定索引位置的列表项
    /// </summary>
    /// <param name="idx"></param>
    public void ShowItem(int idx)
    {
        if(null == _datas)
        {
            return;
        }
        var pos = _itemSpace * idx;
        SetContentPos(pos);
    }
    
    void SetContentSize(float size)
    {
        var contentSize = _content.sizeDelta;

        switch (layoutType)
        {
            case ELayoutType.VERTICAL:
                contentSize.y = size;
                break;
            case ELayoutType.HORIZONTAL:
                contentSize.x = size;
                break;
        }
        
        _content.sizeDelta = contentSize;
    }

    void SetContentPos(float pos)
    {
        var contentPos = _content.localPosition;

        switch (layoutType)
        {
            case ELayoutType.VERTICAL:
                contentPos.y = pos;
                break;
            case ELayoutType.HORIZONTAL:
                contentPos.x = -1 * pos;
                break;
        }
        
        _content.localPosition = contentPos;
    }

    public void Clear()
    {
        for(int i = 0; i < _items.Count; i++)
        {
            GameObject.Destroy(_items[i]);
        }
        _showedDic.Clear();
        _items.Clear();
        _itemPool.Clear();
        _datas = null;
        _scrollRect.velocity = Vector2.one;

        SetContentPos(0);

        switch (layoutType)
        {
            case ELayoutType.VERTICAL:
                SetContentSize(_viewport.height);
                break;
            case ELayoutType.HORIZONTAL:
                SetContentSize(_viewport.width);
                break;
        }
    }

    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScroll);
    }

    private void OnScroll(Vector2 v)
    {        
        RefreshUI();
    }    

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void RefreshUI()
    {       
        if(null == _datas)
        {
            return;
        }

        float localPos;
        if(layoutType == ELayoutType.VERTICAL)
        {
            localPos = _content.localPosition.y;
        }
        else
        {
            localPos = -1 * _content.localPosition.x ;
        }        

        //显示开始的索引
        int startIdx = Mathf.FloorToInt(localPos / _itemSpace);
        if(startIdx < 0)
        {
            startIdx = 0;
        }

        //Item显示开始的位置      
        Vector2 pos = new Vector2(0f, startIdx * _itemSpace);

        int endIdx = startIdx + _itemMaxCount;
        if (endIdx > _datas.Length)
        {
            endIdx = _datas.Length;
        }

        HashSet<int> updateNeedlessSet = new HashSet<int>();      
        Dictionary<int, GameObject> showedDic = new Dictionary<int, GameObject>();

        //找出可以不用更新的Item
        foreach (var entry in _showedDic)
        {
            if(entry.Key >= startIdx && entry.Key < endIdx)
            {
                //不用更新的
                updateNeedlessSet.Add(entry.Key);
                showedDic.Add(entry.Key, entry.Value);
            }
            else
            {
                //加入缓存池
                _itemPool.Enqueue(entry.Value);
            }
        }

        _showedDic = showedDic;

        for (int i = startIdx; i < endIdx; i++)
        {
            if(updateNeedlessSet.Contains(i))
            {
                //已经显示的可以忽略不处理
                continue;
            }

            if(layoutType == ELayoutType.VERTICAL)
            {
                pos.x = 0f;
                pos.y = -1 * i * _itemSpace;
            }
            else
            {
                pos.x = i * _itemSpace;
                pos.y = 0f;                
            }

            CreateItem(i, pos, _datas[i], _itemPool);            
        }                    
    }

 

    public GameObject CreateItem(int idx, Vector2 pos, object data, Queue<GameObject> itemPool)
    {        
        GameObject go = null;
        if(itemPool.Count > 0)
        {
            go = itemPool.Dequeue();
        }
        else
        {
            go = GameObject.Instantiate(_itemPrefab, _content);
            go.name = "item" + idx;
            _items.Add(go);
        }

        _showedDic.Add(idx, go);
        go.transform.localPosition = pos;
        go.SetActive(true);   
        if(null != _onItemUpdate)
        {
            _onItemUpdate.Invoke(idx, data, go);
        }        
        return go;
    }
}
