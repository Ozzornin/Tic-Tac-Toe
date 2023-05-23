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

        public void InitGame()
        {
            for(int i=1; i<=2; i++)
            {
                Console.Write($"\nPlayer {i} nickname: ");
                string name = Console.ReadLine();                
                char sign;
                do {
                    Console.Write($"Player {i} sign: ");
                    try
                    {
                        sign = char.Parse(Console.ReadLine());
                    }
                    catch ( Exception e ) {
                        Console.WriteLine("Wrong sign, try again...");
                        continue;
                    }                     
                    if (Char.IsLetter(sign))
                    {
                        bool isUsed = false;
                        foreach(Player p in Players)
                        {
                            isUsed = p.Sign == sign;                                                                     
                        }
                        if (isUsed)
                        {
                            Console.WriteLine("This sign is used by another player. Try again..");
                            continue;
                        }                        
                        break;
                    }                       
                    else Console.WriteLine("Wrong sign, try again...");
                }
                while (true);                               
                Players.Add(new Player(name, sign));
                Console.Clear();
            }
            GameBoard = new Board();
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
                    if(Moves.Count!=0)
                        Console.WriteLine("\nYou can cancel previous move by typing 'prev'");

                    Console.Write("\nSelect cell: ");
                    n = Console.ReadLine().Trim();

                    if(n == "prev")
                    {
                        if(Moves.Count !=0)
                        {
                            int cell = Moves.Pop().cellNumber;
                            GameBoard.RestoreCell(cell-1);
                            NextPlayer();
                            Update();                          
                        }
                        else
                        {
                            Console.WriteLine("Make the first move!");                            
                        }
                        continue;
                    }

                    try
                    {
                        int.Parse(n);
                    }
                    catch {
                        Console.Write("There is no such cell as " + n);
                        continue;
                    }

                    int number = int.Parse(n);

                    if(number<1 || number > 9)
                    {
                        Console.Write("There is no such cell as " + number);
                        continue;
                    }

                    if (GameBoard.IsCellEmpty(int.Parse(n)-1))
                        break;

                    Console.WriteLine("This cell is not empty! Select another one");                                      
                } while (true);

                GameBoard.MarkCell(int.Parse(n)-1, CurrentPlayer.Sign);                
                Moves.Push(new Cell(int.Parse(n), CurrentPlayer.Sign));
                if(Moves.Count >= 5)
                {
                    foreach(Player p in Players)
                    {
                        if(GameBoard.CheckWinner(p.Sign))
                        {
                            Update();
                            Console.WriteLine(p.Name  + " won!");
                            p.IncrementScore();
                            Console.WriteLine("Do you want to continue the game?(y/n)");
                            if(Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                GameBoard = new Board();
                                Moves = new Stack<Cell>();  
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

            return;
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

            Console.WriteLine("\n\n"+ GameBoard.ToString());
        }

        public void NextPlayer()
        {
            Save();
            int players = Players.Count;
            int i = Players.IndexOf(CurrentPlayer);
            if (i == -1 || i == players - 1) {
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
