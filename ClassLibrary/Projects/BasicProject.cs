#nullable disable

using Spectre.Console;
using System.Text.Json.Serialization;

namespace ClassLibrary;

internal class BasicProject : IEquatable<BasicProject>, ISelectable
{
    // --------- Fields ---------
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Participants")]
    public HashSet<Participant> Participants { get; set; }

    [JsonPropertyName("Tasks")]
    public HashSet<Task> Tasks { get; set; }
    //public DateOnly StartDate {get; set;}
    //public DateOnly EndDate {get; set;}
    //internal ProjectStatus Status{ get; set; }



    // --------- Constructors ---------
    public BasicProject(string name, HashSet<Participant> participants = null, HashSet<Task> tasks = null)
    {
        Name = name;
        
        if (participants is null) Participants = [];
        else Participants = participants;
        
        if (tasks is null) Tasks = [];
        else Tasks = tasks;
        //StartDate = DateOnly.FromDateTime(DateTime.Today);
        //EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14));
    }
    //public BasicProject (string name, DateOnly startDate, DateOnly endDate) 
    //{
    //    Name = name;
    //    Participants = [];
    //    StartDate = startDate;
    //    EndDate = endDate;
    //}



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
    public bool Equals(BasicProject other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (this.Name == null || other.Name == null) return false;

        return Name.Equals(other.Name);
    }



    // --------- Methods ---------
    internal void AddParticipant() 
    {
        AnsiConsole.MarkupLine($">> Please provide a [bold yellow]participant name[/]");
        string participantName = Console.ReadLine().Trim().ToUpper();

        if (!InterfaceExtensions.IsAlphanumeric(participantName))
        {
            AnsiConsole.MarkupLine("[bold red]Command not valid, ADD PARTICIPANT requires a valid participant name (A-Z, 0-9, _ ).[/]");
            return;
        }
        //if(participantName == "EXIT") 
        //{   
        //    AnsiConsole.MarkupLine($"The operation is being aborted...");
        //    return;
        //}
        if(Participants.Contains(participantName))
        {
            AnsiConsole.MarkupLine($"[bold red]The participant [bold yellow]{participantName}[/] already exists.[/]");
            return;
        }

        Participant participant = new(participantName);
        Participants.Add(participant);

        AnsiConsole.MarkupLine($"[bold yellow]{participant}[/] [bold green]has been added to the project.[/]");
    }
    internal void RemoveParticipants() 
    {
        if (Participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        Participant participant = SelectExtensions.SelectObjectOfType(Participants);
        if(participant is null) 
        {   
            AnsiConsole.MarkupLine($"The operation is being aborted...");
            return;
        }
        
        Participants.Remove(participant);
    }

    internal void AddTask()
    {
        AnsiConsole.MarkupLine($">> Please provide a [bold yellow]task name[/]");
        string taskName = Console.ReadLine().Trim().ToUpper();

        if (!InterfaceExtensions.IsAlphanumeric(taskName))
        {
            AnsiConsole.MarkupLine("[bold red]Command not valid, ADD TASK requires a valid task name (A-Z, 0-9, _ ).[/]");
            return;
        }
        //if (taskName == "EXIT") 
        //{   
        //    AnsiConsole.MarkupLine($"The operation is being aborted...");
        //    return;
        //}
        if(Tasks.Contains(taskName.ToUpper())) 
        {
            AnsiConsole.MarkupLine($"[bold red]The task [bold yellow]{taskName}[/] already exists.[/]"); 
            return;
        }
        Task task = new (taskName.ToUpper());
        AnsiConsole.MarkupLine($"[bold green]The task [bold yellow]{taskName}[/] has been created.[/]");
        Tasks.Add(task);
        OpenTask(task);
    }
    internal void OpenTask(Task task = null)
    {
        if (Tasks.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        if(task is null)
        {
            task = SelectExtensions.SelectObjectOfType(Tasks);
        }
        if(task is null) 
        {   
            AnsiConsole.MarkupLine($"The operation is being aborted...");
            return;
        }
        //if(!Tasks.Contains(task))
        //{
        //    AnsiConsole.MarkupLine($"[bold red]The task [bold yellow]{task}[/] does not exist.[/]");
        //    return;
        //}

        TaskInterface.CommandLoop(task);
    }
    internal void RemoveTask() 
    {
        if (Tasks.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
            return;
        }
        Task task = SelectExtensions.SelectObjectOfType(Tasks);
        if (!Tasks.Contains(task)) 
        {
            Tasks.Remove(task);
        }

    }
    internal void ListParticipants() 
    {
        if (Participants.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold blue]No objects found[/]");
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
                    new Markup("[grey]0 tasks[/]")
                );
            } else if (participant.Tasks.Count > 0)
            {
                table.AddRow(
                    new Markup(participant.Name),
                    new Rows(participant.Tasks.Select(x => new Text(x.Name)).ToList()));
            }
        }
        AnsiConsole.Write(table);
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
        AnsiConsole.Write(table);
    }



    // --------- Operators ---------
    public static implicit operator BasicProject(string name)
    {
        return new BasicProject(name);
    }
}
