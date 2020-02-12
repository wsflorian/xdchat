namespace ConsoleGui
{
    public class TextField : Element
    {
        public string Text { get; set; }
        public bool Wrap { get; set; }
        
        public TextField()
            :base(null)
        { }

        public TextField(Element parent)
            :base(parent)
        { }
        
        public override void Render()
        {
            throw new System.NotImplementedException();
        }

        public override void OnFocus()
        {
            throw new System.NotImplementedException();
        }

        public override void OnBlur()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHover()
        {
            throw new System.NotImplementedException();
        }

        public override void OnScroll()
        {
            throw new System.NotImplementedException();
        }
    }
}