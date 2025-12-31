#nullable disable

using Spectre.Console;

using TaskFlow.TUI.Backend.Entities;
using Task = TaskFlow.TUI.Backend.Entities.Task;

namespace TaskFlow.TUI.Backend.UI;

internal class TaskInterface
{
    internal static bool Quit = false;
    internal static string command;
    internal static Task CurrentTask;
    internal static List<string> Commands =
    [
        "ADD ASSIGNEE", 
        "REMOVE ASSIGNEE",
        "CHANGE STATUS",
        "SET STATUS COMPLETED",
        "EXIT"
    ];

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
