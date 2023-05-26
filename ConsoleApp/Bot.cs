using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    //bot is using minimax algorithm to make it unbeatable
    // you can try to beat him, but it's impossible

    [Serializable]
    public class Bot : Player
    {
        private char _playerSign;
        private Board _board;

        public Bot(string name, char sign, char playerSign, Board board) : base(name, sign)
        {
            _playerSign = playerSign;
            _board = board;
        }

        // Starting point of the Minimax algorithm.
        // Here, the bot can choose its next move.
        // However, firstly, it needs to predict all possible game results
        // and then it will choose the best move based on those predictions.
        public int GetBestMove()
        {
            int bestScore = int.MinValue;
            int bestMove = -1;

            for (int move = 0; move < 9; move++)
            {
                if (_board.board[move / 3, move % 3] != this.Sign && _board.board[move / 3, move % 3] != _playerSign)
                {
                    char prevSign = _board.board[move / 3, move % 3];
                    _board.board[move / 3, move % 3] = this.Sign;
                    
                    // oooh sh*t, here we go again...
                    int score = Minimax(_board.board, 0, false);

                    _board.board[move / 3, move % 3] = prevSign;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                }
            }
            return bestMove + 1;
        }




        // This method collects the predicted board state and, based on that,
        // makes the next moves to simulate the end of the game.

        // When the predicted game reaches its conclusion, the method
        // returns a value that depends on the results.

        // It returns -1 when the bot is beaten by a player.
        // ---In this case, the bot's previous move is rejected, and it tries another move.

        // It returns 1 when the bot wins the game.
        // ---In this case, it returns to the first choice and makes that move in the actual game.

        private int Minimax(char[,] board, int depth, bool isMaximizingPlayer)
        {
            int score = Evaluate(board);

            if (score != 0)
                return score;

            if (_board.IsFull())
                return 0;

            if (isMaximizingPlayer)
            {
                int bestScore = int.MinValue;

                for (int move = 0; move < 9; move++)
                {
                    if (board[move / 3, move % 3] != _playerSign && board[move / 3, move % 3] != this.Sign)
                    {
                        char prevSign = board[move / 3, move % 3];
                        board[move / 3, move % 3] = this.Sign;
                        int currentScore = Minimax(board, depth + 1, false);
                        board[move / 3, move % 3] = prevSign;
                        bestScore = Math.Max(bestScore, currentScore);
                        if(bestScore == 1) return bestScore;
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int move = 0; move < 9; move++)
                {
                    if (board[move / 3, move % 3] != _playerSign && board[move / 3, move % 3] != this.Sign)
                    {
                        char prevSign = board[move / 3, move % 3];
                        board[move / 3, move % 3] = _playerSign;
                        int currentScore = Minimax(board, depth + 1, true);
                        board[move / 3, move % 3] = prevSign;
                        bestScore = Math.Min(bestScore, currentScore);
                        if (bestScore == -1) return bestScore;
                    }
                }

                return bestScore;
            }
        }

        private int Evaluate(char[,] board)
        {
            if (CheckWin(board, this.Sign))
                return 1;

            if (CheckWin(board, _playerSign))
                return -1;

            return 0; 
        }

        private bool CheckWin(char[,] board, char sign)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == sign && board[i, 1] == sign && board[i, 2] == sign)
                    return true;

                if (board[0, i] == sign && board[1, i] == sign && board[2, i] == sign)
                    return true;
            }

            if (board[0, 0] == sign && board[1, 1] == sign && board[2, 2] == sign)
                return true;

            if (board[0, 2] == sign && board[1, 1] == sign && board[2, 0] == sign)
                return true;

            return false;
        }       
        
        public static Bot CreateBot(char playerSign, Board board, int points = 0)
        {
            Bot bot = new Bot("Bot", playerSign == 'X'? 'O': 'X', playerSign, board);
            bot.GameScore = points;
            return bot;
        }


    }

}
