using System;

namespace GameLogic
{
    class Attributes
    {
        protected int strength = 1;
        protected int defence = 1;
        protected int attack = 1;
        protected int vitality = 1;
        protected int stamina = 1;
        public int Strength { get { return strength; } set { strength = value; } }
        public int Defence { get { return defence; } set { defence = value; } }
        public int Attack { get { return attack; } set { attack = value; } }
        public int Vitality { get { return vitality; } set { vitality = value; } }
        public int Stamina { get { return stamina; } set { stamina = value; } }
    }
    class Gladiator : Attributes
    {
        protected int health = 10;
        protected int mana = 8;
        protected int minDamage = 1;
        protected int maxDamage = 3;
        protected int blockChance = 1;
        protected bool sleeping = false;
        public int Health { get { return health; } set { health = value; } }
        public int Mana { get { return mana; } set { mana = value; } }
        public int MinDamage { get { return minDamage; } set { minDamage = value; } }
        public int MaxDamage { get { return maxDamage; } set { maxDamage = value; } }
        public int BlockChance { get { return blockChance; } set { blockChance = value; } }
        public bool Sleeping { get { return sleeping; } set { sleeping = value; } }
        public void GladiatorStats()
        {
            Console.WriteLine("Health: " + Health);
            Console.WriteLine("Mana: " + Mana);
            Console.WriteLine("Damage: " + MinDamage + " - " + MaxDamage);
        }
        public bool Blocked(ref Player Player)
        {
            int BlockChance = (Defence / Player.Attack) * 100;
            if (BlockChance > 65) 
                BlockChance = 65;
            Random random = new Random();
            if (BlockChance >= random.Next(101))
                return true;
            return false;
        }
    }
    class Player : Gladiator
    {
        public string? Name;
        public int AttributePoints = 60;
    }
}

