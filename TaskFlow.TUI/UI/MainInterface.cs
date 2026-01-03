#nullable disable

using Spectre.Console;
using System.Text.Json.Serialization;
using TaskFlow.TUI.Entities;
using TaskFlow.TUI.Tools;
using TaskStatus = TaskFlow.TUI.Entities.TaskStatus;

namespace TaskFlow.TUI.UI;

public static class MainInterface
{
    internal static bool Quit = false;
    internal static string command;
    [JsonPropertyName("MainInterface.Projects")]
    // internal static List<BasicProject> Projects = [];
    internal static List<string> Commands = 
        [
            "CREATE PROJECT", 
            "OPEN PROJECT",
            "DELETE PROJECT",
            "SHOW STATUS",
            "SAVE STATE",
            "LOAD STATE",
            "EXIT"
        ];
    public static void RunApp()
    {
        InterfaceExtensions.Seed();
        Quit = false;
        while (!Quit)
        {      
            InterfaceExtensions.CommandNavigator(ref command, Commands);

            CommandRoute();
        }
        Console.WriteLine("Have a nice day!");
    }
    internal static void CommandRoute()
    {
        switch (command)
        {
            case "CREATE PROJECT"   : ProjectFactory.CreateProject();       break;
            case "OPEN PROJECT"     : ProjectFactory.OpenProject();         break;
            case "DELETE PROJECT"   : ProjectFactory.DeleteProject();       break;
            case "SHOW STATUS"      : ShowStatus();                         break;
            case "SAVE STATE"       : Save();                               break;
            case "LOAD STATE"       : Load();                               break;
            case "EXIT"             : {Quit = true;}                        break;

            default: 
                Markup msg = new Markup("[bold OrangeRed1]{0} was not recognized, please try again.[/]", command);
                Logger.Enqueue(msg);
                break;
        }
    }
    internal static void ShowStatus()
    {
        Table table = new();
        table.AddColumns(new TableColumn("Project"), new TableColumn("Progress"));
        table.Border(TableBorder.Rounded);

        foreach (BasicProject project in ProjectFactory.Projects)
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
        InterfaceExtensions.DrawLoopFrame(table);
    }
    internal static void Save()
    {
        //JsonSerializerOptions options = new JsonSerializerOptions
        //{
        //    WriteIndented = true,
        //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //    ReferenceHandler = ReferenceHandler.Preserve
        //};
        //string json = JsonSerializer.Serialize(MainInterface.Projects, options);

        //File.WriteAllText(InterfaceExtensions.filePath, json);
    }
    internal static void Load()
    {
        //if (File.Exists(InterfaceExtensions.filePath))
        //{
        //    string json = File.ReadAllText(InterfaceExtensions.filePath);
        //    MainInterface.Projects = JsonSerializer.Deserialize<HashSet<BasicProject>>(json);
        //    AnsiConsole.MarkupLine("[bold green]A state has been loaded.[/]");
        //}
    }
}
