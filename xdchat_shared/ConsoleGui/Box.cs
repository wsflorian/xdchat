using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using XdChatShared.ConsoleMouseListener;

namespace ConsoleGui
{
    public class Box : Element
    {
        public List<Element> Children { get; private set; }
            = new List<Element>();

        public BorderType BorderType { get; set; }
            = BorderType.Classic;

        public Box()
            :base(null)
        { }

        public Box(Element parent)
            :base(parent)
        { }

        public override void Render()
        {
            switch (BorderType)
            {
                case BorderType.None:
                    RenderChildren();
                    break;
                case BorderType.Classic:
                    RenderChildren(PrintClassicBorder);
                    break;
            }
        }

        public void AddChild(Element child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public override void OnFocus()
        {
            throw new NotImplementedException();
        }

        public override void OnBlur()
        {
            throw new NotImplementedException();
        }

        public override void OnHover(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            this.Render();
            Console.ForegroundColor = ConsoleColor.Gray;
            
            var pos = this.GetAbsolutePos() + new ElemPos(1, 1);
            Console.SetCursorPosition(pos.X,pos.Y);
            Console.Write($"Hovered:{Id}");
        }

        public override void OnScroll()
        {
            throw new NotImplementedException();
        }

        private void RenderChildren(Action borderFunc = null)
        {
            borderFunc?.Invoke();

            foreach (var child in Children)
            {
                child.Render();
            }
        }

        public void SetupAsFrame()
        {
            if (Parent == null)
            {
                this.Position = new ElemPos(1,1);
                Console.SetWindowSize(this.Size.Width + 3, this.Size.Height + 2);
                Console.SetBufferSize(this.Size.Width + 3, this.Size.Height + 2);
            }
        }

        private void PrintClassicBorder()
        {
            const char tlCorner = (char)9556;
            const char trCorner = (char)9559;
            const char blCorner = (char)9562;
            const char brCorner = (char)9565;

            const char tbChar = (char)9552;
            const char lrChar = (char)9553;

            char background = ' ';

            string line;

            ElemPos offset = this.GetCursorOffset();
            ElemPos pos = this.GetAbsolutePos();
            
            for (int y = 0; y < Size.Height; y++)
            {
                line = "";
                for (int x = 0; x < Size.Width; x++)
                {
                    if (Parent == null || Parent.IsPointInside(pos.X + x, pos.Y + y))
                    {
                        line += (x == 0
                            ? (y == 0 ? tlCorner : (y == Size.Height - 1 ? blCorner : lrChar))
                            : (y == 0
                                ? (x == Size.Width - 1 ? trCorner : tbChar)
                                : (x == Size.Width - 1
                                    ? (y == Size.Height - 1 ? brCorner : lrChar)
                                    : (y == Size.Height - 1 ? tbChar : background))));
                    }
                }

                // if (offset.X >= 0 && offset.Y + y >= 0 && offset.X < Console.WindowWidth && offset.Y + y < Console.WindowWidth)
                if(IsPointInConsole(offset.X, offset.Y + y))
                {
                    Console.SetCursorPosition(offset.X, offset.Y + y);
                    Console.Write(line);
                }
            }
        }
    }
}