namespace ClassLibrary;

internal enum ProjectStatus
{
    OnHold = 0,     // An indication in statistics
    NotStarted = 10,  // Default when creating a task
    InProgress = 50,  // A task has been assigned to someone
    Completed = 100,   // The task has been completed
    Cancelled = 200  // The task has been deemed unnecessary, it won't appear in statistics
}
