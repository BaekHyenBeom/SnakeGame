

namespace Snake
{
    class Key       // 개 쓰
    {
        public static int currentMove = 2;
        public static void GetKey()
        {
            ConsoleKeyInfo input;

            while (true)
            {
                input = Console.ReadKey(true);
                SwitchKey(input);
                break;
            }
        }
        static void SwitchKey(ConsoleKeyInfo input)
        {
            if (input.Key == ConsoleKey.LeftArrow && currentMove != 1)
            {
                Console.Write("왼쪽");
                currentMove = 0;
            }
            if (input.Key == ConsoleKey.RightArrow && currentMove != 0)
            {
                Console.Write("오른쪽");
                currentMove = 1;
            }
            if (input.Key == ConsoleKey.UpArrow && currentMove != 3)
            {
                Console.Write("위");
                currentMove = 2;
            }
            if (input.Key == ConsoleKey.DownArrow && currentMove != 2)
            {
                Console.Write("아래");
                currentMove = 3;
            }
        }
    }
}