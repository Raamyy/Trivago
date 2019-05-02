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
        public CustomCanvas currentCanvas;

        public Admin_window()
        {
            InitializeComponent();
            FrontEndHelper.SetAdminWindow(this);
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

        public void InitializeHotelsCanvas(List<Hotel> hotels)
        {
            AdminHotelCanvas hotelsCanvas = AdminHotelCanvas.GetInstance(HotelsCanvas, hotels);
            hotelsCanvas.SetCanvasCoord(TabsRectangle.Width, HeaderRectangle.Height);
            hotelsCanvas.SetCanvasDimensions(Width - TabsRectangle.Width, Height - HeaderRectangle.Height);
            hotelsCanvas.Show();
            currentCanvas = hotelsCanvas;
        }

        public void InitializeRoomsCanvas(List<Room> rooms)
        {
            AdminRoomsCanvas adminRoomsCanvas = AdminRoomsCanvas.GetInstance(RoomsCanvas, rooms);
            adminRoomsCanvas.SetCanvasCoord(TabsRectangle.Width, HeaderRectangle.Height);
            adminRoomsCanvas.SetCanvasDimensions(Width - TabsRectangle.Width, Height - HeaderRectangle.Height);
            adminRoomsCanvas.Show();
            currentCanvas = adminRoomsCanvas;
        }

        private void Hotels_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentCanvas != null)
                currentCanvas.Hide();
            InitializeHotelsCanvas(DataModels.GetInstance().GetAllHotels());
        }

        private void Rooms_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentCanvas != null)
                currentCanvas.Hide();
            InitializeRoomsCanvas(DataModels.GetInstance().GetAllRooms());
        }

        public void Room_Add_Button_Click(object sender, RoutedEventArgs args)
        {
            FrontEndHelper.CreateAddRoomPopupWindow();
        }

        private void Websites_Button_Click(object sender, RoutedEventArgs e)
        {
            if(currentCanvas!=null)
            currentCanvas.Hide();            
            DataModels model = DataModels.GetInstance();
            Admin_WebsitesControlCanvas websitesCanvas = Admin_WebsitesControlCanvas.GetInstance(WebsitesCanvas, model.GetAllWebsites());
            websitesCanvas.SetCanvasCoord(TabsRectangle.Width, HeaderRectangle.Height);
            websitesCanvas.SetCanvasDimensions(Width - TabsRectangle.Width, Height - HeaderRectangle.Height);
            websitesCanvas.Show();
        }

        private void Offers_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentCanvas != null)
                currentCanvas.Hide();
            DataModels model = DataModels.GetInstance();
            AdminOffersControlCanvas offersCanvas = AdminOffersControlCanvas.GetInstance(OffersCanvas, model.GetAllOffers());
            offersCanvas.SetCanvasCoord(TabsRectangle.Width, HeaderRectangle.Height);
            offersCanvas.SetCanvasDimensions(Width - TabsRectangle.Width, Height - HeaderRectangle.Height);
            offersCanvas.Show();
        }
    }
}