namespace FightLogic
{
    using GameLogic;
    class FightSection : Player
    {
        static void Loading(int Timer)
        {
            for (int i = 3; i > 0; i--) 
            {
                Console.WriteLine(i.ToString() + "...");
                System.Threading.Thread.Sleep(Timer);
            }
        }
        static void PreBattle(ref Player Player1, ref Player Player2)
        {
            Console.WriteLine("Getting gladiators' attributes done. Please wait.");
            Loading(1000);
            Console.Clear();
            Console.WriteLine("------------------------------");
            Console.WriteLine("Player name: " + Player1.Name);
            Player1.GladiatorStats();
            Console.WriteLine("------------------------------");
            Console.WriteLine("Player name: " + Player2.Name);
            Player2.GladiatorStats();
            Console.WriteLine("------------------------------");
        }
        protected static bool CriticalHit(ref Player Player1, ref Player Player2)
        {
            int CriticalChance = Convert.ToInt32((Player1.Attack / Player2.Attack) * 100);
            Random random = new Random();
            if (CriticalChance > 100) CriticalChance = 100;
            if (CriticalChance > random.Next(101))
                return true;
            return false;
        }
        protected static void DealDamage(ref Player Player1, ref Player Player2)
        {
            Random random = new Random();
            int Damage = random.Next(Player1.MinDamage, Player1.MaxDamage);
            double CriticalMultiplier = random.NextDouble();
            if (Player1.Sleeping)
            {
                Console.WriteLine("Player " + Player1.Name + " is sleeping! No damage dealt.");
                Player1.Mana += Convert.ToInt32(Math.Round(0.75 * Player1.Stamina));
                Player1.Health += Convert.ToInt32(Math.Round(0.30 * Player1.Stamina));
                Player1.Sleeping = false;
                System.Threading.Thread.Sleep(1000);
            }
            if (Player1.Mana >= Convert.ToInt32(Math.Round(0.30 * Player1.Strength)))
            {
                if (CriticalHit(ref Player1, ref Player2))
                {
                    Player1.Mana -= Convert.ToInt32(Math.Round(0.30 * Player1.Strength));
                    Damage += Convert.ToInt32(Damage * CriticalMultiplier);
                    Console.WriteLine("Player " + Player1.Name + " critical hit! " + Damage + " damage dealt. " + "HP:("+Player1.Health + ")(MP:" + Player1.Mana + ")"); ;
                    Player2.Health -= Damage;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    Player1.Mana -= Convert.ToInt32(Math.Round(0.30 * Player1.Strength));
                    Console.WriteLine("Player " + Player1.Name + " dealt " + Damage + " damage (HP:" + Player1.Health + ")(MP:" + Player1.Mana + ")");
                    Player2.Health -= Damage;
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else
            {
                Console.WriteLine("Player " + Player1.Name + " is sleeping! No damage dealt.");
                Player1.Sleeping = true;
                System.Threading.Thread.Sleep(1000);
            }
        }
        static bool DrawFirstAttacker()
        {
            Random random = new Random();
            int DrawAttacker = random.Next(11);
            if (DrawAttacker % 2 == 0) 
                return true;
            return false;
        }
        public static void Fight(ref Player Player1, ref Player Player2)
        {
            bool AttackFlag;
            PreBattle(ref Player1, ref Player2);
            Console.WriteLine("The fight begins in:");
            Loading(1000);
            Console.Clear();
            if (DrawFirstAttacker())
            {
                if (Player2.Blocked(ref Player1))
                    Console.WriteLine("Player " + Player2.Name + " blocked!");
                else
                    DealDamage(ref Player1, ref Player2);
                AttackFlag = false;
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                if (Player1.Blocked(ref Player2))
                    Console.WriteLine("Player " + Player1.Name + " blocked!");
                else
                    DealDamage(ref Player2, ref Player1);
                AttackFlag = true;
                System.Threading.Thread.Sleep(1000);
            }
            while(Player1.Health > 0 && Player2.Health > 0)
            {
                if (AttackFlag) 
                {
                    if (Player2.Blocked(ref Player1))
                        Console.WriteLine("Player " + Player2.Name + " blocked!");
                    else
                        DealDamage(ref Player1, ref Player2);
                    AttackFlag = false;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    if (Player1.Blocked(ref Player2))
                        Console.WriteLine("Player " + Player1.Name + " blocked!");
                    else
                        DealDamage(ref Player2, ref Player1);
                    AttackFlag = true;
                    System.Threading.Thread.Sleep(1000);
                }
            }
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("------------------------------");
            if (Player1.Health > Player2.Health)
                Console.WriteLine("Player " + Player1.Name + " has won! (HP:" + Player1.Health + ")(MP:" + Player1.Mana + ")");
            else
                Console.WriteLine("Player " + Player2.Name + " has won! (HP:" + Player2.Health + ")(MP:" + Player2.Mana + ")");
        }
    }
}

namespace MainProgram
{
    using GameLogic;
    using FightLogic;

