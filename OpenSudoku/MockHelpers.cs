using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;

namespace OpenSudoku
{
    internal class MockHelpers
    {
        public static ICell BuildMockCell(char[] vals, int row, int col)
        {
            Mock<ICell> mock = new Mock<ICell>();
            mock.SetupGet(m => m.Values).Returns(vals.ToList());
            mock.Setup(m => m.ToString()).Returns(CoordinateHelpers.GetAlphanumericCoordinates(col, row));
            return mock.Object;
        }

        public static IGroup BuildMockGroup(ICell[] cells)
        {
            Mock<IGroup> mock = new Mock<IGroup>();
            mock.SetupGet(m => m.Cells).Returns(cells.ToArray());
            mock.SetupGet(m => m.Index).Returns(BuildIndex(cells));
            return mock.Object;
        }

        private static IReadOnlyDictionary<char, ICollection<ICell>> BuildIndex(ICell[] cells)
        {
            Dictionary<char, ICollection<ICell>> index = new Dictionary<char, ICollection<ICell>>();
            foreach (ICell cell in cells)
            {
                foreach (char value in cell.Values)
                {
                    if (!index.ContainsKey(value))
                        index.Add(value, new Collection<ICell>());
                    index[value].Add(cell);
                }
            }   
            return new ReadOnlyDictionary<char, ICollection<ICell>>(index);
        }

        public static IGrid BuildMockGrid(ICell[][] cells, IGroup[] groups)
        {
            Mock<IGrid> mock = new Mock<IGrid>();
            mock.SetupGet(m => m.Cells).Returns(cells);
            mock.SetupGet(m => m.Groups).Returns(groups);
            mock.SetupGet(m => m.Index).Returns(BuildIndex(cells, groups));
            return mock.Object;
        }

        private static IReadOnlyDictionary<ICell, IReadOnlyCollection<IGroup>> BuildIndex(ICell[][] cells, IGroup[] groups)
        {
            Dictionary<ICell, IReadOnlyCollection<IGroup>> index = new Dictionary<ICell, IReadOnlyCollection<IGroup>>();
            foreach (ICell[] row in cells)
            {
                foreach (ICell cell in row)
                {
                    ICollection<IGroup> cellGroups = new Collection<IGroup>();
                    foreach (IGroup group in groups)
                    {
                        if (group.Cells.Contains(cell))
                            cellGroups.Add(group);
                    }
                    index.Add(cell, cellGroups.ToArray());
                }
            }
            return new ReadOnlyDictionary<ICell, IReadOnlyCollection<IGroup>>(index);
        }

        public static IGrid BuildMockGrid(int rowCount, int colCount, int boxRowCount, int boxColCount)
        {
            char[] cellValues = Enumerable.Range(1, 9).Select(i => Convert.ToChar(i.ToString())).ToArray();
            ICell[][] cells = Enumerable.Range(0, rowCount)
                .Select(row => Enumerable.Range(0, colCount).Select(col => BuildMockCell(cellValues, row, col)).ToArray())
                .ToArray();

            IGroup[] groups;
            {
                IEnumerable<IGroup> rows = Enumerable.Range(0, rowCount)
                    .Select(row => BuildMockGroup(Enumerable.Range(0, colCount).Select(col => cells[row][col]).ToArray()));
                IEnumerable<IGroup> cols = Enumerable.Range(0, colCount)
                    .Select(col => BuildMockGroup(Enumerable.Range(0, rowCount).Select(row => cells[row][col]).ToArray()));

                IEnumerable<(int, int)> boxCoordinates = Enumerable
                    .Range(0, rowCount * colCount / (boxRowCount * boxColCount))
                    .Select(box => (box / boxRowCount, box % boxColCount));
                IEnumerable<IGroup> boxes = boxCoordinates.Select(box =>
                    BuildMockGroup(Enumerable.Range(0, boxRowCount)
                        .Select(row => Enumerable.Range(0, boxColCount).Select(col => cells[box.Item1 * boxRowCount + row][box.Item2 * boxColCount + col]))
                        .Aggregate((IEnumerable<ICell>) new List<ICell>(), (acc, row) => acc.Concat(row)).ToArray()));

                groups = boxes.Concat(cols).Concat(rows).ToArray();
            }

            return BuildMockGrid(cells, groups);
        }
    }
}