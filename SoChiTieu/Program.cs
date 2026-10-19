using System.Globalization;
using Serilog;
using SoChiTieu.Helpers;

namespace SoChiTieu;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Phải gọi trước khi tạo bất kỳ control nào
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) => XuLyLoiChuaBat(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (s, e) => XuLyLoiChuaBat(e.ExceptionObject as Exception);

        ApplicationConfiguration.Initialize();

        try
        {
            AppSettings.Load();   // đọc appsettings.json (+ appsettings.Development.json)
            LogHelper.Init();     // Serilog -> Logs/log-yyyyMMdd.txt
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file cấu hình appsettings.json.\n\n" + ex.Message,
                "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Định dạng ngày / tiền theo kiểu Việt Nam (dd/MM/yyyy, 1.250.000)
        var vi = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = vi;
        CultureInfo.DefaultThreadCurrentUICulture = vi;
        Thread.CurrentThread.CurrentCulture = vi;
        Thread.CurrentThread.CurrentUICulture = vi;

        Log.Information("Ung dung khoi dong");
        try
        {
            Application.Run(new FormChiTieu());
        }
        finally
        {
            Log.Information("Ung dung dong");
            LogHelper.Close();
        }
    }

    private static void XuLyLoiChuaBat(Exception ex)
    {
        Log.Error(ex, "Loi chua duoc bat");
        MessageBox.Show(
            "Đã xảy ra lỗi, thông tin đã được ghi lại.",
            "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
