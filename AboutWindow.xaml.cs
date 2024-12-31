using System.Windows;
using System.IO;
using System.ComponentModel;

namespace JsonZipToolWPF
{
    public partial class AboutWindow : Window, INotifyPropertyChanged
    {
        private string _version = "未知版本";
        public string Version
        {
            get => _version;
            private set
            {
                if (_version != value)
                {
                    _version = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Version)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AboutWindow()
        {
            InitializeComponent();
            LoadVersion();
            DataContext = this;
        }

        private void LoadVersion()
        {
            try
            {
                string versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "version.txt");
                if (File.Exists(versionFile))
                {
                    Version = File.ReadAllText(versionFile).Trim();
                }
                else
                {
                    Version = "未知版本";
                }
            }
            catch
            {
                Version = "未知版本";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorZone_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                    WindowState = WindowState.Maximized;
                else
                    WindowState = WindowState.Normal;
            }
            else
            {
                DragMove();
            }
        }
    }
} 