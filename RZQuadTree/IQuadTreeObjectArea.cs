using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZQuadTree
{
    public interface IQuadTreeObjectArea<in T>
    {
        int GetTop(T obj);

        int GetBottom(T obj);

        int GetLeft(T obj);

        int GetRight(T obj);
    }
}
