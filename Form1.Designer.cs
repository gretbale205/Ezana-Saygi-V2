using System.Drawing;
using System.Windows.Forms;

namespace NamazVaktiApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label LblSaat;
        private System.Windows.Forms.Label LblKalan;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        // Modern Button Style
        void StyleButton(Button btn, Color backColor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Height = 45;
        }
        // ComboBox Style
        void StyleCombo(ComboBox cmb)
        {
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.Font = new Font("Segoe UI", 10F);
            cmb.Height = 30;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LblSaat = new System.Windows.Forms.Label();
            this.LblKalan = new System.Windows.Forms.Label();
            this.txt_hata = new System.Windows.Forms.TextBox();
            this.id_kaydet = new System.Windows.Forms.Button();
            this.LblSes = new System.Windows.Forms.Label();
            this.LblTarih = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmbUlke = new System.Windows.Forms.ComboBox();
            this.cmbSehir = new System.Windows.Forms.ComboBox();
            this.cmbIlce = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnVakitleriGetir = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.xmlvakitler = new System.Windows.Forms.TextBox();
            this.gvakitLBL = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblSaat
            // 
            this.LblSaat.AutoSize = true;
            this.LblSaat.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.LblSaat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.LblSaat.Location = new System.Drawing.Point(40, 94);
            this.LblSaat.Name = "LblSaat";
            this.LblSaat.Size = new System.Drawing.Size(174, 51);
            this.LblSaat.TabIndex = 0;
            this.LblSaat.Text = "07:25:15";
            // 
            // LblKalan
            // 
            this.LblKalan.AutoSize = true;
            this.LblKalan.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.LblKalan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.LblKalan.Location = new System.Drawing.Point(36, 257);
            this.LblKalan.Name = "LblKalan";
            this.LblKalan.Size = new System.Drawing.Size(259, 21);
            this.LblKalan.TabIndex = 1;
            this.LblKalan.Text = "Öğle vaktine kalan süre: 05:25:44";
            // 
            // txt_hata
            // 
            this.txt_hata.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_hata.Location = new System.Drawing.Point(13, 380);
            this.txt_hata.Multiline = true;
            this.txt_hata.Name = "txt_hata";
            this.txt_hata.Size = new System.Drawing.Size(447, 79);
            this.txt_hata.TabIndex = 2;
            this.txt_hata.Tag = "Hatalar Bu Kısım da Görünecek";
            // 
            // id_kaydet
            // 
            this.id_kaydet.Location = new System.Drawing.Point(40, 30);
            this.id_kaydet.Name = "id_kaydet";
            this.id_kaydet.Size = new System.Drawing.Size(180, 45);
            this.id_kaydet.TabIndex = 3;
            this.id_kaydet.Text = "Güncel Vakitleri Al";
            this.id_kaydet.Click += new System.EventHandler(this.id_kaydet_Click);
            // 
            // LblSes
            // 
            this.LblSes.AutoSize = true;
            this.LblSes.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.LblSes.ForeColor = System.Drawing.Color.DimGray;
            this.LblSes.Location = new System.Drawing.Point(36, 170);
            this.LblSes.Name = "LblSes";
            this.LblSes.Size = new System.Drawing.Size(128, 20);
            this.LblSes.TabIndex = 4;
            this.LblSes.Text = "Ses aktif durumda";
            // 
            // LblTarih
            // 
            this.LblTarih.AutoSize = true;
            this.LblTarih.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.LblTarih.ForeColor = System.Drawing.Color.DimGray;
            this.LblTarih.Location = new System.Drawing.Point(36, 218);
            this.LblTarih.Name = "LblTarih";
            this.LblTarih.Size = new System.Drawing.Size(117, 20);
            this.LblTarih.TabIndex = 5;
            this.LblTarih.Text = "Tarih: 24.02.2026";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Ezanı Muhammediye Saygı";
            this.notifyIcon1.Visible = true;
            // 
            // cmbUlke
            // 
            this.cmbUlke.Location = new System.Drawing.Point(260, 115);
            this.cmbUlke.Name = "cmbUlke";
            this.cmbUlke.Size = new System.Drawing.Size(200, 25);
            this.cmbUlke.TabIndex = 6;
            // 
            // cmbSehir
            // 
            this.cmbSehir.Location = new System.Drawing.Point(260, 167);
            this.cmbSehir.Name = "cmbSehir";
            this.cmbSehir.Size = new System.Drawing.Size(200, 25);
            this.cmbSehir.TabIndex = 7;
            // 
            // cmbIlce
            // 
            this.cmbIlce.Location = new System.Drawing.Point(260, 223);
            this.cmbIlce.Name = "cmbIlce";
            this.cmbIlce.Size = new System.Drawing.Size(200, 25);
            this.cmbIlce.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(260, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Ülke";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(260, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "Şehir";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(260, 203);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "İlçe";
            // 
            // btnVakitleriGetir
            // 
            this.btnVakitleriGetir.Location = new System.Drawing.Point(260, 30);
            this.btnVakitleriGetir.Name = "btnVakitleriGetir";
            this.btnVakitleriGetir.Size = new System.Drawing.Size(200, 45);
            this.btnVakitleriGetir.TabIndex = 12;
            this.btnVakitleriGetir.Text = "Seçime Göre Al";
            this.btnVakitleriGetir.Click += new System.EventHandler(this.btnVakitleriGetir_Click);
            // 
            // xmlvakitler
            // 
            this.xmlvakitler.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xmlvakitler.Location = new System.Drawing.Point(316, 287);
            this.xmlvakitler.Name = "xmlvakitler";
            this.xmlvakitler.Size = new System.Drawing.Size(144, 25);
            this.xmlvakitler.TabIndex = 13;
            this.xmlvakitler.Text = "https://ezanvakti.emushaf.net/";
            this.xmlvakitler.Visible = false;
            // 
            // gvakitLBL
            // 
            this.gvakitLBL.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gvakitLBL.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvakitLBL.Location = new System.Drawing.Point(36, 289);
            this.gvakitLBL.Name = "gvakitLBL";
            this.gvakitLBL.Size = new System.Drawing.Size(259, 88);
            this.gvakitLBL.TabIndex = 14;
            this.gvakitLBL.Text = "Günün Vakitleri";
            
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(481, 465);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LblSaat);
            this.Controls.Add(this.LblKalan);
            this.Controls.Add(this.txt_hata);
            this.Controls.Add(this.id_kaydet);
            this.Controls.Add(this.LblSes);
            this.Controls.Add(this.LblTarih);
            this.Controls.Add(this.cmbUlke);
            this.Controls.Add(this.cmbSehir);
            this.Controls.Add(this.cmbIlce);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnVakitleriGetir);
            this.Controls.Add(this.xmlvakitler);
            this.Controls.Add(this.gvakitLBL);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ezanı Muhammediye Saygı";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txt_hata;
        private System.Windows.Forms.Button id_kaydet;
        private System.Windows.Forms.Label LblSes;
        private System.Windows.Forms.Label LblTarih;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ComboBox cmbUlke;
        private System.Windows.Forms.ComboBox cmbSehir;
        private System.Windows.Forms.ComboBox cmbIlce;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnVakitleriGetir;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox xmlvakitler;
        private System.Windows.Forms.Label gvakitLBL;
        private Button button1;
    }
}
