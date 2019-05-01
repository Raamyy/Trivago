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
using Trivago.Front_End;
using Trivago.Models;
namespace Trivago
{
    /// <summary>
    /// Interaction logic for Admin_window.xaml
    /// </summary>
    public partial class Admin_window : Window
    {
        public Admin_window()
        {
            InitializeComponent();
            CustomImage logo = new CustomImage("resources/images/logo.png");
            this.Logo.Source = logo.GetImage().Source;
        }

        private void Button_Mouse_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = new SolidColorBrush(Color.FromRgb(9, 48, 65));
        }

        private void Button_Mouse_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            
            button.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void Websites_Button_Click(object sender, RoutedEventArgs e)
        {
            
            DataModels model = DataModels.GetInstance();
            Admin_WebsitesControlCanvas websitesCanvas = Admin_WebsitesControlCanvas.GetInstance(WebsitesCanvas, model.GetAllWebsites());
            websitesCanvas.Show();
        }
    }
}
