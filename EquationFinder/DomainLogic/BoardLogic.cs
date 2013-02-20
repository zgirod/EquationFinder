using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace EquationFinder.DomainLogic
{
    public static class BoardLogic
    {

        public static int[,] CreateBoard(int boardSize, BigInteger hashKey)
        {

            //create the board
            var board = new int[boardSize, boardSize];

            var totalSize = boardSize * boardSize;
            int count = 1, row = 0, col = 0;
            do
            {

                //get the number for this cell
                var cellNumber = Convert.ToInt32(BigInteger.Remainder(hashKey, new BigInteger(10)).ToString());
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

                //get the next value in the has key
                hashKey = BigInteger.Divide(hashKey, new BigInteger(10));

                //did another row
                count++;

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

        private static BigInteger RandomHash(int number)
        {

            var factorial = new BigInteger(number);
            number--;
            for (; number > 1; number--)
                factorial = factorial * number;

            //get a random number
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = factorial.ToByteArray();
            rng.GetBytes(bytes);

            BigInteger hashKey = new BigInteger(bytes);
            if (hashKey < 0)
                hashKey = hashKey * -1;

            return hashKey;


        }

    }
}
