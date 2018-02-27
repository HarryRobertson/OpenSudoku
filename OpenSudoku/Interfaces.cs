using System.Collections.Generic;

namespace OpenSudoku
{
    public interface ICell 
    {
        IList<char> Values { get; }
    }

    public interface IGroup 
    {
        ICell[] Cells { get; }
        IDictionary<char, IList<ICell>> Index { get; }
    }

    public interface IGrid 
    {
        ICell[][] Cells { get; }
        IGroup[] Groups { get; }
        IDictionary<ICell, IList<IGroup>> Index { get; }
    }
}
