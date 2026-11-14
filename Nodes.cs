using System;
using System.Collections.Generic;

abstract class Node
{
    public abstract bool Match(string s, int i, Func<int, bool> k);
}

class CharNode : Node
{
    private char c;
    private bool any;

    public CharNode(char c, bool any)
    {
        this.c = c;
        this.any = any;
    }

    public override bool Match(string s, int i, Func<int, bool> k)
    {
        return i < s.Length && (any || s[i] == c) && k(i + 1);
    }
}

class SeqNode : Node
{
    private List<Node> items;

    public SeqNode(List<Node> items)
    {
        this.items = items;
    }

    public override bool Match(string s, int i, Func<int, bool> k)
    {
        return MatchFrom(0, s, i, k);
    }

    private bool MatchFrom(int j, string s, int i, Func<int, bool> k)
    {
        if (j == items.Count) return k(i);
        return items[j].Match(s, i, e => MatchFrom(j + 1, s, e, k));
    }
}

class AltNode : Node
{
    private List<Node> variants;

    public AltNode(List<Node> variants)
    {
        this.variants = variants;
    }

    public override bool Match(string s, int i, Func<int, bool> k)
    {
        foreach (Node v in variants)
            if (v.Match(s, i, k)) return true;
        return false;
    }
}

class QuantNode : Node
{
    private Node child;
    private int min, max;

    public QuantNode(Node child, char op)
    {
        this.child = child;
        min = op == '+' ? 1 : 0;
        max = op == '?' ? 1 : int.MaxValue;
    }

    public override bool Match(string s, int i, Func<int, bool> k)
    {
        return Repeat(0, s, i, k);
    }

    private bool Repeat(int cnt, string s, int i, Func<int, bool> k)
    {
        if (cnt < max && child.Match(s, i, e => (e == i && cnt >= min) ? false : Repeat(cnt + 1, s, e, k)))
            return true;
        return cnt >= min && k(i);
    }
}