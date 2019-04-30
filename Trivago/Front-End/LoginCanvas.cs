using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Trivago.Front_End
{
    class LoginCanvas : CustomCanvas
    {
        private static LoginCanvas loginCanvas;
        private string userName;

        public string GetUserName()
        {
            return userName;
        }

        private string password;

        public string GetPassword()
        {
            return password;
        }

        private LoginCanvas(Canvas canvas) : base(canvas)
        {
        }


        public static LoginCanvas GetInstance(Canvas canvas)
        {
            if (loginCanvas == null)
                loginCanvas = new LoginCanvas(canvas);
            return loginCanvas;
        }

        public override void Initialize()
        {
            double textBoxWidth = 300;
            double labelWdith = 135;

            StackPanel loginDataStackPanel = new StackPanel
            {
                Margin = new Thickness(0.3 * canvas.Width, 0.3 * canvas.Height, 0, 0)
            };
            canvas.Children.Add(loginDataStackPanel);

            //set user name
            Grid userNameGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } }
            };

            Label userNameLabel = new Label { Content = "User Name : ", FontSize = 22 };
            Grid.SetColumn(userNameLabel, 0);
            userNameGrid.Children.Add(userNameLabel);

            TextBox userNameTextBox = new TextBox { FontSize = 22, Width = textBoxWidth};
            userNameTextBox.TextChanged += userNameTextBoxChanged;
            Grid.SetColumn(userNameTextBox, 1);
            userNameGrid.Children.Add(userNameTextBox);

            loginDataStackPanel.Children.Add(userNameGrid);

            //sets password
            Grid passwordGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label passwordLabel = new Label
            {
                Content = "Password : ",
                FontSize = 22
            };
            Grid.SetColumn(passwordLabel, 0);
            passwordGrid.Children.Add(passwordLabel);
            
            PasswordBox passwordTextBox = new PasswordBox
            {
                FontSize = 22,
                Width = textBoxWidth
            };
            passwordTextBox.PasswordChanged += passwordTextBoxChanged;
            Grid.SetColumn(passwordTextBox, 1);
            passwordGrid.Children.Add(passwordTextBox);

            loginDataStackPanel.Children.Add(passwordGrid);

            //creates login button
            Button loginButton = FrontEndHelper.CreateButton(0.1 * canvas.Width, 0.075 * canvas.Height, "Login");
            loginButton.Click += FrontEndHelper.GetMainWindow().LoginButtonUserData_Click;
            loginButton.Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0);
            loginDataStackPanel.Children.Add(loginButton);
            
        }

        private void userNameTextBoxChanged(object sender, RoutedEventArgs args)
        {
            TextBox userNameTextBox = (TextBox)sender;
            userName = userNameTextBox.Text;
        }

        private void passwordTextBoxChanged(object sender, RoutedEventArgs args)
        {
            PasswordBox passwordTextBox = (PasswordBox)sender;
            password = passwordTextBox.Password;
        }
    }
}
