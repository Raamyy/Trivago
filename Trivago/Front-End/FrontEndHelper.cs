using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Trivago.Front_End
{
    static class FrontEndHelper
    {
        public static Button CreateButton(double width, double height, string content)
        {
            Button obj = new Button
            {
                Width = width,
                Height = height,
                Content = content,
                Background = new SolidColorBrush(Color.FromRgb(232, 126, 49)),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                FontSize = 20,
                Cursor = Cursors.Hand
            };
            return obj;
        }

        public static Trivago.MainWindow GetMainWindow()
        {
            return (Trivago.MainWindow)App.Current.MainWindow;
        }
    }
}
