namespace textRPG
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // 스테이지 시작되면, 플레이어와 몬스터가 교대로 턴을 진행합니다.
            // 플레이어나 몬스터 중 하나가 죽으면 스테이지가 종료되고, 그 결과를 출력해줍니다.
            // 스테이지가 끝날 때, 플레이어가 살아있다면 보상 아이템 중 하나를 선택하여 사용할 수 있습니다.

            Console.WriteLine();
            Console.WriteLine("간단한 텍스트RPG에 오신 것을 환영합니다.");
            Console.WriteLine("시작하고 싶다면 아무키나 누르세요!");
            Console.WriteLine();

            Console.ReadKey();
            Stage stage = new Stage();

            bool isRetry = false;

            do
            {
                stage.Start();
                isRetry = ReStartChoice();
            } while (isRetry);

        }
        static bool ReStartChoice()
        {
            Console.WriteLine();
            Console.WriteLine("처음부터 다시 시작하시겠습니까?");
            Console.WriteLine("원한다면 R 아니라면 Q를 눌러주십시오.");

            while (true)
            {
                ConsoleKey choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.R:
                        return true;
                    case ConsoleKey.Q:
                        return false;
                    default:
                        Console.WriteLine("키를 제대로 입력해주세요!");
                        break;
                }
            }
        }
    }


    enum CreatureType
    {
        Warrior,
        Monster
    }

    interface ICharacter
    {
        string Name { get; }
        int Health { get; }
        int Attack { get; }
        bool isDead { get; }

        CreatureType type { get; }

        public void TakeDamage(int damage) { }
    }

    class Warrior : ICharacter
    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public bool isDead { get; private set; }

        public CreatureType type { get; }

        public Warrior(string _name, int _health, int _attack, bool _isdead = false)
        {
            Name = _name;
            Health = _health;
            Attack = _attack;
            isDead = _isdead;
            type = CreatureType.Warrior;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                isDead = true;
            }
        }
        public void AttackUp(int strength)
        {
            Attack += strength;
        }
    }

    class Monster : ICharacter
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public bool isDead { get; protected set; }
        public CreatureType type { get; }

        public Monster()
        {
            type = CreatureType.Monster;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                isDead = true;
            }
        }
    }
    class Goblin : Monster
    {
        public Goblin(bool _isdead = false) : base()
        {
            Name = "고블린";
            Health = 30;
            Attack = 5;
            isDead = _isdead;
        }
    }

    class Dragon : Monster
    {
        public Dragon(bool _isdead = false) : base()
        {
            Name = "드래곤";
            Health = 100;
            Attack = 20;
            isDead = _isdead;
        }
    }

    interface IItem
    {
        string Name { get; set; }

        public void Use(Warrior warrior) { }
    }

    class HealthPotion : IItem
    {
        public string Name { get; set; }

        public void Use(Warrior warrior)
        { warrior.TakeDamage(-40); }
    }

    class StrengthPotion : IItem
    {
        public string Name { get; set; }

        public void Use(Warrior warrior)
        { warrior.AttackUp(5); }
    }

    class Stage
    {
        Warrior warrior;
        Monster monster;

        HealthPotion hpPotion = new HealthPotion();
        StrengthPotion strPotion = new StrengthPotion();

        IItem reward;

        Random rand = new Random();

        int delay = 1000;

        bool isHadProlugue;

        public void Start()
        {
            if (!isHadProlugue) // 게임을 재시작할 때 이미 프롤로그를 봣다면 스킵
            {
                warrior = new Warrior("용사", 100, 10);   // 플레이어 소환
                Console.Clear();
                Console.WriteLine($"당신의 이름은 {warrior.Name}.");
                Console.WriteLine($"스파르타 던전을 탐험하면 강해진다는 소문을 듣고,");
                Console.WriteLine($"자신의 능력을 강하게 키우기 위해 이 던전 안을 탐험하려고 한다.");
                isHadProlugue = true; // 프롤로그를 봤으니 트루로 돌리기
                Console.WriteLine();
                Console.WriteLine("계속하려면 아무키나 눌러주십시오.");
                Console.ReadKey();
            }
            else
            {
                warrior = new Warrior("플레이어", 100, 10);  // 플레이어 소환
                Console.Clear();
                Console.WriteLine($"또 다른 누군가가 던전을 탐험하려고 합니다...");
                Console.WriteLine();
                Console.WriteLine("계속하려면 아무키나 눌러주십시오.");
                Console.ReadKey();
            }

            // 몬스터 생성
            MonsterRespawn();

        }

        public void MonsterRespawn()    // 몬스터 생성
        {
            int randomSpawn = rand.Next(0, 2);
            if (randomSpawn == 0) { monster = new Goblin(); }
            else if (randomSpawn == 1) { monster = new Dragon(); }

            BattleStart(monster);
        }

        // 플레이어 전투 시작    

        void BattleStart(Monster monster) // 전투 시작 전
        {
            Console.Clear();

            Console.WriteLine($"{monster.Name}이(가) 나타났습니다!");

            EnemyStatus(monster);

            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine($"{monster.Name}과 싸우시겠습니까?");
            Console.WriteLine("A는 전투, F는 도망입니다.");
            StartChoice();
        }

        void StartChoice()  // 플레이어의 전투 전 선택
        {
            while (true)
            {
                ConsoleKey choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.A:
                        BattleNow();
                        return;
                    case ConsoleKey.F:
                        BattleFlee();
                        return;
                    default:
                        Console.WriteLine("키를 제대로 입력해주세요!");
                        break;
                }
            }
        }

        // 플레이어 전투 중

        void BattleNow()    // 플레이어 전투 중
        {
            Console.Clear();
            Console.WriteLine("# 전투 중! #");
            Console.WriteLine("# - 플레이어의 턴 - #");

            EnemyStatus(monster);

            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine("행동을 선택해주세요!");
            Console.WriteLine("A는 공격, S는 스킬, I는 인벤토리, F는 도망입니다.");
            BattleChoice();
        }

        void BattleChoice() // 플레이어 전투 선택
        {
            while (true)
            {
                ConsoleKey choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.A:
                        Attack();
                        if (monster.isDead) { BattleWin(); }
                        else { EnemyTurn(); }
                        return;
                    case ConsoleKey.S:
                        Skill();
                        if (monster.isDead) { BattleWin(); }
                        else { EnemyTurn(); }
                        return;
                    case ConsoleKey.I:
                        // 아이템
                        Console.WriteLine("I(인벤토리)는 아직 구현중인 기능입니다!");
                        break;
                    case ConsoleKey.F:
                        BattleFlee();
                        return;
                    default:
                        Console.WriteLine("키를 제대로 입력해주세요!");
                        break;
                }
            }
        }
        // 플레이어의 선택 

        void Attack()   // 공격
        {
            Console.Clear();
            Console.WriteLine("# 전투 중! #");
            Console.WriteLine("# - 플레이어의 턴 - #");

            EnemyStatus(monster);

            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine("플레이어가 공격을 시도합니다!");
            Thread.Sleep(delay);
            Console.WriteLine($"{monster.Name}이(가) {warrior.Attack}의 데미지를 입었습니다!");
            monster.TakeDamage(warrior.Attack);
            Thread.Sleep(delay);
        }

        void Skill()    // 스킬
        {
            Console.Clear();
            Console.WriteLine("# 전투 중! #");
            Console.WriteLine("# - 플레이어의 턴 - #");

            EnemyStatus(monster);

            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine("플레이어가 더블 어택을 시도합니다!");
            Thread.Sleep(delay);
            Console.WriteLine($"{monster.Name}이(가) {warrior.Attack}의 데미지를 입었습니다!");
            monster.TakeDamage(warrior.Attack);
            Thread.Sleep(delay / 2);
            Console.WriteLine($"{monster.Name}이(가) {warrior.Attack}의 데미지를 입었습니다!");
            monster.TakeDamage(warrior.Attack);
            Thread.Sleep(delay);
        }

        void Item() // 아이템
        {
            Console.Clear();
            Console.WriteLine("# 전투 중! #");
            Console.WriteLine("# - 플레이어의 턴 - #");

            EnemyStatus(monster);

            PlayerStatus();
        }

        // 적의 턴

        void EnemyTurn()
        {
            Console.Clear();
            Console.WriteLine("# 전투 중! #");
            Console.WriteLine("# - 플레이어의 턴 - #");

            EnemyStatus(monster);

            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine("적이 공격을 시도합니다!");
            Thread.Sleep(delay);
            // 적이 선택지를 고릅니다.
            int select = rand.Next(0, 2);
            switch (select)
            {
                case 0:
                    Console.WriteLine($"{monster.Name}은(는) 아무것도 안하기로 했다...");
                    break;
                case 1:
                    Console.WriteLine($"{monster.Name}이(가) 플레이어에게 {monster.Attack}의 데미지를 가했습니다.");
                    warrior.TakeDamage(monster.Attack);
                    if (warrior.isDead == true) { Thread.Sleep(delay); BattleLose(); return; }
                    break;
                default:
                    break;
            }
            Thread.Sleep(delay);
            BattleNow();
        }

        // 전투 결과

        void BattleFlee() // 도주
        {
            Console.Clear();
            Console.WriteLine("# 전투 후퇴! #");
            Console.WriteLine("당신은 도망을 선택하셨습니다.");
            Thread.Sleep(delay);

            bool isNext = ChooseNextStage();
            if (isNext) { MonsterRespawn(); return; }
            else { return; }
        }

        void BattleWin()    // 승리
        {
            Console.Clear();
            Console.WriteLine("# 전투 결과 #");
            Console.WriteLine("# - 승리! - #");
            Thread.Sleep(delay);
            Console.WriteLine();
            Console.WriteLine($"#{monster.Name}과의 전투에서 승리하셨습니다!#");
            Thread.Sleep(delay);

            ChooseReward();
            Thread.Sleep(delay);
            bool isNext = ChooseNextStage();
            if (isNext) { MonsterRespawn(); return; }
            else { return; }
        }

        void BattleLose()   // 패배
        {
            Console.Clear();
            Console.WriteLine("# 전투 결과 #");
            Console.WriteLine("# - 패배! - #");
            Thread.Sleep(delay);
            Console.WriteLine();
            Console.WriteLine("체력을 모두 소모하셨습니다...");
            Console.WriteLine("Game Over".PadLeft(10));
            Thread.Sleep(delay);
        }

        // 보상 기능
        void ChooseReward() // 승리 보상 선택
        {
            Console.WriteLine();
            Console.WriteLine("보상을 선택해주십시오!");

            Console.WriteLine();
            Console.WriteLine("H  체력 포션 - 40의 체력을 증가시킵니다.");
            Console.WriteLine("S  힘 포션 - 공격력을 영구적으로 5 올려줍니다.");
            Console.WriteLine();
            Console.WriteLine("H키 또는 S키를 눌러주세요");

            while (true)
            {
                ConsoleKey choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.H:
                        hpPotion.Use(warrior);
                        Console.WriteLine("체력이 올랐습니다!");
                        Console.WriteLine($"당신의 체력 : {warrior.Health - 40} - > {warrior.Health}");
                        return;
                    case ConsoleKey.S:
                        strPotion.Use(warrior);
                        Console.WriteLine("공격력이 올랐습니다!");
                        Console.WriteLine($"당신의 공격력 : {warrior.Attack - 5} - > {warrior.Attack}");
                        return;
                    default:
                        Console.WriteLine("키를 제대로 입력해주세요!");
                        break;
                }
            }
        }

        // 다음 스테이지 기능

        bool ChooseNextStage()  // 다음 스테이지 선택
        {
            Console.Clear();
            PlayerStatus();

            Console.WriteLine();
            Console.WriteLine("다음 스테이지로 넘어가시겠습니까?");
            Console.WriteLine("원한다면 A 아니라면 Q를 눌러주십시오.");

            while (true)
            {
                ConsoleKey choice = Console.ReadKey(true).Key;
                switch (choice)
                {
                    case ConsoleKey.A:
                        return true;
                    case ConsoleKey.Q:
                        return false;
                    default:
                        Console.WriteLine("키를 제대로 입력해주세요!");
                        break;
                }
            }
        }

        // 정보창 보여주는 곳
        void EnemyStatus(Monster monster)   // 적
        {
            Console.WriteLine();
            Console.WriteLine("[몬스터의 스탯]");
            Console.WriteLine($"{monster.Name}");
            Console.WriteLine($"HP : {monster.Health}");
            Console.WriteLine($"공격력 : {monster.Attack}");
        }

        void PlayerStatus() // 나
        {
            Console.WriteLine();
            Console.WriteLine("[플레이어의 스탯]");
            Console.WriteLine($"{warrior.Name}");
            Console.WriteLine($"HP : {warrior.Health}");
            Console.WriteLine($"공격력 : {warrior.Attack}");
        }
    }
}
