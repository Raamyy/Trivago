using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Trivago.Front_End
{
    public abstract class CustomCanvas
    {
        protected Canvas canvas;
        public bool IsInitialized;

        public CustomCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            this.canvas.Visibility = Visibility.Hidden;
            IsInitialized = false;
        }

        protected void SetCanvasWidth(double width)
        {
            canvas.Width = width;
        }

        protected void SetCanvasHeight(double height)
        {
            canvas.Height = height;
        }

        public double GetCanvasWidth()
        {
            return canvas.Width;
        }

        public double GetCanvasHeight()
        {
            return canvas.Height;
        }

        public abstract void Initialize();

        public virtual void Hide()
        {
            canvas.Children.Clear();
            FrontEndHelper.GetMainWindow().CurrentCanvas = null;
            canvas.Visibility = Visibility.Hidden;
            IsInitialized = false;
        }

        public virtual void Show()
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }
            FrontEndHelper.GetMainWindow().CurrentCanvas = this;
            canvas.Visibility = Visibility.Visible;
        }
    }
}
