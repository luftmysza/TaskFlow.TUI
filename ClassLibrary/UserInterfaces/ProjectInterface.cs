#nullable disable

using Spectre.Console;

namespace ClassLibrary;

internal class ProjectInterface 
{
    // --------- Fields ---------
    internal static bool Quit = false;
    internal static string command;
    internal static BasicProject CurrentProject;
    internal static HashSet<string> Commands =
        [
            "ADD PARTICIPANT", 
            "REMOVE PARTICIPANT",
            "LIST PARTICIPANTS",
            "ADD TASK",
            "OPEN TASK",
            "REMOVE TASK",
            "SHOW STATUS",
            "EXIT"
        ];


    // --------- Methods ---------
    internal static void CommandLoop(BasicProject Project)
    {   
        Quit = false;
        CurrentProject = Project;
        while (!Quit)
        {
            AnsiConsole.WriteLine($">> PROJECT <{CurrentProject}>");
            
            InterfaceExtensions.CommandNavigator(ref command, Commands);
            
            CommandRoute();
        }
    }
    internal static void CommandRoute()
    {
        switch (command)
        {
            case "ADD PARTICIPANT": CurrentProject.AddParticipant(); break;
            case "REMOVE PARTICIPANT": CurrentProject.RemoveParticipants(); break;
            case "LIST PARTICIPANTS": CurrentProject.ListParticipants(); break;
            case "ADD TASK": CurrentProject.AddTask(); break;
            case "OPEN TASK": CurrentProject.OpenTask(); break;
            case "REMOVE TASK": CurrentProject.RemoveTask(); break;
            case "SHOW STATUS": CurrentProject.ShowStatus(); break;
            case "EXIT": Quit = true; break;

            default: AnsiConsole.WriteLine("{0} was not recognized, please try again.", command); break;
        }
    }
}
