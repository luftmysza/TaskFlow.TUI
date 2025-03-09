#nullable disable

using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    internal static class SelectExtensions
    {
    //    internal static string _____SelectObjectOfType(HashSet<ISelectable> set)
    //    {
    //    Rule openRule = new("Please Select") { Justification = Justify.Left };
    //    AnsiConsole.Write(openRule);

    //    List<string> options = set.Select(x => x.Name).ToList();
    //    options.Add("EXIT");

    //    string select = AnsiConsole.Prompt
    //    (
    //        new SelectionPrompt<string>()
    //        .PageSize(10)
    //        .HighlightStyle(Style.Parse("Yellow"))
    //        .AddChoices(options)
    //    );
    //    return select;
    //}

    internal static T SelectObjectOfType<T>(HashSet<T> set) where T : ISelectable
        {
            Rule openRule = new("Please Select") { Justification = Justify.Left };
            AnsiConsole.Write(openRule);

            List<string> options = set.Select(x => x.Name).ToList();
            options.Add("EXIT");

            string select = AnsiConsole.Prompt
            (
                new SelectionPrompt<string>()
                .PageSize(10)
                .HighlightStyle(Style.Parse("Yellow"))
                .AddChoices(options)
            );

            T lv_return = select == "EXIT" ? default(T) : set.FirstOrDefault(x => x.Name.Equals(select));

            return lv_return;
        }

        //internal static List<string> _____SelectMultipleOfType(HashSet<ISelectable> setBase, HashSet<ISelectable> setExcept = null)
        //{
        //    List<string> set = setBase.Select(x => x.Name).ToList();
        //    if (setExcept != null) set = setBase.Except(setExcept).Select(x => x.Name).ToList();

        //    List<string> options = AnsiConsole.Prompt(
        //        new MultiSelectionPrompt<string>()
        //            .Title("Choose [yellow]objects[/] to add ?")
        //            .NotRequired()
        //            .PageSize(10)
        //            .MoreChoicesText("[grey](Move up and down to reveal more objects)[/]")
        //            .InstructionsText(
        //                "[grey](Press [blue]<space>[/] to toggle an option, " +
        //                "[green]<enter>[/] to accept)[/]")
        //            .AddChoices(set)
        //        );
        //    return options;
        //}
        internal static List<T> SelectMultipleOfType<T>(HashSet<T> setBase, HashSet<T> setExcept = null) where T : ISelectable
        {
            List<T> set = setBase.ToList();
            if (setExcept != null) set = setBase.Except(setExcept).ToList();

            List<T> selects = AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .NotRequired()
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more objects)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle an option, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(set)
                );

            List<T> lv_return = selects;

            return lv_return;
        }
        internal static string SelectEnum<T>(Dictionary<T, string> map)
        {
            List<string> options = map.Select(kvp => kvp.Value).ToList();
            options.Add("EXIT");

            string select = AnsiConsole.Prompt
            (
                new SelectionPrompt<string>()
                .Title(">> Pick the project: ")
                .PageSize(10)
                .HighlightStyle(Style.Parse("Yellow"))
                .AddChoices(options)
            );
            return select;
        }
        //internal static ISelectable ToSelectable(this string name, HashSet<ISelectable> set)
        //{
        //    return set.FirstOrDefault(p => p.Name == name);
        //}
    }
}
