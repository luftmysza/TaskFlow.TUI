using System.ComponentModel;

namespace TaskFlow.TUI.Backend.Entities;

internal enum TaskStatus
{
    [Description("On Hold")]
    OnHold = 0,     // An indication in statistics
    [Description("Not Started")]
    NotStarted = 10,  // Default when creating a task
    [Description("In Progress")]
    InProgress = 50,  // A task has been assigned to someone
    [Description("Completed")]
    Completed = 100,   // The task has been completed
    [Description("Cancelled")]
    Cancelled = 200  // The task has been deemed unnecessary, it won't appear in statistics
}