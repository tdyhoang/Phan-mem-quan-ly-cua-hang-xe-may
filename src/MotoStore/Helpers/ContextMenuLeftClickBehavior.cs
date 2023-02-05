using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows;

namespace MotoStore.Helpers
{
    public static class ContextMenuLeftClickBehavior
    {
        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLeftClickEnabledProperty);
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached("IsLeftClickEnabled", typeof(bool), typeof(ContextMenuLeftClickBehavior), new(default(bool), OnIsLeftClickEnabledChanged));

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                if (e.NewValue is bool newvalue && newvalue)
                {
                    if (uiElement is ButtonBase button)
                        button.Click += OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
                else
                {
                    if (uiElement is ButtonBase button)
                        button.Click -= OnMouseLeftButtonUp;
                    else
                        uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Debug.Print("OnMouseLeftButtonUp");
            if (sender is FrameworkElement fe)
            {
                if (fe.ContextMenu.DataContext == null)
                {
                    fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
                }

                fe.ContextMenu.IsOpen = true;
            }
        }

    }
}
