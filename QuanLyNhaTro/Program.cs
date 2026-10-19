using System.Globalization;
using Serilog;
using QuanLyNhaTro.Helpers;

namespace QuanLyNhaTro;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // ---- Xử lý lỗi toàn cục (phải đăng ký trước khi tạo bất kỳ control nào) ----
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) => XuLyLoiChuaBat(e.Exception, "Application.ThreadException");
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            XuLyLoiChuaBat(e.ExceptionObject as Exception, "AppDomain.UnhandledException");
            if (e.IsTerminating) Log.CloseAndFlush();
        };
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Log.Error(e.Exception, "Loi Task khong duoc quan sat (TaskScheduler.UnobservedTaskException)");
            e.SetObserved();
        };

        ApplicationConfiguration.Initialize();

        try
        {
            AppSettings.Load();
            LogHelper.Init();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file cấu hình appsettings.json.\n\n" + ex.Message,
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var vi = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = vi;
        CultureInfo.DefaultThreadCurrentUICulture = vi;
        Thread.CurrentThread.CurrentCulture = vi;
        Thread.CurrentThread.CurrentUICulture = vi;

        Log.Information("Ung dung khoi dong");
        try
        {
            Application.Run(new FormKhachThue());
        }
        finally
        {
            Log.Information("Ung dung dong");
            LogHelper.Close();
        }
    }

    /// <summary>Mọi lỗi chưa bắt: ghi log Error (đầy đủ stack trace) + thông báo thân thiện, ứng dụng chạy tiếp.</summary>
    private static void XuLyLoiChuaBat(Exception ex, string nguon)
    {
        Log.Error(ex, "Loi chua duoc bat ({Nguon})", nguon);
        try
        {
            MessageBox.Show(
                "Đã xảy ra lỗi, thông tin đã được ghi lại.",
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch
        {
            // Không để việc hiện hộp thoại gây thêm lỗi
        }
    }
}
