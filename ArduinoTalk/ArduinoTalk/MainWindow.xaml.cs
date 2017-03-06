using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace ArduinoTalk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private ObservableCollection<Logger> list1; //class that dynamically collects logger class objects and display them on log screen

        public MainWindow()
        {
            InitializeComponent();
            // collection object to collect the log files and will be posted on log list down on the window 

            list1 = new ObservableCollection<Logger>()
            {
                new Logger() {ID="1", Message="Suceesfull launch of inital screen"}
            };
            listViewBinding.ItemsSource = list1;
            //Excel Setup
      

        }

        communicator comport = new communicator();
        bool PortConnection = false;
        //EXcel data verification
       


        private void Connect_Arduino_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // calls the timer func for every 5 seconds. 
            timer.Tick += timer_Tick;
            timer.Start();
            if (comport.connect(9600, "I'M ARDUINO", 4, 8, 16))
            {
                //  this.textBlock1.Text = "Connection Successful- Connected" + comport.port;
                list1.Add(new Logger { ID = "1", Message = "Successfully Connected to" + comport.port });
                PortConnection = true;
                PresenceLabel.Background = new SolidColorBrush(Colors.Green);
                timer.Start();
                
            }

            else
            {
                //   this.textBlock1.Text = "Connection not Sucessfull";
                PortConnection = false;
                PresenceLabel.Background = new SolidColorBrush(Colors.Red);
                list1.Add(new Logger { ID = "1", Message = "Arduino connection failed, try again or check the usb connection with Arduino" });
                timer.Stop();
            }
        }


        private void onPatternSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // combobox selections for different rain patterns 

            if (cmbSelect.SelectedIndex == 0)
            {

                comport.message(4, 8, 32);
                list1.Add(new Logger { ID = "2", Message = "Intense Rain" });

            }
            else if (cmbSelect.SelectedIndex == 1)
            {
                comport.message(4, 8, 34);
                list1.Add(new Logger { ID = "3", Message = "Moderate Rain" });

            }
            else if (cmbSelect.SelectedIndex == 2)
            {

                comport.message(4, 8, 38);
                list1.Add(new Logger { ID = "4", Message = "Little Rain" });
            }
            else
            {
                comport.message(4, 8, 40);
                list1.Add(new Logger { ID = "5", Message = "Reset to default" });

            }

        }



        private void timer_Tick(object sender, EventArgs e)
        {
            String returnMessage = comport.message(4, 8, 32);
            ReadingsBlock.Text = returnMessage;
        //    int rowIndex = 1;
         //   UpdateExcel("Sheet3", rowIndex,1, returnMessage);
         //   rowIndex++ ;
        }



    }
  
}       




