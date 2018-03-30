using System.Collections.Generic;
using System.Linq;

namespace OpenSudoku
{
    internal static class Extensions
    {
        public static void RemoveExcept(this ICollection<char> list, char value)
        {
            List<char> remove = list.Where(v => v != value).ToList();
            foreach (char c in remove)
            {
                list.Remove(c);
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

        public void Initialise(char[][] charray)
        {
            for (int row = 0; row < charray.Length; row++)
            {
                for (int col = 0; col < charray[row].Length; col++)
                {
                    char value = charray[row][col];
                    ICell cell = _grid.Cells.ElementAt(row).ElementAt(col);

                    // set cell value
                    if (value != ' ')
                        cell.Values.RemoveExcept(value);

                    // update grouped cells
                    IReadOnlyCollection<IGroup> groups = _grid.Index[cell];
                    ICell[] others = groups
                        .Aggregate(new List<ICell>(), (l, g) => l.Concat(g.Cells).ToList())
                        .Distinct()
                        .Where(c=>c!=cell)
                        .ToArray();
                    foreach (ICell other in others)
                    {
                        other.Values.Remove(value);
                    }

                    // check for loners
                    foreach (IGroup g in groups)
                    {
                        foreach (char key in g.Index.Keys)
                        {
                            if (g.Index[key].Count == 1)
                                g.Index[key].ElementAt(0).Values.RemoveExcept(key);
                        }
                    }
                }
            }
        }
    }
}
