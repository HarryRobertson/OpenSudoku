using System;
using System.Collections.Generic;

namespace OpenSudoku
{
    internal class CoordinateHelpers
    {
        private static readonly List<char> XCoordinates = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

        public static (int X, int Y) GetCoordinates(string s)
        {
            if (s.Length != 2) throw new ArgumentException($"s.Length was {s.Length}, must be 2.");

            s = s.ToUpper();

            char i = s[0];
            char j = s[1];

            int x = XCoordinates.IndexOf(i);
            int y = int.Parse(j.ToString()) - 1;
            return (x, y);
        }

        public static string GetAlphanumericCoordinates(int X, int Y)
        {
            if (X > 8 || X < 0 || Y > 8 || Y < 0) throw new ArgumentException("X and Y coordinates must be between 0 and 8.");

            return XCoordinates[X].ToString() + (Y + 1);
        }
    }
}