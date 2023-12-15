using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Animations;

using LightPhotos.Contracts.Services;
using LightPhotos.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;
using static CommunityToolkit.WinUI.UI.Animations.Expressions.ExpressionValues;

namespace LightPhotos.Views;

public sealed partial class ContentGridDetailPage : Page
{
    private bool isScale = false;
    private double positionPressedX;
    private double positionPressedY;
    private bool pointerPressed = false;

    public ContentGridDetailViewModel ViewModel
    {
        get;
    }

    public ContentGridDetailPage()
    {
        ViewModel = App.GetService<ContentGridDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("animationKeyContentGrid", ContentArea);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }

    private void Image_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (PictureImage.RenderTransform is not MatrixTransform matrixTransform)
        {
            return;
        }
        if (isScale)
        {
            matrixTransform.Matrix = Matrix.Identity;
            isScale = false;
        }
        else
        {
            var position = e.GetPosition(PictureImage);
            ScaleImage(2, 2, position.X, position.Y);
            isScale = true;
        }
    }

    private void PictureImage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        PictureImage.RenderTransform = new MatrixTransform();
    }

    private void SetPositionPressed(double x, double y)
    {
        positionPressedX = x;
        positionPressedY = y;
        pointerPressed = true;
    }

    private void MovePosition(double x, double y)
    {
        if (PictureImage.RenderTransform is not MatrixTransform matrixTransform)
        {
            return;
        }
        if (!pointerPressed)
        {
            return;
        }
        var matrix = matrixTransform.Matrix;
        matrix.OffsetX += x;
        matrix.OffsetY += y;
        matrixTransform.Matrix = matrix;
    }

    private void ScaleImage(double scaleX, double scaleY, double centerX, double centerY)
    {
        if (PictureImage.RenderTransform is not MatrixTransform matrixTransform)
        {
            return;
        }
        var matrix = matrixTransform.Matrix;
        var scaleMatrix = matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
        matrixTransform.Matrix = scaleMatrix;
    }

    private void SetPositionReleased()
    {
        positionPressedX = 0;
        positionPressedY = 0;
        pointerPressed = false;
    }

    #region Manipulation

    private void Image_ManipulationStarting(object sender, Microsoft.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
    {
        if (e.Pivot is null)
        {
            return;
        }
        SetPositionPressed(e.Pivot.Center.X, e.Pivot.Center.Y);
    }

    private void Image_ManipulationDelta(object sender, Microsoft.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
    {
        MovePosition(e.Position.X - positionPressedX, e.Position.Y - positionPressedY);
        ScaleImage(e.Delta.Scale, e.Delta.Scale, e.Position.X, e.Position.Y);
    }

    private void Image_ManipulationCompleted(object sender, Microsoft.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
    {
        SetPositionReleased();
    }

    #endregion

    #region Pointer

    private void PictureImage_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        var point = e.GetCurrentPoint(PictureImage).Position;
        SetPositionPressed(point.X, point.Y);
    }

    private void PictureImage_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        var point = e.GetCurrentPoint(PictureImage).Position;
        MovePosition(point.X - positionPressedX, point.Y - positionPressedY);
    }

    private void PictureImage_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        SetPositionReleased();
    }

    private void PictureImage_PointerCanceled(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        SetPositionReleased();
    }

    private void PictureImage_PointerCaptureLost(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        SetPositionReleased();
    }

    private void PictureImage_PointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        // Prevent most handlers along the event route from handling the same event again.
        e.Handled = true;
        var ptrPt = e.GetCurrentPoint(PictureImage);
        if (ptrPt.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Mouse)
        {
            return;
        }
        if (ptrPt.Properties.IsHorizontalMouseWheel)
        {
            return;
        }
        var point = e.GetCurrentPoint(PictureImage).Position;
        if (ptrPt.Properties.MouseWheelDelta > 0)
        {
            ScaleImage(1.05, 1.05, point.X, point.Y);
        }
        else if (ptrPt.Properties.MouseWheelDelta < 0)
        {
            ScaleImage(0.95, 0.95, point.X, point.Y);
        }
    }

    #endregion
}
