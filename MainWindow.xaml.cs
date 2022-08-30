using System;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using System.Windows.Media;
using System.Linq;

namespace SerialMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort port;
        public MainWindow()
        {
            InitializeComponent();
            port = new SerialPort(portList.Text, Int32.Parse(baudRate.Text));


        }

        /*
         * 
         * 
         * ////////////////   Check port is Connected Or not IF Connected Turn On /////////////////////////
         * 
         * 
         */


        private void ConnectPort(object sender, RoutedEventArgs e)
        {
            output.Foreground = Brushes.Black;
            port.Close();
            port.PortName = portList.Text; //"COM3";
            port.BaudRate = Int32.Parse(baudRate.Text);//9600;
            port.Parity = Parity.None;
            try
            {
                port.Open();

                new Thread(Recieve).Start();
            }
            catch
            {
                output.AppendText($"\n No Board at {port.PortName} ");
                if (SerialPort.GetPortNames().Length > 0)
                {
                    output.AppendText($"\n Available Ports are \n ");
                    output.AppendText($"{string.Join("\n", SerialPort.GetPortNames())}");

                }
            }
        }

                /* 
                 * 
                 * 
                 * ////////// PRINT DATA ON OUTPUT ///////////////////////
                 * 
                 * 
                 * */


                public void Recieve()
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        connect.IsEnabled = false;
                        baudRate.IsEnabled = false;
                        portList.IsEnabled = false;
                    });
                    while (port.IsOpen)
                    {
                        try
                        {
                            string outputData = port.ReadLine();
                            this.Dispatcher.Invoke(() =>
                            {
                                if (Int32.Parse(outputData) > 35)
                                {
                                    output.Foreground = Brushes.Red;
                                    output.AppendText(outputData);

                                }
                                else
                                {
                                    output.Foreground = Brushes.Green;
                                    output.AppendText(outputData);
                                }
                            });

                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                output.Foreground = Brushes.Red;
                                output.AppendText("\n Disconnected");
                                connect.IsEnabled = true;
                                baudRate.IsEnabled = true;
                                portList.IsEnabled = true;
                                port.Close();
                                MessageBox.Show(ex.Message);
                            });
                        }
                    }
                }


                /* 
                 * 
                 * 
                 * 
                 * ////////// Get input ///////////////////////
                 * 
                 * 
                 * 
                 */

                private void sendData(object sender, RoutedEventArgs e)
                {
                    if (port.IsOpen || SerialPort.GetPortNames().Contains(port.PortName))
                    {
                        try
                        {
                            port.Open();
                            new Thread(Send).Start();
                        }
                        catch (Exception)
                        {
                            output.Foreground = Brushes.Red;
                            output.AppendText("Unable to open port {port.PortName}");
                        }

                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            output.Foreground = Brushes.Red;
                            output.AppendText($"\n No Board at {port.PortName} ");
                            if (SerialPort.GetPortNames().Length > 0)
                            {
                                output.AppendText($"\n Available Ports are \n ");
                                output.AppendText($"{string.Join("\n", SerialPort.GetPortNames())}");
                            }

                        });
                    }
                }

                /* 
          * 
          * 
          * ////////// Send to Port ///////////////////////
          * 
          * 
          * */

                public void Send()
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        port.WriteLine(input.Text);
                        input.Text = "";
                    });
                }


        }

    }

