using System;

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
            var pos = this.GetCursorOffset();
            var stringToWrite = $"{Prompt}{Value}";
            stringToWrite = stringToWrite.Remove(0, (pos.X < 0 ? pos.X : 0));

            if (stringToWrite.Length > Size.Width)
            {
                stringToWrite = stringToWrite.Remove(Size.Width - 1);
            }
            else
            {
                for (int i = 0; i < Size.Width - stringToWrite.Length; i++)
                {
                    stringToWrite += " ";
                }
            }
            Console.SetCursorPosition((pos.X < 0 ? 0 : pos.X), (pos.Y < 0 ? 0 : pos.Y));
            Console.Write(stringToWrite);
        }

        public override void OnFocus()
        {
            Console.CursorVisible = true;
            this.Render();
            ConsoleKeyInfo newChar = Console.ReadKey();

            while (this.IsFocused && newChar.Key != ConsoleKey.Enter)
            {
                newChar = Console.ReadKey();
            }

            if (newChar.Key == ConsoleKey.Enter)
            {
                // send msg to server
            }
        }

        public override void OnBlur()
        {
            throw new NotImplementedException();
        }

        public override void OnHover(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void OnScroll()
        {
            throw new NotImplementedException();
        }

        public void OnSubmit()
        {
            throw new NotImplementedException();
        }
    }
}