using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EquationFinder.Helpers;
//using System.Security.Cryptography;

namespace EquationFinder.DomainLogic
{
    public static class BoardLogic
    {

        public static int[,] CreateBoard(int boardSize, BigInt hashKey)
        {

            //create the board
            var board = new int[boardSize, boardSize];

            var totalSize = boardSize * boardSize;
            int count = 1, row = 0, col = 0;
            do
            {

                //get the number for this cell
                var cellNumber = Convert.ToInt32(hashKey.ToString().Substring(hashKey.ToString().Length - 1, 1));
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
                hashKey = BigInt.Divide(hashKey, BigInt.Parse("10"));

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

        private static BigInt RandomHash(int number)
        {

            var factorial = Factorial(BigInt.Parse(number.ToString()));

            //get a random number
            //var rng = new RNGCryptoServiceProvider();
            //byte[] bytes = factorial.ToByteArray();
            //string sadfasdf = "Asdf";
            //rng.GetBytes(bytes);

            //BigInteger hashKey = new BigInteger(bytes);
            //if (hashKey < 0)
            //    hashKey = hashKey * -1;

            //return hashKey;

            return BigInt.Parse("1234590328745932750913048572034985");


        }

        static BigInt Factorial(BigInt n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Can't calculate for negative number.", "n");
            }
            return n == 0 ? 1 : n * Factorial(n - 1);
        }

    }
}
