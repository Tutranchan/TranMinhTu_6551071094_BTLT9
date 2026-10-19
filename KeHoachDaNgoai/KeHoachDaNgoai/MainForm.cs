using KeHoachDaNgoai.Helpers; using KeHoachDaNgoai.Models; using KeHoachDaNgoai.Services; using Microsoft.Extensions.Configuration; using Microsoft.EntityFrameworkCore; using Serilog; using System.Globalization; using System.Net.Http;
namespace KeHoachDaNgoai;
public class MainForm:Form {
 readonly AppConfig cfg; readonly TextBox ten=new(){Width=150},diadiem=new(){Width=150},ghichu=new(){Width=150}; readonly DateTimePicker ngay=new(){Format=DateTimePickerFormat.Short}; readonly NumericUpDown songuoi=new(){Minimum=1,Maximum=100,Value=2},ngansach=new(){Maximum=1000000000,Increment=100000,ThousandsSeparator=true}; readonly ComboBox phuongtien=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=130},lang=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=100}; readonly CheckBox hoanthanh=new(){Text="Đã hoàn thành"},chua=new(){Text="Chỉ hiện chuyến chưa hoàn thành"}; readonly TextBox tim=new(){Width=160}; readonly DateTimePicker tu=new(){Format=DateTimePickerFormat.Short},den=new(){Format=DateTimePickerFormat.Short}; readonly DataGridView grid=new(){Dock=DockStyle.Fill,ReadOnly=true,AutoGenerateColumns=true,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill}; readonly Label weather=new(){AutoSize=true,Padding=new Padding(8),Text="Chọn chuyến rồi bấm Xem thời tiết"}; readonly ProgressBar progress=new(){Style=ProgressBarStyle.Marquee,Visible=false,Width=130}; readonly Button viewWeather=new(){Text="Xem thời tiết"}; int id=0; bool en=false;
 public MainForm(IConfiguration c){cfg=AppConfig.Load(c);UserSettings.Load();en=UserSettings.Language=="en"; Text="Kế hoạch dã ngoại - v1.0.0";Width=1180;Height=760;StartPosition=FormStartPosition.CenterScreen; phuongtien.Items.AddRange(new object[]{"Xe máy","Ô tô","Xe khách","Tàu hỏa"});phuongtien.SelectedIndex=0;lang.Items.AddRange(new object[]{"Tiếng Việt","English"});lang.SelectedIndex=en?1:0; var root=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=4,Padding=new Padding(8)};root.RowStyles.Add(new(SizeType.Absolute,190));root.RowStyles.Add(new(SizeType.Absolute,42));root.RowStyles.Add(new(SizeType.Percent,100));root.RowStyles.Add(new(SizeType.Absolute,115));Controls.Add(root);var form=new FlowLayoutPanel{Dock=DockStyle.Fill,AutoScroll=true};root.Controls.Add(form,0,0); Add(form,"Tên chuyến",ten);Add(form,"Địa điểm",diadiem);Add(form,"Ngày đi",ngay);Add(form,"Số người",songuoi);Add(form,"Ngân sách",ngansach);Add(form,"Phương tiện",phuongtien);Add(form,"Ghi chú",ghichu);form.Controls.Add(hoanthanh);form.Controls.Add(lang);lang.SelectedIndexChanged+=(_,_)=>{en=lang.SelectedIndex==1;UserSettings.Language=en?"en":"vi";UserSettings.Save();ApplyLanguage();}; var actions=new FlowLayoutPanel{Dock=DockStyle.Fill};root.Controls.Add(actions,0,1);foreach(var (t,act) in new (string,Action)[]{("Thêm",AddTrip),("Sửa",UpdateTrip),("Xóa",DeleteTrip),("Làm mới",Reset),("Tải lại",LoadGrid)}){var b=new Button{Text=t,AutoSize=true};b.Click+=(_,_)=>act();actions.Controls.Add(b);}actions.Controls.Add(new Label{Text="Tìm:",AutoSize=true,Padding=new Padding(4)});actions.Controls.Add(tim);actions.Controls.Add(chua);actions.Controls.Add(new Label{Text="Từ ngày",AutoSize=true});actions.Controls.Add(tu);actions.Controls.Add(new Label{Text="Đến ngày",AutoSize=true});actions.Controls.Add(den);var filter=new Button{Text="Lọc"};filter.Click+=(_,_)=>LoadGrid();actions.Controls.Add(filter);root.Controls.Add(grid,0,2);grid.SelectionChanged+=(_,_)=>Pick();var bottom=new FlowLayoutPanel{Dock=DockStyle.Fill};root.Controls.Add(bottom,0,3);bottom.Controls.Add(viewWeather);viewWeather.Click+=async(_,_)=>await ShowWeather();bottom.Controls.Add(progress);bottom.Controls.Add(weather);ApplyLanguage();LoadGrid();}
 void Add(FlowLayoutPanel p,string s,Control c){var x=new FlowLayoutPanel{FlowDirection=FlowDirection.TopDown,Width=165,Height=70};x.Controls.Add(new Label{Text=s,AutoSize=true});x.Controls.Add(c);p.Controls.Add(x);} void ApplyLanguage(){if(en){Text="Outdoor Trip Planner - v1.0.0";hoanthanh.Text="Completed";chua.Text="Show incomplete trips only";viewWeather.Text="View weather";}else{Text="Kế hoạch dã ngoại - v1.0.0";hoanthanh.Text="Đã hoàn thành";chua.Text="Chỉ hiện chuyến chưa hoàn thành";viewWeather.Text="Xem thời tiết";}}
 QLDaNgoaiContext Db()=>cfg.CreateDb(); void LoadGrid(){try{using var db=Db();var q=db.ChuyenDi.AsNoTracking().AsQueryable();string s=tim.Text.Trim();if(s!="")q=q.Where(x=>x.TenChuyen.Contains(s)||x.DiaDiem.Contains(s));if(chua.Checked)q=q.Where(x=>!x.DaHoanThanh);if(tu.Value.Date<=den.Value.Date){q=q.Where(x=>x.NgayDi>=tu.Value.Date&&x.NgayDi<=den.Value.Date);}grid.DataSource=q.OrderBy(x=>x.NgayDi).ToList();}catch(Exception ex){Log.Error(ex,"Load trips failed");MessageBox.Show("Không truy cập được CSDL. Kiểm tra connection string và SQL Server.");}}
 void Pick(){if(grid.CurrentRow?.DataBoundItem is not ChuyenDi x)return;id=x.MaChuyen;ten.Text=x.TenChuyen;diadiem.Text=x.DiaDiem;ngay.Value=x.NgayDi<ngay.MinDate?DateTime.Today:x.NgayDi;songuoi.Value=Math.Clamp(x.SoNguoi,1,100);ngansach.Value=Math.Clamp(x.NganSach,0,1000000000);phuongtien.SelectedItem=x.PhuongTien;hoanthanh.Checked=x.DaHoanThanh;ghichu.Text=x.GhiChu??"";}
 bool Valid(bool add){if(string.IsNullOrWhiteSpace(ten.Text)||string.IsNullOrWhiteSpace(diadiem.Text)){MessageBox.Show(en?"Trip name and destination are required.":"Tên chuyến và địa điểm không được để trống.");return false;}if(add&&ngay.Value.Date<DateTime.Today){MessageBox.Show(en?"New trip date cannot be in the past.":"Ngày đi khi thêm mới không được nhỏ hơn hôm nay.");return false;}return true;}
 ChuyenDi Read(ChuyenDi? x=null)=>new(){MaChuyen=x?.MaChuyen??0,TenChuyen=ten.Text.Trim(),DiaDiem=diadiem.Text.Trim(),NgayDi=ngay.Value.Date,SoNguoi=(int)songuoi.Value,NganSach=ngansach.Value,PhuongTien=phuongtien.Text,DaHoanThanh=hoanthanh.Checked,GhiChu=string.IsNullOrWhiteSpace(ghichu.Text)?null:ghichu.Text.Trim()};
 void AddTrip(){if(!Valid(true))return;try{using var db=Db();var x=Read();db.ChuyenDi.Add(x);db.SaveChanges();Log.Information("Added trip {Id}",x.MaChuyen);LoadGrid();Reset();}catch(Exception ex){Log.Error(ex,"Add trip failed");MessageBox.Show("Không thể lưu chuyến đi.");}} void UpdateTrip(){if(id==0||!Valid(false))return;try{using var db=Db();var x=db.ChuyenDi.Find(id);if(x==null)return;var n=Read(x);db.Entry(x).CurrentValues.SetValues(n);db.SaveChanges();Log.Information("Updated trip {Id}",id);LoadGrid();}catch(Exception ex){Log.Error(ex,"Update trip failed");MessageBox.Show("Không thể sửa chuyến đi.");}} void DeleteTrip(){if(id==0)return;if(MessageBox.Show(en?"Delete this trip?":"Bạn có chắc muốn xóa chuyến đi này?","Xác nhận",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{using var db=Db();var x=db.ChuyenDi.Find(id);if(x!=null)db.Remove(x);db.SaveChanges();Log.Information("Deleted trip {Id}",id);LoadGrid();Reset();}catch(Exception ex){Log.Error(ex,"Delete trip failed");MessageBox.Show("Không thể xóa chuyến đi.");}} void Reset(){id=0;ten.Clear();diadiem.Clear();ghichu.Clear();ngay.Value=DateTime.Today;songuoi.Value=2;ngansach.Value=0;phuongtien.SelectedIndex=0;hoanthanh.Checked=false;grid.ClearSelection();}
 async Task ShowWeather()
 {
     if (grid.CurrentRow?.DataBoundItem is not ChuyenDi x)
     {
         MessageBox.Show(en ? "Select a trip first." : "Hãy chọn chuyến đi trước.");
         return;
     }

     viewWeather.Enabled = false;
     progress.Visible = true;
     weather.Text = en ? "Loading forecast..." : "Đang tải dự báo...";
     try
     {
         var service = new WeatherService(cfg.GeocodingUrl, cfg.ForecastUrl);
         var w = await service.GetAsync(x.DiaDiem, x.NgayDi);
         if (w == null)
         {
             weather.Text = en
                 ? "No forecast data (forecast is available for up to 16 days). Try an unaccented place name such as Da Lat."
                 : "Chưa có dữ liệu dự báo (API hỗ trợ tối đa 16 ngày). Thử tên không dấu như Da Lat.";
             return;
         }

         string desc = WeatherText(w.Code, en);
         string rainLabel = en ? "Rain chance" : "Xác suất mưa";
         string advice = "";
         if (w.Rain >= 60)
         {
             advice = en
                 ? ". Bring a raincoat or consider rescheduling."
                 : ". Nên mang áo mưa hoặc cân nhắc dời lịch.";
         }
         string icon = w.Rain >= 60 ? "⚠️ " : "🌤 ";
         string dateText = x.NgayDi.ToString("dd/MM/yyyy");
         weather.Text = icon + x.DiaDiem + " - " + dateText + ": " + desc
             + "; " + w.Min.ToString("0") + "–" + w.Max.ToString("0") + " °C; "
             + rainLabel + ": " + w.Rain + "%" + advice;
         weather.BackColor = w.Rain >= 60 ? System.Drawing.Color.Orange : System.Drawing.SystemColors.Control;
     }
     catch (HttpRequestException ex)
     {
         Log.Warning(ex, "Weather API network error");
         weather.Text = en ? "Network error. Please check your connection." : "Lỗi mạng. Vui lòng kiểm tra kết nối.";
     }
     catch (TaskCanceledException ex)
     {
         Log.Warning(ex, "Weather API timeout");
         weather.Text = en ? "Weather request timed out." : "API thời tiết phản hồi quá lâu.";
     }
     catch (Exception ex)
     {
         Log.Error(ex, "Weather request failed");
         weather.Text = en ? "Could not load weather." : "Không tải được thời tiết.";
     }
     finally
     {
         progress.Visible = false;
         viewWeather.Enabled = true;
     }
 }
 static string WeatherText(int c,bool en)=>c==0?(en?"Clear":"Trời quang"):c<=3?(en?"Cloudy":"Có mây"):c==45||c==48?(en?"Fog":"Sương mù"):((c>=51&&c<=67)||(c>=80&&c<=82))?(en?"Rain":"Mưa"):c>=95&&c<=99?(en?"Thunderstorm":"Dông"):(en?"Unknown":"Không rõ");
}
