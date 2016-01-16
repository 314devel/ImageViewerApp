using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for ThumbnailButton.xaml
    /// </summary>
    public partial class ThumbnailButton : UserControl
    {
        /// <summary>
        /// Defines whether the cursor is in when the mouse button is pressed down.
        /// If the flag is still true when the mouse button is released, then a "Click"
        /// event is generated.
        /// </summary>
        private bool _IsMouseIn = false;

        public delegate void ClickEventHandler(object sender, EventArgs e);
        public event ClickEventHandler Click;

        public ThumbnailButton()
        {
            InitializeComponent();
        }
        
        protected virtual void RaiseClickEvent()
        {
            if (Click != null)
            {
                Click(this, new EventArgs());
            }
        }
        
        private void Control_MouseEnter(object sender, MouseEventArgs e)
        {
            _IsMouseIn = true;
            border.Background = (SolidColorBrush)Application.Current.Resources["Button.MouseOver.Background"];
            border.BorderBrush = (SolidColorBrush)Application.Current.Resources["Button.MouseOver.Border"];
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            _IsMouseIn = false;
            border.Background = Brushes.Transparent;
            border.BorderBrush = Brushes.Transparent;
        }

        private void Control_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            border.Background = (SolidColorBrush)Application.Current.Resources["VaroctoDark"];
            border.BorderBrush = (SolidColorBrush)Application.Current.Resources["ButtonPressedBorder"];
        }

        private void Control_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            border.Background = Brushes.Transparent;
            border.BorderBrush = Brushes.Transparent;

            // Generate a "click" event
            if (_IsMouseIn)
            {
                _IsMouseIn = false;
                RaiseClickEvent();
            }
        }

        private void Control_EnableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    border.Background = Brushes.Transparent;
                    border.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    border.Background = (SolidColorBrush)Application.Current.Resources["Button.Disabled.Background"];
                    border.BorderBrush = (SolidColorBrush)Application.Current.Resources["Button.Disabled.Border"];
                }
            }
        }

    }
}
