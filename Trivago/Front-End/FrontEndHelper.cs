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
       public static MainWindow mainWindow;
       public static Admin_window adminWindow;

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

        public static Trivago.MainWindow GetMainWindow()
        {
            return mainWindow;
        }

        public static Trivago.Admin_window GetAdminWindow()
        {
            return adminWindow;
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

        public static void CreateAddWebsitePopupWindow()
        {
            
            Window popup = new Window
            {
                Width = 330,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "Add Website"
            };

            StackPanel stackPanel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Margin = new Thickness(10, popup.Height/4, 10, 10)
            };

            Grid popupGrid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width =GridLength.Auto},
                        new ColumnDefinition { Width =GridLength.Auto}
                    },
                RowDefinitions =
                {
                    new RowDefinition{ Height = GridLength.Auto},
                    new RowDefinition{ Height = GridLength.Auto}
                }
            };
            popup.Content = stackPanel;
            stackPanel.Children.Add(popupGrid);

            //
            Label websiteNameLabel = new Label
            {
                Content = "Website Link : ",
                FontSize = 17
            };
            Grid.SetColumn(websiteNameLabel, 0);
            Grid.SetRow(websiteNameLabel, 0);
            popupGrid.Children.Add(websiteNameLabel);
            
            //
            Label websiteRatingLabel = new Label
            {
                Content = "Website Rating : ",
                FontSize = 17
            };
            Grid.SetColumn(websiteRatingLabel, 0);
            Grid.SetRow(websiteRatingLabel, 1);
            popupGrid.Children.Add(websiteRatingLabel);
            
            //
            TextBox websiteNameTextBox = new TextBox
            {
                Width = 150,
                Height = 20,
                FontSize = 15
            };
            Grid.SetColumn(websiteNameTextBox, 1);
            Grid.SetRow(websiteNameTextBox, 0);
            popupGrid.Children.Add(websiteNameTextBox);

            //
            TextBox websiteRatingTextBox = new TextBox
            {
                Width = 150,
                Height = 20,
                FontSize = 15
            };
            Grid.SetColumn(websiteRatingTextBox, 1);
            Grid.SetRow(websiteRatingTextBox, 1);
            popupGrid.Children.Add(websiteRatingTextBox);

            
            Button addWebsiteConfirmButton = FrontEndHelper.CreateButton(80, 40, "Add");
            addWebsiteConfirmButton.Margin = new Thickness(0, 10, 0, 0);

            List<TextBox> data = new List<TextBox>();
            data.Add(websiteNameTextBox);
            data.Add(websiteRatingTextBox);

            addWebsiteConfirmButton.Tag = data;
            addWebsiteConfirmButton.Click += AddWebsiteConfirmButton_Click;
            stackPanel.Children.Add(addWebsiteConfirmButton);
                        

            popup.Owner = GetAdminWindow();
            popup.ShowDialog();
        }

        public static void CreateUpdateWebsitePopupWindow(Website website)
        {

            Window popup = new Window
            {
                Width = 330,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "Update Website"
            };

            StackPanel stackPanel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Margin = new Thickness(10, popup.Height / 4, 10, 10)
            };

            Grid popupGrid = new Grid
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width =GridLength.Auto},
                        new ColumnDefinition { Width =GridLength.Auto}
                    },
                RowDefinitions =
                {
                    new RowDefinition{ Height = GridLength.Auto},
                    new RowDefinition{ Height = GridLength.Auto}
                }
            };
            popup.Content = stackPanel;
            stackPanel.Children.Add(popupGrid);

            //
            Label websiteNameLabel = new Label
            {
                Content = "Website Link : ",
                FontSize = 17
            };
            Grid.SetColumn(websiteNameLabel, 0);
            Grid.SetRow(websiteNameLabel, 0);
            popupGrid.Children.Add(websiteNameLabel);

            //
            Label websiteNameLabel2 = new Label
            {
                Content = website.name,
                FontSize = 15
            };
            Grid.SetColumn(websiteNameLabel2, 1);
            Grid.SetRow(websiteNameLabel2, 0);
            popupGrid.Children.Add(websiteNameLabel2);

            //
            Label websiteRatingLabel = new Label
            {
                Content = "Website Rating : ",
                FontSize = 17
            };
            Grid.SetColumn(websiteRatingLabel, 0);
            Grid.SetRow(websiteRatingLabel, 1);
            popupGrid.Children.Add(websiteRatingLabel);

            //
            TextBox websiteRatingTextBox = new TextBox
            {
                Width = 150,
                Height = 20,
                FontSize = 15,
                Text = website.rating.ToString()
            };
            Grid.SetColumn(websiteRatingTextBox, 1);
            Grid.SetRow(websiteRatingTextBox, 1);
            popupGrid.Children.Add(websiteRatingTextBox);


            Button updateWebsiteConfirmButton = FrontEndHelper.CreateButton(80, 40, "Update");
            updateWebsiteConfirmButton.Margin = new Thickness(0, 10, 0, 0);

            List<Object> data = new List<Object>();
            data.Add(website);
            data.Add(websiteRatingTextBox);
            updateWebsiteConfirmButton.Tag = data;

            updateWebsiteConfirmButton.Click += UpdateWebsiteConfirmButton_Click;
            stackPanel.Children.Add(updateWebsiteConfirmButton);


            popup.Owner = GetAdminWindow();
            popup.ShowDialog();
        }

        private static void UpdateWebsiteConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            List<object> data = (List<object>)button.Tag;
            Website website= (Website)data[0];
            TextBox websiteRatingTextBox =(TextBox)data[1];

            string websiteRating = websiteRatingTextBox.Text;
            if (!BackEndHelper.IsNumber(websiteRating))
            {
                MessageBox.Show("Rating must be a number");
                return;
            }
            int newRating = int.Parse(websiteRating);

            website.rating = newRating;
            DataModels database = DataModels.GetInstance();
            
            // TODO: remove comment when function done

            /*
            bool done = database.UpdateWebsite(website);
            if(done)
                MessageBox.Show("Website Updated successfully !");
            else
                MessageBox.Show("Couldn't update website");
            */
        }

        private static void AddWebsiteConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            List<TextBox> data = (List<TextBox>)button.Tag;
            string websiteName = data[0].Text;
            string websiteRating = data[1].Text;

            if(!BackEndHelper.IsNumber(websiteRating))
            {
                MessageBox.Show("Rating must be a number");
                return;
            }
            int rating = int.Parse(websiteRating);
            DataModels database = DataModels.GetInstance();
            Website website = new Website(websiteName, rating);
            bool done = database.AddWebsite(website);
            if (done)
                MessageBox.Show("Website Added successfully !");
            else
                MessageBox.Show("Couldn't add website");
        }
    }
}
