using Spectre.Console;
using System.Text.Json.Serialization;

using TaskFlow.TUI.UI;
using TaskFlow.TUI.Contracts;
using TaskFlow.TUI.Tools;

namespace TaskFlow.TUI.Entities;

internal class BasicProject : IEquatable<BasicProject>, ISelectable
{
    // --------- Fields ---------
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Participants")]
    public List<Participant> Participants { get; set; }

    [JsonPropertyName("Tasks")]
    public List<Task> Tasks { get; set; }
    // public ProjectStatus Status{ get; set; }



    // --------- Constructors ---------
    public BasicProject(string name, IEnumerable<Participant>? participants = null, IEnumerable<Task>? tasks = null)
    {
        Name = name;
        
        if (participants is null) Participants = [];
        else Participants = participants.ToList();
        
        if (tasks is null) Tasks = [];
        else Tasks = tasks.ToList();
    }



    // --------- Interface Implementations ---------
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => Name.GetHashCode();
    public override string ToString() => Name;
    public bool Equals(BasicProject? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (this.Name == null || other.Name == null) return false;

        return Name.Equals(other.Name);
    }



    // --------- Methods ---------
    internal void AddParticipant() 
    {
        TextPrompt<string> prompt = new TextPrompt<string>(">> Please provide a [bold Gold1]participant name[/]");
        string participantName = AnsiConsole.Prompt(prompt);

        Markup msg;

        if (!InterfaceExtensions.IsAlphanumeric(participantName))
        {
            msg = new Markup("[bold OrangeRed1]Command not valid, ADD PARTICIPANT requires a valid participant name (A-Z, 0-9, _ ).[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        if(Participants.Contains(participantName))
        {
            msg = new Markup("bold OrangeRed1]The participant [bold Gold1]{participantName}[/] already exists.[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }

        Participant participant = new(participantName);
        Participants.Add(participant);
        
        msg = new Markup($"[bold Gold1]{participant}[/] has been added to the project.");
        Logger.Enqueue(msg);
        InterfaceExtensions.DrawFrame(msg);
    }
    internal void RemoveParticipants() 
    {
        Markup msg;
        
        if (Participants.Count == 0)
        {
            msg = new Markup("[bold OrangeRed1]No objects found[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        Participant participant = ISelectableExtensions.SelectObjectOfType(Participants);
        if(participant is null) 
        {               
            msg = new Markup("The operation is being aborted...");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }

        Participants.Remove(participant);

        // TODO Enque message
        msg = new Markup($"[bold Gold1]{participant}[/] has been added to the project.");
        InterfaceExtensions.DrawFrame(msg);
    }
    internal void AddTask()
    {
        TextPrompt<string> prompt = new TextPrompt<string>(">> Please provide a [bold Gold1]task name[/]");
        string taskName = AnsiConsole.Prompt(prompt);

        Markup msg;

        if (!InterfaceExtensions.IsAlphanumeric(taskName))
        {
            msg = new Markup("[bold OrangeRed1]Command not valid, ADD TASK requires a valid task name (A-Z, 0-9, _ ).[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        if(Tasks.Contains(taskName.ToUpper())) 
        {
            msg = new Markup("[bold OrangeRed1]The task [bold Gold1]{taskName}[/] already exists.[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        Task task = new (taskName.ToUpper());
        AnsiConsole.MarkupLine($"The task [bold Gold1]{taskName}[/] has been created.");
        Tasks.Add(task);

        
        OpenTask(task);
    }
    internal void OpenTask(Task? task = null)
    {
        Markup msg;
        if (Tasks.Count == 0)
        {
            msg = new Markup ("[bold OrangeRed1]No objects found[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        if(task is null)
        {
            task = ISelectableExtensions.SelectObjectOfType(Tasks);
        }

        TaskInterface.CommandLoop(task);
    }
    internal void RemoveTask() 
    {
        Markup msg;
        if (Tasks.Count == 0)
        {
            msg = new Markup ("[bold OrangeRed1]No objects found[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }
        Task task = ISelectableExtensions.SelectObjectOfType(Tasks);
        if (!Tasks.Contains(task)) 
        {
            Tasks.Remove(task);
        }
    }
    internal void ListParticipants() 
    {
        Markup msg;
        if (Participants.Count == 0)
        {
            msg = new Markup ("[bold OrangeRed1]No objects found[/]");
            InterfaceExtensions.DrawLoopFrame(msg);
            Logger.Enqueue(msg);
            return;
        }

        Table table = new();
        table.AddColumns(new TableColumn("Participant"), new TableColumn("Tasks"));
        table.Border(TableBorder.Rounded);
        table.ShowRowSeparators = true;

        foreach (Participant participant in Participants)
        {
            if (participant.Tasks.Count == 0)
            {
                table.AddRow(
                    new Markup(participant.Name, Color.Grey),
                    new Markup("[bold Grey35]0 tasks[/]")
                );
            } else if (participant.Tasks.Count > 0)
            {
                table.AddRow(
                    new Markup(participant.Name),
                    new Rows(participant.Tasks.Select(x => new Text(x.Name)).ToList()));
            }
        }
        InterfaceExtensions.DrawLoopFrame(table);
    }
    internal void ShowStatus()  
    {
        Table table = new ();
        table.AddColumns(new TableColumn("Task"), new TableColumn("Progress"));
        table.Border(TableBorder.Rounded);

        foreach (Task task in Tasks)
        {
            if (task.Status != TaskStatus.Cancelled && task.Status != TaskStatus.OnHold)
            {
                table.AddRow(
                    new Markup(task.Name),
                    new BreakdownChart()
                        .Width(50)
                        .AddItem("Done", (int)task.Status, Color.Green)
                        .AddItem("Remaining", (int)TaskStatus.Completed - (int)task.Status, Color.Red)
                );
            } else if (task.Status == TaskStatus.OnHold)
            {
                table.AddRow(
                    new Markup(task.Name),
                    new BreakdownChart()
                        .Width(50)
                        .AddItem("OnHold", (int)TaskStatus.Completed, Color.Grey)
                );
            }
        }
        InterfaceExtensions.DrawLoopFrame(table);
    }


    
    // --------- Operators ---------
    public static implicit operator BasicProject(string name)
    {
        return new BasicProject(name);
    }
}
