using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using NUnit.Framework;
using Moq;

namespace OpenSudoku
{
    [TestFixture]
    public class ControllerTests
    {
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

        private static ICell BuildMockCell(char[] vals)
        {
            Mock<ICell> mock = new Mock<ICell>();
            mock.SetupGet(m => m.Values).Returns(vals.ToList());
            return mock.Object;
        }

        private static IGroup BuildMockGroup(ICell[] cells)
        {
            Mock<IGroup> mock = new Mock<IGroup>();
            mock.SetupGet(m => m.Cells).Returns(cells);

            IDictionary<char, IList<ICell>> Index()
            {
                Dictionary<char, IList<ICell>> result = new Dictionary<char, IList<ICell>>();
                foreach (ICell cell in mock.Object.Cells)
                {
                    foreach (char value in cell.Values)
                    {
                        if (!result.ContainsKey(value))
                            result[value] = new List<ICell>();
                        result[value].Add(cell);
                    }
                }
                return result;
            }

            mock.SetupGet(m => m.Index).Returns(Index);

            return mock.Object;
        }

        private static IGrid BuildMockGrid(ICell[][] cells, IGroup[] groups)
        {
            Mock<IGrid> mock = new Mock<IGrid>();
            mock.SetupGet(m => m.Cells).Returns(cells);
            mock.SetupGet(m => m.Groups).Returns(groups);

            Dictionary<ICell, IList<IGroup>> Index()
            {
                Dictionary<ICell, IList<IGroup>> result = new Dictionary<ICell, IList<IGroup>>();
                foreach (ICell[] row in mock.Object.Cells)
                {
                    foreach (ICell cell in row)
                    {
                        result[cell] = mock.Object.Groups.Where(g => g.Cells.Contains(cell)).ToList();
                    }
                }
                return result;
            }

            mock.SetupGet(m => m.Index).Returns(Index);
            return mock.Object;
        }

        [Test]
        public void Test_H8_Is3()
        {
            Func<ICell> fullCell = () => BuildMockCell(new [] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            ICell[][] cells = new ICell[6][];
            cells[0] = new[] { fullCell() };
            cells[1] = new[] { fullCell() };
            cells[2] = new[] { fullCell() };
            cells[3] = new[] { fullCell(), fullCell(), fullCell() };
            cells[4] = new[] { fullCell(), fullCell(), fullCell() };
            cells[5] = new[] { fullCell(), fullCell(), fullCell() };

            IGroup[] groups = new IGroup[2];
            groups[0] = BuildMockGroup(new[]{cells[0][0], cells[1][0], cells[2][0], cells[3][0], cells[4][0], cells[5][0]});
            groups[1] = BuildMockGroup(new[]
            {
                cells[3][0], cells[3][1], cells[3][2],
                cells[4][0], cells[4][1], cells[4][2],
                cells[5][0], cells[5][1], cells[5][2]
            });

            IGrid grid = BuildMockGrid(cells, groups);

            Controller test = new Controller(grid);

            test.Initialise(new[]
            {
                new[] {'3'}, new[] {'4'}, new[] {'1'},
                new[] {'5', '7', '2'}, new[] {' ', ' ', '1'}, new[] {' ', '4', '6'}
            });

            char H8;
            try
            {
                H8 = grid.Cells[4][1].Values.Single(v => v != ' ');
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
            const int r = 9;
            const int c = 9;
            const int br = 3;
            const int bc = 3;

            Func<ICell> fullCell = () => BuildMockCell(new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            ICell[][] cells = new ICell[r][];
            for (int i = 0; i < cells.Length; i++)
            {
                ICell[] row = cells[i] = new ICell[c];
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = fullCell();
                }
            }
            IGroup[] rows = new IGroup[r];
            for (int i = 0; i < rows.Length; i++)
            {
                ICell[] groupCells = new ICell[c];
                for (int j = 0; j < groupCells.Length; j++)
                {
                    groupCells[j] = cells[i][j];
                }
                rows[i] = BuildMockGroup(groupCells);
            }

            IGroup[] cols = new IGroup[c];
            for (int i = 0; i < cols.Length; i++)
            {
                ICell[] groupCells = new ICell[r];
                for (int j = 0; j < groupCells.Length; j++)
                {
                    groupCells[j] = cells[j][i];
                }
                cols[i] = BuildMockGroup(groupCells);
            }

            IGroup[] boxes = new IGroup[r*c/(br*bc)];
            for (int i = 0; i < boxes.Length; i++)
            {
                int row = i / br;
                int col = i % bc;

                ICell[] groupCells = new ICell[br*bc];
                for (int j = 0; j < br; j++)
                {
                    for (int k = 0; k < bc; k++)
                    {
                        groupCells[3 * j + k] = cells[j + row][k + col];
                    }
                }
                boxes[i] = BuildMockGroup(groupCells);
            }

            IGroup[] groups = boxes.ToList().Concat(cols.ToList()).Concat(rows.ToList()).ToArray();

            IGrid grid = BuildMockGrid(cells, groups);

            Controller test = new Controller(grid);
            test.Initialise();

            //GetCoordinate(This).X;
        }

        private static (int X, int Y) GetCoordinate(string s)
        {
            if (s.Length != 2) throw new ArgumentException($"s.Length was {s.Length}, must be 2.");

            char i = s[0];
            char j = s[1];

            int x = new List<char> {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'}.IndexOf(i);
            int y = int.Parse(j.ToString()) - 1;
            return (x, y);
        }
    }
}
