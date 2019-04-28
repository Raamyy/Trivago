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
        private List<MealPlan> mealPlans;

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
            mealPlans = new List<MealPlan>(new MealPlan[rooms.Count]);

            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;
            double cardHeight = 0.7 * canvas.Height;

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
                    Content = "Price: ",
                    FontSize = 22,
                    Margin = new Thickness(0, 0.05 * cardHeight, 0, 0)
                };
                roomDataStackPanel.Children.Add(roomPriceLabel);

                //set view more button
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0.5 * roomDataStackPanel.Width)},
                        new ColumnDefinition { Width = new GridLength(0.5 * roomDataStackPanel.Width)}
                    },
                    Margin = new Thickness(0, 0.05 * cardHeight, 0, 0)
                };

                Label locationLabel = new Label
                {
                    Content = "Loaction: " + room.hotel.location.city + ", " + room.hotel.location.country,
                    FontSize = 22,
                };
                Grid.SetColumn(locationLabel, 0);
                grid.Children.Add(locationLabel);

                Button viewMoreButton = new Button
                {
                    Content = "Reserve",
                    Width = cardWidth * 0.1,
                    Height = cardHeight * 0.1,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(viewMoreButton, 1);
                grid.Children.Add(viewMoreButton);
                roomDataStackPanel.Children.Add(grid);

                Expander viewMoreExpander = new Expander
                {
                    Width = canvas.Width,
                    Margin = new Thickness(0, 0, 0, 0.05 * cardHeight)
                };
                Grid.SetColumnSpan(viewMoreExpander, 2);
                Grid.SetRow(viewMoreExpander, 1);
                roomGrid.Children.Add(viewMoreExpander);

                TabControl MoreDetailsTabs = new TabControl();
                viewMoreExpander.Content = MoreDetailsTabs;

                TabItem MealsTab = new TabItem { Header = "Meals" };
                StackPanel MealsPanel = new StackPanel();
                MealsTab.Content = MealsPanel;
                MoreDetailsTabs.Items.Add(MealsTab);

                for (int j = 0; j < room.hotel.mealPlans.Count; j++)
                {
                    MealPlan mealPlan = room.hotel.mealPlans[j];
                    RadioButton mealPlanRadioButton = new RadioButton
                    {
                        GroupName = i.ToString(),
                        Content = mealPlan,
                        FontSize = 22,
                        Margin = new Thickness(0, 0.025 * cardHeight, 0, 0)
                    };
                    mealPlanRadioButton.Checked += mealRadioButtonChecked;
                    if (j == 0)
                        mealPlanRadioButton.IsChecked = true;
                    MealsPanel.Children.Add(mealPlanRadioButton);
                }
            }
        }

        private void mealRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            RadioButton mealRadioButton = (RadioButton)sender;
            int idx = int.Parse(mealRadioButton.GroupName);
            mealPlans[idx] = (MealPlan)mealRadioButton.Content;
        }
    }
}
