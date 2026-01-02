using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;
using TaskFlow.TUI.Backend.Entities;
using TaskFlow.TUI.Backend.Tools;
using Task = TaskFlow.TUI.Backend.Entities.Task;

namespace TaskFlow.TUI.Backend.UI;

public static class InterfaceExtensions
{
    // --------- Navigator ---------
    internal static void CommandNavigator(ref string command, IEnumerable<string> dictionary)
    {   
        DrawFrame();
        command = AnsiConsole.Prompt
        (
            new SelectionPrompt<string>()
            .Title(">> What would you like to do?")
            .PageSize(10)
            .HighlightStyle(Style.Parse("Gold1"))
            .AddChoices(dictionary)
        );
        
        Markup body = new Markup($">> Selected [bold Gold1]{command}[/]");
        AppendToFrame(body);
    }



    // --------- Frame Management ---------
    public static void DrawLoopFrame(IRenderable body)
    {
        Markup hint = new Markup("[Grey35]Press [Italic]q[/] | [Italic]Esc[/] | [Italic]Enter[/] to close...[/]"); 
        IEnumerable<ConsoleKey> escKeys = new List<ConsoleKey>() {ConsoleKey.Escape, ConsoleKey.Enter, ConsoleKey.Q};

        AppendToFrame(body);
        Console.WriteLine();
        AppendToFrame(hint);
        
        while (true)
        {
            ConsoleKeyInfo keyRead = Console.ReadKey(true);
            if (escKeys.Contains(keyRead.Key))
                return;
        }
    }
    public static void DrawFrame(IRenderable? body = null)
    {
        _drawFrame(body, false, false);
    }
    public static void AppendToFrame(IRenderable body)
    {
        _drawFrame(body, true, true);
    }
    public static void PrepareInputFrame(IRenderable body)
    {
        _drawFrame(body, true, false);
    }
    private static void _drawFrame(IRenderable? body, bool append, bool newLine)
    {
        var appHeader = new FigletText("TaskFlow.TUI")
            .Centered()
            .Color(Color.Orange1);
        
        var appPanel = new Panel(Logger.MessageDump)
            { Height = 7 }
            .Header("Messages")
            .RoundedBorder()
            .Expand();

        var appRule = new Rule();

        if (!append)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(appHeader);
            AnsiConsole.Write(appRule);
            AnsiConsole.Write(appPanel);
        }
        AnsiConsole.Write(body ?? new Markup(string.Empty));

        if (newLine)
                Console.WriteLine();
        
        // (int x, int y) = Console.GetCursorPosition();
        // if (body is not null)
        //     if (newLine)
        //         Console.WriteLine();
        //     else
        //         Console.SetCursorPosition(x + ((Markup)body).Length, y);
    }



    // --------- Tools ---------
    public static bool IsAlphanumeric(string param)    
    {
        if (string.IsNullOrWhiteSpace(param))
            return false;
        for (int i = 0; i < param.Length; i++)
            if (!char.IsDigit(param[i]) && !char.IsLetter(param[i]) && param[i] != '_') return false;

        return true;
    }
    public static bool AreYouSure()
    {
        bool quit = false;
       
        while (!quit) 
        {
            Console.WriteLine("Do you want to proceed? The changes are not reversible. [Y/N]");

            string response = Console.ReadLine()?.Trim()!;
            string[] validInput = ["Y", "N", "YES", "NO"];
                    
            if (validInput.Contains(response.ToUpper()))
            {
                switch (response.ToUpper())
                {
                    case "Y"    :
                    case "y"    :
                    case "yes"  :
                    case "Yes"  : return true; 
                    case "N"    :
                    case "n"    :
                    case "no"   :
                    case "No"   : return true; 
                }
            }
        }

        return false;
    }
    internal static void Seed()
    {
        IEnumerable<Task> tasks = new List<Task>()
        {
            new Task("Create_Jira_Account")
        };
        IEnumerable<Participant> participants = new List<Participant>()
        {
            new Participant("John Doe 1", tasks),
            new Participant("John Doe 2", tasks),
            new Participant("John Doe 3", tasks)
        };
        IEnumerable<BasicProject> projects = new List<BasicProject>()
        {
            new BasicProject("CUSTOMER_SUPPORT", participants, tasks),
        };
        foreach (Task task in tasks)
        {
            task.Assignees.AddRange(participants);
        }
        ProjectFactory.Projects.AddRange(projects);  
    }    
}