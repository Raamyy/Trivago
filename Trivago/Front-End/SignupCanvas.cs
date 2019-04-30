using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Trivago.Front_End
{
    class SignupCanvas : CustomCanvas
    {
        private static SignupCanvas signupCanvas;

        private string userName;

        public string GetUserName()
        {
            return userName;
        }

        private string name;

        public string GetName()
        {
            return name;
        }

        private string email;

        public string GetEmail()
        {
            return email;
        }

        private string password;

        public string GetPassword()
        {
            return password;
        }

        private string creditCardSerial;

        public string GetCreditCardSerial()
        {
            return creditCardSerial;
        }

        private string creditCardCVV;

        public string GetCreditCardCVV()
        {
            return creditCardCVV;
        }

        private DateTime expirationDate;

        public DateTime GetExpirationDate()
        {
            return expirationDate;
        }

        private SignupCanvas(Canvas canvas) : base(canvas)
        {
        }

        public static SignupCanvas GetInstance(Canvas canvas)
        {
            if (signupCanvas == null)
                signupCanvas = new SignupCanvas(canvas);
            return signupCanvas;
        }

        public override void Initialize()
        {
            userName = "";
            name = "";
            email = "";
            password = "";
            creditCardSerial = "";
            creditCardCVV = "";
            expirationDate = new DateTime();

            double textBoxWidth = 300;
            double labelWdith = 200;

            StackPanel signupDataStackPanel = new StackPanel
            {
                Margin = new Thickness(0.3 * canvas.Width, 0.05 * canvas.Height, 0, 0)
            };
            canvas.Children.Add(signupDataStackPanel);

            //set user name
            Grid userNameGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } }
            };

            Label userNameLabel = new Label { Content = "User Name : ", FontSize = 22 };
            Grid.SetColumn(userNameLabel, 0);
            userNameGrid.Children.Add(userNameLabel);

            TextBox userNameTextBox = new TextBox { FontSize = 22, Width = textBoxWidth };
            userNameTextBox.TextChanged += UserNameTextBox_TextChanged;
            Grid.SetColumn(userNameTextBox, 1);
            userNameGrid.Children.Add(userNameTextBox);

            signupDataStackPanel.Children.Add(userNameGrid);

            //set name
            Grid nameGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label nameLabel = new Label { Content = "Name : ", FontSize = 22 };
            Grid.SetColumn(nameLabel, 0);
            nameGrid.Children.Add(nameLabel);

            TextBox nameTextBox = new TextBox { FontSize = 22, Width = textBoxWidth };
            nameTextBox.TextChanged += NameTextBox_TextChanged;
            Grid.SetColumn(nameTextBox, 1);
            nameGrid.Children.Add(nameTextBox);

            signupDataStackPanel.Children.Add(nameGrid);

            //set email
            Grid emailGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label emailLabel = new Label { Content = "Email : ", FontSize = 22 };
            Grid.SetColumn(emailLabel, 0);
            emailGrid.Children.Add(emailLabel);

            TextBox emailTextBox = new TextBox { FontSize = 22, Width = textBoxWidth };
            emailTextBox.TextChanged += EmailTextBox_TextChanged;
            Grid.SetColumn(emailTextBox, 1);
            emailGrid.Children.Add(emailTextBox);

            signupDataStackPanel.Children.Add(emailGrid);

            //set password
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
            passwordTextBox.PasswordChanged += PasswordTextBox_TextChanged;
            Grid.SetColumn(passwordTextBox, 1);
            passwordGrid.Children.Add(passwordTextBox);

            signupDataStackPanel.Children.Add(passwordGrid);

            //set credit card serial number
            Grid creditCardSerialGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label creditCardSerialLabel = new Label { Content = "Credit Card Serial : ", FontSize = 22 };
            Grid.SetColumn(creditCardSerialLabel, 0);
            creditCardSerialGrid.Children.Add(creditCardSerialLabel);

            TextBox creditCardSerialTextBox = new TextBox { FontSize = 22, Width = textBoxWidth };
            creditCardSerialTextBox.TextChanged += CreditCardSerialTextBox_TextChanged;
            Grid.SetColumn(creditCardSerialTextBox, 1);
            creditCardSerialGrid.Children.Add(creditCardSerialTextBox);

            signupDataStackPanel.Children.Add(creditCardSerialGrid);

            //set credit card cvv
            Grid creditCardCVVGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label creditCardCVVLabel = new Label { Content = "Credit Card CVV : ", FontSize = 22 };
            Grid.SetColumn(creditCardCVVLabel, 0);
            creditCardCVVGrid.Children.Add(creditCardCVVLabel);

            TextBox creditCardCVVTextBox = new TextBox { FontSize = 22, Width = textBoxWidth };
            creditCardCVVTextBox.TextChanged += CreditCardCVVTextBox_TextChanged;
            Grid.SetColumn(creditCardCVVTextBox, 1);
            creditCardCVVGrid.Children.Add(creditCardCVVTextBox);

            signupDataStackPanel.Children.Add(creditCardCVVGrid);

            //set credit card expiration date
            Grid creditCardExpirationDateGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition { Width = new GridLength(labelWdith) }, new ColumnDefinition { Width = GridLength.Auto } },
                Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0)
            };

            Label creditCardExpirationDateLabel = new Label { Content = "Expiration Date : ", FontSize = 22 };
            Grid.SetColumn(creditCardExpirationDateLabel, 0);
            creditCardExpirationDateGrid.Children.Add(creditCardExpirationDateLabel);

            DatePicker creditCardExpirationDatePicker = new DatePicker
            {
                Width = textBoxWidth
            };
            creditCardExpirationDatePicker.SelectedDateChanged += CreditCardExpirationDatePicker_SelectedDateChanged;
            Grid.SetColumn(creditCardExpirationDatePicker, 1);
            creditCardExpirationDateGrid.Children.Add(creditCardExpirationDatePicker);
            
            signupDataStackPanel.Children.Add(creditCardExpirationDateGrid);

            //set signup button
            Button signupButton = FrontEndHelper.CreateButton(0.1 * canvas.Width, 0.075 * canvas.Height, "Signup");
            signupButton.Margin = new Thickness(0, 0.05 * canvas.Height, 0, 0);
            signupButton.Click += FrontEndHelper.GetMainWindow().SignupButtonUserData_Click;
            signupDataStackPanel.Children.Add(signupButton);
        }

        private void CreditCardExpirationDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker creditCardExpirationDatePicker = (DatePicker)sender;
            expirationDate = creditCardExpirationDatePicker.SelectedDate.Value;
        }

        private void UserNameTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            TextBox userNameTextBox = (TextBox)sender;
            userName = userNameTextBox.Text;
        }

        private void NameTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            TextBox nameTextBox = (TextBox)sender;
            name = nameTextBox.Text;
        }

        private void EmailTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            TextBox emailTextBox = (TextBox)sender;
            email = emailTextBox.Text;
        }

        private void PasswordTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            PasswordBox passwordTextBox = (PasswordBox)sender;
            password = passwordTextBox.Password;
        }

        private void CreditCardSerialTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            TextBox creditCardSerialTextBox = (TextBox)sender;
            creditCardSerial = creditCardSerialTextBox.Text;
        }

        private void CreditCardCVVTextBox_TextChanged(object sender, RoutedEventArgs args)
        {
            TextBox creditCardCVVTextBox = (TextBox)sender;
            creditCardCVV = creditCardCVVTextBox.Text;
        }


    }
}
