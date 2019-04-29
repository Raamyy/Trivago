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
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;

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
            //AddDatabase();
        }

        private void AddDatabase()
        {
            //AddHotels();
            //AddRoom();
        }

        private void AddRoom()
        {
            OracleConnection connection = new OracleConnection("data source = orcl; user id = scott; password = tiger;");
            connection.Open();
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandText = "Insert into Room values(1, 1, 'Single', :image)";
            command.Parameters.Add("image", new CustomImage(@"resources\images\Room1.jpg").GetByteImage());
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void AddHotels()
        {
            OracleConnection connection = new OracleConnection("data source = orcl; user id = scott; password = tiger;");
            connection.Open();
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandText = "Insert into Hotel values(1, 'conrad', :image, 'Cairo', 'Egypt')";
            command.Parameters.Add("image", new CustomImage(@"resources\images\hotel1.jpg").GetByteImage());
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            connection.Close();
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
            //InitializeRoomsListShowCanvas();
        }

        private void InitializeRoomsListShowCanvas(List<Room> rooms)
        {
            CustomCanvas roomsListCanvas = Front_End.RoomsListShowCanvas.GetInstance(RoomsListShowCanvas, rooms);
            roomsListCanvas.SetCanvasDimensions(Window.Width, Window.Height - NavigationCanvasHeight);
            roomsListCanvas.SetCanvasCoord(0, NavigationCanvasHeight);
            roomsListCanvas.Show();
        }

        private void InitializeHomeCanvas()
        {
            CustomCanvas homeCanvas = Front_End.HomeCanvas.GetInstance(HomeCanvas);
            homeCanvas.SetCanvasDimensions(Window.Width, Window.Height - NavigationCanvasHeight);
            homeCanvas.SetCanvasCoord(0, NavigationCanvasHeight);
            homeCanvas.Show();
        }

        private void InitializeNavigationCanvas()
        {
            CustomCanvas navigationCanvas = Front_End.NavigationCanvas.GetInstance(NavigationCanvas);
            navigationCanvas.SetCanvasCoord(0, 0);
            navigationCanvas.SetCanvasDimensions(Window.Width, NavigationCanvasHeight);
            navigationCanvas.Show();
        }

        public void LogoImage_MouseLeftButtonDown(object sender, RoutedEventArgs args)
        {
            CurrentCanvas.Hide();
            InitializeHomeCanvas();
        }

        public void SearchButton_Click(object sender, RoutedEventArgs args)
        {
            Front_End.HomeCanvas homeCanvas = Front_End.HomeCanvas.GetInstance(HomeCanvas);
            if(homeCanvas.selectedLocations.Count == 0)
            {
                MessageBox.Show("You Must Choose a location !", "Error");
                return;
            }
            List<Room> rooms = DataModels.GetInstance().GetRooms(homeCanvas.selectedLocations, homeCanvas.selectedType.maxGuests,
                homeCanvas.selectedDateRange.Start, homeCanvas.selectedDateRange.End);

            // TODO: Remove this
            for (int i = 0; i < 10; i++)
                rooms.Add(rooms[0]);

            CurrentCanvas.Hide();
            InitializeRoomsListShowCanvas(rooms);
        }
    }
}
