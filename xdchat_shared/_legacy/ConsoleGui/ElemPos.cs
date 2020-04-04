namespace ConsoleGui
{
    public class ElemPos
    {
        private int _y;
        private int _x;
        
        public int Y {
            get => _y; 
            set => _y = value;
        }

        public int X
        {
            get => _x; 
            set => _x = 2*value;
        }

        public ElemPos()
            :this(0,0)
        { }

        public ElemPos(int y, int x)
        {
            Y = y;
            X = x;
        }

        public override string ToString()
        {
            return $"x:{this.X} y:{this.Y}";
        }

        public static ElemPos operator +(ElemPos a)
            => a;
        
        public static ElemPos operator + (ElemPos a, ElemPos b)
            => new ElemPos(a.Y + b.Y, a.X/2 + b.X/2);
        
        public static ElemPos operator + (ElemPos a, ElemSize b)
            => new ElemPos(a.Y + b.Height, a.X/2 + b.Width/2);

        // public static ElemPos operator +(ElemPos a, ElemPos b)
        // {
        //     return new ElemPos(a.Y + b.Y, a.X + b.X);
        // }
    }
}