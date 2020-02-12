namespace ConsoleGui
{
    public class TextInput : Element
    {
        public string Prompt { get; set; }
        public string Value { get; set; }

        public TextInput()
            :base(null)
        { }

        public TextInput(Element parent)
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