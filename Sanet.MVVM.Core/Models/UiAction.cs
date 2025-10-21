using System.Windows.Input;

namespace Sanet.MVVM.Core.Models;

/// <summary>
/// Represents a user action that can be presented in a dialog or UI element.
/// </summary>
public record UiAction
{
    /// <summary>
    /// Gets the title/label of the action (e.g., " Yes\, \No\, \Cancel\).
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the command to execute when this action is selected.
    /// Can be null for actions that simply close the dialog without executing any logic.
    /// </summary>
    public ICommand? Command { get; init; }

    /// <summary>
    /// Gets the parameter to pass to the command when executed.
    /// </summary>
    public object? CommandParameter { get; init; }
}
