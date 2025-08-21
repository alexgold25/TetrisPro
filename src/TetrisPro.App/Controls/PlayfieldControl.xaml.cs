using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace TetrisPro.App.Controls
{
    public partial class PlayfieldControl : UserControl
    {
        public PlayfieldControl() => InitializeComponent();

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(PlayfieldControl),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
