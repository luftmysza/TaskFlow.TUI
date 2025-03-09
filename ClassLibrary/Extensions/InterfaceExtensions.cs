#nullable disable

using Spectre.Console;
using System.Collections.Generic;
using ClassLibrary;

namespace ClassLibrary;

public static class InterfaceExtensions
{
    internal static string filePath = "C:\\Users\\drboo\\Downloads\\serialized.json";
    public static void EntryPoint()
    {
        AnsiConsole.Write(
            new FigletText("Project Manager")
                .Centered()
                .Color(Color.Orange1)
            );
        Seed();
        MainInterface.CommandLoop();

        Console.WriteLine("Thank you for using Ptoject Manager!");
        Console.WriteLine("Have a nice day!");
        Console.Read();
    }
    internal static void CommandNavigator(ref string command, HashSet<string> dictionary)
    {
        command = AnsiConsole.Prompt
        (
            new SelectionPrompt<string>()
            .Title(">> What would you like to do?\n>> ---------------------------")
            .PageSize(10)
            .HighlightStyle(Style.Parse("Yellow"))
            .AddChoices(dictionary)
        );
        AnsiConsole.MarkupLine($">> Selected [bold yellow]{command}[/]\n>> ---------------------------");
    }
    internal static bool IsAlphanumeric(string param)    
    {
        if (string.IsNullOrWhiteSpace(param)) return false;
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
                    case "Y"    : return true; 
                    case "Yes"  : return true; 
                    case "N"    : return false; 
                    case "NO"   : return false; 
                }
            }
        }

        return false;
    }
   

    public static void Seed()
    {
        HashSet<Task> tasks = new HashSet<Task>()
        {
            new Task("Create_Jira_Account")
        };
        HashSet<Participant> participants = new HashSet<Participant>()
        {
            new Participant("Harald_Dietrich", tasks),
            new Participant("Me", tasks),
            new Participant("Piotr_Zyd", tasks)
        };
        HashSet<BasicProject> projects = new HashSet<BasicProject>()
        {
            new BasicProject("CSBO", participants, tasks)
        };
        foreach (Task wa_task in tasks)
        {
            foreach (Participant wa_participant in participants)
            {
                wa_task.Assignees.Add(wa_participant);
            }
        }

        MainInterface.Projects.UnionWith(projects);  
    }    
}



