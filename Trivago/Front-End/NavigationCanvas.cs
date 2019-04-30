using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trivago.Models;
using Trivago.Front_End;

namespace Trivago.Front_End
{
    class NavigationCanvas : CustomCanvas
    {
        static NavigationCanvas navigationCanvas;

        private NavigationCanvas(Canvas canvas) : base(canvas)
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
            canvas.Visibility = Visibility.Hidden;
            IsInitialized = false;
        }

        public static NavigationCanvas GetInstance(Canvas canvas)
        {
            if (navigationCanvas == null)
                navigationCanvas = new NavigationCanvas(canvas);
            return navigationCanvas;
        }

        public override void Initialize()
        {
            canvas.Background = new SolidColorBrush(Color.FromRgb(9, 48, 65));

            double buttonWidth = 120;
            double buttonHeight = 50;
            double buttonSpace = 25;
            double logoWidth = 200;
            double logoHeight = 100;

            Image logoImage = new Image
            {
                MaxWidth = logoWidth,
                MaxHeight = logoHeight,
                Source = new CustomImage("resources/images/logo.png").GetImage().Source,
                Cursor = Cursors.Hand
            };
            logoImage.Loaded += LogoImage_Loaded;
            logoImage.MouseLeftButtonDown += FrontEndHelper.GetMainWindow().LogoImage_MouseLeftButtonDown;
            canvas.Children.Add(logoImage);

            Button signupButton = FrontEndHelper.CreateButton(buttonWidth, buttonHeight, "Sign up");
            Canvas.SetTop(signupButton, buttonSpace);
            Canvas.SetRight(signupButton, 10 + buttonSpace);
            signupButton.Click += FrontEndHelper.GetMainWindow().SignupButton_Click;
            canvas.Children.Add(signupButton);

            Button loginButton = FrontEndHelper.CreateButton(buttonWidth, buttonHeight, "Login");
            Canvas.SetTop(loginButton, buttonSpace);
            Canvas.SetRight(loginButton,buttonWidth + 10 + 2 * buttonSpace);
            loginButton.Click += FrontEndHelper.GetMainWindow().LoginButton_Click;
            canvas.Children.Add(loginButton);
        }

        private void LogoImage_Loaded(object sender, RoutedEventArgs args)
        {
            Image image = (Image)sender;
            Canvas.SetLeft(image, 25);
            Canvas.SetTop(image, (canvas.Height - image.ActualHeight) / 2);
        }

        
    }
}
