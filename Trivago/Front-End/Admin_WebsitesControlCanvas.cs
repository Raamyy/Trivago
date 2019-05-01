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
    class Admin_WebsitesControlCanvas : CustomCanvas
    {
        static Admin_WebsitesControlCanvas admin_WebsitesControlCanvas;
        private List<Website> websites;
        private  DataModels model;

        private Admin_WebsitesControlCanvas(Canvas canvas):base(canvas)
        {

        }
        public static Admin_WebsitesControlCanvas GetInstance(Canvas canvas, List<Website> websites)
        {
            if (admin_WebsitesControlCanvas == null)
                admin_WebsitesControlCanvas = new Admin_WebsitesControlCanvas(canvas);
            admin_WebsitesControlCanvas.websites = websites;
            admin_WebsitesControlCanvas.model = DataModels.GetInstance();
            return admin_WebsitesControlCanvas;
        }
        public override void Initialize()
        {
            
            int boxSpacing = 50;
            double cardWidth = canvas.Width - 0.2 * canvas.Width;
            double cardHeight = 0.7 * canvas.Height;

            canvas.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));

            ScrollViewer websitesScrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(websitesScrollViewer);

            StackPanel websitesCardStackPanel = new StackPanel();
            websitesScrollViewer.Content = websitesCardStackPanel;

            foreach(Website website in websites)
            {
                Border cardBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(3),
                    Width = cardWidth,
                    Margin = new Thickness(0.1 * canvas.Width, boxSpacing, 0.1 * canvas.Width, 0)
                };
                websitesCardStackPanel.Children.Add(cardBorder);
                StackPanel websiteData_SP = new StackPanel();
                Label websiteNameLabel = new Label
                {
                    Content = "Website Link: " + website.name,
                    FontSize = 22,
                    Margin = new Thickness(0, 0.2 * cardHeight, 0, 0)
                };
                websiteData_SP.Children.Add(websiteNameLabel);
                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width =GridLength.Auto},
                        new ColumnDefinition { Width =GridLength.Auto},
                        new ColumnDefinition { Width =GridLength.Auto}
                    }
                };

                Label RatingLabel = new Label
                {
                    Content = "Website Rating: " + website.name,
                    FontSize = 22,
                    Margin = new Thickness(0, 0.2 * cardHeight, 0, 0)
                };
                for (int i = 0; i < website.rating; i++)
                    RatingLabel.Content += "★";

                Grid.SetColumn(RatingLabel, 0);
                grid.Children.Add(RatingLabel);

                Button updateButton = FrontEndHelper.CreateButton(cardWidth * 0.1, cardHeight * 0.1, "Update");
                updateButton.Tag = website;
                updateButton.Click += UpdateButton_Click;
                Grid.SetColumn(updateButton, 1);
                grid.Children.Add(updateButton);

                Button deleteButton = FrontEndHelper.CreateButton(cardWidth * 0.1, cardHeight * 0.1, "Update");
                deleteButton.Tag = website;
                deleteButton.Click += DeleteButton_Click;
                Grid.SetColumn(deleteButton, 2);
                grid.Children.Add(deleteButton);

                websiteData_SP.Children.Add(grid);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Website website = (Website)button.Tag;
            MessageBox.Show("Delete " + website.name);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Website website = (Website)button.Tag;
            MessageBox.Show("update " + website.name);
        }
    }
}
