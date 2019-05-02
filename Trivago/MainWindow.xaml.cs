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
using System.Windows.Controls.Primitives;

namespace Trivago
{   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double NavigationCanvasHeight = 100;
        public CustomCanvas CurrentCanvas;
        public CustomCanvas currentNavigationCanvas;
        public User ActiveUser;

        public MainWindow()
        {
            InitializeComponent();
            FrontEndHelper.SetMainWindow(this);
            InitializeInitialCanvases();
        }

        private void InitializeInitialCanvases()
        {
            InitializeNavigationCanvas();
            InitializeHomeCanvas();
        }

        private void InitializeLoginCanvas()
        {
            CustomCanvas loginCanvas = Front_End.LoginCanvas.GetInstance(LoginCanvas);
            loginCanvas.SetCanvasDimensions(Window.Width, Window.Height - NavigationCanvasHeight);
            loginCanvas.SetCanvasCoord(0, NavigationCanvasHeight);
            loginCanvas.Show();
        }

        private void InitializeSignupCanvas()
        {
            CustomCanvas signupCanvas = Front_End.SignupCanvas.GetInstance(SignupCanvas);
            signupCanvas.SetCanvasDimensions(Window.Width, Window.Height - NavigationCanvasHeight);
            signupCanvas.SetCanvasCoord(0, NavigationCanvasHeight);
            signupCanvas.Show();
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

        private void InitializeBookingsListShowCanvas(List<Booking> bookings)
        {
            CustomCanvas bookingsListShowCanvas = Front_End.BookingsListShow.GetInstance(BookingsListShowCanvas, bookings);
            bookingsListShowCanvas.SetCanvasDimensions(Window.Width, Window.Height - NavigationCanvasHeight);
            bookingsListShowCanvas.SetCanvasCoord(0, NavigationCanvasHeight);
            bookingsListShowCanvas.Show();
        }

        private void InitializeNavigationCanvas()
        {
            CustomCanvas navigationCanvas = Front_End.NavigationCanvas.GetInstance(NavigationCanvas);
            navigationCanvas.SetCanvasCoord(0, 0);
            navigationCanvas.SetCanvasDimensions(Window.Width, NavigationCanvasHeight);
            navigationCanvas.Show();
            currentNavigationCanvas = navigationCanvas;
        }

        private void InitializeLoggedinNavigationCanvas(User user)
        {
            CustomCanvas loggedinNavigationCanvas = Front_End.LoggedinNavigationCanvas.GetInstance(LoggedinNavigationCanvas, user);
            loggedinNavigationCanvas.SetCanvasCoord(0, 0);
            loggedinNavigationCanvas.SetCanvasDimensions(Window.Width, NavigationCanvasHeight);
            loggedinNavigationCanvas.Show();
            currentNavigationCanvas = loggedinNavigationCanvas;
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

        public void ShowAdminPanel()
        {
            Admin_window win = new Admin_window();
            win.Show();
            this.Close();
        }

        public void LoginButton_Click(object sender, RoutedEventArgs args)
        {
            CurrentCanvas.Hide();
            InitializeLoginCanvas();
        }

        public void SignupButton_Click(object sender, RoutedEventArgs args)
        {
            CurrentCanvas.Hide();
            InitializeSignupCanvas();
        }

        public void LoginButtonUserData_Click(object sender, RoutedEventArgs args)
        {
            Front_End.LoginCanvas loginCanvas = Front_End.LoginCanvas.GetInstance(LoginCanvas);
            if(DataModels.GetInstance().isAdmin(loginCanvas.GetUserName(), loginCanvas.GetPassword()))
            {
                ShowAdminPanel();
                return;
            }
            User user = DataModels.GetInstance().LogUser(loginCanvas.GetUserName(), loginCanvas.GetPassword());
            if(user == null)
            {
                MessageBox.Show("Invalid data");
                return;
            }
            Front_End.NavigationCanvas.GetInstance(NavigationCanvas).Hide();
            InitializeLoggedinNavigationCanvas(user);
            ActiveUser = user;

            CurrentCanvas.Hide();
            InitializeHomeCanvas();
        }

        public void SignupButtonUserData_Click(object sender, RoutedEventArgs args)
        {
            Front_End.SignupCanvas signupCanvas = Front_End.SignupCanvas.GetInstance(SignupCanvas);
            if(BackEndHelper.IsValidEmail(signupCanvas.GetEmail()) == false)
            {
                MessageBox.Show("Invalid Email");
                return;
            }
            if(signupCanvas.GetPassword().Length < 8)
            {
                MessageBox.Show("Passowrd length must be more than 8 characters");
                return;
            }
            if(signupCanvas.GetCreditCardSerial().Length > 20 || BackEndHelper.IsNumber(signupCanvas.GetCreditCardSerial()) == false)
            {
                MessageBox.Show("Invalid serial number");
                return;
            }
            if(signupCanvas.GetCreditCardCVV().Length > 4 || signupCanvas.GetCreditCardCVV().Length < 3 || BackEndHelper.IsNumber(signupCanvas.GetCreditCardCVV()) == false)
            {
                MessageBox.Show("Invalid cvv");
                return;
            }
            if(signupCanvas.GetExpirationDate() < DateTime.Today)
            {
                MessageBox.Show("Credit card expired");
                return;
            }
            User user = new User
                (
                    signupCanvas.GetUserName(),
                    signupCanvas.GetEmail(),
                    signupCanvas.GetName(),
                    null,
                    new CreditCard
                        (   signupCanvas.GetCreditCardSerial(),
                            int.Parse(signupCanvas.GetCreditCardCVV()),
                            signupCanvas.GetExpirationDate()
                        )
                 );
            bool valid = DataModels.GetInstance().RegisterUser(user, signupCanvas.GetPassword());
            if(valid == false)
            {
                MessageBox.Show("User name taken");
                return;
            }
            Front_End.NavigationCanvas.GetInstance(NavigationCanvas).Hide();
            InitializeLoggedinNavigationCanvas(user);
            ActiveUser = user;

            CurrentCanvas.Hide();
            InitializeHomeCanvas();
        }

        public void HelloLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            User user = ((LoggedinNavigationCanvas)currentNavigationCanvas).GetActiveUser();
            CurrentCanvas.Hide();
            InitializeBookingsListShowCanvas(DataModels.GetInstance().GetUserBookings(user));
        }

        public void ReserveButton_Click(object sender, RoutedEventArgs args)
        {
            if(ActiveUser == null)
            {
                MessageBox.Show("Please Login");
                return;
            }
            Button reserveButton = (Button)sender;
            int index = (int)reserveButton.Tag;
            RoomsListShowCanvas roomListShowCanvas = (RoomsListShowCanvas)CurrentCanvas;

            FrontEndHelper.CreateReservePopupWindow(roomListShowCanvas, index);
        }
    }
}
