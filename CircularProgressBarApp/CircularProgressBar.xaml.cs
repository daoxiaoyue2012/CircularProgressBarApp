using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CircularProgressBarApp
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        public CircularProgressBar()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(int), typeof(CircularProgressBar), new PropertyMetadata(15));
        public int StrokeThickness
        {
            get { return (int)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(int), typeof(CircularProgressBar), new PropertyMetadata(150, new PropertyChangedCallback(OnRadiusChanged)));
        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(0d, new PropertyChangedCallback(OnAngleChanged)));
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register("Percentage", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(0d, new PropertyChangedCallback(OnPercentageChanged)));
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public static readonly DependencyProperty ArcSegmentColorProperty = DependencyProperty.Register("ArcSegmentColor", typeof(Brush), typeof(CircularProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Green)));
        public Brush ArcSegmentColor
        {
            get { return (Brush)GetValue(ArcSegmentColorProperty); }
            set { SetValue(ArcSegmentColorProperty, value); }
        }

        public static readonly DependencyProperty TextSizeProperty = DependencyProperty.Register("TextSize", typeof(int), typeof(CircularProgressBar), new PropertyMetadata(18));
        public int TextSize
        {
            get { return (int)GetValue(TextSizeProperty); }
            set { SetValue(TextSizeProperty, value); }
        }

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(Brush), typeof(CircularProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Green)));
        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeDashPatternProperty = DependencyProperty.Register("StrokeDashPattern", typeof(DoubleCollection), typeof(CircularProgressBar), new PropertyMetadata(new DoubleCollection()));
        public DoubleCollection StrokeDashPattern
        {
            get { return (DoubleCollection)GetValue(StrokeDashPatternProperty); }
            set { SetValue(StrokeDashPatternProperty, value); }
        }

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar circularProgressBar = (CircularProgressBar)d;
            circularProgressBar.Angle = (circularProgressBar.Percentage * 360) / 100;
        }

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CircularProgressBar circularProgressBar)
            {
                if (circularProgressBar.FindName("BackgroundPath") is Path backgroundPath)
                {
                    int radius = circularProgressBar.Radius;
                    backgroundPath.Width = radius * 2 + circularProgressBar.StrokeThickness;
                    backgroundPath.Height = radius * 2 + circularProgressBar.StrokeThickness;
                    backgroundPath.Margin = new Thickness(circularProgressBar.StrokeThickness,circularProgressBar.StrokeThickness, 0, 0);
                    backgroundPath.Data = Geometry.Parse($"M 0,{radius} A {radius},{radius} 0 1 1 {radius*2},{radius} A {radius},{radius} 0 1 1 0,{radius}");
                }
                circularProgressBar.RenderArc();
            }
        }

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar circularProgressBar = (CircularProgressBar)d;
            circularProgressBar.RenderArc();
        }

        private void RenderArc()
        {
            Point startPoint = new Point(Radius, 0);
            Point endPoint = ComputeCartesianCoordinate(Angle, Radius);

            endPoint.X += Radius;
            endPoint.Y += Radius;

            ForegroundPath.Width = Radius * 2 + StrokeThickness;
            ForegroundPath.Height = Radius * 2 + StrokeThickness;
            ForegroundPath.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

            bool largeArc = Angle > 180.0;
            Size outerArcSize = new Size(Radius, Radius);
            pathFigure.StartPoint = startPoint;

            if (startPoint.X == Math.Round(endPoint.X) && startPoint.Y == Math.Round(endPoint.Y))
            {
                endPoint.X -= 0.01;
            }

            arcSegment.Point = endPoint;
            arcSegment.Size = outerArcSize;
            arcSegment.IsLargeArc = largeArc;
        }

        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            double angleRadius = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRadius);
            double y = radius * Math.Sin(angleRadius);

            return new Point(x, y);
        }
    }
}
