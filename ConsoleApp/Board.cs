using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [Serializable]
    public class Board
    {
        public char[,] board;

        public Board()
        {
            board = new char[3,3];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            int n = 1;
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    board[i, j] = (char)('0' + n++);
                }
            }
        }
        public override string ToString()
        {
            return $" {board[0, 0]} | {board[0, 1]} | {board[0, 2]} \n" +
                $"---+---+---\n" +
                $" {board[1, 0]} | {board[1, 1]} | {board[1, 2]} \n" +
                $"---+---+---\n" +
                $" {board[2, 0]} | {board[2, 1]} | {board[2, 2]} \n";
        }        

        public void MarkCell(int n, char sign)
        {
            try
            {
                board[n/3, n%3] = sign;
            }
            catch
            {
                Console.Write("MarkBoardCellError");
            }            
        }
        public void RestoreCell(int n)
        {
            board[n / 3, n % 3] = (char)('1'+ n);
            Console.WriteLine();
        }

        public bool IsCellEmpty(int n)
        {
            return Char.IsDigit(board[n / 3, n % 3]);
        }

        public bool CheckWinner(char player)
        {
            
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
                {
                    return true;
                }
            }

            
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] == player && board[1, j] == player && board[2, j] == player)
                {
                    return true;
                }
            }

            
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                return true;
            }

            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                return true;
            }

            return false;
        }
    }
}
