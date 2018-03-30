using System.Collections.Generic;
using System.Linq;

namespace OpenSudoku
{
    internal static class Extensions
    {
        public static void RemoveExcept<T>(this ICollection<T> list, T toRemove)
        {
            IEnumerable<T> itemsToRemove = list.Where(v => !v.Equals(toRemove));
            foreach (T item in itemsToRemove.ToArray()) // ToArray() to force enumeration before the loop
            {
                list.Remove(item);
            }
        }

        public static void UpdateCell(this IGrid grid, ICell cell, char value)
        {
            cell.Values.RemoveExcept(value);
            IReadOnlyCollection<IGroup> groups = grid.Index[cell];

            // update grouped cells...
            ICell[] others = groups
                .Aggregate((IEnumerable<ICell>)new List<ICell>(), (acc, group) => acc.Concat(group.Cells))
                .Distinct()
                .Where(c => c != cell)
                .ToArray();
            foreach (ICell other in others)
            {
                other.Values.Remove(value);
                foreach (IGroup group in grid.Index[other])
                {
                    group.Index[value].Remove(other);
                }
            }

            // ... and indices
            foreach (IGroup group in groups)
            {
                group.Index[value].RemoveExcept(cell);
                foreach (char key in group.Index.Keys)
                {
                    if (key != value)
                        group.Index[key].Remove(cell);
                }
            }
        }
    }

    class Controller
    {
        private readonly IGrid _grid;

        public Controller(IGrid grid)
        {
            _grid = grid;
        }

        public void Initialise(char[][] initialValues)
        {
            Queue<(ICell, char)> queuedUpdates = new Queue<(ICell, char)>();
            for (int row = 0; row < initialValues.Length; row++)
            {
                for (int col = 0; col < initialValues[row].Length; col++)
                {
                    char value = initialValues[row][col];
                    if (value != ' ')
                    {
                        ICell cell = _grid.Cells.ElementAt(row).ElementAt(col);
                        queuedUpdates.Enqueue((cell, value));

                    }
                }
            }

            while (queuedUpdates.Count > 0)
            {
                (ICell cell, char value) = queuedUpdates.Dequeue();
                IGroup[] cellGroups = _grid.Index[cell].ToArray();

                // Remove other values, remove value from other cells in groups, and update group indices to reflect
                _grid.UpdateCell(cell, value);

                // check for single-indexed values that haven't yet been updated
                foreach (IGroup group in cellGroups)
                {
                    foreach (char key in group.Index.Keys)
                    {
                        if (group.Index[key].Count == 1)
                        {
                            ICell groupCell = group.Index[key].ElementAt(0);
                            if (groupCell.Values.Count > 1) // hasn't been updated
                                queuedUpdates.Enqueue((groupCell, key));
                        }
                    }
                }

                // check for single-valued cells that haven't yet been indexed
                IEnumerable<ICell> singleCells = cellGroups
                    .Aggregate((IEnumerable<ICell>)new List<ICell>(), (acc, next) => acc.Concat(next.Cells))
                    .Where(singleCell => singleCell.Values.Count == 1);
                foreach (ICell singleCell in singleCells)
                {
                    char singleValue = singleCell.Values.Single();
                    bool isIndexed = !_grid.Index[singleCell].Any(group => group.Index[singleValue].Count > 1);
                    if (!isIndexed)
                        queuedUpdates.Enqueue((singleCell, singleValue));
                }
            }
        }
    }
}
