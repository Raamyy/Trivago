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
    class HomeCanvas : CustomCanvas
    {
        static HomeCanvas homeCanvas;
        public List<Location> selectedLocations;
        public CalendarDateRange selectedDateRange;
        public RoomType selectedType;

        private HomeCanvas(Canvas canvas) : base(canvas)
        {
            selectedLocations = new List<Location>();
            selectedDateRange = new CalendarDateRange();
            selectedType = new RoomType();
        }

        public static HomeCanvas GetInstance(Canvas canvas, double width, double height)
        {
            if (homeCanvas == null)
                homeCanvas = new HomeCanvas(canvas);
            homeCanvas.SetCanvasWidth(width);
            homeCanvas.SetCanvasHeight(height);
            return homeCanvas;
        }

        public override void Initialize()
        {
            double itemsSpacing = 50;

            canvas.Margin = new Thickness(0, 100, 0, 0);
            canvas.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            ComboBox roomTypesComboBox = new ComboBox
            {
                Width = 200,
                Height = 30,
            };
            List<RoomType> roomTypes = DataModels.GetInstance().GetAllRoomTypes();
            roomTypesComboBox.ItemsSource = roomTypes;
            roomTypesComboBox.SelectionChanged += ComboBox_SelectionChanged;
            if (roomTypes.Count > 0)
                roomTypesComboBox.SelectedIndex = 0;
            Canvas.SetLeft(roomTypesComboBox, 3 * itemsSpacing + 600);
            Canvas.SetTop(roomTypesComboBox, canvas.Height / 3);
            canvas.Children.Add(roomTypesComboBox);

            Calendar bookingCalender = new Calendar
            {
                SelectionMode = CalendarSelectionMode.SingleRange
            };
            bookingCalender.SelectedDatesChanged += Calander_SelectedDatesChanged;
            Canvas.SetLeft(bookingCalender, 2 * itemsSpacing + roomTypesComboBox.Width);
            Canvas.SetTop(bookingCalender, canvas.Height / 3);
            canvas.Children.Add(bookingCalender);

            Expander expander = new Expander
            {
                Width = 160,
                Height = 200,
                Header = "Locations"
            };
            ScrollViewer scrollViewer = new ScrollViewer();
            StackPanel stackPanel = new StackPanel();
            expander.Content = scrollViewer;
            scrollViewer.Content = stackPanel;
            List<Location> locations = DataModels.GetInstance().GetAllLocations();
            foreach(Location location in locations)
            {
                CheckBox checkBox = new CheckBox { Content = location, FontSize = 20 };
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;
                stackPanel.Children.Add(checkBox);
            }
            Canvas.SetLeft(expander, itemsSpacing);
            Canvas.SetTop(expander, canvas.Height / 3);
            canvas.Children.Add(expander);

            Button searchButton = FrontEndHelper.CreateButton(150, 60, "Search");
            searchButton.Click += FrontEndHelper.GetMainWindow().SearchButton_Click;
            Canvas.SetLeft(searchButton, canvas.Width / 2 - 100);
            Canvas.SetBottom(searchButton, canvas.Height / 6);
            canvas.Children.Add(searchButton);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            selectedType = (RoomType)comboBox.SelectedItem;
        }

        private void Calander_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;
            DateTime startDate = calendar.SelectedDates[0];
            DateTime endDate = calendar.SelectedDates.Last();

            if(startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            selectedDateRange = new CalendarDateRange(startDate, endDate);
        }

        private void CheckBox_Checked(Object obj, RoutedEventArgs args)
        {
            CheckBox checkBox = (CheckBox)obj;
            Location location = (Location)checkBox.Content;
            selectedLocations.Add(location);
        }

        private void CheckBox_Unchecked(Object obj, RoutedEventArgs args)
        {
            CheckBox checkBox = (CheckBox)obj;
            Location location = (Location)checkBox.Content;
            selectedLocations.Remove(location);
        }
    }
}
