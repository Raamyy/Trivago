using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Trivago.Models;

namespace Trivago.Front_End
{
    class BookingsListShow : CustomCanvas
    {
        private static BookingsListShow bookingListShow;
        private List<Booking> bookings;

        public static BookingsListShow GetInstance(Canvas canvas, List<Booking> bookings)
        {
            if (bookingListShow == null)
                bookingListShow = new BookingsListShow(canvas);
            bookingListShow.bookings = bookings;
            return bookingListShow;
        }

        public BookingsListShow(Canvas canvas) : base(canvas)
        {
        }

        public override void Initialize()
        {
            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;

            ScrollViewer bookingsScrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(bookingsScrollViewer);

            StackPanel bookingCardStackPanel = new StackPanel();
            bookingsScrollViewer.Content = bookingCardStackPanel;

            for(int i = 0; i < bookings.Count; i++)
            {
                Booking booking = bookings[i];

                //initialize card
                Border cardBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3),
                    Width = cardWidth,
                    
                };
                if (i == bookings.Count - 1)
                    cardBorder.Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, boxSpacing);
                else
                    cardBorder.Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, 0);
                bookingCardStackPanel.Children.Add(cardBorder);

                StackPanel bookingStackPanel = new StackPanel
                {
                    Width = cardWidth
                };
                cardBorder.Child = bookingStackPanel;

                //set username label
                Label userNameLabel = new Label
                {
                    Content = "User Name : " + booking.bookingUser.username,
                    FontSize = 22,
                    Margin = new Thickness(0, 15, 0, 0)
                };
                bookingStackPanel.Children.Add(userNameLabel);

                //set bookingnumber label
                Label bookingNumberLabel = new Label
                {
                    Content = "Booking Number : " + booking.number,
                    FontSize = 22
                };
                bookingStackPanel.Children.Add(bookingNumberLabel);

                //set room number and hotel grid
                Grid dataGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(cardWidth / 2) },
                        new ColumnDefinition { Width = new GridLength(cardWidth / 2) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto},
                        new RowDefinition { Height = GridLength.Auto},
                        new RowDefinition { Height = GridLength.Auto},
                        new RowDefinition { Height = GridLength.Auto}
                    }
                };
                bookingStackPanel.Children.Add(dataGrid);
                
                Label hotelLabel = new Label
                {
                    Content = "Hotel : " + booking.bookingRoom.hotel.name,
                    FontSize = 22
                };
                Grid.SetRow(hotelLabel, 0);
                Grid.SetColumn(hotelLabel, 0);
                dataGrid.Children.Add(hotelLabel);

                Label roomLabel = new Label
                {
                    Content = "Room Number : " + booking.bookingRoom.number,
                    FontSize = 22
                };
                Grid.SetRow(roomLabel, 0);
                Grid.SetColumn(roomLabel, 1);
                dataGrid.Children.Add(roomLabel);

                Label websiteLabel = new Label
                {
                    Content = "Website : " + booking.bookingWebsite.name,
                    FontSize = 22
                };
                Grid.SetRow(websiteLabel, 1);
                Grid.SetColumn(websiteLabel, 0);
                dataGrid.Children.Add(websiteLabel);

                Label mealPlanLabel = new Label
                {
                    Content = "Meal Plan : " + booking.bookingMealPlan.name,
                    FontSize = 22
                };
                Grid.SetRow(mealPlanLabel, 1);
                Grid.SetColumn(mealPlanLabel, 1);
                dataGrid.Children.Add(mealPlanLabel);

                Label numberofGuestsLabel = new Label
                {
                    Content = "Number of Guests : " + booking.numberOfGuests,
                    FontSize = 22
                };
                Grid.SetRow(numberofGuestsLabel, 2);
                Grid.SetColumn(numberofGuestsLabel, 0);
                dataGrid.Children.Add(numberofGuestsLabel);

                Label priceLabel = new Label
                {
                    Content = "Price : ",
                    FontSize = 22
                };
                Grid.SetRow(priceLabel, 2);
                Grid.SetColumn(priceLabel, 1);
                dataGrid.Children.Add(priceLabel);

                Button reviewButton = FrontEndHelper.CreateButton(0.15 * cardWidth, 50, "Review");
                reviewButton.Margin = new Thickness(0, 15, 0, 15);
                reviewButton.Click += ReviewButton_Click;
                reviewButton.Tag = booking;
                Grid.SetRow(reviewButton, 3);
                Grid.SetColumn(reviewButton, 0);
                dataGrid.Children.Add(reviewButton);

                Button refundButton = FrontEndHelper.CreateButton(0.15 * cardWidth, 50, "Refund");
                refundButton.Margin = new Thickness(0, 15, 0, 15);
                refundButton.Click += RefundButton_Click;
                refundButton.Tag = booking;
                Grid.SetRow(refundButton, 3);
                Grid.SetColumn(refundButton, 1);
                dataGrid.Children.Add(refundButton);
                
            }
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            FrontEndHelper.CreateAddReviewPopupWindow( (Booking)((Button)sender).Tag  );
        }

        private void RefundButton_Click(object sender, RoutedEventArgs e)
        {
            DataModels database = DataModels.GetInstance();
            Button button = (Button)sender;
            Booking booking = (Booking)button.Tag;
            User user = FrontEndHelper.GetMainWindow().ActiveUser;

            if(DateTime.Now > booking.startDate.Subtract(TimeSpan.FromDays(3)))
            {
                MessageBox.Show("This booking is not refundable");
                return;
            }
            if (database.DeleteBooking(booking.number))
                MessageBox.Show("Refunded.");
            else
                MessageBox.Show("Error");
        }
    }
}
