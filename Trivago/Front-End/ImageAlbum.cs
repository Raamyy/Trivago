using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Trivago.Front_End
{
    class ImageAlbum
    {
        private List<CustomImage> customImages;
        private int pointer;
        private Image viewedImage;
        private Button backButton;
        private Button nextButton;
        private double buttonOpacity;
        public int Pointer { get => pointer; }

        public ImageAlbum(Canvas currentCanvas, double x, double y, double width, double height, List<CustomImage> customImages)
        {
            this.customImages = customImages;
            pointer = 0;
            buttonOpacity = 0.3;

            viewedImage = new Image
            {
                Width = width,
                Height = height,
            };
            UpdatePointerByValue(0);
            Canvas.SetLeft(viewedImage, x);
            Canvas.SetTop(viewedImage, y);
            currentCanvas.Children.Add(viewedImage);

            backButton = new Button
            {
                Width = 40,
                Height = 20,
                Opacity = buttonOpacity,
                Content = "Back",
            };
            backButton.Click += BackButton_Click;
            backButton.MouseEnter += Button_MouseEnter;
            backButton.MouseLeave += Button_MouseLeave;
            Canvas.SetLeft(backButton, x);
            Canvas.SetTop(backButton, y + height / 2);
            currentCanvas.Children.Add(backButton);

            nextButton = new Button
            {
                Width = 40,
                Height = 20,
                Opacity = buttonOpacity,
                Content = "Next"
            };
            nextButton.Click += NextButton_Click;
            nextButton.MouseEnter += Button_MouseEnter;
            nextButton.MouseLeave += Button_MouseLeave;
            Canvas.SetLeft(nextButton, x + width - nextButton.Width);
            Canvas.SetTop(nextButton, y + height / 2);
            currentCanvas.Children.Add(nextButton);
        }

        public void AddImage(CustomImage newImage)
        {
            customImages.Add(newImage);
        }

        public void RemoveImage(CustomImage deleteImage)
        {
            bool deleted = customImages.Remove(deleteImage);
            if (deleted)
                UpdatePointerByValue(-1);
        }

        private void UpdatePointerByValue(int val)
        {
            if (customImages.Count == 0)
                pointer = 0;
            else
                pointer = (Pointer + val + customImages.Count) % customImages.Count;
            SetViewImage();
        }

        private void SetViewImage()
        {
            if (Pointer < customImages.Count)
                viewedImage.Source = customImages[Pointer].GetImage().Source;
            else
                viewedImage.Source = CustomImage.NoImage.GetImage().Source;
        }

        private void Button_MouseEnter(object sender, RoutedEventArgs args)
        {
            Button button = (Button)sender;
            button.Opacity = 1;
        }

        private void Button_MouseLeave(object sender, RoutedEventArgs args)
        {
            Button button = (Button)sender;
            button.Opacity = buttonOpacity;
        }

        private void NextButton_Click(object sender, RoutedEventArgs args)
        {
            UpdatePointerByValue(1);
        }

        private void BackButton_Click(object sender, RoutedEventArgs args)
        {
            UpdatePointerByValue(-1);
        }
    }
}
