using System;
using System.Windows;
using MotoStore.Services.Contracts;

namespace MotoStore.Services;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Show(Type windowType)
    {
        if (!typeof(Window).IsAssignableFrom(windowType))
            throw new InvalidOperationException($"The window class should be derived from {typeof(Window)}.");

        var windowInstance = _serviceProvider.GetService(windowType) as Window;

        windowInstance?.Show();
    }

    public T Show<T>() where T : class
    {
        if (!typeof(Window).IsAssignableFrom(typeof(T)))
            throw new InvalidOperationException($"The window class should be derived from {typeof(Window)}.");

        var windowInstance = _serviceProvider.GetService(typeof(T)) as Window;

        if (windowInstance == null)
            throw new InvalidOperationException("Window is not registered as service.");

        windowInstance.Show();

        return (T)Convert.ChangeType(windowInstance, typeof(T));
    }
}