using System.Windows;

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Link the window's content presenter to the Page State Machine's
            PageStateMachine.Instance.ContentPresenter = _Content;
        }
        
    }
}
