#nullable disable

using Spectre.Console;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace ClassLibrary;

internal class Task : IEquatable<Task>, ISelectable
{
    // --------- Fields ---------
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Assignees")]
    public HashSet<Participant> Assignees { get; set; }

    [JsonPropertyName("Status")]
    public TaskStatus Status{ get; set; }
    private static readonly Dictionary<TaskStatus, string> TaskStatusStrings = new()
    {
        { TaskStatus.NotStarted, "Not Started" },
        { TaskStatus.InProgress, "In Progress" },
        { TaskStatus.Completed, "Completed" },
        { TaskStatus.OnHold, "On Hold" },
        { TaskStatus.Cancelled, "Cancelled" }
    };

    // --------- Constructors ---------
    public Task(string name, HashSet<Participant> assignees = null, TaskStatus status = TaskStatus.NotStarted) 
    {
        Name = name;
        
        if (assignees is null) Assignees = [];
        
        else Assignees = assignees;   
        Status = status;
    }



    // --------- Overrides ---------
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    public override string ToString()
    {
        return this.Name;
    }



    // --------- Interface Implementations ---------
    public bool Equals(Task other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (this.Name == null || other.Name == null) return false;

        return Name.Equals(other.Name);
    }



    // --------- Methods ---------
    internal void TaskAssigneeAdd(string participant = null)
    {
        if (this.Status == TaskStatus.Completed && this.Status == TaskStatus.Cancelled)
        {
            AnsiConsole.MarkupLine("[bold blue]The task can no longer be changed[/]");
        }
        if (ProjectInterface.CurrentProject.Participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        List<Participant> participants = SelectExtensions.SelectMultipleOfType
            (ProjectInterface.CurrentProject.Participants, Assignees);

        if (participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]Nothing found[/]");
            return;
        }
        
        foreach (Participant wa_participant in participants)
        {
            wa_participant.AddTask(this.Name);
            Assignees.Add(wa_participant);
        }

        //foreach (Participant wa_participant in participants)
        //{
        //    Participant participantCast = ProjectInterface.CurrentProject.Participants.FirstOrDefault(x => x.Name == wa_participant);
        //    Assignees.Add(participantCast);
        //    participantCast.AddTask(this.Name);
        //}

        this.Status = TaskStatus.InProgress;
        AnsiConsole.MarkupLine("[bold green]The participants have been added to the task.[/]");
    }
    internal void TaskAssigneeRemove(string participant = null)
    {
        if (this.Status == TaskStatus.Completed && this.Status == TaskStatus.Cancelled)
        {
            AnsiConsole.MarkupLine("[bold blue]The task can no longer be changed[/]");
        }
        if (ProjectInterface.CurrentProject.Participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        List<Participant> participants = SelectExtensions.SelectMultipleOfType(this.Assignees);

        if (participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]Nothing found[/]");
            return;
        }

        foreach (Participant wa_participant in participants)
        {
            Assignees.Remove(wa_participant);
            wa_participant.RemoveTask(this.Name);
        }

        //Status = TaskStatus.InProgress;
        AnsiConsole.MarkupLine($"[bold green]The participants have been removed from the task.[/]");
    }
    internal void TaskStatusChange()
    {
        string newStatus = SelectExtensions.SelectEnum(TaskStatusStrings);
        TaskStatus newStatusCast = TaskStatusStrings.FirstOrDefault(kvp => kvp.Value == newStatus).Key;
        if (Status == newStatusCast)
        {
            AnsiConsole.MarkupLine($"[bold blue]The task [/][bold yellow]{this}[/][bold blue] is already [bold yellow]{newStatus}[/].[/]");
            return;
        }
        if (newStatusCast != TaskStatus.OnHold && ((int)Status > (int)newStatusCast || (int)Status >= 100))
        {
            AnsiConsole.MarkupLine($"[bold blue]The selected status is no longer applicable to this task.[/]");
            return;
        }
        this.Status = newStatusCast;
        AnsiConsole.MarkupLine($"[bold green]The task status has been set to [bold yellow]{newStatus}[/].[/]");
    }
    internal void TaskStatusSetCompleted()
    {
        if(Status == TaskStatus.Cancelled)
        {
            AnsiConsole.MarkupLine($"[bold blue]The task [/][bold yellow]{this}[/][bold blue] is already cancelled.[/]");
            return;
        }
        if (Status == TaskStatus.Completed)
        {
            AnsiConsole.MarkupLine($"[bold blue]The task [/][bold yellow]{this}[/][bold blue] has been already completed.[/]");
            return;
        }
        Status = TaskStatus.Completed;
        Assignees.Clear();
        AnsiConsole.MarkupLine($"[bold green]The task [/][bold yellow]{this}[/][bold green] is completed.[/]");
    }



    // --------- Operators ---------
    public static implicit operator Task(string name)
    { 
        return new Task(name);
    }
}