using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Balazs11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thickness ballStartPosition;
        Thickness ballCurrentPosition;
        Thickness ballNextPosition;
        Thickness padStartPosition;
        Thickness padCurrentPosition;

        ThicknessAnimation ballAnimation;
        ThicknessAnimation padAnimation;

        DateTime startTime;

        bool isStarted;

        double speedFactor;
        public MainWindow()
        {
            InitializeComponent();
            ballStartPosition = ellipseBall.Margin;
            padStartPosition = padCurrentPosition = rectanglePad.Margin;
            isStarted = false;
            newGameMenu.Click += NewGame;
            exitMenu.Click += Exit;
            KeyDown += Navigate;
            LayoutUpdated += BallLayoutUpdated;
        }

        #region Esemény kezelők
        void NewGame(object sender, RoutedEventArgs args)
        {
            StartGame();
        }

        void Navigate(object sender, KeyEventArgs args)
        {
            switch(args.Key)
            {
                case Key.Left:
                    if (rectanglePad.Margin.Left >= 100)
                        AnimatePad(-100);
                    break;
                case Key.Right:
                    if (rectanglePad.Margin.Left + 120 <= grid.ActualWidth - 100)
                        AnimatePad(100);
                    break;
            }
        }

        void BallLayoutUpdated(object sender, EventArgs args)
        {
            if(isStarted)
            {
                if((-ellipseBall.Margin.Top -40 + rectanglePad.Margin.Top < 0.5) &&(ellipseBall.Margin.Left+20 >= rectanglePad.Margin.Left && ellipseBall.Margin.Left+20 <= rectanglePad.Margin.Left+120))
                {
                    if(ballCurrentPosition.Left < ballNextPosition.Left)//Balrol
                    {
                        speedFactor *= 1.05;
                        ballNextPosition = new Thickness(ballNextPosition.Left + 200, 0, 0, 0);
                        AnimateBall();
                    }
                    else//Jobbrol
                    {
                        speedFactor *= 1.05;
                        ballNextPosition = new Thickness(ballNextPosition.Left -200, 0, 0, 0);
                        AnimateBall();
                    }
                }
                else if(ellipseBall.Margin.Top == 0)
                {
                    ballNextPosition = new Thickness(ballNextPosition.Left,Height, 0, 0);
                    AnimateBall();
                }
                else if(ellipseBall.Margin.Left == 0)
                {
                    ballNextPosition = new Thickness(grid.ActualWidth, ballNextPosition.Top, 0, 0);
                    AnimateBall();
                }
                else if(-ellipseBall.Margin.Left-40+ grid.ActualWidth < 0.5)
                {
                    ballNextPosition = new Thickness(0, ballNextPosition.Top, 0, 0);
                    AnimateBall();
                }
                else if(ellipseBall.Margin.Top == Height)
                {
                    StopGame();
                    MessageBox.Show("Játék Vége");
                }
            }
        }

        void Exit(object sender, RoutedEventArgs args)
        {
            Close();
        }
        #endregion


        private void StartGame()
        {
            startTime = DateTime.Now;
            isStarted = true;
            ellipseBall.Margin = ballStartPosition;
            ballCurrentPosition = ballStartPosition;
            ballNextPosition = new Thickness(400, 0, 0, 0);
            rectanglePad.Margin = padStartPosition;

            speedFactor = 1;

            AnimateBall();
        }
        private void StopGame()
        {
            isStarted = false;
        }

        private void AnimateBall()
        {
            ballCurrentPosition = ellipseBall.Margin;
            ballAnimation = new ThicknessAnimation();
            ballAnimation.From = ballCurrentPosition;
            ballAnimation.To = ballNextPosition;
            ballAnimation.Duration = TimeSpan.FromMilliseconds(5);
            double dis = BallTravelDistance();
            double speed = speedFactor / dis;
            ballAnimation.SpeedRatio = speed;
            ellipseBall.BeginAnimation(Ellipse.MarginProperty, ballAnimation, HandoffBehavior.SnapshotAndReplace);
        }        
        private void AnimatePad(Int32 value)
        {
            padCurrentPosition = rectanglePad.Margin;
            padAnimation = new ThicknessAnimation();
            padAnimation.From = padCurrentPosition;
            padAnimation.To = new Thickness(padCurrentPosition.Left + value, padCurrentPosition.Top, 0, 0);
            padAnimation.Duration = TimeSpan.FromMilliseconds(100);
            rectanglePad.BeginAnimation(Rectangle.MarginProperty, padAnimation, HandoffBehavior.SnapshotAndReplace);
        }
        private double BallTravelDistance()
        {
            return Math.Sqrt(Math.Pow(ballCurrentPosition.Left - ballNextPosition.Left, 2) + Math.Pow(ballCurrentPosition.Top - ballNextPosition.Top, 2));
        }
    }
}
