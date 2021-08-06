using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class InfiniteListView : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup, ILayoutController
    {
        enum Layout
        {
            Horizontal,
            Vertical,
        }

        [SerializeField]
        private InfiniteListView.Layout _renderLayout=Layout.Vertical;

        [SerializeField]
        private RectTransform _mask;

        [SerializeField]
        private RectTransform _rendererItemPrefab;

        [SerializeField]
        private int _splitCount = 2;

        private Vector2 _itemSize;

        [SerializeField]
        private float _spacing = 5.0f;

        private List<InfiniteListItem> _items = new List<InfiniteListItem>();

        public List<InfiniteListItem> Items => _items;

        private Vector2 _maskSize;

        private Vector2 _dragMin = Vector2.zero;
        private Vector2 _dragMax = Vector2.zero;
        private Vector2 _dragPosition;
        private Vector2 _renderMin = Vector2.zero;
        private Vector2 _renderMax = Vector2.zero;

        private float _maxLength;
        public float MaxLength => _maxLength;

        private List<RectTransform> _renderItems = new List<RectTransform>();
        private Queue<RectTransform> _rendererItemCachePool;

        private Action<List<InfiniteListItem>> _onRenderItems;
        private Action _onRenderHeader;
        private Action _onRenderTail;

        protected override void Start()
        {
            base.Start();

            //Setup(60, () =>
            //{
            //    StartCoroutine(TestItems(50));
            //}, (items) =>
            //{
            //    foreach (var item in items)
            //    {
            //        item.RenderItem.transform.Find("Text").GetComponent<Text>().text = item.Id.ToString("d5");
            //    }
            //});

        }


        //IEnumerator TestItems(int num)
        //{
        //    for (int i = 0; i < num; i++)
        //    {
        //        yield return null;
        //        AddItem(1,_itemSize.x* UnityEngine.Random.Range(1.0f,1.75f));
        //    }
        //}

        public void Setup(int renderMax, Action cacheInstantiateComplete, Action<List<InfiniteListItem>> onRenderLists,Action onRenderHeader=null,Action onRenderTail=null)
        {
            _maskSize = _mask.rect.size;
            //_itemSize = _rendererItemPrefab.rect.size;
            float splitLengthSize = _renderLayout == Layout.Vertical ? _maskSize.x : _maskSize.y;
            _itemSize.y = _itemSize.x = (splitLengthSize - _spacing * (_splitCount + 1)) / _splitCount;
            //_spacing = (_maskSize.x - _itemSize.x * _itemColumns) / (_itemColumns + 1);
            _renderMin = new Vector2(-_maskSize.x, -_maskSize.y);
            _renderMax = new Vector2(_maskSize.x , _maskSize.y);
            Debug.Log($"InfiniteListView mask size: {_maskSize}. item prefab size: {_itemSize}. spacing: {_spacing} renderMin: {_renderMin} renderMax: {_renderMax}");
            _onRenderItems = onRenderLists;
            _onRenderHeader = onRenderHeader;
            _onRenderTail = onRenderTail;
            StartCoroutine(InstantiateRenderCache(renderMax, cacheInstantiateComplete));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="extLength">正为绝对值，负为比例</param>
        /// <returns></returns>
        public InfiniteListView AddItem(int num = 1, float extLength = 0.0f)
        {
            if (num == 0)
                return this;

            if (extLength < 0)
            {
                extLength = Mathf.Abs(extLength) * _itemSize.x;
            }

            if (num > 0)
            {
               
                int idIndex = _items.Count;
                for (int i = 0; i < num; i++)
                {
                    int id = idIndex + i;
                    InfiniteListItem item = new InfiniteListItem() { Id = id };
                    int arrayIndex = id % _splitCount;
                    //int groupIndex = id / _splitCount;
                    if (_renderLayout == Layout.Vertical)
                    {
                        item.Size = new Vector2(_itemSize.x, extLength > 0.0f ? extLength : _itemSize.y);
                    }
                    else 
                    {
                        item.Size = new Vector2(extLength > 0.0f ? extLength : _itemSize.x, _itemSize.y);
                    }

                    Vector2 nextPosition;
                    int lastId = id - _splitCount;
                    if (lastId >= 0)
                    {
                        var lastItem = _items[lastId];
                        nextPosition.x = _renderLayout == Layout.Vertical ? lastItem.Position.x : lastItem.Position.x + lastItem.Size.x * 0.5f + _spacing + item.Size.x * 0.5f;
                        nextPosition.y = _renderLayout == Layout.Vertical ? lastItem.Position.y - lastItem.Size.y * 0.5f - _spacing - item.Size.y * 0.5f: lastItem.Position.y;
                    }
                    else
                    {
                        //Vector2 startPos = _itemSize * 0.5f;
                        //startPos += Vector2.one * _spacing;
                        //nextPosition.x = _renderLayout == Layout.Vertical ? startPos.x + arrayIndex * (_itemSize.x + _spacing) : -item.Size.x * 0.5f - _spacing;
                        //nextPosition.y = _renderLayout == Layout.Vertical ? -item.Size.y * 0.5f - _spacing : startPos.y + arrayIndex * (_itemSize.y + _spacing);
                        Vector2 startPosition = new Vector2(-_maskSize.x * 0.5f, _maskSize.y * 0.5f);
                        startPosition.x += _spacing + item.Size.x*0.5f;
                        startPosition.y -= _spacing + item.Size.y * 0.5f;

                        nextPosition.x= _renderLayout == Layout.Vertical ?0 + arrayIndex * (_itemSize.x + _spacing) :0;
                        nextPosition.y = _renderLayout == Layout.Vertical ? 0 : 0 + -arrayIndex * (_itemSize.y + _spacing);

                        nextPosition += startPosition;

                    }
                    //Debug.Log($"id: {id} position: {nextPosition}");
                    //new Vector2(startPos.x + arrayIndex * (_itemSize.x + _spacing), startPos.y)

                    item.Position = nextPosition;

                    _items.Add(item);
                }
            }
            else if (num < 0)
            {
                num = Mathf.Abs(num);
                for (int i = 0; i < num; i++)
                {
                    if (_items.Count > 0)
                    {
                        _items.RemoveAt(_items.Count - 1);
                    }
                }
            }

            _maxLength = 0.0f;
            _dragMin = Vector2.zero;
            _dragMax = Vector2.zero;
            if (_items.Count > 0)
            {
                float startPos = _renderLayout == Layout.Vertical ? _items[0].Position.y : _items[0].Position.x;
                float startSize = _renderLayout == Layout.Vertical ? _items[0].Size.y * 0.5f + _spacing : _items[0].Size.x * 0.5f + _spacing;

                for (int i = _items.Count - 1; i >= _items.Count - _splitCount; i--)
                {
                    if (i >= 0)
                    {
                        var lastItem = _items[i];
                        float maxLength = _renderLayout == Layout.Vertical?
                            (-(lastItem.Position.y- startPos) + lastItem.Size.y*0.5f + _spacing+ startSize) : 
                            ((lastItem.Position.x- startPos) + lastItem.Size.x*0.5f + _spacing+ startSize);
                        if (maxLength > _maxLength)
                        {
                            _maxLength = maxLength;
                        }
                    }
                }

                if (_renderLayout == Layout.Vertical)
                {
                    _dragMax.y = _maxLength - _maskSize.y;
                }
                else
                {
                    _dragMin.x = -(_maxLength-_maskSize.x);
                }
            }

            RebuildItemRenderer();
            return this;
        }

        public void ClearItem()
        {
            AddItem(-_items.Count);
        }

        public void SortItem()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                item.Id = i;
                _items[i] = item;
            }
        }

        private void RebuildItemRenderer(float dragFloat=0.0f)
        {
            foreach (var item in _renderItems)
            {
                item.gameObject.SetActive(false);
            }

            if (_items == null || _items.Count == 0)
                return;

            ClampDragPosition(dragFloat);

            if (_renderItems.Count > _items.Count)
            {
                for (int i = _items.Count; i < _renderItems.Count; i++)
                {
                    RectTransform temp = _renderItems[i];
                    temp.gameObject.SetActive(false);
                    _rendererItemCachePool.Enqueue(temp);
                    _renderItems.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                for (int i = _renderItems.Count; i < _items.Count; i++)
                {
                    if (_rendererItemCachePool.Count > 0)
                    {
                        RectTransform temp = _rendererItemCachePool.Dequeue();
                        temp.gameObject.SetActive(true);
                        _renderItems.Add(temp);
                    }
                    else
                    {
                        break;
                    }
                }
            }

  

            int renderIndex = -1;

            List<InfiniteListItem> renderItems = new List<InfiniteListItem>();

            //Vector2 startPosition = new Vector2(-_maskSize.x*0.5f, _maskSize.y*0.5f);
            //startPosition.x += _spacing+_items[0].Size.x*0.5f;
            //startPosition.y -= (_spacing + _items[0].Size.y * 0.5f);

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                Vector2 renderPostion =  _dragPosition + item.Position;
                //Debug.Log($"id:{item.Id} _dragPosition: {_dragPosition} item.Position: {item.Position} renderPostion�� {renderPostion}");
                if (OutRenderBounds(renderPostion))
                {
                    item.RenderItem = null;
                }
                else
                {
                    renderIndex++;
                    if (renderIndex < _renderItems.Count)
                    {
                        item.RenderItem = _renderItems[renderIndex];
                        item.RenderItem.anchoredPosition = renderPostion;
                        item.RenderItem.sizeDelta = item.Size;
                        item.RenderItem.gameObject.SetActive(true);
                        renderItems.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _onRenderItems?.Invoke(renderItems);
            if (renderItems.Contains(_items[0]))
            {
                _onRenderHeader?.Invoke();
            }
            if (renderItems.Contains(_items[_items.Count-1]))
            {
                _onRenderTail?.Invoke();
            }
        }

        private IEnumerator InstantiateRenderCache(int renderMax, Action cacheInstantiateComplete)
        {
            int hasCount = (_renderItems == null ? 0 : _renderItems.Count) + (_rendererItemCachePool == null ? 0 : _rendererItemCachePool.Count);
            if (renderMax > hasCount)
            {
                if (_rendererItemCachePool == null)
                    _rendererItemCachePool = new Queue<RectTransform>();

                for (int i = hasCount; i < renderMax; i++)
                {
                    GameObject clone = GameObject.Instantiate(_rendererItemPrefab.gameObject);
                    clone.transform.SetParent(_mask.transform);
                    clone.transform.localScale = Vector3.one;
                    clone.SetActive(false);
                    _rendererItemCachePool.Enqueue(clone.GetComponent<RectTransform>());
                    yield return null;
                }
            }

            cacheInstantiateComplete?.Invoke();
        }


        #region Interface
        public float minWidth => throw new System.NotImplementedException();

        public float preferredWidth => throw new System.NotImplementedException();

        public float flexibleWidth => throw new System.NotImplementedException();

        public float minHeight => throw new System.NotImplementedException();

        public float preferredHeight => throw new System.NotImplementedException();

        public float flexibleHeight => throw new System.NotImplementedException();

        public int layoutPriority => throw new System.NotImplementedException();


        #endregion

        #region Interface

        public void SetLayoutHorizontal()
        {
            //throw new System.NotImplementedException();
        }

        public void SetLayoutVertical()
        {
            //throw new System.NotImplementedException();
        }

        public void CalculateLayoutInputHorizontal()
        {
            //throw new System.NotImplementedException();
        }

        public void CalculateLayoutInputVertical()
        {
            //throw new System.NotImplementedException();
        }

        public void GraphicUpdateComplete()
        {
            //throw new System.NotImplementedException();
        }

        public void LayoutComplete()
        {
            //throw new System.NotImplementedException();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_renderLayout == Layout.Vertical)
            {
                _dragPosition.y += eventData.delta.y;
            }
            else
            {
                _dragPosition.x += eventData.delta.x;
            }
            RebuildItemRenderer(10.0f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            RebuildItemRenderer();
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public void OnScroll(PointerEventData eventData)
        {
        }

        public void Rebuild(CanvasUpdate executing)
        {
            //RebuildItemRenderer();
        }

        public void Rebuild()
        {
            RebuildItemRenderer();   
        }

        #endregion

        private void ClampDragPosition(float floatDrag = 0.0f)
        {
            //Debug.Log($"_maxLength: {_maxLength} _maskSize.y:{_maskSize.y} _maskSize.x:{_maskSize.x}");
            bool canDrag = _renderLayout == Layout.Vertical ? _maxLength > _maskSize.y : _maxLength > _maskSize.x;
            if (canDrag)
            {
                _dragPosition.x = Mathf.Clamp(_dragPosition.x, _dragMin.x - floatDrag, _dragMax.x + floatDrag);
                _dragPosition.y = Mathf.Clamp(_dragPosition.y, _dragMin.y - floatDrag, _dragMax.y + floatDrag);
            }
            else
            {
                _dragPosition = Vector2.zero;
            }
            //Debug.Log($"_dragPosition: {_dragPosition} {_dragMin} {_dragMax}");
        }

        private bool OutRenderBounds(Vector2 renderPosition)
        {
            if (renderPosition.x < _renderMin.x || renderPosition.x > _renderMax.x)
                return true;

            if (renderPosition.y < _renderMin.y || renderPosition.y > _renderMax.y)
                return true;

            return false;
        }
    }



    public struct InfiniteListItem
    {
        public int Id;
        public bool InRenderer
        {
            get
            {
                return RenderItem != null;
            }
        }
        public Vector2 Size;
        public Vector2 Position;
        public RectTransform RenderItem;
    }
}