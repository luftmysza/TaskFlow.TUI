#nullable disable

using Spectre.Console;
using Spectre.Console.Rendering;
using TaskFlow.TUI.Backend.Entities;
using Task = TaskFlow.TUI.Backend.Entities.Task;

namespace TaskFlow.TUI.Backend.UI;

public static class InterfaceExtensions
{
    internal static void RedrawFrame(IRenderable body = null)
    {
        DrawFrame(body, false, false);
    }
    internal static void AppendToFrame(IRenderable body = null)
    {
        DrawFrame(body, true, true);
    }
    private static void DrawFrame(IRenderable body, bool append, bool newLine)
    {
        var appHeader = new FigletText("TaskFlow TUI")
            .Centered()
            .Color(Color.Orange1);
        
        var rule = new Rule();

        if (!append)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(appHeader);
            AnsiConsole.Write(rule);
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
    public static void EntryPoint()
    {
        MainInterface.CommandLoop();

        Console.WriteLine("Have a nice day!");
        Console.Read();
    }
    internal static void CommandNavigator(ref string command, IEnumerable<string> dictionary)
    {   
        RedrawFrame();
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
    internal static bool IsAlphanumeric(string param)    
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

            string response = Console.ReadLine().Trim();
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
    public static void Seed()
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
        foreach (Task wa_task in tasks)
        {
            foreach (Participant wa_participant in participants)
            {
                wa_task.Assignees.Add(wa_participant);
            }
        }

        MainInterface.Projects.AddRange(projects);  
    }    
}