using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading ;
using System.IO.Ports;

namespace ArduinoTalk
{
    class communicator
    {
        public string port = "";
        static SerialPort currentPort;

        /* Function to establish connection between Arduino and C# application
         * It sends a message "I am Arduino" to all the available COM ports 
         * Port will be choosen based on  Echo message from Arduino 
         * Message is sent in 3 byte encoding 
         * baudrate, recognisingText, 3 byte parameters are passed to the function
         *
         */
        public Boolean connect(int baud, string recognizeText, byte paramone, byte paramtwo, byte paramthree)
        {
            try
            {

                byte[] buffer = new byte[3];
                buffer[0] = Convert.ToByte(paramone);
                buffer[1] = Convert.ToByte(paramtwo);
                buffer[2] = Convert.ToByte(paramthree);

                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                string[] ports = SerialPort.GetPortNames();
                foreach (string newport in ports)
                {
                    currentPort = new SerialPort(newport, baud);
                    currentPort.Open();
                    currentPort.Write(buffer, 0, 3);
                    Thread.Sleep(200);
                    int count = currentPort.BytesToRead;
                    string returnMessage = "";
                    while (count > 0)
                    {
                        intReturnASCII = currentPort.ReadByte();
                        returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                        count--;
                    }

                    currentPort.Close();
                    port = newport;
                    if (returnMessage.Contains(recognizeText))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        
        
        /* this method works as a Message communicator between arduino and 
         * desktop application. Takes 3 Byte arguments and returns a string, which
         * is being received from Arduino 
         * 
         * 
         * 
         * 
         */

        public string message(byte paramone, byte paramtwo, byte paramthree)
        {
            try
            {
                byte[] buffer = new byte[3];
                buffer[0] = Convert.ToByte(paramone);
                buffer[1] = Convert.ToByte(paramtwo);
                buffer[2] = Convert.ToByte(paramthree);
                currentPort.Open();
                currentPort.Write(buffer, 0, 3);
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                Thread.Sleep(200);
                int count = currentPort.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = currentPort.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                currentPort.Close();
                return returnMessage;
            }
            catch (Exception e)
            {
                return "Error";
            }

        }

    }
}

