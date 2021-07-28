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

        [SerializeField]
        private RectTransform _mask;

        [SerializeField]
        private RectTransform _rendererItemPrefab;

        [SerializeField]
        private int _columns = 2;

        private Vector2 _itemSize;

        [SerializeField]
        private float _spacing = 5.0f;

        private List<InfiniteListItem> _items = new List<InfiniteListItem>();

        private Vector2 _maskSize;

        private Vector2 _dragMin = Vector2.zero;
        private Vector2 _dragMax = Vector2.zero;
        private Vector2 _dragPosition;
        private Vector2 _renderMin = Vector2.zero;
        private Vector2 _renderMax = Vector2.zero;

        private float _maxHeight;

        private List<RectTransform> _renderItems = new List<RectTransform>();
        private Queue<RectTransform> _rendererItemCachePool;

        private Action<List<InfiniteListItem>> _onRenderItems;

        protected override void Start()
        {
            base.Start();

            Setup(60, () =>
            {
                StartCoroutine(TestItems(9999));
            }, (items) =>
            {
                foreach (var item in items)
                {
                    item.RenderItem.transform.Find("Text").GetComponent<Text>().text = item.Id.ToString("d5");
                }
            });

        }


        IEnumerator TestItems(int num)
        {
            for (int i = 0; i < num; i++)
            {
                yield return null;
                AddItem(1, UnityEngine.Random.Range(1.3f, 1.7f) * _itemSize.y);
            }
            //yield return new WaitForSeconds(5.0f);
            //for (int i = 0; i < 18; i++)
            //{
            //    yield return new WaitForSeconds(1.0f);
            //    AddItem(-1);
            //}
        }

        public void Setup(int renderMax, Action cacheInstantiateComplete, Action<List<InfiniteListItem>> onRenderLists)
        {
            _maskSize = _mask.rect.size;
            _itemSize = _rendererItemPrefab.rect.size;
            _itemSize.y = _itemSize.x = (_maskSize.x - _spacing * (_columns + 1)) / _columns;
            //_spacing = (_maskSize.x - _itemSize.x * _itemColumns) / (_itemColumns + 1);
            Debug.Log($"InfiniteListView mask size: {_maskSize}. item prefab size: {_itemSize}. spacing: {_spacing}");
            _renderMin = new Vector2(0 - _itemSize.x, -_maskSize.y - _itemSize.y);
            _renderMax = new Vector2(_maskSize.x + _itemSize.x, 0 + _itemSize.y);
            _onRenderItems = onRenderLists;
            StartCoroutine(InstantiateRenderCache(renderMax, cacheInstantiateComplete));
        }

        public InfiniteListView AddItem(int num = 1, float height = 0.0f)
        {
            if (num == 0)
                return this;

            if (num > 0)
            {
                Vector2 startPos = _itemSize * 0.5f;
                if (_items.Count > 0)
                {
                    startPos = _items[0].Size * 0.5f;
                }
                startPos += Vector2.one * _spacing;

                int idIndex = _items.Count;
                for (int i = 0; i < num; i++)
                {
                    int id = idIndex + i;
                    InfiniteListItem item = new InfiniteListItem() { Id = id };
                    int arrayIndex = id % _columns;
                    int groupIndex = id / _columns;
                    item.Size = new Vector2(_itemSize.x, height > 0.0f ? height : _itemSize.y);
                    int lastId = id - _columns;
                    if (lastId >= 0)
                    {
                        var lastItem = _items[lastId];
                        startPos.y = lastItem.Position.y - lastItem.Size.y * 0.5f - _spacing - item.Size.y * 0.5f;
                    }
                    else
                    {
                        startPos.y = -item.Size.y * 0.5f - _spacing;
                    }
                    item.Position = new Vector2(startPos.x + arrayIndex * (_itemSize.x + _spacing), startPos.y);
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

            _maxHeight = 0.0f;
            _dragMax = Vector2.zero;
            if (_items.Count > 0)
            {
                for (int i = _items.Count - 1; i >= _items.Count - _columns; i--)
                {
                    if (i >= 0)
                    {
                        var lastItem = _items[i];
                        float maxHeight = -lastItem.Position.y + lastItem.Size.y * 0.5f + _spacing;
                        if (maxHeight > _maxHeight)
                        {
                            _maxHeight = maxHeight;
                        }
                    }
                }

                _dragMax.y = _maxHeight - _maskSize.y;
            }

            RebuildItemRenderer();
            return this;
        }

        private void RebuildItemRenderer()
        {
            ClampDragPosition(10.0f);

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

            foreach (var item in _renderItems)
            {
                item.gameObject.SetActive(false);
            }

            int renderIndex = -1;

            List<InfiniteListItem> renderItems = new List<InfiniteListItem>();

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                Vector2 renderPostion = _dragPosition + item.Position;

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
                        item.RenderItem.localPosition = renderPostion;
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
        }

        private IEnumerator InstantiateRenderCache(int renderMax, Action cacheInstantiateComplete)
        {
            if (_rendererItemCachePool == null || _rendererItemCachePool.Count == 0)
            {
                _rendererItemCachePool = new Queue<RectTransform>();
                for (int i = 0; i < renderMax; i++)
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
            throw new System.NotImplementedException();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragPosition.y += eventData.delta.y;
            RebuildItemRenderer();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();

            ClampDragPosition();
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
            throw new System.NotImplementedException();
        }

        #endregion


        private void ClampDragPosition(float floatDrag = 0.0f)
        {
            if (_maxHeight > _maskSize.y)
            {
                _dragPosition.x = Mathf.Clamp(_dragPosition.x, _dragMin.x - floatDrag, _dragMax.x + floatDrag);
                _dragPosition.y = Mathf.Clamp(_dragPosition.y, _dragMin.y - floatDrag, _dragMax.y + floatDrag);
            }
            else
            {
                _dragPosition = Vector2.zero;
            }
            Debug.Log($"_dragPosition: {_dragPosition} {_dragMin} {_dragMax}");
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