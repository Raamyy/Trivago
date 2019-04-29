using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trivago.Models;

namespace Trivago.Front_End
{
    class BookingsListShow : CustomCanvas
    {
        private static BookingsListShow bookingListShow;
        private List<Booking> bookings;

        public static BookingsListShow GetInstance(Canvas canvas, List<Booking> bookings)
        {
            if (bookingListShow == null)
                bookingListShow = new BookingsListShow(canvas);
            bookingListShow.bookings = bookings;
            return bookingListShow;
        }

        public BookingsListShow(Canvas canvas) : base(canvas)
        {
        }

        public override void Initialize()
        {
            ScrollViewer bookingsScrollViewer = new ScrollViewer
            {
                Height = canvas.Height
            };
            canvas.Children.Add(bookingsScrollViewer);

            StackPanel bookingCardStackPanel = new StackPanel();
            bookingsScrollViewer.Content = bookingCardStackPanel;
        }
    }
}
