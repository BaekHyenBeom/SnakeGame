

namespace Snake
{
    internal class Program
    {
        // 프레임 관련 로직 이곳에서 빌려왔습니다. -> (https://velog.io/@yarogono/C-%EC%BD%98%EC%86%94-%EC%8A%A4%EB%84%A4%EC%9D%B4%ED%81%AC-%EA%B2%8C%EC%9E%84-%EA%B5%AC%ED%98%84)
        private int _sumTick = 0;
        private const int WAIT_TICK = 1250 / 10;
        private const int MOVE_TICK = 1250;     // 이 쪽을 1000단위를 건드려주면 속도를 조정해줄 수 있다.

        void Board(string[,] board, bool isGameOver, int score)
        {
            Console.Clear();
            Console.WriteLine();
            Console.Write("".PadRight(10));

            Paint(isGameOver);
            for (int i = 0; i < board.GetLength(1) + 4; i++)  // 벽짓기
            {
                Console.Write("■");
            }
            ResetColor();

            Console.WriteLine();
            for (int i = 0; i < board.GetLength(0); i++)    // 공간 구현
            {
                Console.Write("".PadRight(10));
                Paint(isGameOver); Console.Write("■"); Console.Write("■"); ResetColor();
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == "@")
                    {
                        Paint(isGameOver, 1); Console.Write("@");
                    }

                    else if (board[i, j] != null)
                    {
                        Paint(isGameOver, "");  Console.Write(board[i, j]);
                    }
                    else
                    {
                        Console.Write("·");
                    }
                    Console.ResetColor();
                }
                Paint(isGameOver); Console.Write("■"); Console.Write("■"); ResetColor();
                Console.WriteLine();
            }


