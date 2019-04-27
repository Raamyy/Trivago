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
            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;
            double cardHeight = 0.5 * canvas.Height;

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
                    Height = cardHeight,
                    Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, 0)
                };
                Grid roomGrid = new Grid
                {
                    Width = cardWidth,
                    Height = cardHeight,
                };
                roomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4 * cardWidth) } );
                roomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.6 * cardWidth) });
                cardBorder.Child = roomGrid;
                roomCardStackPanel.Children.Add(cardBorder);

                //set card image
                Image roomImage = room.image.GetImage();
                roomImage.MaxWidth = 0.4 * cardWidth;
                roomImage.MaxHeight = 0.8 * cardHeight;
                Grid.SetColumn(roomImage, 0);
                roomGrid.Children.Add(roomImage);
                
                //set roomdatastackpanel
                StackPanel roomDataStackPanel = new StackPanel
                {
                    Width = 0.6 * cardWidth,
                    Height = cardHeight
                };
                Grid.SetColumn(roomDataStackPanel, 1);
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
                    FontSize = 22
                };
                Grid.SetColumn(locationLabel, 0);
                grid.Children.Add(locationLabel);

                Button viewMoreButton = new Button
                {
                    Content = "View More",
                    Width = cardWidth * 0.1,
                    Height = cardHeight * 0.1,
                    Margin = new Thickness(0, 0.05 * cardHeight, 0.05 * cardWidth, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(viewMoreButton, 1);
                grid.Children.Add(viewMoreButton);

                roomDataStackPanel.Children.Add(grid);
            }
        }
    }
}
