#nullable disable

using System.Collections;
using System.Text.Json.Serialization;

namespace ClassLibrary;

internal class Participant : IEquatable<Participant>, IEnumerable<Task>, ISelectable
{
    // --------- Fields ---------
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Tasks")]
    public HashSet<Task> Tasks { get; set; }



    // --------- Constructors ---------
    public Participant(string name, HashSet<Task> tasks = null)
    {
        Name = name;
        
        if (tasks is null) Tasks = [];
        else Tasks = tasks;
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
    public bool Equals(Participant other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (this.Name == null || other.Name == null) return false;

        return Name.Equals(other.Name);
    }
    public IEnumerator<Task> GetEnumerator()
    {
        foreach (Task task in Tasks) yield return task;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }



    // --------- Methods ---------
    internal void AddTask(string taskName)
    {
        if (!Tasks.Contains(taskName)) 
        {
            Task task = new(taskName);
            Tasks.Add(task);
        }
    }
    internal void RemoveTask(string taskName)
    {
        if (Tasks.Contains(taskName)) 
        {
            Tasks.Remove(taskName);
        }
    }
    internal void ShowMyTasks() 
    {
        Console.WriteLine("");
        Console.WriteLine("User Task");

        foreach (Task task in Tasks)
        {
            Console.WriteLine(this.Name + "--->" + task.Name);
        }
        Console.WriteLine("");
    }



    // --------- Operators ---------
    public static implicit operator Participant(string name)
    {
        return new Participant(name);
    }
}

