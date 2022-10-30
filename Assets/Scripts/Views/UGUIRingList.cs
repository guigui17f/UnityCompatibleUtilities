using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GUIGUI17F
{
    public class UGUIRingList : MonoBehaviour
    {
        public struct RingListPosition
        {
            public int DataIndex;
            public Vector2? ContentPosition;
        }

        public ScrollRect ScrollView;
        public RectTransform ContentRect;
        public GridLayoutGroup LayoutGroup;
        public List<CanvasRenderer> ItemList;

        private bool _itemListInitialized;
        private bool _scrollViewInitialized;
        private bool _updateRequired;
        private int _itemCount;
        private int _lastItemIndex;
        private int _totalDataCount;
        private int _firstDataIndex;
        private int _lastDataIndex;
        private int _focusDataIndex;
        private float _listLength;
        private Vector2 _contentOriginPosition;
        private Vector2[] _itemOriginPositions;
        private CanvasRenderer[] _itemOriginOrders;
        private Dictionary<CanvasRenderer, RectTransform> _itemRectDictionary;
        private Vector2? _contentStartPosition;
        private Action<int, CanvasRenderer> _refreshCallback;
        private bool _enableRingList;

        /// <summary>
        /// initialize ring list items
        /// </summary>
        /// <param name="item">list item template</param>
        /// <param name="count">list item ui elements total count</param>
        /// <param name="useOrigin">whether use the template as the first list item</param>
        public void InitializeItemList(GameObject item, int count, bool useOrigin)
        {
            if (_itemListInitialized)
            {
                return;
            }

            LayoutGroup.enabled = false;
            ItemList = new List<CanvasRenderer>();
            _itemRectDictionary = new Dictionary<CanvasRenderer, RectTransform>();
            int i = 0;
            if (useOrigin)
            {
                RectTransform originRect = item.GetComponent<RectTransform>();
                CanvasRenderer originCanvasRenderer = item.GetComponent<CanvasRenderer>();
                originRect.SetParent(LayoutGroup.transform, false);
                originRect.anchoredPosition = Vector2.zero;
                ItemList.Add(originCanvasRenderer);
                _itemRectDictionary.Add(originCanvasRenderer, originRect);
                item.SetActive(true);
                i = 1;
            }
            for (; i < count; i++)
            {
                GameObject go = Instantiate(item, LayoutGroup.transform);
                RectTransform rect = go.GetComponent<RectTransform>();
                CanvasRenderer canvasRenderer = go.GetComponent<CanvasRenderer>();
                if (ScrollView.vertical)
                {
                    rect.anchoredPosition = new Vector2(0, -i);
                }
                else
                {
                    rect.anchoredPosition = new Vector2(i, 0);
                }
                ItemList.Add(canvasRenderer);
                _itemRectDictionary.Add(canvasRenderer, rect);
                go.SetActive(true);
            }

            _itemCount = ItemList.Count;
            if (ScrollView.vertical)
            {
                _listLength = _itemCount * (LayoutGroup.cellSize.y + LayoutGroup.spacing.y);
            }
            else
            {
                _listLength = _itemCount * (LayoutGroup.cellSize.x + LayoutGroup.spacing.x);
            }
            _lastItemIndex = _itemCount - 1;
            _itemOriginOrders = new CanvasRenderer[_itemCount];
            for (i = 0; i < _itemOriginOrders.Length; i++)
            {
                _itemOriginOrders[i] = ItemList[i];
            }
            ItemList.ForEach(listItem => listItem.GetComponent<MaskableGraphic>().onCullStateChanged.AddListener(OnItemCullStateChanged));
            _itemListInitialized = true;

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(InitializeScrollView());
            }
        }

        /// <summary>
        /// setup ring list data
        /// </summary>
        /// <param name="dataCount">total count of the list data</param>
        /// <param name="refreshCallback">item data refresh callback，parameter0: data index, parameter1: item renderer</param>
        /// <param name="startIndex">ring list item data start index</param>
        /// <param name="contentPosition">ring list scrollView start position</param>
        public void SetupRingList(int dataCount, Action<int, CanvasRenderer> refreshCallback, int startIndex = 0, Vector2? contentPosition = null)
        {
            _totalDataCount = dataCount;
            _refreshCallback = refreshCallback;
            _firstDataIndex = Mathf.Min(startIndex, _totalDataCount - _itemCount);
            _firstDataIndex = Mathf.Max(0, _firstDataIndex);
            _lastDataIndex = Mathf.Min(_firstDataIndex + _itemCount - 1, _totalDataCount - 1);
            _focusDataIndex = Mathf.Clamp(startIndex, 0, _totalDataCount - 1);
            if (_scrollViewInitialized)
            {
                UpdateCurrentList(contentPosition);
            }
            else
            {
                _contentStartPosition = contentPosition;
                _updateRequired = true;
            }
        }

        /// <summary>
        /// get current list position data
        /// </summary>
        /// <returns>position data, used for recover ring list to a certain position</returns>
        public RingListPosition GetCurrentPosition()
        {
            return new RingListPosition
            {
                DataIndex = _firstDataIndex,
                ContentPosition = ContentRect.anchoredPosition
            };
        }

        /// <summary>
        /// set the ring list to a certain position
        /// </summary>
        /// <param name="startIndex">ring list item data start index</param>
        /// <param name="contentPosition">ring list scrollView start position</param>
        public void SetListPosition(int startIndex, Vector2? contentPosition = null)
        {
            _firstDataIndex = Mathf.Min(startIndex, _totalDataCount - _itemCount);
            _firstDataIndex = Mathf.Max(0, _firstDataIndex);
            _lastDataIndex = Mathf.Min(_firstDataIndex + _itemCount - 1, _totalDataCount - 1);
            _focusDataIndex = Mathf.Clamp(startIndex, 0, _totalDataCount - 1);
            if (_scrollViewInitialized)
            {
                UpdateCurrentList(contentPosition);
            }
            else
            {
                _contentStartPosition = contentPosition;
                _updateRequired = true;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(InitializeScrollView());
        }

        private IEnumerator InitializeScrollView()
        {
            if (!_itemListInitialized || _scrollViewInitialized)
            {
                yield break;
            }

            LayoutGroup.enabled = true;
            if (ItemList.Count > 1)
            {
                RectTransform rect0 = _itemRectDictionary[ItemList[0]];
                RectTransform rect1 = _itemRectDictionary[ItemList[1]];
                if (ScrollView.vertical)
                {
                    while (Mathf.Abs(rect0.anchoredPosition.y - rect1.anchoredPosition.y) < LayoutGroup.cellSize.y)
                    {
                        Debug.Log("RingList: waiting for gridlayout initialize");
                        yield return null;
                    }
                }
                else
                {
                    while (Mathf.Abs(rect0.anchoredPosition.x - rect1.anchoredPosition.x) < LayoutGroup.cellSize.x)
                    {
                        Debug.Log("RingList: waiting for gridlayout initialize");
                        yield return null;
                    }
                }
            }
            _contentOriginPosition = ContentRect.anchoredPosition;
            _itemOriginPositions = new Vector2[_itemCount];
            for (int i = 0; i < _itemOriginPositions.Length; i++)
            {
                _itemOriginPositions[i] = _itemRectDictionary[ItemList[i]].anchoredPosition;
            }
            _scrollViewInitialized = true;

            if (_updateRequired)
            {
                UpdateCurrentList(_contentStartPosition);
            }
        }

        private void UpdateCurrentList(Vector2? contentPosition)
        {
            _updateRequired = false;
            LayoutGroup.enabled = false;
            ScrollView.StopMovement();
            _enableRingList = false;
            for (int i = 0; i < _itemOriginOrders.Length; i++)
            {
                ItemList[i] = _itemOriginOrders[i];
            }
            Vector2 contentDeltaPosition;
            Vector2 itemDeltaPosition;
            float contentLength;
            if (ScrollView.vertical)
            {
                contentLength = _totalDataCount * LayoutGroup.cellSize.y + (_totalDataCount - 1) * LayoutGroup.spacing.y + LayoutGroup.padding.vertical;
                float moveLength = (LayoutGroup.cellSize.y + LayoutGroup.spacing.y) * _focusDataIndex;
                if (moveLength > contentLength - ScrollView.viewport.sizeDelta.y)
                {
                    moveLength = contentLength - ScrollView.viewport.sizeDelta.y;
                }
                contentDeltaPosition = new Vector3(0, -moveLength);
                itemDeltaPosition = new Vector3(0, (LayoutGroup.cellSize.y + LayoutGroup.spacing.y) * -_firstDataIndex);
            }
            else
            {
                contentLength = _totalDataCount * LayoutGroup.cellSize.x + (_totalDataCount - 1) * LayoutGroup.spacing.x;
                float moveLength = (LayoutGroup.cellSize.x + LayoutGroup.spacing.x) * _focusDataIndex;
                if (moveLength > contentLength - ScrollView.viewport.sizeDelta.x)
                {
                    moveLength = contentLength - ScrollView.viewport.sizeDelta.x;
                }
                contentDeltaPosition = new Vector3(moveLength, 0);
                itemDeltaPosition = new Vector3((LayoutGroup.cellSize.x + LayoutGroup.spacing.x) * _firstDataIndex, 0);
            }
            ContentRect.anchoredPosition = contentPosition ?? _contentOriginPosition - contentDeltaPosition;

            for (int i = 0; i < _itemOriginPositions.Length; i++)
            {
                RectTransform rect = _itemRectDictionary[ItemList[i]];
                rect.anchoredPosition = _itemOriginPositions[i] + itemDeltaPosition;
            }
            if (ScrollView.vertical)
            {
                ContentRect.sizeDelta = new Vector2(LayoutGroup.cellSize.x, contentLength);
            }
            else
            {
                ContentRect.sizeDelta = new Vector2(contentLength, LayoutGroup.cellSize.y + LayoutGroup.padding.horizontal);
            }

            if (_refreshCallback != null)
            {
                for (int i = 0; i < _itemCount; i++)
                {
                    if (_firstDataIndex + i < _totalDataCount)
                    {
                        _refreshCallback(_firstDataIndex + i, ItemList[i]);
                        ItemList[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        ItemList[i].gameObject.SetActive(false);
                    }
                }
            }
            _enableRingList = _totalDataCount > _itemCount;
        }

        private void OnItemCullStateChanged(bool cull)
        {
            if (!_enableRingList || _refreshCallback == null)
            {
                return;
            }

            if (!ItemList[0].cull && ItemList[_lastItemIndex].cull)
            {
                if (_firstDataIndex <= 0)
                {
                    return;
                }
                //put the last item to the top of this list
                CanvasRenderer tempItem = ItemList[_lastItemIndex];
                _firstDataIndex -= 1;
                _lastDataIndex -= 1;
                _refreshCallback(_firstDataIndex, tempItem);
                RectTransform rectTransform = _itemRectDictionary[tempItem];
                Vector2 originPosition = rectTransform.anchoredPosition;
                if (ScrollView.vertical)
                {
                    rectTransform.anchoredPosition = new Vector2(originPosition.x, originPosition.y + _listLength);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(originPosition.x - _listLength, originPosition.y);
                }
                ItemList.RemoveAt(_lastItemIndex);
                ItemList.Insert(0, tempItem);
            }
            else if (!ItemList[_lastItemIndex].cull && ItemList[0].cull)
            {
                if (_lastDataIndex >= _totalDataCount - 1)
                {
                    return;
                }
                //put the first item to the bottom of this list
                CanvasRenderer tempItem = ItemList[0];
                _firstDataIndex += 1;
                _lastDataIndex += 1;
                _refreshCallback(_lastDataIndex, tempItem);
                RectTransform rectTransform = _itemRectDictionary[tempItem];
                Vector2 originPosition = rectTransform.anchoredPosition;
                if (ScrollView.vertical)
                {
                    rectTransform.anchoredPosition = new Vector2(originPosition.x, originPosition.y - _listLength);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(originPosition.x + _listLength, originPosition.y);
                }
                ItemList.RemoveAt(0);
                ItemList.Add(tempItem);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Format Components")]
        private void FormatComponents()
        {
            Mask[] masks = GetComponentsInChildren<Mask>(true);
            for (int i = 0; i < masks.Length; i++)
            {
                masks[i].gameObject.AddComponent<RectMask2D>();
                DestroyImmediate(masks[i]);
            }
            if (ScrollView.vertical)
            {
                ContentRect.anchorMin = new Vector2(0.5f, 1f);
                ContentRect.anchorMax = new Vector2(0.5f, 1f);
                ContentRect.pivot = new Vector2(0.5f, 1f);
            }
            else
            {
                ContentRect.anchorMin = new Vector2(0f, 0.5f);
                ContentRect.anchorMax = new Vector2(0f, 0.5f);
                ContentRect.pivot = new Vector2(0f, 0.5f);
            }
        }
#endif
    }
}