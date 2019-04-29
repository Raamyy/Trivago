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

        public static HomeCanvas GetInstance(Canvas canvas)
        {
            if (homeCanvas == null)
                homeCanvas = new HomeCanvas(canvas);
            return homeCanvas;
        }

        public override void Initialize()
        {
            selectedLocations = new List<Location>();
            selectedDateRange = null;
            selectedType = null;

            double itemsSpacing = 150;
            
            //set canvas background
            canvas.Background = new SolidColorBrush(Color.FromRgb(239, 239, 239));

            //creates room type combobox
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
            Canvas.SetLeft(roomTypesComboBox, 3 * itemsSpacing + 500);
            Canvas.SetTop(roomTypesComboBox, canvas.Height / 4);
            canvas.Children.Add(roomTypesComboBox);

            //creates booking range calendar
            Calendar bookingCalendar = new Calendar
            {
                SelectionMode = CalendarSelectionMode.SingleRange
            };
            selectedDateRange = new CalendarDateRange(DateTime.Today, DateTime.Today);
            bookingCalendar.SelectedDatesChanged += Calander_SelectedDatesChanged;
            Canvas.SetLeft(bookingCalendar, 2 * itemsSpacing + roomTypesComboBox.Width+40);
            Canvas.SetTop(bookingCalendar, canvas.Height / 4);
            canvas.Children.Add(bookingCalendar);

            //creates locations check boxes
            Expander locationsExpander = new Expander
            {
                Width = 160,
                Height = 200,
                Header = "Locations"
            };
            ScrollViewer locationsScrollViewer = new ScrollViewer();
            StackPanel locationsStackPanel = new StackPanel();
            locationsExpander.Content = locationsScrollViewer;
            locationsScrollViewer.Content = locationsStackPanel;
            List<Location> locations = DataModels.GetInstance().GetAllLocations();
            foreach(Location location in locations)
            {
                CheckBox locationCheckBox = new CheckBox { Content = location, FontSize = 20 };
                locationCheckBox.Checked += CheckBox_Checked;
                locationCheckBox.Unchecked += CheckBox_Unchecked;
                locationsStackPanel.Children.Add(locationCheckBox);
            }
            Canvas.SetLeft(locationsExpander, itemsSpacing);
            Canvas.SetTop(locationsExpander, canvas.Height / 4);
            canvas.Children.Add(locationsExpander);

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
