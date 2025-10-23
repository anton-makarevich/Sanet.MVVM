namespace Sanet.MVVM.Core.ViewModels;

/// <summary>
/// Interface for ViewModels that can provide a result when shown as a dialog or overlay.
/// </summary>
/// <typeparam name="TResult">The type of result this ViewModel provides</typeparam>
public interface IResultProvider<TResult>
{
    /// <summary>
    /// Gets a task that completes when the ViewModel has a result to return.
    /// The task should complete with the result value, or default(TResult) if cancelled.
    /// </summary>
    Task<TResult> GetResultAsync();
}