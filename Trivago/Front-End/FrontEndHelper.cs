using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trivago.Models;

namespace Trivago.Front_End
{
    static class FrontEndHelper
    {
        private static Trivago.MainWindow mainWindow;
        private static Trivago.Admin_window adminWindow;

        public static void SetMainWindow(MainWindow window)
        {
            mainWindow = window;
        }

        public static Trivago.MainWindow GetMainWindow()
        {
            return (Trivago.MainWindow)App.Current.MainWindow;
        }

        public static void SetAdminWindow(Admin_window window)
        {
            adminWindow = window;
        }

        public static Admin_window GetAdminWindow()
        {
            return adminWindow;
        }



        public static Button CreateButton(double width, double height, string content)
        {
            Button obj = new Button
            {
                Width = width,
                Height = height,
                Content = content,
                Background = new SolidColorBrush(Color.FromRgb(0, 127, 175)),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontSize = 20,
                Cursor = Cursors.Hand
            };
            obj.MouseEnter += button_MouseEnter;
            obj.MouseLeave += button_MouseLeave;
            return obj;
        }

        private static void button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = new SolidColorBrush(Color.FromRgb(0, 127, 175));
        }

        private static void button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = new SolidColorBrush(Color.FromRgb(0, 95, 127));
        }
        
        public static void CreateReservePopupWindow(RoomsListShowCanvas roomListShowCanvas, int index)
        {
            DataModels database = DataModels.GetInstance();

            Window popup = new Window
            {
                Width = 350,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "Confirm Booking"
            };

            Grid popupGrid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
            };
            popup.Content = popupGrid;

            StackPanel reservePopupStackPanel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Margin = new Thickness(10, 10, 10, 10)
            };
            popupGrid.Children.Add(reservePopupStackPanel);

            Grid numberofGuestsGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };
            reservePopupStackPanel.Children.Add(numberofGuestsGrid);

            Label numberofGuestsLabel = new Label
            {
                Content = "Number of Guests : ",
                FontSize = 22
            };
            Grid.SetColumn(numberofGuestsLabel, 0);
            numberofGuestsGrid.Children.Add(numberofGuestsLabel);

            TextBox numberofGuestTextBox = new TextBox
            {
                Width = 50,
                FontSize = 22
            };
            Grid.SetColumn(numberofGuestTextBox, 1);
            numberofGuestsGrid.Children.Add(numberofGuestTextBox);

            Label mealLabel = new Label
            {
                Content = "Meal Plan : " + roomListShowCanvas.GetSelectedMealPlan(index).name,
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            reservePopupStackPanel.Children.Add(mealLabel);

            Label websiteLabel = new Label
            {
                Content = "Website : " + roomListShowCanvas.GetSelectedWebsite(index).name,
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            reservePopupStackPanel.Children.Add(websiteLabel);

            Label priceLabel = new Label
            {
                Content = "Price : ",
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            reservePopupStackPanel.Children.Add(priceLabel);

            Calendar datePicker = new Calendar
            {
                SelectionMode = CalendarSelectionMode.SingleRange,
                Margin = new Thickness(0, 10, 0, 0)
            };
            datePicker.DisplayDateStart = DateTime.Today;
            List<Booking> bookings = database.GetRoomBookings(roomListShowCanvas.GetSelectedRoom(index));
            foreach (Booking booking in bookings)
            {
                datePicker.BlackoutDates.Add(new CalendarDateRange(booking.startDate, booking.endDate));
            }
            reservePopupStackPanel.Children.Add(datePicker);

            Button confirmButton = FrontEndHelper.CreateButton(80, 40, "Confirm");
            confirmButton.Margin = new Thickness(0, 10, 0, 0);
            reservePopupStackPanel.Children.Add(confirmButton);

            popup.Owner = GetMainWindow();
            popup.ShowDialog();
        }

        public static void CreateAddRoomPopupWindow()
        {
            double windowWidth = 500;
            double itemsWidth = 150;

            Window popup = new Window
            {
                Width = windowWidth,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "Add Room"
            };

            StackPanel addDataStackPanel = new StackPanel
            {
                Width = windowWidth,
                Margin = new Thickness(20, 0.15 * popup.Height, 0, 0)
            };
            popup.Content = addDataStackPanel;

            Grid addDataGrid = new Grid
            {
                Width = windowWidth,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };
            addDataStackPanel.Children.Add(addDataGrid);

            //add hotel licence number
            Label hotelLabel = new Label
            {
                Content = "Hotel : ",
                FontSize = 22
            };
            Grid.SetRow(hotelLabel, 0);
            Grid.SetColumn(hotelLabel, 0);
            addDataGrid.Children.Add(hotelLabel);

            ComboBox hotelComboBox = new ComboBox
            {
                FontSize = 22,
                Width = itemsWidth
            };
            List<Hotel> hotels = DataModels.GetInstance().GetAllHotels();
            foreach(Hotel hotel in hotels)
            {
                ComboBoxItem hotelItem = new ComboBoxItem
                {
                    Content = hotel
                };
                hotelComboBox.Items.Add(hotelItem);
            }
            if (hotelComboBox.Items.Count > 0)
                hotelComboBox.SelectedIndex = 0;
            Grid.SetRow(hotelComboBox, 0);
            Grid.SetColumn(hotelComboBox, 1);
            addDataGrid.Children.Add(hotelComboBox);

            //add room number
            Label roomNumberLabel = new Label
            {
                Content = "Room Number : ",
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(roomNumberLabel, 1);
            Grid.SetColumn(roomNumberLabel, 0);
            addDataGrid.Children.Add(roomNumberLabel);

            TextBox roomNumberTextBox = new TextBox
            {
                FontSize = 22,
                Width = itemsWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(roomNumberTextBox, 1);
            Grid.SetColumn(roomNumberTextBox, 1);
            addDataGrid.Children.Add(roomNumberTextBox);

            //add room type
            Label roomTypeLabel = new Label
            {
                Content = "Room Type : ",
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(roomTypeLabel, 2);
            Grid.SetColumn(roomTypeLabel, 0);
            addDataGrid.Children.Add(roomTypeLabel);

            ComboBox roomTypeComboBox = new ComboBox
            {
                Width = itemsWidth,
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(roomTypeComboBox, 2);
            Grid.SetColumn(roomTypeComboBox, 1);
            addDataGrid.Children.Add(roomTypeComboBox);

            List<RoomType> roomTypes = DataModels.GetInstance().GetAllRoomTypes();
            for (int j = 0; j < roomTypes.Count; j++)
            {
                RoomType roomType = roomTypes[j];
                ComboBoxItem roomTypeItem = new ComboBoxItem
                {
                    Content = roomType
                };
                roomTypeComboBox.Items.Add(roomTypeItem);
            }
            if (roomTypeComboBox.Items.Count > 0)
                roomTypeComboBox.SelectedIndex = 0;

            //add room photo
            Label roomPhotoLabel = new Label
            {
                Content = "Room Photo : ",
                FontSize = 22,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(roomPhotoLabel, 3);
            Grid.SetColumn(roomPhotoLabel, 0);
            addDataGrid.Children.Add(roomPhotoLabel);

            Button browseRoomPhotoButton = CreateButton(itemsWidth, hotelLabel.Height, "Browse");
            browseRoomPhotoButton.Margin = new Thickness(0, 10, 0, 0);
            browseRoomPhotoButton.Click += BrowseRoomPhotoButton_Click;
            browseRoomPhotoButton.Tag = "";
            Grid.SetRow(browseRoomPhotoButton, 3);
            Grid.SetColumn(browseRoomPhotoButton, 1);
            addDataGrid.Children.Add(browseRoomPhotoButton);

            Button addRoomButton = CreateButton(100, 50, "Add");
            addRoomButton.Margin = new Thickness(170, 30, 0, 0);
            addRoomButton.HorizontalAlignment = HorizontalAlignment.Left;
            addRoomButton.Click += AddRoomButton_Click;
            List<object> tags = new List<object>();
            tags.Add(hotelComboBox);
            tags.Add(roomNumberTextBox);
            tags.Add(roomTypeComboBox);
            tags.Add(browseRoomPhotoButton);
            tags.Add(popup);
            addRoomButton.Tag = tags;
            addDataStackPanel.Children.Add(addRoomButton);
            popup.ShowDialog();
        }

        private static void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            Button addRoomButton = (Button)sender;
            List<object> objects = (List<object>)addRoomButton.Tag;
            ComboBox hotelComboBox = (ComboBox)objects[0];
            TextBox roomNumberTextBox = (TextBox)objects[1];
            ComboBox roomTypeComboBox = (ComboBox)objects[2];
            Button browseRoomPhotoButton = (Button)objects[3];
            Window popup = (Window)objects[4];
            DataModels database = DataModels.GetInstance();

            if(hotelComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a hotel");
                return;
            }
            Hotel hotel = (Hotel)((ComboBoxItem)hotelComboBox.SelectedItem).Content;

            int roomNumber;
            if(!int.TryParse(roomNumberTextBox.Text, out roomNumber))
            {
                MessageBox.Show("Please enter a valid room number");
                return;
            }

            if(roomTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a room type");
                return;
            }
            RoomType roomType = (RoomType)((ComboBoxItem)roomTypeComboBox.SelectedItem).Content;

            if((string)browseRoomPhotoButton.Tag == "")
            {
                MessageBox.Show("Please select a photo path");
                return;
            }
            CustomImage roomImage = new CustomImage((string)browseRoomPhotoButton.Tag);

            Room room = new Room(roomNumber, hotel, roomType, roomImage, new List<RoomView>());
            if(database.AddRoom(room) == true)
            {
                MessageBox.Show("Added");
                if (adminWindow.currentCanvas != null)
                    adminWindow.currentCanvas.Hide();
                adminWindow.InitializeRoomsCanvas(database.GetAllRooms());
                popup.Close();
                return;
            }
            else
            {
                MessageBox.Show("please enter a valid room number");
                return;
            }
        }

        private static void BrowseRoomPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|PNG Files (*.png)|*.png",
                Title = "Select Room Photo"
            };
            dlg.ShowDialog();
            ((Button)sender).Tag = dlg.FileName.ToString();
        }
    }
}
