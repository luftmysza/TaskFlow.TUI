#nullable disable

using Spectre.Console;
using System.Reflection;
using System.ComponentModel;
using TaskFlow.TUI.Backend.Contracts;

namespace TaskFlow.TUI.Backend.UI;

internal static class SelectExtensions
{
    internal static T SelectObjectOfType<T>(IEnumerable<T> set) where T : ISelectable
    {
        Rule openRule = new("Please Select") { Justification = Justify.Left };
        AnsiConsole.Write(openRule);

        List<T> options = set.ToList();
        options.Add(default);
        
        T lv_return = AnsiConsole.Prompt
        (
            new SelectionPrompt<T>()
                .PageSize(10)
                .HighlightStyle(Style.Parse("Yellow"))
                .AddChoices(options)
        );

        return lv_return;
    }

    internal static List<T> SelectMultipleOfType<T>(IEnumerable<T> setBase, IEnumerable<T> setExcept = null) where T : ISelectable
    {
        Rule openRule = new("Please Select") { Justification = Justify.Left };
        AnsiConsole.Write(openRule);

        List<T> set = setBase.ToList();
        if (setExcept != null) set = set.Except(setExcept).ToList();

        List<T> lv_return = AnsiConsole.Prompt
        (
            new MultiSelectionPrompt<T>()
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more objects)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle an option, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(set)
        );

        return lv_return;
    }
    internal static T SelectEnum<T>() where T : notnull, Enum 
    {
        List<T> options = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        T lv_return = AnsiConsole.Prompt
        (
                new SelectionPrompt<T>()
                    .Title(">> Pick the project: ")
                    .PageSize(10)
                    .HighlightStyle(Style.Parse("Yellow"))
                    .UseConverter(e => 
                    e.GetEnumDescription())
                    .AddChoices(options)
        );
        return lv_return;
    }

    public static string GetEnumDescription<T>(this T enumValue) where T : Enum
    {
        Type type = enumValue.GetType();
        FieldInfo field = type.GetField(enumValue.ToString());
        DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? enumValue.ToString();
    }
}