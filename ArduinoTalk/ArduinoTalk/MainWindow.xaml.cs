using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Windows.Threading;

namespace ArduinoTalk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Logger> list1 ; //class that dynamically collects logger class objects and display them on log screen

        public MainWindow()
        {
            InitializeComponent();
            list1 = new ObservableCollection<Logger>()
            {
                new Logger() {ID="1", Message="Suceesfull launch of inital screen"}
            };
            listViewBinding.ItemsSource = list1;
        }

        communicator comport = new communicator();
        Boolean PortConnection = false;
     
        

        private void Connect_Arduino_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            if (comport.connect(9600, "I'M ARDUINO", 4, 8, 16))
            {
              //  this.textBlock1.Text = "Connection Successful- Connected" + comport.port;
                list1.Add(new Logger { ID = "1", Message = "Successfully Connected to" + comport.port } );
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
            else {

                comport.message(4, 8, 38);
                list1.Add(new Logger { ID = "4", Message = "Little Rain" });
            }
        
        }
        
    

    private void timer_Tick(object sender, EventArgs e)
    {
        
         ReadingsBlock.Text = DateTime.Now.ToLongTimeString();
        
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        
       
    }
  
}       