            Console.Write("".PadRight(10));
            Paint(isGameOver);
            for (int i = 0; i < board.GetLength(1) + 4; i++)  // 벽짓기
            {
                Console.Write("■");
            }
            ResetColor();
            Console.WriteLine();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"현재 점수 : {score}".PadLeft(14 + board.GetLength(0))); 
        }

        public void Paint(bool isGameOver)
        {
            if (isGameOver) { Red(); } else { White(); }
        }
        public void Paint(bool isGameOver, int red)
        {
            if (isGameOver) { Red(false); } else { Red(1); }
        }
        public void Paint(bool isGameOver, string yellow)
        {
            if (isGameOver) { Red(false); } else { Console.ForegroundColor = ConsoleColor.Green; }
        }

        public void White()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Red()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Red;
        }
        public void Red(int i)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        public void Red(bool noBackground)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        public void ResetColor()
        { Console.ResetColor(); }


        class SnakeBody()
        {
            public string[] body { get; private set; }   
            private int top;
            public int size { get; private set; }
            public int score { get; private set; }

            public SnakeBody(int boardSize) : this()
            {
                body = new string[boardSize * boardSize * 2];

                // 뱀 모양
                body[0] = "▲";
                body[1] = "●";

                // 초기 뱀 크기
                size = 1;
            }

            public void HeadChange(int i)
            {
                switch (i)
                {
                    case 0:
                        body[0] = "◀";
                        break;
                    case 1:
                        body[0] = "▶";
                        break;
                    case 2:
                        body[0] = "▲";
                        break;
                    case 3:
                        body[0] = "▼";
                        break;
                }
            }

            public void Eat()
            {
                size++;
                score++;
            }
        }

        class MoveSnake
        {
            // 뱀의 현재 좌표
            public int x { get; private set; }
            public int y { get; private set; }
            public int[] moved { get; private set; }
            public List<int> movedHistory { get; private set; }

            public MoveSnake(int a, int b)
            {
                x = a;
                y = b;
                moved = [0, 0, 0, 0];   // moved[0] = 왼쪽 | [1] = 오른쪽 | [2] = 위쪽 | [3] = 아래쪽
                movedHistory = new List<int>();
            }

            public void Move(int a, SnakeBody body, string[,] board, ref bool gameOver, Program pg)
            {
                if (body.size - 1 < movedHistory.Count())
                {
                    board[(x + (moved[2] - moved[3])), (y + (moved[0] - moved[1]))] = null;
                    moved[movedHistory[0]] -= 1;
                    movedHistory.RemoveAt(0);
                }
                switch (a)
                {
                    case 0:
                        y -= 1;
                        moved[0] += 1;
                        movedHistory.Add(0);
                        board[x, y + 1] = body.body[1];
                        body.HeadChange(0);
                        break;
                    case 1:
                        y += 1;
                        moved[1] += 1;
                        movedHistory.Add(1);
                        board[x, y - 1] = body.body[1];
                        body.HeadChange(1);
                        break;
                    case 2:
                        x -= 1;
                        moved[2] += 1;
                        movedHistory.Add(2);
                        board[x + 1, y] = body.body[1];
                        body.HeadChange(2);
                        break;
                    case 3:
                        x += 1;
                        moved[3] += 1;
                        movedHistory.Add(3);
                        board[x - 1, y] = body.body[1];
                        body.HeadChange(3);
                        break;
                    default:
                        break;
                }

                    try
                {
                    if (board[x, y] == "●" && a != -1) { gameOver = true; Thread.Sleep(1000); }      // 공간에 몸이 있다면
                    if (board[x, y] == "@") { board[x, y] = null; body.Eat(); pg.MakeApple(board); }    // 사과를 먹었다면 로직 (게임오버 판정보다 사과를 먼저 생성한다면 버그가 생긴다.)
                    board[x, y] = body.body[0];
                }
                catch
                {
                    gameOver = true;
                    Thread.Sleep(1000);
                    return;
                }

                

            }
        }

        // 사과 생성 메서드
        public void MakeApple(string[,] board)
        {
            Random rand = new Random();
            int x = 0; int y = x;

            while (true)
            {
                x = rand.Next(0, board.GetLength(0));
                y = rand.Next(0, board.GetLength(1));
                if (board[x, y] == null)    // 혹시나 사과가 이상한 곳에 생성되기라도 한다면 다시 굴리기로 했다.
                {
                    board[x, y] = "@";
                    break;
                }
            }
            

        }

        // 이동키 감지를 위한 클래스
        class Key
        {
            int pastKey = 2;

            public void MoveKey(ref int currentMove)
            {
                if (Console.KeyAvailable)   // 키를 입력했을 때에만 리드키 감지
                {
                    ConsoleKeyInfo consoleKey = Console.ReadKey(true);

                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            if (pastKey == 1) { break; }
                            currentMove = 0;
                            pastKey = 0;
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            if (pastKey == 0) { break; }
                            currentMove = 1;
                            pastKey = 1;
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            if (pastKey == 3) { break; }
                            currentMove = 2;
                            pastKey = 2;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            if (pastKey == 2) { break; }
                            currentMove = 3;
                            pastKey = 3;
                            break;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            int boardSize = 10; // 보드 크기 정하는 곳. 크기가 24를 초과 시 콘솔 출력에 문제가 생깁니다.
            string[,] board = new String[boardSize, boardSize * 2];   // 실질적인 공간

            
            // 게임 실행을 위한 클래스 가져오기
            Program pg = new Program();
            SnakeBody snake = new SnakeBody(boardSize);
            MoveSnake move = new MoveSnake(board.GetLength(0) - 3, board.GetLength(1) / 2);     // 초기 좌표
            Key moveKey = new Key();

            // 초기 좌표에 뱀 머리 넣기
            board[move.x, move.y] = snake.body[0];

            // 세팅에 필요한 변수 선언
            bool isGameOver = false;
            int stack = 0;
            int currentMove = -1;

            // 프레임 관련 로직
            int lastTick = 0;

            // 보드 출력
            pg.Board(board, isGameOver, snake.score);

            // 사과 만들기
            pg.MakeApple(board);

            Console.CursorVisible = false;  // 그 입력바 같은 거 뜨는 거 막는 코드 

            


            while (isGameOver == false)
            {

                // 다른 곳에서 빌려온 프레임 관리법
                int currentTick = System.Environment.TickCount;

                if (currentTick - lastTick < WAIT_TICK)
                    continue;

                int deltaTick = currentTick - lastTick;
                lastTick = currentTick;

                pg._sumTick += deltaTick;
                if (pg._sumTick >= MOVE_TICK)
                {
                    // 게임 관련 로직
                    moveKey.MoveKey(ref currentMove);
                    move.Move(currentMove, snake, board, ref isGameOver, pg);
                    pg.Board(board, isGameOver, snake.score);    //공간을 플레이어에게 출력
                }
               
            }
            pg.Board(board, isGameOver, snake.score); // 게임 오버 이후의 보드 출력
            Console.Beep();
            Console.WriteLine("게임 오버!".PadLeft(14 + board.GetLength(0)));    
            Console.Write("엔터를 누르시면 나가실 수 있습니다.".PadLeft(14));
            Console.ReadLine();
        }
    }
}

