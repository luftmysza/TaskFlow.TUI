#nullable disable

using Spectre.Console;
using System.Reflection;
using System.ComponentModel;

namespace TaskFlow.TUI.Contracts;

internal static class ISelectableExtensions
{
    internal static T SelectObjectOfType<T>(IEnumerable<T> set) where T : ISelectable
    {
        Rule openRule = new("Please Select") { Justification = Justify.Left };
        AnsiConsole.Write(openRule);

        List<T> selectOptions = set.ToList();
        // selectOptions.Add(default);
        
        T selectedSingle = AnsiConsole.Prompt
        (
            new SelectionPrompt<T>()
                .PageSize(10)
                .HighlightStyle(Style.Parse("Gold1"))
                .AddChoices(selectOptions)
        );

        return selectedSingle;
    }
    internal static List<T> SelectMultipleOfType<T>(IEnumerable<T> setBase, IEnumerable<T> setExcept = null) where T : ISelectable
    {
        Rule openRule = new("Please Select") { Justification = Justify.Left };
        AnsiConsole.Write(openRule);

        List<T> selectOptions = setBase.ToList();
        if (setExcept is not null) selectOptions = selectOptions.Except(setExcept).ToList();

        List<T> lv_return = AnsiConsole.Prompt
        (
            new MultiSelectionPrompt<T>()
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more objects)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle an option, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(selectOptions)
        );

        return lv_return;
    }
    internal static T SelectEnum<T>() where T : notnull, Enum 
    {
        List<T> options = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        T lv_return = AnsiConsole.Prompt
        (
                new SelectionPrompt<T>()
                    .Title($">> Pick {nameof(T)}: ")
                    .PageSize(10)
                    .HighlightStyle(Style.Parse("Gold1"))
                    .UseConverter(e => 
                    e.GetEnumDescription())
                    .AddChoices(options)
        );
        return lv_return;
    }

    internal static string GetEnumDescription<T>(this T enumValue) where T : Enum
    {
        Type type = enumValue.GetType();
        FieldInfo field = type.GetField(enumValue.ToString());
        DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute?.Description ?? enumValue.ToString();
    }
}