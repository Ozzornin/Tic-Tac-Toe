using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [Serializable]
    public class Game
    {
        public List<Player> Players = new List<Player>();
        public Board GameBoard;
        public Stack<Cell> Moves = new Stack<Cell>();
        public Player CurrentPlayer;
        private int _playerCount = 1;

        public void Run()
        {
            InitGame();
            StartGame();
            EndGame();
        }

        public void InitGame()
        {
            Console.Write("\nDo you want to play with bot?\n Press Y to confirm\n");
            if (Console.ReadKey().Key == ConsoleKey.Y)
                _playerCount = 1;

            for (int i = 1; i <= _playerCount; i++)
            {
                AddPlayer(i);
            }

            GameBoard = new Board();
            if (_playerCount == 1)
            {
                Players.Add(Bot.CreateBot(Players[0].Sign, GameBoard));
            }
        }

        private void AddPlayer(int playerIndex)
        {
            Console.Write($"\nPlayer {playerIndex} nickname: ");
            string name = Console.ReadLine();
            char sign;
            do
            {
                Console.Write($"Player {playerIndex} sign: ");
                try
                {
                    sign = char.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wrong sign, try again...");
                    continue;
                }
                if (Char.IsLetter(sign))
                {
                    bool isUsed = Players.Any(p => p.Sign == sign);
                    if (isUsed)
                    {
                        Console.WriteLine("This sign is used by another player. Try again..");
                        continue;
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong sign, try again...");
                }
            } while (true);

            Players.Add(new Player(name, sign));
            Console.Clear();
        }

        public void StartGame()
        {
            NextPlayer();
            Update();
            do
            {
                string n;
                do
                {
                    if (CurrentPlayer is Bot)
                    {
                        Bot botPlayer = (Bot)CurrentPlayer;
                        int num = botPlayer.GetBestMove();
                        n = num.ToString();
                        break;
                    }
                    if (Moves.Count != 0)
                        Console.WriteLine("\nYou can cancel previous move by typing 'prev'");

                    Console.Write("\nSelect cell: ");
                    n = Console.ReadLine().Trim();

                    if (n == "prev")
                    {
                        Undo();
                        continue;
                    }                    

                    try
                    {
                        int.Parse(n);
                    }
                    catch
                    {
                        Console.Write("There is no such cell as " + n);
                        continue;
                    }

                    int number = int.Parse(n);

                    if (number < 1 || number > 9)
                    {
                        Console.Write("There is no such cell as " + number);
                        continue;
                    }

                    if (GameBoard.IsCellEmpty(number - 1))
                        break;

                    Console.WriteLine("This cell is not empty! Select another one");
                } while (true);

                GameBoard.MarkCell(int.Parse(n) - 1, CurrentPlayer.Sign);
                Moves.Push(new Cell(int.Parse(n), CurrentPlayer.Sign));
                if (Moves.Count >= 5)
                {
                    for(int i=0; i<Players.Count; i++)
                    {
                        if (GameBoard.CheckWinner(Players[i].Sign))
                        {
                            Update();
                            Console.WriteLine(Players[i].Name + " won!");
                            Players[i].IncrementScore();
                            if (WantsToContinue())
                            {
                                ResetGameState();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                NextPlayer();
                Update();
            } while (true);
        }

        private bool WantsToContinue()
        {
            Console.WriteLine("Do you want to continue the game? (y/n)");
            return Console.ReadKey().Key == ConsoleKey.Y;
        }

        private void Undo()
        {
            bool singleGame = Players[1] is Bot;
            int requiredMoves = singleGame ? 0 : 1;
            if (Moves.Count > requiredMoves)
            {
                int cell;
                if (singleGame)
                {
                    cell = Moves.Pop().cellNumber;
                    GameBoard.RestoreCell(cell - 1);
                }
                else
                    NextPlayer();
                cell = Moves.Pop().cellNumber;
                GameBoard.RestoreCell(cell - 1);               
                Update();
            }
            else
            {
                Console.WriteLine("You can't do this now");
            }
        }

        private void ResetGameState()
        {
            GameBoard = new Board();
            if (Players[1] is Bot)
                Players[1] = Bot.CreateBot(Players[0].Sign, GameBoard, Players[1].GameScore);
            Moves = new Stack<Cell>();
        }

        public void EndGame()
        {
            Console.Clear();
            Console.WriteLine("Game over\n" +
                "Players results:\n");
            Console.Write("\nPlayer".PadRight(16) + "|");
            foreach (Player p in Players)
                Console.Write(p.Name.ToString().PadRight(10) + "|");
            Console.Write("\nScore".PadRight(16) + "|");
            foreach (Player p in Players)
                Console.Write(p.GameScore.ToString().PadRight(10) + "|");

        }

        public void Update()
        {
            Console.Clear();

            Console.Write("Player".PadRight(15) + "|");
            foreach (Player p in Players)
            {
                if (CurrentPlayer.Equals(p))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(p.Name.PadRight(10) + "|");

                Console.ResetColor();
            }

            Console.Write("\nSign".PadRight(16) + "|");
            foreach (Player p in Players)
                Console.Write(p.Sign.ToString().PadRight(10) + "|");

            Console.Write("\nScore".PadRight(16) + "|");
            foreach (Player p in Players)
                Console.Write(p.GameScore.ToString().PadRight(10) + "|");

            Console.WriteLine("\n\n" + GameBoard.ToString());
        }

        public void NextPlayer()
        {
            Save();
            int players = Players.Count;
            int i = Players.IndexOf(CurrentPlayer);
            if (i == -1 || i == players - 1)
            {
                CurrentPlayer = Players[0];
                return;
            }
            CurrentPlayer = Players[i + 1];
        }

        private void Save()
        {
            using (Stream stream = File.Open("game.bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }
    }

}
