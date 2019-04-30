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
    class LoggedinNavigationCanvas : CustomCanvas
    {
        private static LoggedinNavigationCanvas loggedinNavigationCanvas;
        private User user;

        public User GetActiveUser()
        {
            return user;
        }

        public static LoggedinNavigationCanvas GetInstance(Canvas canvas, User user)
        {
            if (loggedinNavigationCanvas == null)
                loggedinNavigationCanvas = new LoggedinNavigationCanvas(canvas);
            loggedinNavigationCanvas.user = user;
            return loggedinNavigationCanvas;
        }

        private LoggedinNavigationCanvas(Canvas canvas) : base(canvas)
        {
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
            canvas.Visibility = Visibility.Hidden;
            IsInitialized = false;
        }

        public override void Initialize()
        {
            canvas.Background = new SolidColorBrush(Color.FromRgb(9, 48, 65));
            
            //load image
            Image logoImage = new Image
            {
                MaxWidth = 200,
                MaxHeight = 100,
                Source = new CustomImage("resources/images/logo.png").GetImage().Source,
                Cursor = Cursors.Hand
            };
            logoImage.Loaded += LogoImage_Loaded;
            logoImage.MouseLeftButtonDown += FrontEndHelper.GetMainWindow().LogoImage_MouseLeftButtonDown;
            canvas.Children.Add(logoImage);

            //load hello label
            Label helloLabel = new Label
            {
                Content = "Hello " + user.username,
                FontSize = 22,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                Cursor = Cursors.Hand,
            };
            helloLabel.MouseLeftButtonDown += FrontEndHelper.GetMainWindow().HelloLabel_MouseLeftButtonDown;
            Canvas.SetRight(helloLabel, 0.05 * canvas.Width);
            Canvas.SetTop(helloLabel, 0.3 * canvas.Height);
            canvas.Children.Add(helloLabel);

        }

        private void LogoImage_Loaded(object sender, RoutedEventArgs args)
        {
            Image image = (Image)sender;
            Canvas.SetLeft(image, 25);
            Canvas.SetTop(image, (canvas.Height - image.ActualHeight) / 2);
        }
    }
}
