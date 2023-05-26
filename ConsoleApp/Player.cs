using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }
        public char Sign { get; set; }
        public int GameScore { get; set; }

        public Player(string name, char sign) {
            Name = name;
            Sign = sign;
            GameScore = 0;
        }

        public void IncrementScore()
        {
            GameScore++;
        }

        internal int GetBestMove()
        {
            throw new NotImplementedException();
        }
    }
}
