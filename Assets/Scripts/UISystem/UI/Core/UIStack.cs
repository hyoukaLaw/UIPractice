using System.Collections.Generic;
using UIModule.Data;

namespace UIModule.Core
{
    public class UIStack
    {
        private readonly Stack<UIPanelData> _stack = new Stack<UIPanelData>();

        public void Push(UIPanelData panel)
        {
            _stack.Push(panel);
        }

        public UIPanelData Pop()
        {
            if (_stack.Count == 0)
            {
                return null;
            }
            return _stack.Pop();
        }

        public UIPanelData Peek()
        {
            if (_stack.Count == 0)
            {
                return null;
            }
            return _stack.Peek();
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public int Count => _stack.Count;
    }
}
