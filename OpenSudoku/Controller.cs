using System.Collections.Generic;
using System.Linq;

namespace OpenSudoku
{
    internal static class Extensions
    {
        public static void RemoveExcept(this IList<char> list, char value)
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
            for (int i = 0; i < charray.Length; i++)
            {
                for (int j = 0; j < charray[i].Length; j++)
                {
                    char value = charray[i][j];
                    ICell cell = _grid.Cells[i][j];

                    // set cell value
                    if (value != ' ')
                        cell.Values.RemoveExcept(value);

                    // update grouped cells
                    IList<IGroup> groups = _grid.Index[cell];
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
                                g.Index[key][0].Values.RemoveExcept(key);
                        }
                    }
                }
            }
        }
    }
}
