using System;
using Microsoft.VisualBasic;
using Spectre.Console;

namespace TaskFlow.TUI.Backend.Tools;

public static class Logger
{
    private static Queue<Markup> _messages = new Queue<Markup>();
    public static Rows MessageDump 
    { 
        get
        {
            var rows = new List<Markup>();
            foreach (Markup msg in _messages)
                rows.Add(msg);
            return new Rows(rows);
        }
    }

    public static void Enqueue(Markup msg)
    {
        if (_messages.Count == 5)
            _messages.Dequeue();
        _messages.Enqueue(msg);
    }
}
