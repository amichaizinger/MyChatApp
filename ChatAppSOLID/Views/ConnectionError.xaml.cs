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
using System.Windows.Shapes;

namespace ChatAppSOLID
{
    /// <summary>
    /// Interaction logic for ConnectionError.xaml
    /// </summary>
    public partial class ConnectionError : Window
    {
        public ConnectionError(string error)
        {
            InitializeComponent();
            ErrorMessage.Text = error;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Allow dragging the window by clicking and dragging the border
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