    class MainProgram : FightSection
    {
        static void PointsValidation(ref string UserInput)
        {
            int i = 0;
            if (UserInput.Length > 2) 
            {
                UserInput = UserInput.Remove(2, (UserInput.Length - 2));
            }
            while (i < UserInput.Length)
            {
                if (char.IsDigit(UserInput[i]))
                {
                    i++;
                }
                else
                {
                    UserInput = UserInput.Remove(i, 1);
                }
            }
        }
        static void AttributesSetting(ref Player Player)
        {
            string? Input;
            char? Choice;
            int Points;
            int CurrentState;
            while (Player.AttributePoints > 0)
            {
                Console.WriteLine(Player.Name + " here's the list of attributes. Choose one by typing number from 1 to 5 and start adding points to it. \nBe aware of that you can have maximum of 30 points to a single attribute.");
                Console.WriteLine
                    (
                        "1. Strength: " + Player.Strength + "\n" +
                        "2. Defence: " + Player.Defence + "\n" +
                        "3. Attack: " + Player.Attack + "\n" +
                        "4. Vitality: " + Player.Vitality + "\n" +
                        "5. Stamina: " + Player.Stamina + "\n" +
                        "Points left: " + Player.AttributePoints
                    );
                Console.WriteLine("Choose attribute.");
                Input = Console.ReadLine();
                Choice = Input[0];
                while (Choice > '5' || Choice < '0')
                {
                    Console.WriteLine("Invalid attribute given. Try again.");
                    Input = Console.ReadLine();
                    Choice = Input[0];
                }
                switch (Choice)
                {
                    case '1':
                        Console.WriteLine("Add points.");
                        Input = Console.ReadLine();
                        PointsValidation(ref Input);
                        if (String.IsNullOrEmpty(Input)) Input = "0";
                        if (Convert.ToInt32(Input) > 30) Input = "30";
                        Points = Convert.ToInt32(Input);
                        if (Points < 0) Points = Math.Abs(Points);
                        if (Points > Player.AttributePoints) Points = Player.AttributePoints;
                        if (Player.Strength == 30) 
                        {
                            Console.WriteLine("You have reached a limit of possible points to this attribute.");
                            Console.WriteLine("No points added.");
                            System.Threading.Thread.Sleep(1000);
                            break;
                        }
                        if (Player.Strength + Points > 30) 
                        { 
                            CurrentState = Player.Strength;
                            Player.AttributePoints -= (30 - CurrentState);
                            Player.Strength = 30;
                            Console.WriteLine("Added " + (30 - CurrentState) + " to strength.");
                            break;
                        } 
                        else Player.Strength += Points;
                        Player.MaxDamage = 2 * (Player.Strength);
                        Player.MinDamage = Convert.ToInt32(Player.MaxDamage - (Math.Round(Player.MaxDamage * 0.75)));
                        Player.AttributePoints -= Points;
                        Console.WriteLine("Added " + Points + " to strength.");
                        break;
                    case '2':
                        Console.WriteLine("Add points.");
                        Input = Console.ReadLine();
                        PointsValidation(ref Input);
                        if (String.IsNullOrEmpty(Input)) Input = "0";
                        if (Convert.ToInt32(Input) > 30) Input = "30";
                        Points = Convert.ToInt32(Input);
                        if (Points < 0) Points = Math.Abs(Points);
                        if (Points > Player.AttributePoints) Points = Player.AttributePoints;
                        if (Points > 30) Points = 30;
                        if (Player.Defence == 30)
                        {
                            Console.WriteLine("You have reached a limit of possible points to this attribute.");
                            Console.WriteLine("No points added.");
                            System.Threading.Thread.Sleep(1000);
                            break;
                        }
                        if (Player.Defence + Points > 30)
                        {
                            CurrentState = Player.Defence;
                            Player.AttributePoints -= (30 - CurrentState);
                            Player.Defence = 30;
                            Console.WriteLine("Added " + (30 - CurrentState) + " to strength.");
                            break;
                        }
                        else Player.Defence += Points;
                        Player.AttributePoints -= Points;
                        Console.WriteLine("Added " + Points + " to defence.");
                        break;
                    case '3':
                        Console.WriteLine("Add points.");
                        Input = Console.ReadLine();
                        PointsValidation(ref Input);
                        if (String.IsNullOrEmpty(Input)) Input = "0";
                        if (Convert.ToInt32(Input) > 30) Input = "30";
                        Points = Convert.ToInt32(Input);
                        if (Points < 0) Points = Math.Abs(Points);
                        if (Points > Player.AttributePoints) Points = Player.AttributePoints;
                        if (Points > 30) Points = 30;
                        if (Player.Attack == 30)
                        {
                            Console.WriteLine("You have reached a limit of possible points to this attribute.");
                            Console.WriteLine("No points added.");
                            System.Threading.Thread.Sleep(1000);
                            break;
                        }
                        if (Player.Attack + Points > 30)
                        {
                            CurrentState = Player.Attack;
                            Player.AttributePoints -= (30 - CurrentState);
                            Player.Attack = 30;
                            Console.WriteLine("Added " + (30 - CurrentState) + " to strength.");
                            break;
                        }
                        else Player.Attack += Points;
                        Player.AttributePoints -= Points;
                        Console.WriteLine("Added " + Points + " to attack.");
                        break;
                    case '4':
                        Console.WriteLine("Add points.");
                        Input = Console.ReadLine();
                        PointsValidation(ref Input);
                        if (String.IsNullOrEmpty(Input)) Input = "0";
                        if (Convert.ToInt32(Input) > 30) Input = "30";
                        Points = Convert.ToInt32(Input);
                        if (Points < 0) Points = Math.Abs(Points);
                        if (Points > Player.AttributePoints) Points = Player.AttributePoints;
                        if (Points > 30) Points = 30;
                        if (Player.Vitality == 30)
                        {
                            Console.WriteLine("You have reached a limit of possible points to this attribute.");
                            Console.WriteLine("No points added.");
                            System.Threading.Thread.Sleep(1000);
                            break;
                        }
                        if (Player.Vitality + Points > 30)
                        {
                            CurrentState = Player.Vitality;
                            Player.AttributePoints -= (30 - CurrentState);
                            Player.Vitality = 30;
                            Console.WriteLine("Added " + (30 - CurrentState) + " to strength.");
                            break;
                        }
                        else Player.Vitality += Points;
                        Player.Health = 10 * (Player.Vitality);
                        Player.AttributePoints -= Points;
                        Console.WriteLine("Added " + Points + " to vitality.");
                        break;
                    case '5':
                        Console.WriteLine("Add points.");
                        Input = Console.ReadLine();
                        PointsValidation(ref Input);
                        if (String.IsNullOrEmpty(Input)) Input = "0";
                        if (Convert.ToInt32(Input) > 30) Input = "30";
                        Points = Convert.ToInt32(Input);
                        if (Points < 0) Points = Math.Abs(Points);
                        if (Points > Player.AttributePoints) Points = Player.AttributePoints;
                        if (Points > 30) Points = 30;
                        if (Player.Stamina == 30)
                        {
                            Console.WriteLine("You have reached a limit of possible points to this attribute.");
                            Console.WriteLine("No points added.");
                            System.Threading.Thread.Sleep(1000);
                            break;
                        }
                        if (Player.Stamina + Points > 30)
                        {
                            CurrentState = Player.Stamina;
                            Player.AttributePoints -= (30 - CurrentState);
                            Player.Stamina = 30;
                            Console.WriteLine("Added " + (30 - CurrentState) + " to strength.");
                            break;
                        }
                        else Player.Stamina += Points;
                        Player.Mana = 8 * (Player.Stamina);
                        Player.AttributePoints -= Points;
                        Console.WriteLine("Added " + Points + " to stamina.");
                        break;
                }
                Console.WriteLine(Player.Name + " has " + Player.AttributePoints + " points left.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
        }
        static void StartGame() 
        {
            Player Player1 = new Player();
            Player Player2 = new Player();
            Console.WriteLine("Player 1 enter name please.");
            string Player1Name = Console.ReadLine();
            while(Player1Name.Length > 20)
            {
                Console.Clear();   
                Console.WriteLine("Name too long. The limit is 20 letters.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Player 1 enter name please.");
                Player1Name = Console.ReadLine();
            }
            while (String.IsNullOrEmpty(Player1Name))
            {
                Console.Clear();
                Console.WriteLine("You can't insert empty name.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Player 1 enter name please.");
                Player1Name = Console.ReadLine();
            }
            Player1.Name = Player1Name;
            Console.Clear();
            Console.WriteLine("Player 2 enter name please.");
            string Player2Name = Console.ReadLine();
            while (Player2Name.Length > 20)
            {
                Console.Clear();
                Console.WriteLine("Name too long. The limit is 20 letters.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Player 2 enter name please.");
                Player2Name = Console.ReadLine();
            }
            while (String.IsNullOrEmpty(Player2Name))
            {
                Console.Clear();
                Console.WriteLine("You can't insert empty name.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Player 2 enter name please.");
                Player2Name = Console.ReadLine();
            }
            while (Player1Name == Player2Name) 
            {
                Console.Clear();
                Console.WriteLine("You can't use the same name as Your opponent!. Try again.");
                System.Threading.Thread.Sleep(1500);
                Console.Clear();
                Console.WriteLine("Player 2 enter name please.");
                Player2Name = Console.ReadLine();
            }
            Player2.Name = Player2Name;
            Console.Clear();
            AttributesSetting(ref Player1);
            AttributesSetting(ref Player2);
            Fight(ref Player1, ref Player2);
        }
        static void Main()
        {
            StartGame();
        }
    }
}



