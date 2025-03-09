#nullable disable

using System.Security;
using Spectre.Console;

namespace ClassLibrary;

internal class TaskInterface
{
    // --------- Fields ---------
    internal static bool Quit = false;
    internal static string command;
    internal static Task CurrentTask;
    internal static HashSet<string> Commands =
    [
        "ADD ASSIGNEE", 
        "REMOVE ASSIGNEE",
        "CHANGE STATUS",
        "SET STATUS COMPLETED",
        "EXIT"
    ];

    // --------- Methods ---------
    internal static void CommandLoop(Task task)
    {   
        Quit = false;
        CurrentTask = task;
        while (!Quit) 
        {
            Console.WriteLine($">> PROJECT <{ProjectInterface.CurrentProject}> : TASK <{CurrentTask}>");

            InterfaceExtensions.CommandNavigator(ref command, Commands);

            CommandRoute();
        }
    }
    internal static void CommandRoute()
    {
        switch (command)
        {
            case "ADD ASSIGNEE": CurrentTask.TaskAssigneeAdd(); break;
            case "REMOVE ASSIGNEE": CurrentTask.TaskAssigneeRemove(); break;
            case "CHANGE STATUS": CurrentTask.TaskStatusChange(); break;
            case "SET STATUS COMPLETED": CurrentTask.TaskStatusSetCompleted(); break;
            case "EXIT": Quit = true; break;

            default: Console.WriteLine("{0} was not recognized, please try again.", command); break;
        }

    }
}
