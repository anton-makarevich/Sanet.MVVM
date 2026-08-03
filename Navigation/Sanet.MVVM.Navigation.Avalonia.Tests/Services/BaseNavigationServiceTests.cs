using Sanet.MVVM.Core.ViewModels;
using Sanet.MVVM.Core.Views;
using Sanet.MVVM.Navigation.Avalonia.Services;

namespace Sanet.MVVM.Navigation.Avalonia.Tests.Services;

public class BaseNavigationServiceTests
{
    private readonly TestServiceProvider _container = new();
    private readonly TestNavigationService _sut;

    public BaseNavigationServiceTests()
    {
        _sut = new TestNavigationService(_container);
    }

    [Fact]
    public async Task GetNewViewModelAsyncDisposesExistingViewModelViaDisposeAsync()
    {
        _container.Register(() => new AsyncDisposeVm());
        var first = await _sut.GetNewViewModelAsync<AsyncDisposeVm>();
        Assert.NotNull(first);

        var second = await _sut.GetNewViewModelAsync<AsyncDisposeVm>();

        Assert.True(first.DisposeAsyncCalled);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task GetNewViewModelAsyncDisposesExistingViewModelViaDispose()
    {
        _container.Register(() => new SyncDisposeVm());
        var first = await _sut.GetNewViewModelAsync<SyncDisposeVm>();
        Assert.NotNull(first);

        var second = await _sut.GetNewViewModelAsync<SyncDisposeVm>();

        Assert.True(first.DisposeCalled);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task GetNewViewModelAsyncUsesDisposeAsyncWhenViewModelImplementsBoth()
    {
        _container.Register(() => new BothDisposeVm());
        var first = await _sut.GetNewViewModelAsync<BothDisposeVm>();
        Assert.NotNull(first);

        var second = await _sut.GetNewViewModelAsync<BothDisposeVm>();

        Assert.True(first.DisposeAsyncCalled);
        Assert.False(first.DisposeCalled);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task GetNewViewModelAsyncCreatesAndCachesViewModelWhenNoneExists()
    {
        _container.Register(() => new SimpleVm());
        var first = await _sut.GetNewViewModelAsync<SimpleVm>();

        Assert.NotNull(first);
        Assert.True(_sut.HasViewModel<SimpleVm>());
    }

    [Fact]
    public async Task GetNewViewModelAsyncWaitsForDisposeAsyncToCompleteBeforeCreatingNewViewModel()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _container.Register(() => new GatedAsyncDisposeVm(tcs));
        var first = await _sut.GetNewViewModelAsync<GatedAsyncDisposeVm>();
        Assert.NotNull(first);

        var getNewTask = _sut.GetNewViewModelAsync<GatedAsyncDisposeVm>();
        await Task.Delay(100);

        Assert.True(first.DisposeAsyncCalled);
        Assert.False(getNewTask.IsCompleted);

        tcs.SetResult();
        var second = await getNewTask;

        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetNewViewModelDisposesExistingViewModelSynchronously()
    {
        _container.Register(() => new SyncDisposeVm());
        var first = _sut.GetNewViewModel<SyncDisposeVm>();
        Assert.NotNull(first);

        var second = _sut.GetNewViewModel<SyncDisposeVm>();

        Assert.True(first.DisposeCalled);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    private sealed class TestServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<object?>> _factories = new();

        public void Register<T>(Func<T> factory) where T : BaseViewModel
        {
            _factories[typeof(T)] = () => factory();
        }

        public object? GetService(Type serviceType)
        {
            return _factories.TryGetValue(serviceType, out var factory) ? factory() : null;
        }
    }

    private sealed class TestNavigationService(IServiceProvider container)
        : BaseNavigationService(container)
    {
        protected override IBaseView? GetCurrentView() => null;
        protected override void SetMainWindowContent(IBaseView view) { }
    }

    private sealed class SimpleVm : BaseViewModel
    {
    }

    private sealed class AsyncDisposeVm : BaseViewModel, IAsyncDisposable
    {
        public bool DisposeAsyncCalled { get; private set; }

        public ValueTask DisposeAsync()
        {
            DisposeAsyncCalled = true;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class SyncDisposeVm : BaseViewModel, IDisposable
    {
        public bool DisposeCalled { get; private set; }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    private sealed class BothDisposeVm : BaseViewModel, IAsyncDisposable, IDisposable
    {
        public bool DisposeAsyncCalled { get; private set; }
        public bool DisposeCalled { get; private set; }

        public ValueTask DisposeAsync()
        {
            DisposeAsyncCalled = true;
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    private sealed class GatedAsyncDisposeVm : BaseViewModel, IAsyncDisposable
    {
        private readonly TaskCompletionSource _tcs;

        public GatedAsyncDisposeVm(TaskCompletionSource tcs)
        {
            _tcs = tcs;
        }

        public bool DisposeAsyncCalled { get; private set; }

        public async ValueTask DisposeAsync()
        {
            DisposeAsyncCalled = true;
            await _tcs.Task;
        }
    }
}
