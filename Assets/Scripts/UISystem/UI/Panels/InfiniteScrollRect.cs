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
         private int _rowCount = 1;
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
             _scrollType = scrollType;
             if (_scrollType == ScrollType.Vertical)
             {
                 _columnCount = Mathf.Max(1, columnCount);
             }
             else
             {
                 // Horizontal 模式使用独立行数配置，避免与 Vertical 列数语义耦合。
                 _rowCount = Mathf.Max(1, columnCount);
             }
             _isInitialized = false;
             _startRowIndex = 0;
             _startColumnIndex = 0;
             _lastScrollPosition = 0f;
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
                 // Horizontal 模式下使用固定行数 _rowCount 计算可见数量。
                 int rowCount = Mathf.Max(1, _rowCount);
                 int columnCountVisible = Mathf.CeilToInt(_viewportWidth / (_itemWidth + _spaceHorizontal)) + 2;
                 _visibleItemCount = rowCount * columnCountVisible;
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
            float threshold = _scrollType == ScrollType.Vertical ? _itemHeight * 0.5f : _itemWidth * 0.5f;

            if (Mathf.Abs(currentPosition - _lastScrollPosition) > threshold)
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
                int rowCount = Mathf.Max(1, _rowCount);
                int totalColumns = Mathf.CeilToInt((float)_itemCount / rowCount);
                float contentWidth = totalColumns * _itemWidth + Mathf.Max(0, totalColumns - 1) * _spaceHorizontal + _paddingLeft;
                _content.sizeDelta = new Vector2(contentWidth, _content.sizeDelta.y);
            }
        }

        private void UpdateVisibleItems()
        {
            if (_itemCount <= 0)
            {
                return;
            }
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
                       int dataIndex = (rowIndex * _columnCount + columnIndex);
                       if (dataIndex < 0 || dataIndex >= _itemCount)
                       {
                           continue;
                       }
                       UpdateItem(i, rowIndex, columnIndex, dataIndex);
                  }
                  _isInitialized = true;
              }
          }

          private void UpdateHorizontalScroll(float scrollPosition)
          {
              int newStartColumn = Mathf.FloorToInt(scrollPosition / (_itemWidth + _spaceHorizontal)) - 1;
              int rowCount = Mathf.Max(1, _rowCount);

              if (!_isInitialized || newStartColumn != _startColumnIndex)
              {
                  _startColumnIndex = newStartColumn;

                  for (int i = 0; i < _visibleItemCount; i++)
                  {
                      // Horizontal 模式按“列优先”映射数据：同一列自上而下填满后再到下一列。
                      int rowIndex = i % rowCount;
                      int columnIndex = _startColumnIndex + (i / rowCount);
                      int dataIndex = columnIndex * rowCount + rowIndex;
                       if (dataIndex < 0 || dataIndex >= _itemCount)
                       {
                          continue;
                       }
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
                      float topOffset = GetGridTopOffset();
                      Vector2 anchoredPosition = new Vector2(
                          leftOffset + columnIndex * (_itemWidth + _spaceHorizontal) + _itemWidth / 2f,
                          -topOffset - rowIndex * (_itemHeight + _spaceVertical) - _itemHeight / 2f);
                      rectTransform.anchoredPosition = anchoredPosition;
                      Log.LogInfo($"anchoredPosition:{anchoredPosition} cellIndex:{cellIndex} rowIndex:{rowIndex} columnIndex:{columnIndex} dataIndex:{dataIndex}");
                  }
                  _itemPool.UpdateItemData(cellIndex, dataIndex);
             } 
         }

         private float GetGridLeftOffset()
         {
             // Vertical: 横向居中 + paddingLeft下限; Horizontal: 固定使用paddingLeft。
             if (_scrollType == ScrollType.Horizontal)
             {
                 return _paddingLeft;
             }
             if (_viewport == null)
             {
                 return _paddingLeft;
             }
             int columnCountForLayout = GetLayoutColumnCount();
             float gridWidth = columnCountForLayout * _itemWidth + Mathf.Max(0, columnCountForLayout - 1) * _spaceHorizontal;
             float centeredOffset = (_viewport.rect.width - gridWidth) * 0.5f;
             return Mathf.Max(_paddingLeft, centeredOffset);
         }

         private float GetGridTopOffset()
         {
             // Vertical: 固定使用paddingTop; Horizontal: 纵向居中 + paddingTop下限。
             if (_scrollType == ScrollType.Vertical)
             {
                 return _paddingTop;
             }
             if (_viewport == null)
             {
                 return _paddingTop;
             }
             int rowCountForLayout = GetLayoutRowCount();
             float gridHeight = rowCountForLayout * _itemHeight + Mathf.Max(0, rowCountForLayout - 1) * _spaceVertical;
             float centeredOffset = (_viewport.rect.height - gridHeight) * 0.5f;
             return Mathf.Max(_paddingTop, centeredOffset);
         }

         private int GetLayoutColumnCount()
         {
             if (_scrollType == ScrollType.Vertical)
             {
                 return Mathf.Max(1, _columnCount);
             }
             int rowCount = Mathf.Max(1, _rowCount);
             return Mathf.CeilToInt((float)_itemCount / rowCount);
         }

         private int GetLayoutRowCount()
         {
             if (_scrollType == ScrollType.Vertical)
             {
                 int columnCount = Mathf.Max(1, _columnCount);
                 return Mathf.CeilToInt((float)_itemCount / columnCount);
             }
             return Mathf.Max(1, _rowCount);
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
                return _startColumnIndex * _rowCount;
            }
        }
    }

    public enum ScrollType
    {
        Vertical,
        Horizontal
    }
}
