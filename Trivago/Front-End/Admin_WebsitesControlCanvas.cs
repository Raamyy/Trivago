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
        private DataModels model;

        private Admin_WebsitesControlCanvas(Canvas canvas):base(canvas)
        {

        }
        public Admin_WebsitesControlCanvas GetInstance(Canvas canvas, List<Website> websites)
        {
            if (admin_WebsitesControlCanvas == null)
                admin_WebsitesControlCanvas = new Admin_WebsitesControlCanvas(canvas);
            admin_WebsitesControlCanvas.websites = websites;
            model = DataModels.GetInstance();
            return admin_WebsitesControlCanvas;
        }
        public override void Initialize()
        {
            websites = DataModels.GetInstance().GetAllWebsites();
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

                Grid websiteGrid = new Grid
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

            }
        }
    }
}
