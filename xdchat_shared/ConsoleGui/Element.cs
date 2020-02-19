using System;
using System.ComponentModel.Design;

namespace ConsoleGui
{
    public abstract class Element
    {
        public Element Parent { get; set; }
            = null;
        public ElemSize Size { get; set; }
            = new ElemSize(2, 2);

        public ElemPos Position { get; set; }
            = new ElemPos(0,0);
        
        public string Id { get; set; }

        public Element(Element parent)
        {
            this.Parent = parent;
        }

        private bool _focus;
        public bool IsFocused
        {
            get => _focus;
            set
            {
                if (value)
                {
                    if (_focus == false)
                    {
                        var parent = this.Parent;
                        while (parent.Parent != null)
                        {
                            parent = parent.Parent;
                        }
                        SetElemFocus(parent);
                        _focus = true;
                        OnFocus();
                    }
                }
                else
                {
                    _focus = false;
                }
            }
        }

        private void SetElemFocus(Element elem)
        {
            elem.IsFocused = false;
            
            if (elem.GetType() == typeof(Box))
            {
                foreach (var child in ((Box)elem).Children)
                {
                    SetElemFocus(child);
                }
            }
        }

        public ElemPos GetAbsolutePos()
            => (Parent != null ? Position + Parent.GetAbsolutePos() : Position);

        public ElemPos GetCursorOffset()
            => (Parent != null
                ? Parent.GetAbsolutePos() + new ElemPos(
                      this.Position.Y,
                      this.Position.X < 0 ? 0 : this.Position.X/2)
                : new ElemPos(
                    this.Position.Y,
                    this.Position.X < 0 ? 0 : this.Position.X/2));

        public bool IsPointInside(int x, int y)
        {
            ElemPos innerBounds = this.GetAbsolutePos();
            ElemPos outerBounds = innerBounds + Size;
            
            return (x >= innerBounds.X && x < outerBounds.X && y >= innerBounds.Y && y < outerBounds.Y);
        }
        public abstract void Render();
        public abstract void OnFocus();
        public abstract void OnBlur();
        public abstract void OnHover(int x, int y);
        public abstract void OnScroll();

        public static bool IsPointInConsole(int x, int y)
            => (x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight);
    }
}