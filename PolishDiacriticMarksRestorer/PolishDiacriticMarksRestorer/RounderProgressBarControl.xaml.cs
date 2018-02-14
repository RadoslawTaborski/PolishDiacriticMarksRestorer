using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Interaction logic for RounderProgressBarControl.xaml
    /// </summary>
    public partial class RounderProgressBarControl
    {
        private delegate void VoidDelegete();
        private Timer _timer;
        private Timer _timer2;
        public new bool Loaded;
        private double _angle;

        public RounderProgressBarControl()
        {
            InitializeComponent();
            ((FrameworkElement) this).Loaded += OnLoaded;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
            _timer2 = new Timer(1);
            _timer2.Elapsed += OnTimerElapsed2;
            _timer2.Start();
            Loaded = true;
        }

        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RotationCanvas.Dispatcher.Invoke
            (
                new VoidDelegete(
                    delegate
                    {
                        SpinnerRotate.Angle += 30;
                        if (Math.Abs(SpinnerRotate.Angle - 360) < 0.01)
                        {
                            SpinnerRotate.Angle = 0;
                        }
                    }
                ),
                null
            );

        }

        public void OnTimerElapsed2(object sender, ElapsedEventArgs e)
        {
            RotationCanvas.Dispatcher.Invoke
            (
                new VoidDelegete(
                    delegate
                    {
                        _angle += 1.5;
                        RenderTransform = new RotateTransform(_angle, Width/2, Height/2);

                        if (Math.Abs(_angle - 360.0) < 0.01)
                        {
                            _angle = 0;
                        }
                    }
                ),
                null
            );

        }
    }
}
