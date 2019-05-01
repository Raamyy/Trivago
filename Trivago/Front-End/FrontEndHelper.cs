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
            return (Trivago.MainWindow)App.Current.MainWindow;
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
    }
}
