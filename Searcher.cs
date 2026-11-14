using System.Collections.Generic;

class Searcher
{
    public List<int[]> FindAll(Node tree, string s)
    {
        var result = new List<int[]>();
        int i = 0, end = -1;
        while (i < s.Length)
        {
            if (tree.Match(s, i, x => { end = x; return true; }) && end > i)
            {
                result.Add(new int[] { i, end - i });
                i = end;
            }
            else i++;
        }
        return result;
    }
}