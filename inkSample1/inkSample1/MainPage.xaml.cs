using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace inkSample1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool timerStopped = false;
        DispatcherTimer timer = new DispatcherTimer();
        Point initPos=new Windows.Foundation.Point(0,0);
        public MainPage()
        {
            this.InitializeComponent();
            canvasX.InkPresenter.InputDeviceTypes =
        Windows.UI.Core.CoreInputDeviceTypes.Mouse |
        Windows.UI.Core.CoreInputDeviceTypes.Pen|
        Windows.UI.Core.CoreInputDeviceTypes.Touch;
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;
            canvasX.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            // By default, the InkPresenter processes input modified by
            // a secondary affordance (pen barrel button, right mouse
            // button, or similar) as ink.
            // To pass through modified input to the app for custom processing
            // on the app UI thread instead of the background ink thread, set
            // InputProcessingConfiguration.RightDragAction to LeaveUnprocessed.
            canvasX.InkPresenter.InputProcessingConfiguration.RightDragAction =
                InkInputRightDragAction.LeaveUnprocessed;

            // Listen for unprocessed pointer events from modified input.
            // The input is used to provide selection functionality.
            canvasX.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted1;
            canvasX.PointerMoved += CanvasX_PointerMoved1;
            canvasX.InkPresenter.UnprocessedInput.PointerPressed +=
                UnprocessedInput_PointerPressed;
            canvasX.InkPresenter.UnprocessedInput.PointerMoved +=
                UnprocessedInput_PointerMoved;
            canvasX.InkPresenter.UnprocessedInput.PointerReleased +=
                UnprocessedInput_PointerReleased;

            // Listen for new ink or erase strokes to clean up selection UI.
            canvasX.InkPresenter.StrokeInput.StrokeStarted +=
                StrokeInput_StrokeStarted;
            canvasX.InkPresenter.StrokesErased +=
                InkPresenter_StrokesErased;
        }

        private void CanvasX_PointerMoved1(object sender, PointerRoutedEventArgs e)
        {
            timer.Stop();
            timerStopped = false;
        }

        private void StrokeInput_StrokeStarted1(InkStrokeInput sender, PointerEventArgs args)
        {

            if (timerStopped == true)
            {

            }
            else
            {
                timer.Interval = new System.TimeSpan(0,0,2);
                initPos = args.CurrentPoint.Position;
                timer.Tick += Timer_Tick;
                timer.Start();
                while (timerStopped == true) ;
                var finPos = args.CurrentPoint.Position;
                var finMag = Math.Pow(((finPos.X * finPos.X) + (finPos.Y * finPos.Y)), 0.5);
                var initMag = Math.Pow(((initPos.X * initPos.X) + (initPos.Y * initPos.Y)), 0.5);
                if (finMag - initMag < 15)
                {
                    canvasX.InkPresenter.IsInputEnabled = false;
                    canvasX.PointerPressed += CanvasX_PointerPressed;
                    canvasX.PointerMoved -= CanvasX_PointerMoved1;
                    canvasX.PointerMoved += CanvasX_PointerMoved;
                    canvasX.PointerReleased += CanvasX_PointerReleased;
                }
            }
        }

        private void CanvasX_PointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Add the final point to the Polyline object and
            // select strokes within the lasso area.
            // Draw a bounding box on the selection canvas
            // around the selected ink strokes.
            lasso.Points.Add(args.GetCurrentPoint(canvasX).Position);

            boundingRect =
              canvasX.InkPresenter.StrokeContainer.SelectWithPolyLine(
                lasso.Points);

            DrawBoundingRect();
        }

        private void CanvasX_PointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Add a point to the lasso Polyline object.
            lasso.Points.Add(args.GetCurrentPoint(canvasX).Position);
        }

        private void CanvasX_PointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Initialize a selection lasso.
            lasso = new Polyline()
            {
                Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection() { 5, 2 },
            };

            lasso.Points.Add(args.GetCurrentPoint(canvasX).Position);

            selectionCanvas.Children.Add(lasso);
        }

        private void Timer_Tick(object sender, object e)
        {
            (sender as DispatcherTimer).Stop();
            timerStopped = true;

        }

        // Stroke selection tool.
        private Polyline lasso;
        // Stroke selection area.
        private Rect boundingRect;

        private void UnprocessedInput_PointerPressed(
      InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Initialize a selection lasso.
            lasso = new Polyline()
            {
                Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection() { 5, 2 },
            };

            lasso.Points.Add(args.CurrentPoint.RawPosition);

            selectionCanvas.Children.Add(lasso);
        }

        private void UnprocessedInput_PointerMoved(
          InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Add a point to the lasso Polyline object.
            lasso.Points.Add(args.CurrentPoint.RawPosition);
        }

        private void UnprocessedInput_PointerReleased(
          InkUnprocessedInput sender, PointerEventArgs args)
        {
            // Add the final point to the Polyline object and
            // select strokes within the lasso area.
            // Draw a bounding box on the selection canvas
            // around the selected ink strokes.
            lasso.Points.Add(args.CurrentPoint.RawPosition);

            boundingRect =
              canvasX.InkPresenter.StrokeContainer.SelectWithPolyLine(
                lasso.Points);

            DrawBoundingRect();
        }

        private void DrawBoundingRect()
        {
            // Clear all existing content from the selection canvas.
            selectionCanvas.Children.Clear();

            // Draw a bounding rectangle only if there are ink strokes
            // within the lasso area.
            if (!((boundingRect.Width == 0) ||
              (boundingRect.Height == 0) ||
              boundingRect.IsEmpty))
            {
                var rectangle = new Rectangle()
                {
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Blue),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection() { 5, 2 },
                    Width = boundingRect.Width,
                    Height = boundingRect.Height
                };

                Canvas.SetLeft(rectangle, boundingRect.X);
                Canvas.SetTop(rectangle, boundingRect.Y);

                selectionCanvas.Children.Add(rectangle);
            }
        }

        // Handle new ink or erase strokes to clean up selection UI.
        private void StrokeInput_StrokeStarted(
          InkStrokeInput sender, Windows.UI.Core.PointerEventArgs args)
        {
            ClearSelection();
        }

        private void InkPresenter_StrokesErased(
          InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            ClearSelection();
        }

        // Clean up selection UI.
        private void ClearSelection()
        {
            var strokes = canvasX.InkPresenter.StrokeContainer.GetStrokes();
            foreach (var stroke in strokes)
            {
                stroke.Selected = false;
            }
            ClearDrawnBoundingRect();
        }

        private void ClearDrawnBoundingRect()
        {
            if (selectionCanvas.Children.Any())
            {
                selectionCanvas.Children.Clear();
                boundingRect = Rect.Empty;
            }
        }
    }
}
