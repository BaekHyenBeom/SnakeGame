namespace BlackJack
{
    internal class Program
    {

        public void GameManager(ref bool isRetry)
        {
            Deck deck = new Deck();
            Hand playerHand = new Hand(false);
            Hand dealerHand = new Hand(false);

            bool drawCard = true;   // 플레이어가 카드를 받을지 말지 정하는 bool

            
            
            Board();
            // 초기에 2장 뽑기
            GetCard(dealerHand);
            GetCard(dealerHand);
            GetCard(playerHand);
            GetCard(playerHand);    

            // 게임 진행 로직
            do  
            {
                if (playerHand.score < 21 && playerHand.softScore < 21 && drawCard) { drawCard = Select(); }  // 카드 선택 (이미 블랙잭일 경우나 카드를 선택하지 않았을 경우 넘어가기)
                else { drawCard = false; }  // 자동 넘김이 작동되었을 경우 비활성화 전환

                if (drawCard)  // 플레이어가 버스트 되거나 카드 채우기를 멈추지 않았을 때
                {
                    GetCard(playerHand);
                }

                if (playerHand.isBurst || playerHand.score > 20 || playerHand.softScore > 20) { drawCard = false; } // 딜러가 버스트되거나 플레이어가 블랙잭이 되거나 버스트 되었을 경우에는 카드 선택 비활성화

                if (dealerHand.score < 17 && dealerHand.softScore < 17 && !playerHand.isBurst)  // 딜러는 17점 이상이면 더 이상 카드 드로우를 하지 않음
                {
                    GetCard(dealerHand);
                }

                
                if (dealerHand.score > 16 || dealerHand.softScore > 16)  // 딜러의 핸드가 17점 이상이라면
                {
                    if (dealerHand.isBurst) { drawCard = false; } // 딜러가 버스트인 경우 카드 드로우 비활성화
                    if (dealerHand.softScore > 16) { dealerHand.SetSoftScore(); }    // 소프트스코어 판정일 경우 score 덮어쓰기
                    if (dealerHand.score < playerHand.score || dealerHand.score < playerHand.softScore) { drawCard = false; }   // 플레이어할 때 플레이어가 높을 경우 드로우 카드 자동 비활성화
                }
                
            } while (dealerHand.score < 17 && dealerHand.softScore < 17 && !playerHand.isBurst || drawCard ); // 딜러의 점수가 17점을 넘어가고, 플레이어가 카드 드로우를 비활성화하거나 (하지만 플레이어가 버스트 된다면 종료)

            // 사용 가능한 소프트스코어가 존재한다면 
            if (playerHand.softScore > playerHand.score) { playerHand.SetSoftScore(); }
            if (dealerHand.softScore > dealerHand.score) { dealerHand.SetSoftScore(); }

            MatchEnd();

            // 다시 시작할 건지 물어보는 코드
            isRetry = Retry();

            // 서로의 패와 점수 현황을 보여주는 메서드
            void Board()
            {
                Console.Clear();
                Console.Write($"현재 딜러의 핸드 : ");
                CardPrint(dealerHand.cards);
                Console.WriteLine();
                if (dealerHand.score == 21 || dealerHand.softScore == 21) { Console.Write($"딜러의 현재 점수 : "); Black(); Console.WriteLine("블랙잭!"); Console.ResetColor(); }
                else if (dealerHand.softScore > 0) { Console.Write($"딜러의 현재 점수 : {dealerHand.score} "); Console.WriteLine($"({dealerHand.softScore})"); }
                else { Console.WriteLine($"딜러의 현재 점수 : {dealerHand.score}"); }
                Console.WriteLine();
                if (dealerHand.softScore > dealerHand.score) { Console.Write($"{dealerHand.score} ({dealerHand.softScore})".PadLeft(15)); }
                else { Console.Write($"{dealerHand.score}".PadLeft(15)); }
                if (playerHand.softScore > playerHand.score) { Console.Write($" vs {playerHand.score} ({playerHand.softScore})"); }
                else { Console.Write($" vs {playerHand.score}"); }
                Console.WriteLine();
                Console.WriteLine();

                Console.Write($"현재 플레이어의 핸드 : ");
                CardPrint(playerHand.cards);
                Console.WriteLine();
                if (playerHand.score == 21 || playerHand.softScore == 21) { Console.Write($"플레이어의 현재 점수 : "); Black(); Console.WriteLine("블랙잭!"); Console.ResetColor(); }
                else if (playerHand.softScore > 0) { Console.Write($"플레이어의 현재 점수 : {playerHand.score} "); Console.WriteLine($"({playerHand.softScore})"); }
                else { Console.WriteLine($"플레이어의 현재 점수 : {playerHand.score}"); }
            }

            // 판 끝나고 호출되는 메서드
            void MatchEnd()
            {
                if (playerHand.isBurst)
                {
                    Console.WriteLine();
                    Console.WriteLine("버스트 되셨습니다!");
                }

                Console.WriteLine();
                if (dealerHand.score == 21) { Black(); Console.Write($"딜러 : 블랙잭!".PadLeft(5)); Console.ResetColor(); Console.Write(" vs "); }
                else if (dealerHand.score > 21) { { Red(false); Console.Write($"딜러: 버스트"); Console.ResetColor(); } Console.Write(" vs "); }
                else { Console.Write($"딜러 : {dealerHand.score} vs ".PadLeft(5)); }

                if (playerHand.score == 21) { Black(); Console.WriteLine($"플레이어 : 블랙잭!"); Console.ResetColor(); }
                else if (playerHand.score > 21) { { Red(false); Console.WriteLine($"플레이어 : 버스트"); Console.ResetColor(); } }
                else { Console.WriteLine($"플레이어 : {playerHand.score}"); }

                Console.WriteLine();
                if (playerHand.isBurst) { Console.WriteLine("플레이어의 패배입니다;"); }
                else if (playerHand.score == dealerHand.score) { Console.WriteLine("무승부입니다!"); }
                else if (playerHand.score > dealerHand.score || dealerHand.isBurst) { Console.WriteLine("플레이어의 승리입니다!"); }
                else { Console.WriteLine("플레이어의 패배입니다;"); }
            }

            // 카드 출력을 위한 메서드
            void CardPrint(List<string> cards)
            {
                foreach (string _ in cards)
                {
                    string[] a = _.Split();
                    switch (a[0])
                    {
                        case "1":
                            a[0] = "A";
                            break;
                        case "11":
                            a[0] = "J";
                            break;
                        case "12":
                            a[0] = "Q";
                            break;
                        case "13":
                            a[0] = "K";
                            break;
                        default:
                            break;
                    }
                    switch (a[1])
                    {
                        case "C":
                            Black();
                            a[1] = "Clover";
                            break;
                        case "S":
                            Black();
                            a[1] = "Spade";
                            break;
                        case "H":
                            Red();
                            a[1] = "Heart";
                            break;
                        case "D":
                            Red();
                            a[1] = "Diamond";
                            break;
                    }
                    Console.Write($"{a[0]} {a[1]}");
                    Console.ResetColor();
                    Console.Write("  ");
                }
            }

            // 각 핸드의 카드를 가져오게 하는 메서드
            void GetCard(Hand hand)
            {
                hand.GetCard(deck);
                hand.Score();
                Console.WriteLine();
                Console.WriteLine("카드를 뽑는 중입니다.");
                Thread.Sleep(500);
                Console.Beep();
                Board();
            }
        }

        // 다시 시작을 도와주는 메서드
        private bool Retry()
        {
            Console.WriteLine();
            Console.Write("다시 하시겠습니까?\n R키를 누르면 다시\n Q키를 누르면 나가집니다.");
            for (; ; )
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.R) { Console.WriteLine(); Console.WriteLine(); Console.WriteLine("다시 세팅 중입니다.".PadLeft(5)); Thread.Sleep(1000); return true; }
                else if (input.Key == ConsoleKey.Q) { Console.WriteLine(); Console.WriteLine(); Console.WriteLine("나가는 중입니다.".PadLeft(5)); Thread.Sleep(1000); return false; }
                else { Console.WriteLine("제대로 입력해주세요!"); }
            }

        }

        // 이 밑에있는 애들은 색깔 도우기용 메서드
        void Red()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
        }
        void Red(bool background)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        void Black()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        // 선택을 도우는 메서드
        bool Select()
        {
            Console.WriteLine();
            Console.WriteLine("카드를 뽑으시겠습니까?\n 원한다면 A키 아니라면 F키를 눌러주세요");
            
            for (; ; )
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.A) { return true; }
                else if (input.Key == ConsoleKey.F) { return false; }
                else { Console.WriteLine("제대로 입력해주세요!"); }
            }

        }


        // 덱을 섞어주는 메서드
        class Deck
        {
           public List<string> cards { get; private set; }
           Random rand = new Random();

            public Deck()
            {
                cards = new List<string>();
                for (int i = 1; i < 14; i++)
                {
                    cards.Add($"{i} S");
                    cards.Add($"{i} H");
                    cards.Add($"{i} C");
                    cards.Add($"{i} D");

                    cards = cards.OrderBy(_ => rand.Next()).ToList();
                }                
            }

            public void removeCard(int a)
            {
                cards.Remove(cards[a]);
            }
        }

        // 각 플레이어들의 핸드 관리
        class Hand
        {
            public bool isBurst { get; private set; }
            public int chips { get; private set; }
            public List<string> cards { get; private set; }

            public int score { get; private set; }
            public int softScore { get; private set; }
            public int A { get; private set; }

            Random rand = new Random();

            public Hand(int startChips)
            {
                chips = startChips;
                cards = new List<string>();
            }

            public Hand(bool noChips)
            {
                cards = new List<string>();
            }

            // 랜덤한 카드를 받아오는 메서드
            public void GetCard(Deck deck)
            {
                int a = rand.Next(0, deck.cards.Count);
                cards.Add(deck.cards[a]);
                deck.removeCard(a);
            }

            // 점수 계산 메서드
            public void Score()
            {
                score = 0;
                // 일단 받은 카드를 분류해줘야함.
                for (int i = 0; i < cards.Count; i++)
                {
                    string[] a = cards[i].Split();
                    int b = int.Parse(a[0]);
                    if (b == 1)
                    { A++; score += 1; }
                    else if (b > 10) { score += 10; }
                    else { score += b; }
                }
                if (score > 21) { isBurst = true; }
                SoftScore();
            }

            // A가 있을 때 판정해주는 메서드
            public void SoftScore()
            {
                softScore = 0;
                int scaling = 0;
                do
                {
                    if (A > 0)
                    {
                        softScore = score + ((A-scaling) * 11) - ((A - scaling) * 1);
                    }
                    if (softScore < 22) { break; }
                    scaling++;
                    if (A == scaling) { softScore = 0; break; }
                }
                while (true);
                    
            }

            public void SetSoftScore()
            {
                score = softScore;
            }

            public void ClearHand()
            {
                cards.Clear();
            }
    }

        static void Main(string[] args)
        {
            Program pg = new Program();
            bool Retry = false;

            Console.WriteLine("콘솔 블랙잭에 오신 걸 환영합니다.");
            Console.WriteLine("최대한 21에 가깝게 뽑으시면 됩니다. 만약에 21을 초과 시 딜러의 패 유무에 관계없이 플레이어가 무조건 패배합니다.");
            Console.WriteLine("숫자 카드는 숫자 그대로 계산에 반영되고 JQK는 10의 점수로 계산됩니다.\n그리고 A는 1 또는 11로 플레이어 마음대로 계산할 수 있습니다. (물론 계산은 저희가 알아서 해드리겠습니다.)");
            Console.WriteLine();
            Console.WriteLine("준비가 되셨다면 아무키나 입력하세요");
            Console.ReadKey(true);

            do
            {
                pg.GameManager(ref Retry);
            } while (Retry);
            Console.WriteLine();
            Console.WriteLine("게임해주셔서 감사합니다.");
        }
    }
}
