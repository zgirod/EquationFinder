using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EquationFinder.Helpers;

namespace EquationFinder.DomainLogic
{
    public static class BoardLogic
    {

        public static int[,] CreateBoard(int boardSize, string boardValues)
        {

            //create the board
            var board = new int[boardSize, boardSize];

            var totalSize = boardSize * boardSize;
            int count = 1, row = 0, col = 0, i = 0;
            do
            {

                //get the number for this cell
                var cellNumber = Convert.ToInt32(boardValues[i].ToString());
                if (cellNumber == 0)
                    cellNumber = 10;

                //assign this number to the cell
                board[row, col] = cellNumber;

                //go to the next column
                col++;

                //if we reached the end of the board, go to the next row
                if (col >= boardSize)
                {
                    col = 0;
                    row++;
                }

                //did another row
                count++;
                i++;

            }
            while (count <= totalSize);


            return board;

        }

        public static int[,] CreateBoard(int boardSize)
        {

            //get a random borad for our size
            var hashKey = BoardLogic.RandomHash(boardSize * boardSize);

            //return the board
            return BoardLogic.CreateBoard(boardSize, hashKey);

        }

        private static string RandomHash(int number)
        {

            var boardValues = "";
            var random = new Random();
            for(int i = 0; i <= number; i++)
                boardValues += random.Next(0, 10);
            return boardValues;
            
        }

    }

}
