using System.Configuration;
using System.Data;
using System;
using System.Windows;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace TBSA_Printer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            // Ensure that all logs are written and resources are released
            Log.Information("Application is shutting down.");
            Log.CloseAndFlush(); // Important to flush and close the logger on exit
            base.OnExit(e);
        }
    }
    
}
