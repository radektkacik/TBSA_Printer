using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using System.Printing;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using TBSA_Printer.Models;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;



namespace TBSA_Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("button_Click Start"); 

            string printerIp = tBIP.Text;

            int printerPort = Int32.Parse(tBPort.Text);

            TcpConnection Conn = new(printerIp, printerPort);

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");  
            string currentTime = DateTime.Now.ToString("HH:mm:ss");      

            string zplCommand = $@"^XA 
                                ^FO50,50
                                ^A0N,50,50  
                                ^FDFunguje!^FS                                 

                                ^FO50,150                 
                                ^BCN,100,Y,N,N            
                                ^FD1234567890^FS

                                ^FO50,300
                                ^BQN,2,10
                                ^FDQA,https://www.taurid.cz^FS

                                ^FO50,650
                                ^A0N,40,40
                                ^FD{currentDate} {currentTime} ^FS
                                ^XZ";

            using (var context = new LogTestContext()) // Nahraďte YourDbContext jménem generovaného DbContextu
            {
                // Přečtení jednoho uživatele z tabulky Users, který má ID = 1
                //var user = context.Eftests.OrderByDescending(u => u.Id).FirstOrDefault();                  

                var user = context.Eftests.FromSqlRaw("SELECT * from dbo.fcTest()").AsEnumerable().FirstOrDefault();        //Volání funkce z DB
                if (user != null)
                {
                    Log.Information($"User ID: {user.Id}, Name: {user.Name}, Email: {user.Text}");
                }
                else
                {
                    Log.Information("User not found.");
                }
            }

            try
            {              
                Conn.Open();
                if(checkPrinterStatus(Conn))
                { 
                    Conn.Write(Encoding.UTF8.GetBytes(zplCommand));
                    if (postPrintCheckPrinterStatus(Conn))
                    {
                        lblStatus.Foreground = Brushes.Green;
                        lblStatus.Content = "Success";
                        Log.Information("Printed successfully");
                        
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Log.Error(ex.Message);
                lblStatus.Foreground = Brushes.Red;
                lblStatus.Content = ex.Message;
            }
            finally
            {
                Conn.Close();                
            }
             

        }

        private void tBPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))

                e.Handled = true;
        }

        // check prior to printing
        private bool checkPrinterStatus(Connection connection)
        {
            return true; // TODO

            ZebraPrinter printer = ZebraPrinterFactory.GetLinkOsPrinter(connection, PrinterLanguage.ZPL);
            if (null == printer)
            {
                printer = ZebraPrinterFactory.GetInstance(PrinterLanguage.ZPL, connection);
            }
            PrinterStatus printerStatus = printer.GetCurrentStatus();
            if (printerStatus.isReadyToPrint)
            {
                Console.WriteLine("Ready To Print");
                return true;
            }
            else if (printerStatus.isPaused)
            {
                Console.WriteLine("Cannot Print because the printer is paused.");
                Log.Error("Cannot Print because the printer is paused.");
            }
            else if (printerStatus.isHeadOpen)
            {
                Console.WriteLine("Cannot Print because the printer head is open.");
                Log.Error("Cannot Print because the printer head is open.");
            }
            else if (printerStatus.isPaperOut)
            {
                Console.WriteLine("Cannot Print because the paper is out.");
                Log.Error("Cannot Print because the paper is out");
            }
            else
            {
                Console.WriteLine("Cannot Print.");
                Log.Error("Cannot Print.");
            }
            return false;
        }

        // Check during / after printing
        private bool postPrintCheckPrinterStatus(Connection connection)
        {
            return true; // TODO
            ZebraPrinter printer = ZebraPrinterFactory.GetLinkOsPrinter(connection, PrinterLanguage.ZPL);
            if (null == printer)
            {
                printer = ZebraPrinterFactory.GetInstance(PrinterLanguage.ZPL, connection);
            }
            PrinterStatus printerStatus = printer.GetCurrentStatus();

            // loop while printing until print is complete or there is an error
            while ((printerStatus.numberOfFormatsInReceiveBuffer > 0) && (printerStatus.isReadyToPrint))
            {
                Thread.Sleep(500);
                printerStatus = printer.GetCurrentStatus();
            }
            if (printerStatus.isReadyToPrint)
            {
                Console.WriteLine("Ready To Print");
                return true;
            }
            else if (printerStatus.isPaused)
            {
                Console.WriteLine("Cannot Print because the printer is paused.");
                Log.Error("Cannot Print because the printer is paused.");
            }
            else if (printerStatus.isHeadOpen)
            {
                Console.WriteLine("Cannot Print because the printer head is open.");
                Log.Error("Cannot Print because the printer head is open.");
            }
            else if (printerStatus.isPaperOut)
            {
                Console.WriteLine("Cannot Print because the paper is out.");
                Log.Error("Cannot Print because the paper is out");
            }
            else
            {
                Console.WriteLine("Cannot Print.");
                Log.Error("Cannot Print.");
            }
            return false;
        }
    }
}