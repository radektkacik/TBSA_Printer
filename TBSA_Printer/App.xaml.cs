using System.Configuration;
using System.Data;
using System;
using System.Windows;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Diagnostics;

namespace TBSA_Printer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string _connectionString = "Server=192.168.130.31;Database=LogTest;User Id=sa;Password=Taurid1*;TrustServerCertificate=True;";
        private const string _schemaName = "dbo";
        private const string _tableName = "LogEvents";
        protected override void OnStartup(StartupEventArgs e)
        {
            
            base.OnStartup(e);

            Log.Logger = new LoggerConfiguration().WriteTo
            .MSSqlServer(                               //Výstup do DB
                connectionString: _connectionString,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = _tableName,
                    SchemaName = _schemaName,
                    AutoCreateSqlTable = true
                },
                restrictedToMinimumLevel: LogEventLevel.Debug,
                formatProvider: null,
                columnOptions: null,
                logEventFormatter: null)
            .WriteTo.Debug()            // Výstup do Debug Output Window
            .WriteTo.Console()          // Přidává výstup do konzole
            .CreateLogger();
            

            Log.Information("Application started.");
        }
        protected override void OnExit(ExitEventArgs e)
        {
            // Ensure that all logs are written and resources are released
            Log.Information("Application is shutting down.");
            Log.CloseAndFlush(); // Important to flush and close the logger on exit
            base.OnExit(e);
        }
    }
    
}
