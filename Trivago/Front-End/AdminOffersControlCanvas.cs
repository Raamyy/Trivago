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
    class AdminOffersControlCanvas : CustomCanvas
    {
        static AdminOffersControlCanvas adminOffersControlCanvas;
        List<Offer> offers;
        private DataModels model;

        private AdminOffersControlCanvas(Canvas canvas) : base(canvas)
        {

        }
        public override void Hide()
        {
            canvas.Children.Clear();
            FrontEndHelper.GetAdminWindow().currentCanvas = null;
            canvas.Visibility = Visibility.Hidden;
            IsInitialized = false;
        }
        public override void Show()
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }
            FrontEndHelper.GetAdminWindow().currentCanvas = this;
            canvas.Visibility = Visibility.Visible;
        }

        public static AdminOffersControlCanvas GetInstance(Canvas canvas, List<Offer>offers)
        {
            if (adminOffersControlCanvas == null)
                adminOffersControlCanvas = new AdminOffersControlCanvas(canvas);
            adminOffersControlCanvas.offers = offers;
            adminOffersControlCanvas.model = DataModels.GetInstance();
            return adminOffersControlCanvas;
        }

        public override void Initialize()
        {
            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;
            double cardHeight = 0.7 * canvas.Height;

            canvas.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(scrollViewer);

            StackPanel offersCardStackPanel = new StackPanel();
            scrollViewer.Content = offersCardStackPanel;

            Button addOfferButton = FrontEndHelper.CreateButton(150, 50, "Add Offer");
            addOfferButton.Click += AddOfferButton_Click;
            addOfferButton.Margin = new Thickness(canvas.Width - addOfferButton.Width - 100, 6, 0, 0);
            offersCardStackPanel.Children.Add(addOfferButton);

            foreach(Offer offer in offers)
            {
                Border cardBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3),
                    Width = cardWidth,
                    Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, 0)
                };
                offersCardStackPanel.Children.Add(cardBorder);

                StackPanel offerDataStackPanel = new StackPanel();
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto},
                        new ColumnDefinition { Width =GridLength.Auto}
                    }
                };
                cardBorder.Child = grid;
                //offerDataStackPanel.Children.Add(grid);

                Image roomImage = offer.room.image.GetImage();
                roomImage.MaxWidth = 0.35 * cardWidth;
                roomImage.MaxHeight = 0.85 * cardHeight;
                Grid.SetColumn(roomImage, 0);
                Grid.SetRow(roomImage, 0);
                roomImage.Margin = new Thickness(10, 10, 60, 10);
                grid.Children.Add(roomImage);

                StackPanel offerData = new StackPanel();
                Grid.SetColumn(offerData, 1);
                grid.Children.Add(offerData);

                Label websiteNameLabel = new Label
                {
                    Content = "Website Link: "+offer.website.name,
                    FontSize = 22,
                };
                offerData.Children.Add(websiteNameLabel);

                Label hotelNameLabel = new Label
                {
                    Content = "Hotel Name: " + offer.room.hotel.name,
                    FontSize = 22,
                };
                offerData.Children.Add(hotelNameLabel);

                Label roomNumberLabel = new Label
                {
                    Content = "Room Number: " + offer.room.number.ToString(),
                    FontSize = 22,
                };
                offerData.Children.Add(roomNumberLabel);

                Label roomPriceLabel = new Label
                {
                    Content = "Price: " + offer.price,
                    FontSize = 22,
                };
                offerData.Children.Add(roomPriceLabel);

                Grid buttonsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(),
                        new ColumnDefinition()
                    }
                };

                Button updateOfferButton = FrontEndHelper.CreateButton(cardWidth * 0.09, cardHeight * 0.09, "Update");
                updateOfferButton.Tag = offer;
                updateOfferButton.Click += UpdateOfferButton_Click;
               updateOfferButton.Margin = new Thickness(5, 2.5, 2.5, 3);
                Grid.SetColumn(updateOfferButton, 0);
                buttonsGrid.Children.Add(updateOfferButton);

                Button deleteOfferButton = FrontEndHelper.CreateButton(cardWidth * 0.09, cardHeight * 0.09, "Delete");
                deleteOfferButton.Tag = offer;
                deleteOfferButton.Click += DeleteOfferButton_Click;
                deleteOfferButton.Margin = new Thickness(5, 2.5, 2.5, 3);
                Grid.SetColumn(deleteOfferButton, 1);
                buttonsGrid.Children.Add(deleteOfferButton);
                
                offerData.Children.Add(buttonsGrid);
            }

        }

        private void DeleteOfferButton_Click(object sender, RoutedEventArgs e)
        {
            DataModels model = DataModels.GetInstance();
            Button button = (Button)sender;
            Offer offer = (Offer)button.Tag;

            if (MessageBox.Show($"Are you sure to delete Offer ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // ToDo: un comment this when function done
                //model.DeleteOffer(offer);
            }
        }

        private void UpdateOfferButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Offer offer = (Offer)button.Tag;
            FrontEndHelper.CreateUpdateOfferPopupWindow(offer);
        }

        private void AddOfferButton_Click(object sender, RoutedEventArgs e)
        {
            FrontEndHelper.createAddOfferPopupWindow();
        }
    }
}
