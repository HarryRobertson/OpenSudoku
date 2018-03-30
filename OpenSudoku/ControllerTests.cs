using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void Test_H8_Is3()
        {
            IEnumerable<int> enumerable = Enumerable.Range(1,9);
            IEnumerable<char> @select = enumerable.Select(i => Convert.ToChar(i.ToString()));
            char[] cellValues = @select.ToArray();
            ICell Cell(int row, int col) => MockHelpers.BuildMockCell(cellValues, row, col);
            ICell[][] cells = new ICell[6][];
            cells[0] = new[] { Cell(0,6) };
            cells[1] = new[] { Cell(3,6) };
            cells[2] = new[] { Cell(5,6) };
            cells[3] = new[] { Cell(6,6), Cell(6,7), Cell(6,8) };
            cells[4] = new[] { Cell(7,6), Cell(7,7), Cell(7,8) };
            cells[5] = new[] { Cell(8,6), Cell(8,7), Cell(8,8) };

            IGroup[] groups = new IGroup[2];
            groups[0] = MockHelpers.BuildMockGroup(new[]{cells[0][0], cells[1][0], cells[2][0], cells[3][0], cells[4][0], cells[5][0]});
            groups[1] = MockHelpers.BuildMockGroup(new[]
            {
                cells[3][0], cells[3][1], cells[3][2],
                cells[4][0], cells[4][1], cells[4][2],
                cells[5][0], cells[5][1], cells[5][2]
            });

            IGrid grid = MockHelpers.BuildMockGrid(cells, groups);

            Controller test = new Controller(grid);

            test.Initialise(new[]
            {
                new[] {'3'}, new[] {'4'}, new[] {'1'},
                new[] {'5', '7', '2'}, new[] {' ', ' ', '1'}, new[] {' ', '4', '6'}
            });

            char H8;
            try
            {
                H8 = grid.Cells.ElementAt(4).ElementAt(1).Values.Single(v => v != ' ');
            }
            catch (InvalidOperationException)
            {
                H8 = ' ';
            }

            Assert.AreEqual('3', H8);
        }

        [TestCase("C2",'1')]
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
