namespace AnwesenheitUndBewegung
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using System.Diagnostics;
    using AnwesenheitUndBewegung.SkeletonDetection;
    using AnwesenheitUndBewegung.Utilities;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
         /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;
        private SkeletonTracker skeletonTracker = new SkeletonTracker();
        private bool previouslyPresent = false;
        private bool previouslyMoving = false;
        private bool messageSent = false;
        private bool warningSent = false;
        private bool notificationSent = false;
        private string notificationSentText;
        private string warningSentText;
        private Stopwatch absentTimer = new Stopwatch();
        private Stopwatch noMovementTimer = new Stopwatch();
        private Stopwatch notificationTimer = new Stopwatch();
        private int acceptedMinutesAbsent;
        private int acceptedMinutesNotMoving;
        private int minutesUntilNotification;
        private string acceptedTimeAbsent;
        private string acceptedTimeNotMoving;
        private string timeUntilNotification;
        private string noMovementText = "";
        private string absentText ="";
        public event PropertyChangedEventHandler PropertyChanged;
        public int UserId { get; set; }
        private int previousUserId = 0;
        private DatabaseHandler databaseHandler;

        public string WarningSentText
        {
            get { return warningSentText; }
            set
            {
                if (value != warningSentText)
                {
                    warningSentText = value;
                    OnPropertyChanged("WarningSentText");
                }
            }
        }
        public string NotificationSentText
        {
            get { return notificationSentText; }
            set
            {
                if (value != notificationSentText)
                {
                    notificationSentText = value;
                    OnPropertyChanged("NotificationSentText");
                }
            }
        }
        public string AcceptedTimeAbsent
        {
            get { return acceptedTimeAbsent; }
            set
            {
                if (value != acceptedTimeAbsent)
                {
                    int checkMinutes = timeAllowedConverter(value);
                    if (checkMinutes<1 || checkMinutes > 59)
                    {
                        acceptedTimeAbsent = "10";
                        acceptedMinutesAbsent = 10;
                    }
                    else
                    {
                        acceptedTimeAbsent = value;
                        acceptedMinutesAbsent = checkMinutes;
                    }
                    OnPropertyChanged("AcceptedTimeAbsent");
                }
            }
        }
        public string AcceptedTimeNotMoving
        {
            get { return acceptedTimeNotMoving; }
            set
            {
                if (value != acceptedTimeNotMoving)
                {
                    int checkMinutes = timeAllowedConverter(value);
                    if (checkMinutes < 1 || checkMinutes > 59)
                    {
                        acceptedTimeNotMoving = "10";
                        acceptedMinutesNotMoving = 10;
                    }
                    else
                    {
                        acceptedTimeNotMoving = value;
                        acceptedMinutesNotMoving = checkMinutes;
                    }
                    OnPropertyChanged("AcceptedTimeNotMoving");
                }
            }
        }
        public string TimeUntilNotification
        {
            get { return timeUntilNotification; }
            set
            {
                if (value != timeUntilNotification)
                {
                    int checkMinutes = timeAllowedConverter(value);
                    if (checkMinutes < 1 || checkMinutes > 59)
                    {
                        timeUntilNotification = "10";
                        minutesUntilNotification = 10;
                    }
                    else
                    {
                        timeUntilNotification = value;
                        minutesUntilNotification = checkMinutes;
                    }
                    OnPropertyChanged("TimeUntilNotification");
                }
            }
        }
        public string NoMovementText
        {
            get { return noMovementText; }
            set
            {
                if(value != noMovementText)
                {
                    noMovementText = value;
                    OnPropertyChanged("NoMovementText");
                }
            }
        }
        public string AbsentText
        {
            get { return absentText; }
            set
            {
                if (value != absentText)
                {
                    absentText = value;
                    OnPropertyChanged("AbsentText");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent(); 
        }
        
        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    this.sensor.DepthStream.Range = DepthRange.Near;
                    this.sensor.SkeletonStream.EnableTrackingInNearRange = true;
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
            
            presenceLabel.Background = new SolidColorBrush(Colors.White);
            movementLabel.Background = new SolidColorBrush(Colors.White);
            visGrid.DataContext = this;
            AcceptedTimeAbsent = "10";
            AcceptedTimeNotMoving = "10";
            TimeUntilNotification = "10";
            WarningSentText = "";
            NotificationSentText = "";
            databaseHandler = new DatabaseHandler();
            
            
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (UserId == 0) {
                movementLabel.Background = new SolidColorBrush(Colors.White);
                presenceLabel.Background = new SolidColorBrush(Colors.White);
                NoMovementText = "";
                AbsentText = "Please log in";
                return;
            }
            
            if (UserId != previousUserId)
            {
                AbsentText = "";
            }
            

            skeletonTracker.newSkeleton(sender, e);
            bool[] isPresentAndMoving = skeletonTracker.skeletonMovingAndPresent();
            bool isPresent = isPresentAndMoving[0];
            bool isMoving = isPresentAndMoving[1];

            if (isPresent)
            {
                if (previouslyPresent)
                {
                    if (isMoving)
                    {
                        if (previouslyMoving)
                        {   
                            //do nothing
                        }
                        else
                        {
                            //resettimer and change Visualization to green and hide the timer
                            NoMovementText = "";
                            noMovementTimer.Reset();
                            movementLabel.Background = new SolidColorBrush(Colors.Green);

                        }
                    }
                    else
                    {
                        if (previouslyMoving)
                        {
                            //starttimer and change Visualization from green to red
                            noMovementTimer.Start();
                            movementLabel.Background = new SolidColorBrush(Colors.Red);

                        }
                        else
                        {
                            TimeSpan ts = noMovementTimer.Elapsed;
                            
                            if (!messageSent && (ts.Minutes >= acceptedMinutesNotMoving))
                            {
                                WarningSentText = "A warning was sent";
                                warningSent = true;
                                messageSent = true;
                                notificationTimer.Start();
                                DateTime notMovingStart = DateTime.Now-ts;
                                MessageBoxResult result = MessageBox.Show("Click OK to confirm you're awake", "Confirmation");
                                if (result == MessageBoxResult.OK)
                                {
                                    messageSent = false;
                                    warningSent = false;
                                    notificationSent = false;
                                    NotificationSentText = "";
                                    WarningSentText = "";
                                    
                                    DateTime notMovingStop = DateTime.Now;
                                    if((notMovingStop-notMovingStart).TotalMinutes>=(acceptedMinutesNotMoving+minutesUntilNotification))
                                    {
                                        databaseHandler.addNotMovingTime(notMovingStart, notMovingStop);
                                    }
                                    
                                    notificationTimer.Reset();
                                    noMovementTimer.Restart();

                                }

                            }
                            else if (!notificationSent && messageSent)
                            {
                                if (notificationTimer.Elapsed.Minutes >= minutesUntilNotification)
                                {
                                    notificationSent = true;
                                    NotificationSentText = "A notification was sent";
                                }
                            }
                            
                            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                            NoMovementText = "" + elapsedTime;
                            
                        }
                    }
                }
                else
                {
                    //resettimer and change Visualization to green and hide the timer
                    absentTimer.Reset();
                    AbsentText = "";
                    presenceLabel.Background = new SolidColorBrush(Colors.Green);
                }
            }
            else
            {
                if (previouslyPresent)
                {
                    //starttimer and change Visualization from green to red and show the timer
                    absentTimer.Start();
                    presenceLabel.Background = new SolidColorBrush(Colors.Red);
                    movementLabel.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    //checktimer if time is greater than timeallowed
                    TimeSpan ts = absentTimer.Elapsed;

                    if (!messageSent && (ts.Minutes >= acceptedMinutesAbsent))
                    {
                        WarningSentText = "A warning was sent";
                        warningSent = true;
                        messageSent = true;
                        notificationTimer.Start();
                        DateTime absentTimeStart = DateTime.Now-ts;
                        MessageBoxResult result = MessageBox.Show("Click OK to confirm your return", "Confirmation");
                        if (result == MessageBoxResult.OK)
                        {   
                            messageSent = false;
                            warningSent = false;
                            notificationSent = false;
                            NotificationSentText = "";
                            WarningSentText = "";
                            DateTime absentTimeStop = DateTime.Now;
                            TimeSpan timeAbsent = absentTimeStop - absentTimeStart;
                            if((absentTimeStop-absentTimeStart).TotalMinutes>=(acceptedMinutesAbsent+minutesUntilNotification))
                            {
                                this.databaseHandler.addAbsentTime(absentTimeStart, absentTimeStop);
                            }

                            notificationTimer.Reset();
                            absentTimer.Restart();

                        }
                    }
                    else if (!notificationSent&&messageSent)
                    {
                        if (notificationTimer.Elapsed.Minutes >= minutesUntilNotification)
                        {
                            notificationSent = true;
                            NotificationSentText="A notification was sent";
                        }
                        
                    }
                    

                    string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                    AbsentText = "" + elapsedTime;
                }
                absentTimer.Start();
                presenceLabel.Background = new SolidColorBrush(Colors.Red);
                movementLabel.Background = new SolidColorBrush(Colors.White);
            }

            previouslyPresent = isPresent;
            previouslyMoving = isMoving;
            previousUserId = UserId;

        }

        private int timeAllowedConverter(string input)
        {
            int output = -1;
            try
            {
                output = Convert.ToInt16(input);
            }
            catch(FormatException e)
            {
            }
            catch(OverflowException e)
            {
            }
            
            return output;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    
    }
}
