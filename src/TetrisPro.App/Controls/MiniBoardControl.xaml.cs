using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace TetrisPro.App.Controls
{
    public partial class MiniBoardControl : UserControl
    {
        public MiniBoardControl() => InitializeComponent();

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(MiniBoardControl),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
