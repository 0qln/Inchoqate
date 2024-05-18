using System.Collections;

namespace Inchoqate.Miscellaneous
{
    public static class ListExtensions
    {
        public static void Pop(this IList list)
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}
