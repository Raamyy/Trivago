using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Trivago.Models;

namespace Trivago.Front_End
{
    class RoomsListShowCanvas : CustomCanvas
    {
        private static RoomsListShowCanvas roomsListShowCanvas;
        private List<Room> rooms;
        private List<MealPlan> selectedMealPlan;
        private List<Tuple<Website, int>> selectedWebsitePrice;

        public MealPlan GetSelectedMealPlan(int index)
        {
            return selectedMealPlan[index];
        }

        public Room GetSelectedRoom(int index)
        {
            return rooms[index];
        }

        public Website GetSelectedWebsite(int index)
        {
            return selectedWebsitePrice[index].Item1;
        }

        public int GetSelectedRoomPrice(int index)
        {
            return selectedWebsitePrice[index].Item2;
        }

        private RoomsListShowCanvas(Canvas canvas) : base(canvas)
        {
        }
        
        public static RoomsListShowCanvas GetInstance(Canvas canvas, List<Room> rooms)
        {
            if (roomsListShowCanvas == null)
                roomsListShowCanvas = new RoomsListShowCanvas(canvas);
            roomsListShowCanvas.rooms = rooms;
            return roomsListShowCanvas;
        }


        public override void Initialize()
        {
            selectedMealPlan = new List<MealPlan>(new MealPlan[rooms.Count]);
            selectedWebsitePrice = new List<Tuple<Website, int>>(new Tuple<Website, int>[rooms.Count]);
            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;
            double cardHeight = 0.7 * canvas.Height;

            canvas.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));

