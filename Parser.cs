using System;
using System.Collections.Generic;

class RegexParseException : Exception
{
    public RegexParseException(string message) : base(message) { }
}

class Parser
{
    private string p;
    private int pos;

    public Node Parse(string expr)
    {
        p = expr;
        pos = 0;
        Node tree = ParseAlt();
        if (pos < p.Length) throw new RegexParseException("лишняя закрывающая скобка ')'");
        return tree;
    }

    private char Cur()
    {
        return pos < p.Length ? p[pos] : '\0';
    }

    private bool IsQuant(char c)
    {
        return c == '*' || c == '+' || c == '?';
    }

    private Node ParseAlt()
    {
        var list = new List<Node> { ParseSeq() };
        while (Cur() == '|')
        {
            pos++;
            list.Add(ParseSeq());
        }
        return new AltNode(list);
    }

    private Node ParseSeq()
    {
        var list = new List<Node>();
        while (pos < p.Length && Cur() != '|' && Cur() != ')') list.Add(ParseItem());
        if (list.Count == 0) throw new RegexParseException("пустая альтернатива или пустые скобки");
        return new SeqNode(list);
    }

    private Node ParseItem()
    {
        char c = Cur();
        Node n;
        if (IsQuant(c)) throw new RegexParseException("квантор '" + c + "' не к чему применить");
        pos++;
        if (c == '(')
        {
            n = ParseAlt();
            if (Cur() != ')') throw new RegexParseException("не закрыта скобка '('");
            pos++;
        }
        else if (c == '\\')
        {
            if (pos >= p.Length) throw new RegexParseException("символ '\\' в конце выражения");
            n = new CharNode(p[pos], false);
            pos++;
        }
        else n = new CharNode(c, c == '.');

        if (IsQuant(Cur()))
        {
            n = new QuantNode(n, Cur());
            pos++;
            if (IsQuant(Cur())) throw new RegexParseException("повторный квантор '" + Cur() + "'");
        }
        return n;
    }
}