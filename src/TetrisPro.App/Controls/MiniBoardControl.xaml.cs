using System.Windows;
using System.Windows.Controls;

namespace TetrisPro.App.Controls;

public partial class MiniBoardControl : UserControl
{
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(MiniBoardControl), new PropertyMetadata(string.Empty));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public MiniBoardControl()
    {
        InitializeComponent();
    }
}
