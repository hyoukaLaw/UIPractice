using UIModule.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class InfiniteScrollRect : MonoBehaviour
    {
         [SerializeField]
         private ScrollRect _scrollRect;
         [SerializeField]
         private RectTransform _content;
         [SerializeField]
         private RectTransform _viewport;
         [SerializeField]
         private BagItemPool _itemPool;
         [SerializeField]
         private int _itemCount;
         [SerializeField]
         private float _itemHeight = 100f;
         [SerializeField]
         private float _itemWidth = 100f;
         [SerializeField]
         private int _columnCount = 1;
         [SerializeField] 
         private float _paddingTop = 50f;
         [SerializeField] 
         private float _paddingLeft = 50f;
         [SerializeField] 
         private float _spaceHorizontal = 10f;
         [SerializeField] 
         private float _spaceVertical = 10f;
 
         private ScrollType _scrollType = ScrollType.Vertical;

        private int _visibleItemCount;
        private float _viewportHeight;
        private float _viewportWidth;

         private int _startRowIndex = 0;
         private int _startColumnIndex = 0;
         private bool _isInitialized = false;

         private float _lastScrollPosition;

        private void Awake()
        {
            if (_scrollRect == null)
            {
                _scrollRect = GetComponent<ScrollRect>();
            }
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
            if (_content == null && _scrollRect != null)
            {
                _content = _scrollRect.content;
            }
            if (_viewport == null && _scrollRect != null)
            {
                _viewport = _scrollRect.viewport;
            }
        }

          private void Update()
          {
              if (!_isInitialized)
              {
                  _lastScrollPosition = GetScrollPosition();
                  CalculateVisibleItemCount();
                  UpdateContentHeight();
                  InitializePool();
                  UpdateVisibleItems();
              }
          }

        private void OnDestroy()
        {
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            }
        }

         public void Initialize(int itemCount, float itemHeight, float itemWidth, int columnCount, ScrollType scrollType)
         {
             _itemCount = itemCount;
             _itemHeight = itemHeight;
             _itemWidth = itemWidth;
             _columnCount = columnCount;
             _scrollType = scrollType;
         }
 
         private void InitializePool()
         {
             if (_itemPool != null)
             {
                 _itemPool.InitializePool(_visibleItemCount);
             }
         }

        private void CalculateVisibleItemCount()
        {
            if (_viewport == null)
            {
                return;
            }
            _viewportHeight = _viewport.rect.height;
            _viewportWidth = _viewport.rect.width;
            Log.LogInfo($"CalculateVisibleItemCount _viewportHeight:{_viewportHeight}, _viewportWidth:{_viewportWidth}");
            if (_scrollType == ScrollType.Vertical)
            {
                int rowCount = Mathf.CeilToInt(_viewportHeight / (_itemHeight+_spaceVertical) ) + 2;
                _visibleItemCount = rowCount * _columnCount;
            }
            else
            {
                int columnCountVisible = Mathf.CeilToInt(_viewportWidth / _itemWidth) + 2;
                int rowCount = Mathf.CeilToInt(_itemCount / (float)_columnCount);
                _visibleItemCount = columnCountVisible * rowCount;
            }
        }

        private float GetScrollPosition()
        {
            if (_content == null)
            {
                return 0f;
            }

            if (_scrollType == ScrollType.Vertical)
            {
                return _content.anchoredPosition.y;
            }
            else
            {
                return -_content.anchoredPosition.x;
            }
        }

        private void OnScrollValueChanged(Vector2 position)
        {
            if (_viewport != null)
            {
                Log.LogInfo($"OnScrollValueChanged viewportWidth:{_viewport.rect.width}");
            }
            float currentPosition = GetScrollPosition();

            if (Mathf.Abs(currentPosition - _lastScrollPosition) > _itemHeight / 2f)
            {
                _lastScrollPosition = currentPosition;
                UpdateVisibleItems();
            }
        }

        private void UpdateContentHeight()
        {
            if (_content == null)
            {
                return;
            }
            if (_scrollType == ScrollType.Vertical)
            {
                int totalRows = Mathf.CeilToInt((float)_itemCount / _columnCount);
                float contentHeight = totalRows * (_itemHeight + _spaceVertical) + _paddingTop;
                _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentHeight);
            }
            else
            {
                int totalColumns = Mathf.CeilToInt((float)_itemCount / _columnCount);
                float contentWidth = totalColumns * _itemWidth;
                _content.sizeDelta = new Vector2(contentWidth, _content.sizeDelta.y);
            }
        }

        private void UpdateVisibleItems()
        {
            float scrollPosition = GetScrollPosition();

            if (_scrollType == ScrollType.Vertical)
            {
                UpdateVerticalScroll(scrollPosition);
            }
            else
            {
                UpdateHorizontalScroll(scrollPosition);
            }
        }

          private void UpdateVerticalScroll(float scrollPosition)
          {
              int newStartRow = Mathf.FloorToInt(scrollPosition / (_itemHeight + _spaceVertical)) - 1;
              if (!_isInitialized || newStartRow != _startRowIndex)
              {
                  _startRowIndex = newStartRow;
                  for (int i = 0; i < _visibleItemCount; i++)
                  {
                      int rowIndex = _startRowIndex + (i / _columnCount);
                      int columnIndex = i % _columnCount;
                      int dataIndex = (rowIndex * _columnCount + columnIndex) % _itemCount;
                      if ((rowIndex * _columnCount + columnIndex) < 0 ||
                          (rowIndex * _columnCount + columnIndex) >= _itemCount)
                          continue;
                      UpdateItem(i, rowIndex, columnIndex, dataIndex);
                  }
                  _isInitialized = true;
              }
          }

          private void UpdateHorizontalScroll(float scrollPosition)
          {
              int newStartColumn = Mathf.FloorToInt(scrollPosition / _itemWidth);

              if (!_isInitialized || newStartColumn != _startColumnIndex)
              {
                  _startColumnIndex = newStartColumn;

                  for (int i = 0; i < _visibleItemCount; i++)
                  {
                      int columnIndex = _startColumnIndex + (i % _columnCount);
                      int rowIndex = i / _columnCount;
                      int dataIndex = (rowIndex * _columnCount + columnIndex) % _itemCount;
                      UpdateItem(i, rowIndex, columnIndex, dataIndex);
                  }
                  _isInitialized = true;
              }
          }

         private void UpdateItem(int cellIndex, int rowIndex, int columnIndex, int dataIndex)
         {
             if (_itemPool == null)
             {
                 return;
             }
             MonoBagListItem item = _itemPool.GetOrCreate(cellIndex);
             if (item != null)
             {
                 RectTransform rectTransform = item.GetComponent<RectTransform>();
                 if (rectTransform != null)
                 {
                     float leftOffset = GetGridLeftOffset();
                     Vector2 anchoredPosition = new Vector2(
                         leftOffset + columnIndex * (_itemWidth + _spaceHorizontal) + _itemWidth / 2f,
                         -rowIndex * _itemHeight - _itemHeight/2f - _paddingTop - rowIndex * _spaceVertical);
                     rectTransform.anchoredPosition = anchoredPosition;
                     Log.LogInfo($"anchoredPosition:{anchoredPosition} cellIndex:{cellIndex} rowIndex:{rowIndex} columnIndex:{columnIndex} dataIndex:{dataIndex}");
                 }
                 _itemPool.UpdateItemData(cellIndex, dataIndex);
             }
         }

         private float GetGridLeftOffset()
         {
             if (_viewport == null)
             {
                 return _paddingLeft;
             }
             float gridWidth = _columnCount * _itemWidth + (_columnCount - 1) * _spaceHorizontal;
             float centeredOffset = (_viewport.rect.width - gridWidth) * 0.5f;
             return Mathf.Max(_paddingLeft, centeredOffset);
         }

        public int GetVisibleItemCount()
        {
            return _visibleItemCount;
        }

        public int GetStartIndex()
        {
            if (_scrollType == ScrollType.Vertical)
            {
                return _startRowIndex * _columnCount;
            }
            else
            {
                return _startColumnIndex * _columnCount;
            }
        }
    }

    public enum ScrollType
    {
        Vertical,
        Horizontal
    }
}
