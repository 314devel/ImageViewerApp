using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Xml;
using System.Windows.Input;
using System.Data;
using System.Windows.Media;

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for ViewImagesPage.xaml
    /// </summary>
    public partial class ViewImagesPage : UserControl
    {
        
        public ViewImagesPage()
        {

            InitializeComponent();
            
        }
        
        #region cursor helper functions
        public static void waitCursor()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
        }
        

        public static void normalCursor()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });

        }
        #endregion cursor helper functions

        private void PageUnloading(object sender, RoutedEventArgs e)
        {

        }

        private void ScanSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the wrap panel
            ThumbnailPanel.Children.Clear();

            // Clear the selected image
            SelectedImage.Source = null;

            // Get the thumbnails

            // Add a thumbnail to the wrap panel
            // DUDE: Use this as an example
            ThumbnailButton tb = new ThumbnailButton();
            //tb.image.BeginInit();
            //tb.image.Source = thumbnail;
            //tb.image.EndInit();
            //tb.Margin = new Thickness(10, 5, 5, 10); ;
            //tb.Click += Thumbnail_Click;
            //ThumbnailPanel.Children.Add(tb);

        }

        private void Thumbnail_Click(object sender, EventArgs e)
        {
            // DUDE: update SelectedImage here
            //ThumbnailButton tb = sender as ThumbnailButton;
            //// Load the full image
            //SelectedImage.BeginInit();
            //SelectedImage.Source = tb.image.Source; // not sure this will capture the full image
            //SelectedImage.EndInit();
        }
    }
    
}
