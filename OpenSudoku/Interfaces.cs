using System.Collections.Generic;

namespace OpenSudoku
{
    public interface ICell 
    {
        ICollection<char> Values { get; }
    }

    public interface IGroup 
    {
        IReadOnlyCollection<ICell>  Cells { get; }
        IReadOnlyDictionary<char, ICollection<ICell>> Index { get; }
    }

    public interface IGrid 
    {
        IReadOnlyCollection<IReadOnlyCollection<ICell>> Cells { get; }
        IReadOnlyCollection<IGroup> Groups { get; }
        IReadOnlyDictionary<ICell, IReadOnlyCollection<IGroup>> Index { get; }
    }
}
