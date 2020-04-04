using System;
using System.Text;

namespace XdChatShared
{
    public class ConsoleExtend
    {
        public static string ReadLinePrefill(string prefix, string defaultValue)
        {
            Console.CursorVisible = true;
            Console.Write($"{prefix}{defaultValue}");
            
            string returnValue = defaultValue;
            ConsoleKeyInfo newChar = Console.ReadKey();
            int currentStringPos = defaultValue.Length;

            while (newChar.Key != ConsoleKey.Enter)
            {
                switch (newChar.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (currentStringPos > 0)
                        {
                            if (currentStringPos < returnValue.Length)
                            {
                                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                Console.Write(returnValue[currentStringPos]);
                            }

                            Console.SetCursorPosition(Console.CursorLeft - 2, Console.CursorTop);
                            currentStringPos--;
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(returnValue[currentStringPos]);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (currentStringPos < returnValue.Length)
                        {
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(returnValue[currentStringPos]);
                            currentStringPos++;
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }

                        break;
                    case ConsoleKey.Backspace:
                        if (currentStringPos > 0)
                        {
                            currentStringPos--;
                            returnValue = returnValue.Remove(currentStringPos, 1);
                            ReDrawLine(prefix.Length, returnValue, currentStringPos);
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.CursorLeft + 1,Console.CursorTop);
                        }

                        break;
                    default:
                        if (newChar.KeyChar >= 0x20)
                        {
                            if (currentStringPos < returnValue.Length)
                            {
                                StringBuilder sb = new StringBuilder(returnValue);
                                sb.Insert(currentStringPos, newChar.KeyChar);
                                returnValue = sb.ToString();
                                
                                ReDrawLine(prefix.Length, returnValue, currentStringPos);
                            }
                            else
                            {
                                returnValue += newChar.KeyChar;
                            }

                            currentStringPos++;
                        }
                        else
                        {
                            ReDrawLine(prefix.Length, returnValue, currentStringPos);
                        }

                        break;
                }
                
                newChar = Console.ReadKey();
            }

            Console.CursorVisible = false;
            return returnValue;
        }

        private static void ReDrawLine(int prefixLength, string returnValue, int currentStringPos)
        {
            Console.SetCursorPosition(prefixLength, Console.CursorTop);
            Console.Write($"{returnValue} ");
            Console.SetCursorPosition(prefixLength + currentStringPos, Console.CursorTop);
        }
    }
}