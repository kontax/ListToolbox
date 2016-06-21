using System.Windows;
using System.Windows.Controls;

namespace BIApps.ListToolbox.WpfFrontend.Controls {
    /// <summary>
    /// Interaction logic for PresetTextBox.xaml
    /// </summary>
    public partial class PresetTextBox : UserControl {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(PresetTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty InitialTextProperty = DependencyProperty.Register(
            "InitialText", typeof(string), typeof(PresetTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            "TextWrapping", typeof(TextWrapping), typeof(PresetTextBox), new PropertyMetadata(default(TextWrapping)));

        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register(
            "AcceptsReturn", typeof(bool), typeof(PresetTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength", typeof(int), typeof(PresetTextBox), new PropertyMetadata(default(int)));

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string InitialText {
            get { return (string)GetValue(InitialTextProperty); }
            set { SetValue(InitialTextProperty, value); }
        }

        public TextWrapping TextWrapping {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public bool AcceptsReturn {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public int MaxLength {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public PresetTextBox() {
            InitializeComponent();
        }
    }
}
