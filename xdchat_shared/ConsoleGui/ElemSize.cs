namespace ConsoleGui
{
    public class ElemSize
    {
        private static int MinSize = 2;

        private int _height;
        private int _width;

        public int Height
        {
            get => _height; 
            set => _height = (value < MinSize ? MinSize: value);
        }
        public int Width {
            get => _width; 
            set => _width = (2*value < MinSize ? MinSize: 2 * value); 
        }

        public ElemSize()
            :this(MinSize,MinSize)
        { }

        public ElemSize(int height, int width)
        {
            Height = height;
            Width = width;
        }
    }
}