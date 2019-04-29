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
using Trivago.Models;
using Trivago.Front_End;

namespace Trivago
{   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double NavigationCanvasHeight = 100;
        public CustomCanvas CurrentCanvas;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCanvases();
            // for testing
            Review rev =  DataModels.GetInstance().GetReview(1); 
            List<Room> rooms = DataModels.GetInstance().GetAllRooms();
            var x = DataModels.GetInstance().GetWebsitePricesForRoom(rooms[2]);
            var y = DataModels.GetInstance().GetRoomBookings(rooms[2]);
            List<Hotel> hotels = DataModels.GetInstance().GetAllHotels();
        }
        
        public Canvas GetHomeCanvas()
        {
            return HomeCanvas;
        }

        public Canvas GetNavigationCanvas()
        {
            return NavigationCanvas;
        }

        private void InitializeCanvases()
        {
            InitializeNavigationCanvas();
            InitializeHomeCanvas();
        }

        private void InitializeHomeCanvas()
        {
            CustomCanvas homeCanvas = Front_End.HomeCanvas.GetInstance(HomeCanvas, Window.Width, Window.Height - NavigationCanvas.Height);
            homeCanvas.Show();
        }

        private void InitializeNavigationCanvas()
        {
            CustomCanvas navigationCanvas = Front_End.NavigationCanvas.GetInstance(NavigationCanvas, Window.Width, 100);
            navigationCanvas.Show();
        }

        public void LogoImage_MouseLeftButtonDown(object sender, RoutedEventArgs args)
        {
            CurrentCanvas.Hide();
            CurrentCanvas = Trivago.Front_End.HomeCanvas.GetInstance(HomeCanvas, Window.Width, Window.Height - NavigationCanvas.Height);
            CurrentCanvas.Show();
        }

        public void SearchButton_Click(object sender, RoutedEventArgs args)
        {
            Front_End.HomeCanvas homeCanvas = Front_End.HomeCanvas.GetInstance(HomeCanvas, HomeCanvas.Width, HomeCanvas.Height);
            List<Location> locations = homeCanvas.selectedLocations;
            
            String output = "";
            foreach (Location loc in locations)
                output += loc.ToString() + '\n';
            output += homeCanvas.selectedDateRange.Start.ToString() + '\n';
            output += homeCanvas.selectedDateRange.End.ToString() + '\n';
            output += homeCanvas.selectedType.ToString();

            List<Room> rooms = DataModels.GetInstance().GetRooms(locations, homeCanvas.selectedType.maxGuests,
                homeCanvas.selectedDateRange.Start, homeCanvas.selectedDateRange.End);

            MessageBox.Show("DONE");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DataModels database = DataModels.GetInstance();
            var x = database.GetUserBookings(database.GetUser("Ramy"));
            MessageBox.Show("DONE");
        }
    }
}
