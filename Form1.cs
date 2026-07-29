
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamazVaktiApp
{
    public partial class MainForm : Form
    {
        private DateTime nextPrayerTime = DateTime.MinValue;
        private string nextPrayerName = string.Empty;
        private DateTime soundReopenTime = DateTime.MinValue;
        private System.Windows.Forms.Timer CountdownTimer;
        private static readonly HttpClient httpClient = new HttpClient();

        private const string JsonUlkeUrl = "https://ezanvakti.emushaf.net/ulkeler";
        private const string JsonSehirUrl = "https://ezanvakti.emushaf.net/sehirler/";
        private const string JsonIlceUrl = "https://ezanvakti.emushaf.net/ilceler/";
        private const string JsonVakitlerUrl = "https://ezanvakti.emushaf.net/vakitler/";

        private NotifyIcon notifyIcon;
        private const string DosyaAdi = "vakitlink.txt";
        private const string AppFolderName = "EzanaSaygi";
        private static string ProgramDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppFolderName);
        private static string LinkDosyaYolu => Path.Combine(ProgramDataFolderPath, DosyaAdi);
        private static string OutputFilePathJson => Path.Combine(ProgramDataFolderPath, "vakitjson.json");

        public string JsonUrl;

        private const int DefaultUlkeID = 2;
        private const int DefaultSehirID = 541;
        private const int DefaultIlceID = 9577;

        private DateTime lastUpdateTime = DateTime.MinValue; // Son güncelleme zamanı
        private const int UpdateIntervalDays = 1; // 1 günde bir güncelleme


        private void InitializeNotifyIcon()
        {
            // NotifyIcon nesnesi oluşturuluyor
            notifyIcon = new NotifyIcon();

            // İkon belirliyoruz
            notifyIcon.Icon = new Icon("kabe1.ico");  // Burada 'icon.ico' yerine kendi simgenizin dosya yolunu kullanın
            notifyIcon.Visible = true;  // Simgeyi görünür yapıyoruz

            // Sağ tıklama menüsü ekliyoruz
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Gizle (Hide)", null, Hide_Click);
            contextMenu.Items.Add("Göster (Show)", null, Show_Click);
            contextMenu.Items.Add("Çık (Exit)", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;

            // Uygulama arka planda çalışırken sağ alt köşede simgeyi görüntüle
            notifyIcon.Text = "Uygulama Çalışıyor";  // Simge üzerine gelen açıklama
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            // Uygulamayı kapatma
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            // Uygulamayı gizliyor (Formu gizliyoruz)
            this.Hide();
            notifyIcon.Visible = true;  // NotifyIcon görünür oluyor
        }

        // Formu gösterme işlemi
        private void Show_Click(object sender, EventArgs e)
        {
            // Uygulamayı tekrar görünür hale getiriyoruz
            this.Show();
            this.WindowState = FormWindowState.Normal;  // Pencereyi normal boyutunda gösteriyoruz
            this.Activate();  // Pencereyi aktive ediyoruz
        }

        public MainForm()
        {
            InitializeComponent();
            EnsureProgramDataFolderExists(); // ProgramData klasörünü oluştur
            InitializeDefaultLinkFile(); // Varsayılan link dosyasını oluştur
            CalculateNextPrayerTime();
            InitializeCountdownTimer();
            LoadUlkelerAsync();
            InitializeNotifyIcon();
            CheckAutoStart(); // Uygulamanın otomatik başlatılmasını kontrol et
            LoadTodaysPrayerTimes();

            notifyIcon.Click += (sender, e) => this.Show();
        }

        // Otomatik başlatma için kayıt defteri ekleme
        private void CheckAutoStart()
        {
            // Uygulamanın otomatik olarak başlatılmasını sağlamak için kayıt defteri kontrolü
            string appName = "NotifyIconExample";
            string appPath = Application.ExecutablePath;

            // Kayıt defterinden uygulamanın otomatik olarak başlatılıp başlatılmadığını kontrol ediyoruz
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key.GetValue(appName) == null)
            {
                // Eğer yoksa, ekleyelim
                key.SetValue(appName, appPath);
            }
        }

          
        
        private void EnsureProgramDataFolderExists()
        {
            try
            {
                if (!Directory.Exists(ProgramDataFolderPath))
                {
                    Directory.CreateDirectory(ProgramDataFolderPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ProgramData klasörü oluşturulurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDefaultLinkFile()
        {
            try
            {
                if (!File.Exists(LinkDosyaYolu))
                {
                    // Varsayılan linki dosyaya yaz
                    File.WriteAllText(LinkDosyaYolu, "https://ezanvakti.emushaf.net/vakitler/9577");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Varsayılan link dosyası oluşturulurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadUlkelerAsync()
        {
            var ulkeler = await GetJsonDataAsync<List<Ulke>>(JsonUlkeUrl);
            cmbUlke.DataSource = ulkeler;
            cmbUlke.DisplayMember = "UlkeAdi";
            cmbUlke.ValueMember = "UlkeID";
        }

        private async void cmbUlke_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbUlke.SelectedValue != null && int.TryParse(cmbUlke.SelectedValue.ToString(), out int ulkeID))
            {
                await LoadSehirData(ulkeID);
            }
        }

        private async void cmbSehir_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbSehir.SelectedValue != null && int.TryParse(cmbSehir.SelectedValue.ToString(), out int sehirID))
            {
                await LoadIlceData(sehirID);
            }
        }

        private async void cmbIlce_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbIlce.SelectedValue != null && int.TryParse(cmbIlce.SelectedValue.ToString(), out int ilceID))
            {
                JsonUrl = $"{JsonVakitlerUrl}{ilceID}";
                File.WriteAllText(LinkDosyaYolu, JsonUrl); // Link dosyasını güncelle
                await DownloadAndSaveDataAsync();
            }
        }

        private async Task<T> GetJsonDataAsync<T>(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private async Task DownloadAndSaveDataAsync(bool isManualUpdate = false)
        {
            try
            {
                // Manuel güncelleme yapılıyorsa, zaman kontrolünü atla
                if (!isManualUpdate && (DateTime.Now - lastUpdateTime).TotalDays < UpdateIntervalDays)
                {
                    MessageBox.Show($"Veriler en son {lastUpdateTime:dd.MM.yyyy} tarihinde güncellenmiştir. 29 günde bir güncelleme yapılır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var jsonResponse = await httpClient.GetStringAsync(JsonUrl);
                var formattedJson = JValue.Parse(jsonResponse).ToString(Formatting.Indented);
                File.WriteAllText(OutputFilePathJson, formattedJson);
                lastUpdateTime = DateTime.Now; // Güncelleme zamanını kaydet
                CalculateNextPrayerTime();
                MessageBox.Show($"Veri başarıyla indirildi ve kaydedildi. Uygulama yeniden başlatıcak. Son güncelleme: {lastUpdateTime:dd.MM.yyyy}", "Başarı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Application.Restart();
                Environment.Exit(0);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri indirme veya kaydetme sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CalculateNextPrayerTime()
        {
            string jsonFilePath = OutputFilePathJson;
            if (!File.Exists(jsonFilePath)) return;

            var jsonText = File.ReadAllText(jsonFilePath);
            var jsonArray = JArray.Parse(jsonText);

            var todayDate = DateTime.Now;
            DateTime closestTime = DateTime.MaxValue;
            string closestPrayerName = string.Empty;

            foreach (var prayerTime in jsonArray)
            {
                if (DateTime.TryParseExact(prayerTime["MiladiTarihKisa"]?.ToString(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime prayerDate) && prayerDate.Date == todayDate.Date)
                {
                    var timeDict = new Dictionary<string, string>
                    {
                        { "Imsak", "İmsak" },
                        { "Gunes", "Güneş" },
                        { "Ogle", "Öğle" },
                        { "Ikindi", "İkindi" },
                        { "Aksam", "Akşam" },
                        { "Yatsi", "Yatsı" }
                    };

                    foreach (var timePair in timeDict)
                    {
                        if (DateTime.TryParseExact(prayerTime[timePair.Key]?.ToString(), "H:mm", null, System.Globalization.DateTimeStyles.None, out DateTime prayerTimeDateTime))
                        {
                            var prayerDateTime = new DateTime(prayerDate.Year, prayerDate.Month, prayerDate.Day, prayerTimeDateTime.Hour, prayerTimeDateTime.Minute, 0);
                            if (prayerDateTime > todayDate && prayerDateTime < closestTime)
                            {
                                closestTime = prayerDateTime;
                                closestPrayerName = timePair.Value;
                            }
                        }
                    }
                }
            }

            nextPrayerTime = closestTime;
            nextPrayerName = closestPrayerName;

            // Eğer en yakın vakit bulunamazsa, bir sonraki günün ilk vaktini ayarla
            if (nextPrayerTime == DateTime.MaxValue)
            {
                nextPrayerTime = todayDate.AddDays(1).Date + new TimeSpan(5, 0, 0); // Örneğin, ertesi günün imsak vakti
                nextPrayerName = "İmsak";
            }
        }

        private void InitializeCountdownTimer()
        {
            CountdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            CountdownTimer.Tick += CountdownTimer_Tick;
            CountdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (nextPrayerTime != DateTime.MinValue)
            {
                var timeLeft = nextPrayerTime - DateTime.Now;
                if (timeLeft.TotalSeconds <= 0)
                {
                    CalculateNextPrayerTime();
                }
                else
                {
                    LblKalan.Text = $"{nextPrayerName} vaktine kalan süre: {timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
                    LblSaat.Text = DateTime.Now.ToString("HH:mm:ss");
                    LblTarih.Text = $"Güncel Tarih: {nextPrayerTime:dd.MM.yyyy}";

                    // Ses durumunu kontrol et
                    if (timeLeft.TotalSeconds <= 20 && timeLeft.TotalSeconds > 10 && soundReopenTime == DateTime.MinValue)
                    {
                        ExecuteNircmdCommand("mutesysvolume", "1"); // Sesi kapat
                        soundReopenTime = DateTime.Now.AddMinutes(7); // 7 dakika sonra sesi aç
                    }

                    // Sesin açılmasına kalan süreyi göster
                    if (soundReopenTime != DateTime.MinValue && soundReopenTime > DateTime.Now)
                    {
                        TimeSpan soundTimeLeft = soundReopenTime - DateTime.Now;
                        LblSes.Text = $"Sesin açılmasına kalan süre: {soundTimeLeft.Minutes:D2}:{soundTimeLeft.Seconds:D2}";
                    }
                    else
                    {
                        ExecuteNircmdCommand("mutesysvolume", "0"); // Sesi aç
                        LblSes.Text = "Ses aktif durumda.";
                        soundReopenTime = DateTime.MinValue; // Ses açıldıktan sonra zamanı sıfırla
                    }
                }
            }
        }

        private void ExecuteNircmdCommand(string command, string args)
        {
            Process.Start("nircmd.exe", $"{command} {args}");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            Application.Exit();
        }

        private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private async Task LoadUlkeData(int ulkeID)
        {
            var ulkeler = await GetJsonDataAsync<List<Ulke>>(JsonUlkeUrl);
            cmbUlke.DataSource = ulkeler;
            cmbUlke.DisplayMember = "UlkeAdi";
            cmbUlke.ValueMember = "UlkeID";
            cmbUlke.SelectedValue = ulkeID;
        }

        private async Task LoadSehirData(int ulkeID)
        {
            var sehirler = await GetJsonDataAsync<List<Sehir>>($"{JsonSehirUrl}{ulkeID}");
            cmbSehir.DataSource = sehirler;
            cmbSehir.DisplayMember = "SehirAdi";
            cmbSehir.ValueMember = "SehirID";
            cmbSehir.SelectedValue = DefaultSehirID;
        }

        private async Task LoadIlceData(int sehirID)
        {
            var ilceler = await GetJsonDataAsync<List<Ilce>>($"{JsonIlceUrl}{sehirID}");
            cmbIlce.DataSource = ilceler;
            cmbIlce.DisplayMember = "IlceAdi";
            cmbIlce.ValueMember = "IlceID";
            cmbIlce.SelectedValue = DefaultIlceID;
        }

        private async void id_kaydet_Click(object sender, EventArgs e)
        {
            await DownloadAndSaveDataAsync(true); // Manuel güncelleme
        }

        private async void btnVakitleriGetir_Click(object sender, EventArgs e)
        {
            if (cmbIlce.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir ilçe seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ilceID = cmbIlce.SelectedValue.ToString();
            JsonUrl = $"{JsonVakitlerUrl}{ilceID}";
            File.WriteAllText(LinkDosyaYolu, JsonUrl); // Link dosyasını güncelle
            await DownloadAndSaveDataAsync(true); // Manuel güncelleme

            Application.Restart();
            Environment.Exit(0);
        }

        private void LoadJsonUrlFromFile()
        {
            try
            {
                if (File.Exists(LinkDosyaYolu))
                {
                    JsonUrl = File.ReadAllText(LinkDosyaYolu);
                }
                else
                {
                    txt_hata.Text += "LoadJsonUrl Dosya bulunamadı: " + LinkDosyaYolu;
                }
            }
            catch (Exception ex)
            {
                txt_hata.Text += "LoadJsonUrl Bir hata oluştu: " + ex.Message;
            }
        }

        private void TimerAyarla()
        {
            // 29 gün sonra tetiklenecek şekilde ayarla
            var dueTime = TimeSpan.FromDays(UpdateIntervalDays);
            var timer = new System.Threading.Timer(async _ => await DownloadAndSaveDataAsync(), null, dueTime, Timeout.InfiniteTimeSpan);
        }


        private void LoadTodaysPrayerTimes()
        {
            try
            {
                // JSON dosyasını oku
                if (!File.Exists(OutputFilePathJson))
                {
                    MessageBox.Show("Vakit verileri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string jsonText = File.ReadAllText(OutputFilePathJson);
                JArray prayerTimesArray = JArray.Parse(jsonText);

                // Bugünün tarihini "dd.MM.yyyy" formatında al
                string todayDate = DateTime.Now.ToString("dd.MM.yyyy");

                // Bugüne ait vakitleri bul
                var todaysPrayerTimes = prayerTimesArray
                    .FirstOrDefault(p => p["MiladiTarihKisa"]?.ToString() == todayDate);

                if (todaysPrayerTimes == null)
                {
                    MessageBox.Show("Bugüne ait vakit bilgisi bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Vakitleri formatla ve Label'a yazdır
                gvakitLBL.Text = $@"Günün Vakitleri.
            İmsak: {todaysPrayerTimes["Imsak"]}      Güneş: {todaysPrayerTimes["Gunes"]}
            Öğle:  {todaysPrayerTimes["Ogle"]}       İkindi:{todaysPrayerTimes["Ikindi"]}
            Akşam: {todaysPrayerTimes["Aksam"]}      Yatsı: {todaysPrayerTimes["Yatsi"]}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Vakitler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }


        public class Ulke
        {
            public string UlkeAdi { get; set; }
            public string UlkeID { get; set; }
        }

        public class Sehir
        {
            public string SehirAdi { get; set; }
            public string SehirID { get; set; }
        }

        public class Ilce
        {
            public string IlceAdi { get; set; }
            public string IlceID { get; set; }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            LoadUlkeData(DefaultUlkeID);               // Ülkeleri yükle
            LoadSehirData(DefaultUlkeID);              // Doğru parametre: Ülke ID
            LoadIlceData(DefaultSehirID);              // Doğru parametre: Şehir ID
            LoadJsonUrlFromFile();
            TimerAyarla();
            this.Hide();
            this.WindowState = FormWindowState.Minimized;


            SetPlaceholder(txt_hata, "Hata mesajı burada görünecek... ");

            // 💾 Kaydedilmiş ayarı oku
            ThemeManager.IsDark = Ezanı_Muhammediye_Saygı.Properties.Settings.Default.DarkMode;

            // 🎨 Temayı uygula
            ThemeManager.ApplyTheme(this);
        }

        private void MainForm_Resize_1(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }



        public static class ThemeManager
        {
            public static bool IsDark = false;

            public static void ApplyTheme(Form form)
            {
                // DARK
                Color bgMainDark = Color.FromArgb(28, 32, 38);
                Color bgSurfaceDark = Color.FromArgb(36, 41, 48);
                Color buttonDark = Color.FromArgb(58, 123, 213);
                Color textPrimaryDark = Color.FromArgb(230, 230, 230);
                Color textSecondaryDark = Color.FromArgb(170, 170, 170);
                
                // LIGHT (Yumuşatılmış)
                Color bgMainLight = Color.FromArgb(248, 249, 252);
                Color bgSurfaceLight = Color.White;
                Color buttonLight = Color.FromArgb(60, 90, 150);
                Color textPrimaryLight = Color.FromArgb(40, 40, 40);
                Color textSecondaryLight = Color.FromArgb(90, 90, 90);

                Color bgMain = IsDark ? bgMainDark : bgMainLight;
                Color bgSurface = IsDark ? bgSurfaceDark : bgSurfaceLight;
                Color buttonColor = IsDark ? buttonDark : buttonLight;
                Color textPrimary = IsDark ? textPrimaryDark : textPrimaryLight;
                Color textSecondary = IsDark ? textSecondaryDark : textSecondaryLight;

                form.BackColor = bgMain;

                foreach (Control ctrl in form.Controls)
                {
                    ApplyControlTheme(ctrl, bgMain, bgSurface, buttonColor, textPrimary, textSecondary);
                }

                // 🔥 button1 yazısını değiştir
                Button themeButton = form.Controls["button1"] as Button;
                if (themeButton != null)
                {
                    themeButton.Text = IsDark ? "Açık Mod" : "Koyu Mod";
                }
            }

            private static void ApplyControlTheme(Control ctrl,
                Color bgMain,
                Color bgSurface,
                Color buttonColor,
                Color textPrimary,
                Color textSecondary)
            {
                if (ctrl is Label)
                {
                    ctrl.ForeColor = textSecondary;
                    ctrl.BackColor = Color.Transparent;
                }
                else if (ctrl is Button btn)
                {
                    btn.BackColor = buttonColor;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
                else if (ctrl is TextBox || ctrl is ComboBox)
                {
                    ctrl.BackColor = bgSurface;
                    ctrl.ForeColor = textPrimary;
                }
                else
                {
                    ctrl.BackColor = bgMain;
                    ctrl.ForeColor = textPrimary;
                }

                foreach (Control child in ctrl.Controls)
                {
                    ApplyControlTheme(child, bgMain, bgSurface, buttonColor, textPrimary, textSecondary);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ThemeManager.IsDark = !ThemeManager.IsDark;

            Ezanı_Muhammediye_Saygı.Properties.Settings.Default.DarkMode = ThemeManager.IsDark;
            Ezanı_Muhammediye_Saygı.Properties.Settings.Default.Save();

            ThemeManager.ApplyTheme(this);
        }
    }
}