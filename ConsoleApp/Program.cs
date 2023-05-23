using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to load your last game?(y/n)");
            if(Console.ReadKey().Key == ConsoleKey.Y)
            {
                Game savedGame;
                using (Stream stream = File.Open("game.bin", FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    savedGame = (Game)formatter.Deserialize(stream);
                }
                savedGame.StartGame();
                savedGame.EndGame();
                Console.ReadLine();
                return;
            }
            Game game = new Game();
            game.InitGame();
            game.StartGame();
            game.EndGame();
            Console.ReadLine();
            
        }
    }
}
