using System.Text.Json.Serialization;
using Spectre.Console;
using TaskFlow.TUI.Backend.UI;
using TaskFlow.TUI.Backend.Contracts;
using TaskFlow.TUI.Backend.Tools;

namespace TaskFlow.TUI.Backend.Entities;

internal static class ProjectFactory
{
    internal static List<BasicProject> Projects = [];
    internal static void CreateProject()
    {
        TextPrompt<string> prompt = new TextPrompt<string>($">> Please provide a [bold Gold1]project name[/]: ");
        string projectName = AnsiConsole.Prompt(prompt);

        Markup msg;

        if (!InterfaceExtensions.IsAlphanumeric(projectName))
        {
            msg = new Markup("[bold OrangeRed1]Command not valid, CREATE requires a valid project name (A-Z, 0-9, _ ).[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        if (Projects.Contains(projectName))
        {
            msg = new Markup("[bold OrangeRed1]The project {projectName} already exists.[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }

        BasicProject project = new (projectName);
        Projects.Add(project);

        msg = new Markup($"The project [bold Gold1]{projectName}[/] has been successfully created.");
        Logger.Enqueue(msg);
        InterfaceExtensions.DrawFrame(msg);
        
        ProjectInterface.CommandLoop(project);
    }
    internal static void OpenProject()
    {
        Markup msg;
        if (Projects.Count == 0)
        {
            msg = new Markup("[bold OrangeRed1]No objects found[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        BasicProject project = ISelectableExtensions.SelectObjectOfType(Projects);
        if (project is null) 
        {   
            msg = new Markup("[bold OrangeRed1]Something went wrong, the operation is being aborted[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        ProjectInterface.CommandLoop(project);
    }
    internal static void DeleteProject()    
    {
        if (Projects.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold OrangeRed1]No objects found[/]");
            return;
        }
        BasicProject project = ISelectableExtensions.SelectObjectOfType(Projects);
        if (project is null)
        {   
            AnsiConsole.MarkupLine($"The operation is being aborted...");
            return;
        }
        if (InterfaceExtensions.AreYouSure()) 
        {
            Projects.Remove(project);
            AnsiConsole.MarkupLine($"The project [bold Gold1]{project}[/] has been successfully deleted.");
        }
        return;
    }
}
