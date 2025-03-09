#nullable disable

using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassLibrary;

internal static class MainInterface
{
    // --------- Fields ---------
    internal static bool Quit = false;
    internal static string command;

    [JsonPropertyName("MainInterface.Projects")]
    public static HashSet<BasicProject> Projects = [];

    internal static HashSet<string> Commands = 
        [
            "CREATE PROJECT", 
            "OPEN PROJECT",
            "DELETE PROJECT",
            "SHOW STATUS",
            "SAVE STATE",
            "LOAD STATE",
            "EXIT"
        ];


    // --------- Methods ---------
    internal static void CommandLoop()
    {
        Quit = false;
        while (!Quit)
        {      
            InterfaceExtensions.CommandNavigator(ref command, Commands);

            CommandRoute();
        }
        //do you wanna save?
        //bye bye
    }
    internal static void CommandRoute()
    {
        switch (command)
        {
            case "CREATE PROJECT"   : CreateProject()   ; break;
            case "OPEN PROJECT"     : OpenProject()     ; break;
            case "DELETE PROJECT"   : DeleteProject()   ; break;
            case "SHOW STATUS"      : ShowStatus()      ; break;
            case "SAVE STATE"       : Save(); break;
            case "LOAD STATE"       : Load(); break;
            case "EXIT"             : { Save(); Quit = true; }       ; break;
                       
            default: AnsiConsole.MarkupLine("[bold red]{0} was not recognized, please try again.[/]", command); break;
        }
    }
    internal static void CreateProject()
    {
        AnsiConsole.MarkupLine($">> Please provide a [bold yellow]project name[/]");
        string projectName = Console.ReadLine().Trim().ToUpper();

        if (!InterfaceExtensions.IsAlphanumeric(projectName))
        {
            AnsiConsole.MarkupLine("[bold red]Command not valid, CREATE requires a valid project name (A-Z, 0-9, _ ).[/]");
            return;
        }
        //if(projectName == "EXIT") 
        //{   
        //    AnsiConsole.MarkupLine($"The operation is being aborted...");
        //    return;
        //}
        if(Projects.Contains(projectName))
        {
            AnsiConsole.MarkupLine($"[bold red]The project {projectName} already exists.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[bold green]The project[/] [bold yellow]{projectName}[/] [bold green]has been successfully created.[/]");

        BasicProject project = new (projectName);
        Projects.Add(project);
        ProjectInterface.CommandLoop(project);
    }
    internal static void OpenProject()
    {
        if (Projects.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        //string project = InterfaceExtensions.SelectObjectOfType(new HashSet<ISelectable>(Projects.Cast<ISelectable>()));
        BasicProject project = SelectExtensions.SelectObjectOfType(Projects);
        //if (project == "EXIT")
        if (project is null) 
        {   
            AnsiConsole.MarkupLine($"The operation is being aborted...");
            return;
        }
        ProjectInterface.CommandLoop(project);
    }
    internal static void DeleteProject()    
    {
        if (Projects.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        //string project = InterfaceExtensions.SelectObjectOfType(new HashSet<ISelectable>(Projects.Cast<ISelectable>()));
        BasicProject project = SelectExtensions.SelectObjectOfType(Projects);
        //if(project == "EXIT")
        if (project is null)
        {   
            AnsiConsole.MarkupLine($"The operation is being aborted...");
            return;
        }
        if (InterfaceExtensions.AreYouSure()) 
        {
            Projects.Remove(project);
            AnsiConsole.MarkupLine($"[bold green]The project[/] [bold yellow]{project}[/] [bold green]has been successfully deleted.[/]");
        }
        return;
    }
    internal static void ShowStatus()
    {
        Table table = new();
        table.AddColumns(new TableColumn("Project"), new TableColumn("Progress"));
        table.Border(TableBorder.Rounded);

        foreach (BasicProject project in Projects)
        {
            table.AddRow(
                new Markup(project.Name),
                new BreakdownChart()
                    .Width(50)
                    .AddItem(
                        "Done",
                        project.Tasks.Where(x => x.Status == TaskStatus.Completed).Count(),
                        Color.Green
                    )
                    .AddItem(
                        "On Hold",
                        project.Tasks.Where(x => x.Status == TaskStatus.OnHold).Count(),
                        Color.Grey
                    )
                    .AddItem(
                        "In Progress",
                        project.Tasks.Where(x => x.Status == TaskStatus.NotStarted || x.Status == TaskStatus.InProgress).Count(),
                        Color.Red)
            );        
        }
        AnsiConsole.Write(table);
    }
    public static void Save()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve
        };
        string json = JsonSerializer.Serialize(MainInterface.Projects, options); 
 
        File.WriteAllText(InterfaceExtensions.filePath, json);   
    }
    public static void Load()
    {
        if (File.Exists(InterfaceExtensions.filePath))
        {
            string json = File.ReadAllText(InterfaceExtensions.filePath);
            MainInterface.Projects = JsonSerializer.Deserialize<HashSet<BasicProject>>(json);
            AnsiConsole.MarkupLine("[bold green]A state has been loaded.[/]");
        }
    }
}
