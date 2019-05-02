using Microsoft.Win32;
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
    class AdminRoomsCanvas : CustomCanvas
    {
        private static AdminRoomsCanvas adminRoomsCanvas;
        private List<Room> rooms;

        public static AdminRoomsCanvas GetInstance(Canvas canvas, List<Room> rooms)
        {
            if (adminRoomsCanvas == null)
                adminRoomsCanvas = new AdminRoomsCanvas(canvas);
            adminRoomsCanvas.rooms = rooms;
            return adminRoomsCanvas;
        }

        public override void Show()
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }
            canvas.Visibility = Visibility.Visible;
        }

        public override void Hide()
        {
            canvas.Children.Clear();
            IsInitialized = false;
            canvas.Visibility = Visibility.Hidden;
        }

        public AdminRoomsCanvas(Canvas canvas) : base(canvas)
        {
        }

        public override void Initialize()
        {
            double cardWidth = 0.8 * canvas.Width;

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(scrollViewer);

            StackPanel roomCardsStackPanel = new StackPanel
            {
                Width = canvas.Width
            };
            scrollViewer.Content = roomCardsStackPanel;

            Button AddButton = FrontEndHelper.CreateButton(120, 60, "Add");
            AddButton.Click += Room_Add_Button_Click;
            AddButton.Margin = new Thickness(0.8 * cardWidth, 20, 0, 0);
            roomCardsStackPanel.Children.Add(AddButton);

            for(int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];

                Border border = new Border
                {
                    Width = cardWidth,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3),
                };
                if (i == rooms.Count - 1)
                    border.Margin = new Thickness(0.1 * canvas.Width, 0.1 * canvas.Height, 0.1 * canvas.Width, 25);
                else
                    border.Margin = new Thickness(0.1 * canvas.Width, 0.1 * canvas.Height, 0.1 * canvas.Width, 0);
                roomCardsStackPanel.Children.Add(border);

                StackPanel cardStackPanel = new StackPanel
                {
                    Width = cardWidth
                };
                border.Child = cardStackPanel;

                Grid dataGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0.5 * cardWidth) },
                        new ColumnDefinition { Width = new GridLength(0.5 * cardWidth) }
                    },
                    Margin = new Thickness(0, 20, 0, 20)
                };
                cardStackPanel.Children.Add(dataGrid);

                Image roomImage = new Image
                {
                    Source = room.image.GetImage().Source
                };
                roomImage.Width = 0.4 * cardWidth;
                Grid.SetColumn(roomImage, 0);
                dataGrid.Children.Add(roomImage);

                StackPanel dataStackPanel = new StackPanel
                {
                    Width = 0.5 * cardWidth
                };
                Grid.SetColumn(dataStackPanel, 1);
                dataGrid.Children.Add(dataStackPanel);

                Label hotelLabel = new Label
                {
                    Content = "Hotel : " + room.hotel.name,
                    FontSize = 22
                };
                dataStackPanel.Children.Add(hotelLabel);

                Label roomNumberLabel = new Label
                {
                    Content = "Room Number : " + room.number,
                    FontSize = 22
                };
                dataStackPanel.Children.Add(roomNumberLabel);

                Label roomTypeLabel = new Label
                {
                    Content = "Room Type : " + room.type.name,
                    FontSize = 22
                };
                dataStackPanel.Children.Add(roomTypeLabel);

                StackPanel viewsStackPanel = new StackPanel
                {
                    Width = cardWidth
                };
                cardStackPanel.Children.Add(viewsStackPanel);

                Grid buttonsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };
                cardStackPanel.Children.Add(buttonsGrid);

                Button changePhotoButton = FrontEndHelper.CreateButton(160, 40, "Change Photo");
                changePhotoButton.Click += ChangePhotoButton_Click;
                changePhotoButton.Tag = room;
                Grid.SetColumn(changePhotoButton, 0);
                changePhotoButton.Margin = new Thickness(0.55 * cardWidth, 0, 0, 0);
                buttonsGrid.Children.Add(changePhotoButton);

                Button deleteButton = FrontEndHelper.CreateButton(80, 40, "Delete");
                deleteButton.Margin = new Thickness(10, 0, 0, 0);
                deleteButton.Tag = room;
                deleteButton.Click += DeleteButton_Click;
                Grid.SetColumn(deleteButton, 1);
                buttonsGrid.Children.Add(deleteButton);

                Button addViewbutton = FrontEndHelper.CreateButton(90, 40, "Add view");
                addViewbutton.Margin = new Thickness(10, 0, 0, 0);
                addViewbutton.Click += AddViewbutton_Click;
                addViewbutton.Tag = new List<object>();
                ((List<object>)addViewbutton.Tag).Add(room);
                Grid.SetColumn(addViewbutton, 2);
                buttonsGrid.Children.Add(addViewbutton);
                
                Label title = new Label
                {
                    Content = "Room Views",
                    FontSize = 22,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                cardStackPanel.Children.Add(title);

                ListBox viewsList = new ListBox
                {
                    Width = cardWidth
                };
                ((List<object>)addViewbutton.Tag).Add(viewsList);
                cardStackPanel.Children.Add(viewsList);

                foreach (RoomView view in room.views)
                {
                    ListBoxItem viewListBoxItem = new ListBoxItem
                    {
                        Content = "View : " + view.view,
                        FontSize = 22
                    };
                    viewsList.Items.Add(viewListBoxItem);
                }
            }
        }

        private void Room_Add_Button_Click(object sender, RoutedEventArgs args)
        {
            FrontEndHelper.CreateAddRoomPopupWindow();
        }

        private void AddViewbutton_Click(object sender, RoutedEventArgs e)
        {
            Button addViewButton = (Button)sender;
            List<object> objects = (List<object>)sender;
            Room room = (Room)objects[0];
            ListBox viewsList = (ListBox)objects[1];

            //FrontEndHelper.CreateAddRoomViewPopupWindow(room, viewsList);
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|PNG Files (*.png)|*.png",
                Title = "Select Room Photo"
            };
            dlg.ShowDialog();
            if (dlg.FileName.ToString() == "")
                return;
            Room room = (Room)((Button)sender).Tag;
            room.image = new CustomImage(dlg.FileName.ToString());
            //DataModels.GetInstance().UpdateRoom(room);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            DataModels database = DataModels.GetInstance();

            // remove rroom
            //database.deleteRoom();

            Admin_window adminWindow = FrontEndHelper.GetAdminWindow();
            if (adminWindow.currentCanvas != null)
                adminWindow.currentCanvas.Hide();
            adminWindow.InitializeRoomsCanvas(database.GetAllRooms());
        }
    }
}
