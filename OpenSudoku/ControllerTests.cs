﻿using System.Linq;
using NUnit.Framework;

namespace OpenSudoku
{
    [TestFixture]
    public class ControllerTests
    {
        // PUZZLE:
        //         A   B   C    D   E   F    G   H   I
        //      =========================================
        //   1  || 5 | 8 |   || 1 |   |   || 3 | 2 |   ||
        //      -----------------------------------------
        //   2  || 3 |   |   || 2 | 5 |   ||   |   |   ||
        //      -----------------------------------------
        //   3  || 4 | 2 | 9 ||   |   |   ||   |   |   ||
        //      =========================================
        //   4  ||   | 1 | 6 || 3 |   | 8 || 4 |   |   ||
        //      -----------------------------------------
        //   5  || 8 |   |   ||   |   |   ||   |   | 3 ||
        //      -----------------------------------------
        //   6  ||   |   | 4 || 6 |   | 5 || 1 | 9 |   ||
        //      =========================================
        //   7  ||   |   |   ||   |   |   || 5 | 7 | 2 ||
        //      -----------------------------------------
        //   8  ||   |   |   ||   | 6 | 9 ||   |   | 1 ||
        //      -----------------------------------------
        //   9  ||   | 5 | 3 ||   |   | 2 ||   | 4 | 6 ||
        //      =========================================

        // SOLUTION:
        //         A   B   C    D   E   F    G   H   I
        //      =========================================
        //   1  || 5 | 8 | 7 || 1 | 9 | 6 || 3 | 2 | 4 ||
        //      -----------------------------------------
        //   2  || 3 | 6 | 1 || 2 | 5 | 4 || 7 | 8 | 9 ||
        //      -----------------------------------------
        //   3  || 4 | 2 | 9 || 8 | 3 | 7 || 6 | 1 | 5 ||
        //      =========================================
        //   4  || 9 | 1 | 6 || 3 | 2 | 8 || 4 | 5 | 7 ||
        //      -----------------------------------------
        //   5  || 8 | 7 | 5 || 9 | 4 | 1 || 2 | 6 | 3 ||
        //      -----------------------------------------
        //   6  || 2 | 3 | 4 || 6 | 7 | 5 || 1 | 9 | 8 ||
        //      =========================================
        //   7  || 6 | 9 | 8 || 4 | 1 | 3 || 5 | 7 | 2 ||
        //      -----------------------------------------
        //   8  || 7 | 4 | 2 || 5 | 6 | 9 || 8 | 3 | 1 ||
        //      -----------------------------------------
        //   9  || 1 | 5 | 3 || 7 | 8 | 2 || 9 | 4 | 6 ||
        //      =========================================

        private readonly char[][] _values = 
        {
            new[] {'5','8',' ','1',' ',' ','3','2',' '},
            new[] {'3',' ',' ','2','5',' ',' ',' ',' '},
            new[] {'4','2','9',' ',' ',' ',' ',' ',' '},
            new[] {' ','1','6','3',' ','8','4',' ',' '},
            new[] {'8',' ',' ',' ',' ',' ',' ',' ','3'},
            new[] {' ',' ','4','6',' ','5','1','9',' '},
            new[] {' ',' ',' ',' ',' ',' ','5','7','2'},
            new[] {' ',' ',' ',' ','6','9',' ',' ','1'},
            new[] {' ','5','3',' ',' ','2',' ','4','6'}
        };

        [TestCase("C1",'7')]
        [TestCase("C2",'1')]
        [TestCase("H8",'3')]
        public void Test_This_Is_That(string This, char That)
        {
            const int rowCount = 9;
            const int colCount = 9;
            const int boxRowCount = 3;
            const int boxColCount = 3;

            IGrid grid = MockHelpers.BuildMockGrid(rowCount, colCount, boxRowCount, boxColCount);

            Controller test = new Controller(grid);
            test.Initialise(_values);

            (int col, int row) coords = CoordinateHelpers.GetCoordinates(This);
            ICell cell = grid.Cells.ElementAt(coords.row).ElementAt(coords.col);

            Assert.AreEqual(1, cell.Values.Count);
            Assert.AreEqual(That, cell.Values.Single());
        }
    }
}
