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
    class AdminHotelCanvas : CustomCanvas
    {
        private static AdminHotelCanvas adminHotelCanvas;
        private List<Hotel> hotels;

        public AdminHotelCanvas(Canvas canvas) : base(canvas)
        {

        }

        public override void Show()
        {
            if(!IsInitialized)
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

        public static AdminHotelCanvas GetInstance(Canvas canvas, List<Hotel> hotels)
        {
            if (adminHotelCanvas == null)
                adminHotelCanvas = new AdminHotelCanvas(canvas);
            adminHotelCanvas.hotels = hotels;
            return adminHotelCanvas;
        }

        public override void Initialize()
        {
            double cardWidth = 0.8 * canvas.Width;
            ScrollViewer scrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(scrollViewer);

            StackPanel hotelCardsStackPanel = new StackPanel
            {
                Width = canvas.Width
            };
            scrollViewer.Content = hotelCardsStackPanel;

            for(int i = 0; i < hotels.Count; i++)
            {
                Hotel hotel = hotels[i];

                Border border = new Border
                {
                    Width = cardWidth,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3)
                };
                if (i == hotels.Count - 1)
                    border.Margin = new Thickness(0.1 * canvas.Width, 0.1 * canvas.Height, 0.1 * canvas.Width, 25);
                else
                    border.Margin = new Thickness(0.1 * canvas.Width, 0.1 * canvas.Height, 0.1 * canvas.Width, 0);

                hotelCardsStackPanel.Children.Add(border);

                StackPanel cardStackPanel = new StackPanel();
                border.Child = cardStackPanel;

                Grid hotelDataGrid = new Grid
                {
                    Width = cardWidth,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto}
                    },
                    Margin = new Thickness(0, 20, 0, 20)
                };
                cardStackPanel.Children.Add(hotelDataGrid);

                Image hotelImaege = new Image
                {
                    Source = hotel.image.GetImage().Source,
                    MaxWidth = 0.4 * cardWidth,
                    Margin = new Thickness(0.05 * cardWidth, 0, 0.05 * cardWidth, 0)
                };
                Grid.SetColumn(hotelImaege, 0);
                hotelDataGrid.Children.Add(hotelImaege);

                StackPanel hotelDataStackPanel = new StackPanel
                {
                    Width = 0.5 * cardWidth
                };
                Grid.SetColumn(hotelDataStackPanel, 1);
                hotelDataGrid.Children.Add(hotelDataStackPanel);

                Label hotelName = new Label
                {
                    Content = "Name : " + hotel.name,
                    FontSize = 22
                };
                hotelDataStackPanel.Children.Add(hotelName);

                Label hotelLicenseNumber = new Label
                {
                    Content = "License Number : " + hotel.licenseNumber,
                    FontSize = 22
                };
                hotelDataStackPanel.Children.Add(hotelLicenseNumber);

                Label locationLabal = new Label
                {
                    Content = "Location : " + hotel.location.city + " , " + hotel.location.country,
                    FontSize = 22
                };
                hotelDataStackPanel.Children.Add(locationLabal);

                Grid buttonsGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Margin = new Thickness(0.75 * cardWidth, 0, 0, 0)
                };
                cardStackPanel.Children.Add(buttonsGrid);

                Button deleteButton = FrontEndHelper.CreateButton(80, 40, "Delete");
                Grid.SetColumn(deleteButton, 0);
                buttonsGrid.Children.Add(deleteButton);

                Button updateButton = FrontEndHelper.CreateButton(80, 40, "Update");
                updateButton.Margin = new Thickness(0.05 * cardWidth, 0, 0, 0);
                Grid.SetColumn(updateButton, 1);
                buttonsGrid.Children.Add(updateButton);

                //add expander
                Expander moreDataExpander = new Expander
                {
                    Width = cardWidth,
                    Header = "More Data"
                };
                cardStackPanel.Children.Add(moreDataExpander);

                TabControl moreDataTabs = new TabControl();
                moreDataExpander.Content = moreDataTabs;
                moreDataTabs.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                
                //add meals tab
                TabItem MealsTab = new TabItem { Header = "Meals" };
                StackPanel MealsPanel = new StackPanel();
                MealsTab.Content = MealsPanel;
                moreDataTabs.Items.Add(MealsTab);


                for (int j = 0; j < hotel.mealPlans.Count; j++)
                {
                    MealPlan mealPlan = hotel.mealPlans[j];
                    Label mealPlanLabel = new Label
                    {
                        Content = mealPlan,
                        FontSize = 22,
                    };
                    MealsPanel.Children.Add(mealPlanLabel);
                }

                //add hotel facilities
                TabItem hotelFacilitiesPhotosTab = new TabItem { Header = "Photos" };
                Canvas hotelPhotosCanvas = new Canvas
                {
                    Width = cardWidth,
                    Height = 300
                };
                hotelFacilitiesPhotosTab.Content = hotelPhotosCanvas;
                moreDataTabs.Items.Add(hotelFacilitiesPhotosTab);

                List<CustomImage> images = new List<CustomImage>();
                images.Add(hotel.image);
                foreach (HotelFacility facility in hotel.facilities)
                    images.Add(facility.image);
                ImageAlbum hotelAlbum = new ImageAlbum(hotelPhotosCanvas, 25, 25, 250, 250, images);
            }
        }
    }
}
