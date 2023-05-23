using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [Serializable]
    public class Cell
    {
        public int cellNumber { get; set; }
        public char cellSigh { get; set; }

        public Cell(int cellNumber, char cellSigh)
        {
            this.cellNumber = cellNumber;
            this.cellSigh = cellSigh;
        }
    }
}
