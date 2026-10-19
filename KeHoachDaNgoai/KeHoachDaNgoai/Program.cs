using Microsoft.Extensions.Configuration; using Serilog; using System.Globalization;
namespace KeHoachDaNgoai;
internal static class Program
{
 [STAThread] static void Main() {
  var cfg = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", false, true).AddJsonFile("appsettings.Development.json", true, true).Build();
  Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File(Path.Combine(AppContext.BaseDirectory,"Logs","log-.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit:14).CreateLogger();
  Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
  Application.ThreadException += (_,e) => { Log.Error(e.Exception,"UI exception"); MessageBox.Show("Đã xảy ra lỗi, thông tin đã được ghi lại.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning); };
  AppDomain.CurrentDomain.UnhandledException += (_,e) => Log.Error(e.ExceptionObject as Exception,"Unhandled exception");
  ApplicationConfiguration.Initialize();
  try { Application.Run(new MainForm(cfg)); } catch(Exception ex) { Log.Error(ex,"Application failed"); MessageBox.Show("Không thể mở ứng dụng. Kiểm tra cấu hình và file Logs."); } finally { Log.CloseAndFlush(); }
 }
}