            ScrollViewer roomScrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            }; 
            canvas.Children.Add(roomScrollViewer);

            StackPanel roomCardStackPanel = new StackPanel();
            roomScrollViewer.Content = roomCardStackPanel;

            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];
                List<Tuple<Website, int>> websitePrice = DataModels.GetInstance().GetWebsitePricesForRoom(room);
                string bestPrice = "NaN";
                if(websitePrice.Count == 0)
                continue;
                else
                bestPrice = websitePrice[0].Item2.ToString();
                //creates room card
                Border cardBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3),
                    Width = cardWidth,
                    Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, 0)
                };
                Grid roomGrid = new Grid
                {
                    Width = cardWidth,

                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0.4 * cardWidth) },
                        new ColumnDefinition { Width = new GridLength(0.6 * cardWidth) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };
                cardBorder.Child = roomGrid;
                roomCardStackPanel.Children.Add(cardBorder);

                //set card image
                Image roomImage = room.image.GetImage();
                roomImage.MaxWidth = 0.35 * cardWidth;
                roomImage.MaxHeight = 0.85 * cardHeight;
                Grid.SetColumn(roomImage, 0);
                Grid.SetRow(roomImage, 0);
                roomGrid.Children.Add(roomImage);

                //set roomdatastackpanel
                StackPanel roomDataStackPanel = new StackPanel
                {
                    Width = 0.6 * cardWidth,
                    Height = 0.85 * cardHeight,
                };
                Grid.SetColumn(roomDataStackPanel, 1);
                Grid.SetRow(roomDataStackPanel, 0);
                roomGrid.Children.Add(roomDataStackPanel);

                //set room type
                Label roomTypeLabel = new Label
                {
                    Content = "Room Type: " + room.type.name,
                    FontSize = 22,
                    Margin = new Thickness(0, 0.2 * cardHeight, 0, 0)
                };
                roomDataStackPanel.Children.Add(roomTypeLabel);

                //set hotel name
                Label roomHotelLabel = new Label
                {
                    Content = "Hotel: " + room.hotel.name,
                    FontSize = 22,
                    Margin = new Thickness(0, 0.05 * cardHeight, 0, 0)
                };
                roomDataStackPanel.Children.Add(roomHotelLabel);

                //set room price
                Label roomPriceLabel = new Label
                {
                    Content = "Best Price: "+ bestPrice,
                    FontSize = 22,
                    Margin = new Thickness(0, 0.05 * cardHeight, 0, 0)
                };
                roomDataStackPanel.Children.Add(roomPriceLabel);

                //set Reserve button and location
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0.5 * roomDataStackPanel.Width)},
                        new ColumnDefinition { Width = new GridLength(0.5 * roomDataStackPanel.Width)}
                    },
                    Margin = new Thickness(0, 0.05 * cardHeight, 0, 0)
                };
                roomDataStackPanel.Children.Add(grid);
                
                Label locationLabel = new Label
                {
                    Content = "Loaction: " + room.hotel.location.city + ", " + room.hotel.location.country,
                    FontSize = 22,
                };
                Grid.SetColumn(locationLabel, 0);
                grid.Children.Add(locationLabel);

                Button reserveButton = FrontEndHelper.CreateButton(cardWidth * 0.1, cardHeight * 0.1, "Reserve");
                reserveButton.Tag = i;
                reserveButton.Click += FrontEndHelper.GetMainWindow().ReserveButton_Click;
                Grid.SetColumn(reserveButton, 1);
                grid.Children.Add(reserveButton);

                //creates view more expander
                Expander viewMoreExpander = new Expander
                {
                    Width = canvas.Width,
                    Margin = new Thickness(0, 0, 0, 0.05 * cardHeight),
                    Header = "More Data"
                };
                Grid.SetColumnSpan(viewMoreExpander, 2);
                Grid.SetRow(viewMoreExpander, 1);
                roomGrid.Children.Add(viewMoreExpander);

                //creates tabs
                TabControl MoreDetailsTabs = new TabControl();
                viewMoreExpander.Content = MoreDetailsTabs;
                MoreDetailsTabs.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));

                //creates meals tab
                TabItem MealsTab = new TabItem { Header = "Meals" };
                StackPanel MealsPanel = new StackPanel();
                MealsTab.Content = MealsPanel;
                MoreDetailsTabs.Items.Add(MealsTab);

                for (int j = 0; j < room.hotel.mealPlans.Count; j++)
                {
                    MealPlan mealPlan = room.hotel.mealPlans[j];
                    RadioButton mealPlanRadioButton = new RadioButton
                    {
                        GroupName = "MealPlanRadioGroup " + i.ToString(),
                        Content = mealPlan,
                        FontSize = 22,
                        Margin = new Thickness(0, 0.025 * cardHeight, 0, 0)
                    };
                    mealPlanRadioButton.Checked += mealRadioButtonChecked;
                    if (j == 0)
                        mealPlanRadioButton.IsChecked = true;
                    MealsPanel.Children.Add(mealPlanRadioButton);
                }

                //creates website and prices tab
                TabItem websitesTab = new TabItem { Header = "Websites" };
                StackPanel websitesPanel = new StackPanel();
                websitesTab.Content = websitesPanel;
                MoreDetailsTabs.Items.Add(websitesTab);

                
                for (int j = 0; j < websitePrice.Count; j++)
                {
                    RadioButton websitePriceRadioButton = new RadioButton
                    {
                        GroupName = "WebsitePriceRadioGroup " + i.ToString(),
                        Tag = websitePrice[j],
                        Content = websitePrice[j].Item1.name + " , " + websitePrice[j].Item2.ToString(),
                        FontSize = 22,
                        Margin = new Thickness(0, 0.025 * cardHeight, 0, 0)
                    };
                    websitePriceRadioButton.Checked += webstiePriceRadioButtonChecked;
                    if (j == 0)
                        websitePriceRadioButton.IsChecked = true;
                    websitesPanel.Children.Add(websitePriceRadioButton);
                }

                //creates room view tab
                TabItem roomViewsTab = new TabItem { Header = "Views" };
                StackPanel roomViewsPanel = new StackPanel();
                roomViewsTab.Content = roomViewsPanel;
                MoreDetailsTabs.Items.Add(roomViewsTab);

                foreach (RoomView view in room.views)
                {
                    Label viewLabel = new Label
                    {
                        Content = view.view,
                        FontSize = 22,
                        Margin = new Thickness(0, 0.025 * cardHeight, 0, 0)
                    };
                    roomViewsPanel.Children.Add(viewLabel);
                }

                //create room photos
                TabItem hotelPhotosTab = new TabItem { Header = "Photos" };
                Canvas hotelPhotosCanvas = new Canvas
                {
                    Width = cardWidth,
                    Height = 300
                };
                hotelPhotosTab.Content = hotelPhotosCanvas;
                MoreDetailsTabs.Items.Add(hotelPhotosTab);

                List<CustomImage> images = new List<CustomImage>();
                images.Add(room.hotel.image);
                foreach (HotelFacility facility in room.hotel.facilities)
                    images.Add(facility.image);
                foreach (PlaceOfIntrest placeOfIntrest in room.hotel.location.placesOfIntrest)
                    images.Add(placeOfIntrest.image);
                ImageAlbum hotelAlbum = new ImageAlbum(hotelPhotosCanvas, 25, 25, 250, 250, images);

                //creates room reviews
                TabItem roomReviewsTab = new TabItem { Header = "Room Reviews" };
                StackPanel roomReviewsStackPanel = new StackPanel();
                roomReviewsTab.Content = roomReviewsStackPanel;
                MoreDetailsTabs.Items.Add(roomReviewsTab);

                List<Booking> roomBookings = DataModels.GetInstance().GetRoomBookings(room);
                for (int j = 0; j < roomBookings.Count; j++)
                {
                    Booking booking = roomBookings[j];
                    if (booking.bookingReview == null)
                        continue;
                    Border roomBookingCardBorder = new Border
                    {
                        Width = 0.8 * cardWidth,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        BorderThickness = new Thickness(3),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    if (j == roomBookings.Count - 1)
                        roomBookingCardBorder.Margin = new Thickness(0.1 * cardWidth, 25, 0.1 * cardWidth, 25);
                    else
                        roomBookingCardBorder.Margin = new Thickness(0.1 * cardWidth, 25, 0.1 * cardWidth, 0);
                    StackPanel roomBookingCardPanel = new StackPanel();
                    roomBookingCardBorder.Child = roomBookingCardPanel;
                    roomReviewsStackPanel.Children.Add(roomBookingCardBorder);

                    Label userNameLabel = new Label
                    {
                        Content = "User Name : " + booking.bookingUser.username,
                        FontSize = 22
                    };
                    roomBookingCardPanel.Children.Add(userNameLabel);

                    Label ratingLabel = new Label
                    {
                        Content = "Rating : " + booking.bookingReview.rating,
                        FontSize = 22
                    };
                    roomBookingCardPanel.Children.Add(ratingLabel);

                    Label startDateLabel = new Label
                    {
                        Content = "From " + booking.startDate.ToShortDateString() + " To " + booking.endDate.ToShortDateString(),
                        FontSize = 22
                    };
                    roomBookingCardPanel.Children.Add(startDateLabel);

                    TextBlock description = new TextBlock
                    {
                        Width = 0.8 * cardWidth - 5,
                        Text = "Description : " + booking.bookingReview.description,
                        FontSize = 22,
                        Margin = new Thickness(0, 0, 0, 10),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Padding = new Thickness(5, 0, 0, 0)
                    };
                    roomBookingCardPanel.Children.Add(description);
                }
            }
        }

        private void mealRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            RadioButton mealRadioButton = (RadioButton)sender;
            string[] s = mealRadioButton.GroupName.Split(separator: ' ');
            int idx = int.Parse(s[1]);
            selectedMealPlan[idx] = (MealPlan)mealRadioButton.Content;
        }

        private void webstiePriceRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            RadioButton websitePriceRadioButton = (RadioButton)sender;
            string[] s = websitePriceRadioButton.GroupName.Split(separator: ' ');
            int idx = int.Parse(s[1]);
            selectedWebsitePrice[idx] = (Tuple<Website, int>)websitePriceRadioButton.Tag;
        }
    }
}
