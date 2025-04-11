using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using rotaSİstemi.rotaSİstemi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rotaSİstemi
{
    public partial class Form1 : Form
    {
        private RotaHesaplayici rotaHesaplayici;
        private Label lblAd, lblBasEnlem, lblBasBoylam, lblBitEnlem, lblBitBoylam, lblSonuc, lblYolcuTipi, lblOdemeTipi;
        private TextBox textBox1, textBox2, textBox3, textBox4, textBox5;
        private RichTextBox richTextBoxSonuc;
        private ComboBox comboBoxYolcuTipi, comboBoxOdemeTipi;
        private GMapControl gMapControl;
        private List<(string Aciklama, List<RotaAdimi> Rota, double Maliyet, int Sure, double Mesafe)> alternatifRotalar;
        private RotaHesaplayici.Konum baslangic;
        private RotaHesaplayici.Konum bitis;
        private Button btnMoveMarkers; // New button for moving markers
        private bool isMovingMarkers = false; // Flag to track moving mode
        private GMarkerGoogle startMarker, endMarker; // Store markers for dragging

        public Form1()
        {
            InitializeComponent();
            rotaHesaplayici = new RotaHesaplayici();
            FormuTasarimla();
        }

        private void FormuTasarimla()
        {
            this.Text = "Rota Planlama Sistemi";
            this.Size = new Size(1200, 1000);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            gMapControl = new GMapControl
            {
                Location = new Point(550, 20),
                Size = new Size(430, 350),
                MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance,
                MinZoom = 1,
                MaxZoom = 20,
                Zoom = 13,
                DragButton = MouseButtons.Left,
                Manager = { Mode = AccessMode.ServerAndCache },
                BorderStyle = BorderStyle.FixedSingle
            };
            gMapControl.Position = new PointLatLng(40.78259, 29.94628);
            gMapControl.MouseDoubleClick += GMapControl_MouseDoubleClick;
            gMapControl.OnMarkerClick += GMapControl_OnMarkerClick;

            // New button for moving markers
            btnMoveMarkers = new Button
            {
                Text = "Konum Çubuklarını Hareket Ettir",
                Location = new Point(130, 350),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(100, 149, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMoveMarkers.Click += BtnMoveMarkers_Click;

            // Legend Panel
            Panel legendPanel = new Panel
            {
                Location = new Point(550, 380),
                Size = new Size(430, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 252),
                Padding = new Padding(10),
                AutoScroll = true,
            };

            Label legendTitle = new Label
            {
                Text = "Rota Türleri",
                Location = new Point(10, 10),
                Size = new Size(100, 25),
                ForeColor = Color.FromArgb(50, 50, 50),
            };

            Button btnTaksi = new Button
            {
                Text = "🚖 Taksi Rotasını Göster",
                Location = new Point(10, 40),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(255, 245, 157),
                FlatStyle = FlatStyle.Flat,
                Tag = "Taksi"
            };
            btnTaksi.Click += LegendButton_Click;

            Button btnOtobus = new Button
            {
                Text = "🚍 Otobüs Rotasını Göster",
                Location = new Point(10, 80),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(173, 216, 230),
                FlatStyle = FlatStyle.Flat,
                Tag = "Otobüs"
            };
            btnOtobus.Click += LegendButton_Click;

            Button btnTramvay = new Button
            {
                Text = "🚋 Tramvay Rotasını Göster",
                Location = new Point(10, 120),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(144, 238, 144),
                FlatStyle = FlatStyle.Flat,
                Tag = "Tramvay"
            };
            btnTramvay.Click += LegendButton_Click;

            Button btnHibrit = new Button
            {
                Text = "🔄 Tramvay-Otobüs Rotasını Göster",
                Location = new Point(10, 160),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(255, 218, 185),
                FlatStyle = FlatStyle.Flat,
                Tag = "Tramvay-Otobüs"
            };
            btnHibrit.Click += LegendButton_Click;

            legendPanel.Controls.AddRange(new Control[] { legendTitle, btnTaksi, btnOtobus, btnTramvay, btnHibrit });

            lblAd = new Label { Text = "Yolcu Adı:", Location = new Point(20, 20), Size = new Size(100, 25), };
            textBox1 = new TextBox { Location = new Point(130, 20), Size = new Size(250, 25), BorderStyle = BorderStyle.FixedSingle };

            lblBasEnlem = new Label { Text = "Başlangıç Enlem:", Location = new Point(20, 60), Size = new Size(100, 25) };
            textBox2 = new TextBox { Location = new Point(130, 60), Size = new Size(250, 25), Text = "40,766797", BorderStyle = BorderStyle.FixedSingle };

            lblBasBoylam = new Label { Text = "Başlangıç Boylam:", Location = new Point(20, 100), Size = new Size(100, 25), };
            textBox3 = new TextBox { Location = new Point(130, 100), Size = new Size(250, 25), Text = "29,870412", BorderStyle = BorderStyle.FixedSingle };

            lblBitEnlem = new Label { Text = "Bitiş Enlem:", Location = new Point(20, 140), Size = new Size(100, 25), };
            textBox4 = new TextBox { Location = new Point(130, 140), Size = new Size(250, 25), Text = "40,76543", BorderStyle = BorderStyle.FixedSingle };

            lblBitBoylam = new Label { Text = "Bitiş Boylam:", Location = new Point(20, 180), Size = new Size(100, 25), };
            textBox5 = new TextBox { Location = new Point(130, 180), Size = new Size(250, 25), Text = "29,96965", BorderStyle = BorderStyle.FixedSingle };

            lblYolcuTipi = new Label { Text = "Yolcu Tipi:", Location = new Point(20, 220), Size = new Size(100, 25) };
            comboBoxYolcuTipi = new ComboBox
            {
                Location = new Point(130, 220),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Öğrenci", "Yaşlı", "Genel" },
                SelectedIndex = 0,
                BackColor = Color.White
            };

            lblOdemeTipi = new Label { Text = "Ödeme Tipi:", Location = new Point(20, 260), Size = new Size(100, 25), };
            comboBoxOdemeTipi = new ComboBox
            {
                Location = new Point(130, 260),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Nakit", "Kredi Kartı", "Kentkart" },
                SelectedIndex = 0,
                BackColor = Color.White
            };

            button1 = new Button
            {
                Text = "Rota Hesapla ve Karşılaştır",
                Location = new Point(130, 300),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            button1.Click += button1_Click;

            lblSonuc = new Label
            {
                Text = "Rota Karşılaştırma ve Planlama Sonuçları:",
                Location = new Point(20, 560),
                Size = new Size(400, 25),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            richTextBoxSonuc = new RichTextBox
            {
                Location = new Point(20, 600),
                Size = new Size(960, 360),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.FromArgb(255, 255, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            this.Controls.AddRange(new Control[]
            {
                gMapControl, lblAd, textBox1, lblBasEnlem, textBox2, lblBasBoylam, textBox3,
                lblBitEnlem, textBox4, lblBitBoylam, textBox5, lblYolcuTipi, comboBoxYolcuTipi,
                lblOdemeTipi, comboBoxOdemeTipi, button1, btnMoveMarkers, lblSonuc, richTextBoxSonuc, legendPanel
            });
        }

        private void GMapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !isMovingMarkers)
            {
                var point = gMapControl.FromLocalToLatLng(e.X, e.Y);
                if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    textBox2.Text = point.Lat.ToString();
                    textBox3.Text = point.Lng.ToString();
                    gMapControl.Overlays.Clear();
                    startMarker = new GMarkerGoogle(new PointLatLng(point.Lat, point.Lng), GMarkerGoogleType.green);
                    gMapControl.Overlays.Add(new GMapOverlay("start") { Markers = { startMarker } });
                }
                else if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
                {
                    textBox4.Text = point.Lat.ToString();
                    textBox5.Text = point.Lng.ToString();
                    endMarker = new GMarkerGoogle(new PointLatLng(point.Lat, point.Lng), GMarkerGoogleType.red);
                    gMapControl.Overlays.Add(new GMapOverlay("end") { Markers = { endMarker } });
                }
                gMapControl.Refresh();
            }
        }

        private void BtnMoveMarkers_Click(object sender, EventArgs e)
        {
            isMovingMarkers = !isMovingMarkers;
            btnMoveMarkers.Text = isMovingMarkers ? "Hareket Modunu Kapat" : "Konum Çubuklarını Hareket Ettir";
            btnMoveMarkers.BackColor = isMovingMarkers ? Color.FromArgb(220, 20, 60) : Color.FromArgb(100, 149, 237);

            if (isMovingMarkers)
            {

                gMapControl.MouseMove += GMapControl_MouseMove;
                gMapControl.MouseUp += GMapControl_MouseUp;
            }
            else
            {

                gMapControl.MouseMove -= GMapControl_MouseMove;
                gMapControl.MouseUp -= GMapControl_MouseUp;
            }
        }

        private void GMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingMarkers && e.Button == MouseButtons.Left)
            {
                var point = gMapControl.FromLocalToLatLng(e.X, e.Y);
                if (startMarker != null && startMarker.IsMouseOver)
                {
                    startMarker.Position = point;
                    textBox2.Text = point.Lat.ToString();
                    textBox3.Text = point.Lng.ToString();
                    gMapControl.Refresh();
                }
                else if (endMarker != null && endMarker.IsMouseOver)
                {
                    endMarker.Position = point;
                    textBox4.Text = point.Lat.ToString();
                    textBox5.Text = point.Lng.ToString();
                    gMapControl.Refresh();
                }
            }
        }

        private void GMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMovingMarkers)
            {
                gMapControl.Refresh();
            }
        }

        private void LegendButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null || alternatifRotalar == null || !alternatifRotalar.Any()) return;

            string routeType = clickedButton.Tag.ToString();
            var selectedRoute = alternatifRotalar.FirstOrDefault(r => r.Aciklama.Contains(routeType));
            if (selectedRoute.Rota == null || !selectedRoute.Rota.Any()) return;

            gMapControl.Overlays.Clear();
            var markersOverlay = new GMapOverlay("points");
            var routesOverlay = new GMapOverlay("routes");
            var stopsOverlay = new GMapOverlay("stops");

            startMarker = new GMarkerGoogle(new PointLatLng(baslangic.Enlem, baslangic.Boylam), GMarkerGoogleType.green)
            {
                ToolTipText = "Başlangıç Noktası",
                ToolTipMode = MarkerTooltipMode.OnMouseOver
            };
            endMarker = new GMarkerGoogle(new PointLatLng(bitis.Enlem, bitis.Boylam), GMarkerGoogleType.red)
            {
                ToolTipText = "Bitiş Noktası",
                ToolTipMode = MarkerTooltipMode.OnMouseOver
            };
            markersOverlay.Markers.Add(startMarker);
            markersOverlay.Markers.Add(endMarker);

            var duraklar = rotaHesaplayici.GetType().GetField("duraklar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(rotaHesaplayici) as List<Durak> ?? new List<Durak>();
            foreach (var durak in duraklar)
            {
                var stopMarker = durak.Tur switch
                {
                    "tram" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.blue_dot),
                    "bus" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.gray_small),
                    _ => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.black_small)
                };
                stopMarker.ToolTipText = $"{(durak.Tur == "tram" ? "Tramvay" : "Otobüs")} Durağı: {durak.Ad}";
                stopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                stopsOverlay.Markers.Add(stopMarker);
            }

            List<PointLatLng> routePoints = new List<PointLatLng> { new PointLatLng(baslangic.Enlem, baslangic.Boylam) };
            int adimSayisi = 0;

            foreach (var adim in selectedRoute.Rota)
            {
                adimSayisi++;
                Color routeColor = adim.KullanilanArac.AracTuru switch
                {
                    "Taksi" => Color.Yellow,
                    "Otobüs" => Color.Blue,
                    "Tramvay" => Color.Green,
                    "Yaya" => Color.Gray,
                    _ => Color.Black
                };

                var baslangicDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BaslangicDurak);
                var bitisDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BitisDurak);

                PointLatLng baslangicPoint = baslangicDurak != null
                    ? new PointLatLng(baslangicDurak.Enlem, baslangicDurak.Boylam)
                    : routePoints.Last();

                PointLatLng bitisPoint = adimSayisi == selectedRoute.Rota.Count
                    ? new PointLatLng(bitis.Enlem, bitis.Boylam)
                    : (bitisDurak != null ? new PointLatLng(bitisDurak.Enlem, bitisDurak.Boylam) : baslangicPoint);

                routePoints.Add(bitisPoint);

                var segmentPoints = new List<PointLatLng> { baslangicPoint, bitisPoint };
                var route = new GMapRoute(segmentPoints, $"Step{adimSayisi}_{adim.KullanilanArac.AracTuru}")
                {
                    Stroke = new Pen(routeColor, 3)
                };
                routesOverlay.Routes.Add(route);

                if (adimSayisi < selectedRoute.Rota.Count)
                {
                    var transferMarker = new GMarkerGoogle(bitisPoint, GMarkerGoogleType.orange)
                    {
                        ToolTipText = $"Aktarma: {adim.BitisDurak}",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };
                    markersOverlay.Markers.Add(transferMarker);
                }
            }

            gMapControl.Overlays.Add(stopsOverlay);
            gMapControl.Overlays.Add(routesOverlay);
            gMapControl.Overlays.Add(markersOverlay);
            gMapControl.ZoomAndCenterMarkers(null);
            gMapControl.Refresh();

            richTextBoxSonuc.SelectAll();
            richTextBoxSonuc.SelectionBackColor = Color.WhiteSmoke;
            int startIndex = richTextBoxSonuc.Text.IndexOf($"【 {selectedRoute.Aciklama switch { "Sadece Taksi (Hızlı ama maliyetli)" => "🚖 Taksi ile Direkt (Hızlı, Konforlu ama Pahalı)", "Sadece Otobüs (Uygun maliyetli)" => "🚍 Otobüs ile Ekonomik (Ucuz ama Daha Yavaş)", "Sadece Tramvay (Rahat ve dengeli)" => "🚋 Tramvay ile Konforlu (Dengeli Fiyat ve Hız)", "Otobüs-Tramvay Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)", "Tramvay-Otobüs Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)", _ => selectedRoute.Aciklama }} 】");
            if (startIndex >= 0)
            {
                int endIndex = richTextBoxSonuc.Text.IndexOf("════", startIndex);
                richTextBoxSonuc.Select(startIndex, endIndex - startIndex);
                richTextBoxSonuc.SelectionBackColor = Color.LightYellow;
            }
        }

        private void GMapControl_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag == null || alternatifRotalar == null || !alternatifRotalar.Any()) return;

            string routeType = item.Tag.ToString();
            var selectedRoute = alternatifRotalar.FirstOrDefault(r => r.Aciklama.Contains(routeType));
            if (selectedRoute.Rota == null || !selectedRoute.Rota.Any()) return;

            gMapControl.Overlays.Clear();
            var markersOverlay = new GMapOverlay("points");
            var routesOverlay = new GMapOverlay("routes");
            var stopsOverlay = new GMapOverlay("stops");

            startMarker = new GMarkerGoogle(new PointLatLng(baslangic.Enlem, baslangic.Boylam), GMarkerGoogleType.green)
            {
                ToolTipText = "Başlangıç Noktası",
                ToolTipMode = MarkerTooltipMode.OnMouseOver
            };
            endMarker = new GMarkerGoogle(new PointLatLng(bitis.Enlem, bitis.Boylam), GMarkerGoogleType.red)
            {
                ToolTipText = "Bitiş Noktası",
                ToolTipMode = MarkerTooltipMode.OnMouseOver
            };
            markersOverlay.Markers.Add(startMarker);
            markersOverlay.Markers.Add(endMarker);

            var duraklar = rotaHesaplayici.GetType().GetField("duraklar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(rotaHesaplayici) as List<Durak> ?? new List<Durak>();
            foreach (var durak in duraklar)
            {
                var stopMarker = durak.Tur switch
                {
                    "tram" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.blue_dot),
                    "bus" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.gray_small),
                    _ => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.black_small)
                };
                stopMarker.ToolTipText = $"{(durak.Tur == "tram" ? "Tramvay" : "Otobüs")} Durağı: {durak.Ad}";
                stopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                stopsOverlay.Markers.Add(stopMarker);
            }

            List<PointLatLng> routePoints = new List<PointLatLng> { new PointLatLng(baslangic.Enlem, baslangic.Boylam) };
            int adimSayisi = 0;

            foreach (var adim in selectedRoute.Rota)
            {
                adimSayisi++;
                Color routeColor = adim.KullanilanArac.AracTuru switch
                {
                    "Taksi" => Color.Yellow,
                    "Otobüs" => Color.Blue,
                    "Tramvay" => Color.Green,
                    "Yaya" => Color.Gray,
                    _ => Color.Black
                };

                var baslangicDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BaslangicDurak);
                var bitisDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BitisDurak);

                PointLatLng baslangicPoint = baslangicDurak != null
                    ? new PointLatLng(baslangicDurak.Enlem, baslangicDurak.Boylam)
                    : routePoints.Last();

                PointLatLng bitisPoint = adimSayisi == selectedRoute.Rota.Count
                    ? new PointLatLng(bitis.Enlem, bitis.Boylam)
                    : (bitisDurak != null ? new PointLatLng(bitisDurak.Enlem, bitisDurak.Boylam) : baslangicPoint);

                routePoints.Add(bitisPoint);

                var segmentPoints = new List<PointLatLng> { baslangicPoint, bitisPoint };
                var route = new GMapRoute(segmentPoints, $"Step{adimSayisi}_{adim.KullanilanArac.AracTuru}")
                {
                    Stroke = new Pen(routeColor, 3)
                };
                routesOverlay.Routes.Add(route);

                if (adimSayisi < selectedRoute.Rota.Count)
                {
                    var transferMarker = new GMarkerGoogle(bitisPoint, GMarkerGoogleType.orange)
                    {
                        ToolTipText = $"Aktarma: {adim.BitisDurak}",
                        ToolTipMode = MarkerTooltipMode.OnMouseOver
                    };
                    markersOverlay.Markers.Add(transferMarker);
                }
            }

            var legendOverlay = new GMapOverlay("legend");
            legendOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(40.78259 + 0.04, 29.94628 + 0.06), GMarkerGoogleType.yellow) { Tag = "Taksi", ToolTipText = "Taksi Rotasını Göster", ToolTipMode = MarkerTooltipMode.Always });
            legendOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(40.78259 + 0.045, 29.94628 + 0.06), GMarkerGoogleType.blue) { Tag = "Otobüs", ToolTipText = "Otobüs Rotasını Göster", ToolTipMode = MarkerTooltipMode.Always });
            legendOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(40.78259 + 0.05, 29.94628 + 0.06), GMarkerGoogleType.green) { Tag = "Tramvay", ToolTipText = "Tramvay Rotasını Göster", ToolTipMode = MarkerTooltipMode.Always });
            legendOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(40.78259 + 0.055, 29.94628 + 0.06), GMarkerGoogleType.orange) { Tag = "Tramvay-Otobüs", ToolTipText = "Tramvay-Otobüs Aktarmalı Rotayı Göster", ToolTipMode = MarkerTooltipMode.Always });

            gMapControl.Overlays.Add(stopsOverlay);
            gMapControl.Overlays.Add(routesOverlay);
            gMapControl.Overlays.Add(markersOverlay);
            gMapControl.Overlays.Add(legendOverlay);
            gMapControl.ZoomAndCenterMarkers(null);
            gMapControl.Refresh();

            richTextBoxSonuc.SelectAll();
            richTextBoxSonuc.SelectionBackColor = Color.WhiteSmoke;
            int startIndex = richTextBoxSonuc.Text.IndexOf($"【 {selectedRoute.Aciklama switch { "Sadece Taksi (Hızlı ama maliyetli)" => "🚖 Taksi ile Direkt (Hızlı, Konforlu ama Pahalı)", "Sadece Otobüs (Uygun maliyetli)" => "🚍 Otobüs ile Ekonomik (Ucuz ama Daha Yavaş)", "Sadece Tramvay (Rahat ve dengeli)" => "🚋 Tramvay ile Konforlu (Dengeli Fiyat ve Hız)", "Otobüs-Tramvay Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)", "Tramvay-Otobüs Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)", _ => selectedRoute.Aciklama }} 】");
            if (startIndex >= 0)
            {
                int endIndex = richTextBoxSonuc.Text.IndexOf("════", startIndex);
                richTextBoxSonuc.Select(startIndex, endIndex - startIndex);
                richTextBoxSonuc.SelectionBackColor = Color.LightYellow;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Yolcu yolcu = comboBoxYolcuTipi.SelectedItem.ToString() switch
                {
                    "Öğrenci" => new Ogrenci(textBox1.Text),
                    "Yaşlı" => new Yasli(textBox1.Text),
                    "Genel" => new Genel(textBox1.Text),
                    _ => new Genel(textBox1.Text)
                };

                baslangic = new RotaHesaplayici.Konum
                {
                    Enlem = double.Parse(textBox2.Text),
                    Boylam = double.Parse(textBox3.Text)
                };
                bitis = new RotaHesaplayici.Konum
                {
                    Enlem = double.Parse(textBox4.Text),
                    Boylam = double.Parse(textBox5.Text)
                };

                alternatifRotalar = rotaHesaplayici.RotaPlanla(yolcu, baslangic, bitis);

                gMapControl.Overlays.Clear();
                var markersOverlay = new GMapOverlay("points");
                var routesOverlay = new GMapOverlay("routes");
                var stopsOverlay = new GMapOverlay("stops");

                startMarker = new GMarkerGoogle(new PointLatLng(baslangic.Enlem, baslangic.Boylam), GMarkerGoogleType.green)
                {
                    ToolTipText = "Başlangıç Noktası",
                    ToolTipMode = MarkerTooltipMode.OnMouseOver
                };
                endMarker = new GMarkerGoogle(new PointLatLng(bitis.Enlem, bitis.Boylam), GMarkerGoogleType.red)
                {
                    ToolTipText = "Bitiş Noktası",
                    ToolTipMode = MarkerTooltipMode.OnMouseOver
                };
                markersOverlay.Markers.Add(startMarker);
                markersOverlay.Markers.Add(endMarker);

                var duraklar = rotaHesaplayici.GetType().GetField("duraklar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(rotaHesaplayici) as List<Durak> ?? new List<Durak>();
                foreach (var durak in duraklar)
                {
                    var stopMarker = durak.Tur switch
                    {
                        "tram" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.blue_dot),
                        "bus" => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.gray_small),
                        _ => new GMarkerGoogle(new PointLatLng(durak.Enlem, durak.Boylam), GMarkerGoogleType.black_small)
                    };
                    stopMarker.ToolTipText = $"{(durak.Tur == "tram" ? "Tramvay" : "Otobüs")} Durağı: {durak.Ad}";
                    stopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    stopsOverlay.Markers.Add(stopMarker);
                }

                foreach (var (aciklama, rotaAdimlari, _, _, _) in alternatifRotalar)
                {
                    List<PointLatLng> routePoints = new List<PointLatLng> { new PointLatLng(baslangic.Enlem, baslangic.Boylam) };
                    int adimSayisi = 0;

                    foreach (var adim in rotaAdimlari)
                    {
                        adimSayisi++;
                        Color routeColor = adim.KullanilanArac.AracTuru switch
                        {
                            "Taksi" => Color.Yellow,
                            "Otobüs" => Color.Blue,
                            "Tramvay" => Color.Green,
                            "Yaya" => Color.Gray,
                            _ => Color.Black
                        };

                        var baslangicDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BaslangicDurak);
                        var bitisDurak = duraklar.FirstOrDefault(d => d.Ad == adim.BitisDurak);

                        PointLatLng baslangicPoint = baslangicDurak != null
                            ? new PointLatLng(baslangicDurak.Enlem, baslangicDurak.Boylam)
                            : routePoints.Last();

                        PointLatLng bitisPoint = adimSayisi == rotaAdimlari.Count
                            ? new PointLatLng(bitis.Enlem, bitis.Boylam)
                            : (bitisDurak != null ? new PointLatLng(bitisDurak.Enlem, bitisDurak.Boylam) : baslangicPoint);

                        routePoints.Add(bitisPoint);

                        var segmentPoints = new List<PointLatLng> { baslangicPoint, bitisPoint };
                        var route = new GMapRoute(segmentPoints, $"{aciklama}_Step{adimSayisi}")
                        {
                            Stroke = new Pen(routeColor, 3)
                        };
                        routesOverlay.Routes.Add(route);

                        if (adimSayisi < rotaAdimlari.Count)
                        {
                            var transferMarker = new GMarkerGoogle(bitisPoint, GMarkerGoogleType.orange)
                            {
                                ToolTipText = $"Aktarma: {adim.BitisDurak}",
                                ToolTipMode = MarkerTooltipMode.OnMouseOver
                            };
                            markersOverlay.Markers.Add(transferMarker);
                        }
                    }
                }

                gMapControl.Overlays.Add(stopsOverlay);
                gMapControl.Overlays.Add(routesOverlay);
                gMapControl.Overlays.Add(markersOverlay);
                gMapControl.ZoomAndCenterMarkers(null);

                StringBuilder sonuc = new StringBuilder();
                sonuc.AppendLine("╔════════════════════════════════════════════════════════════════════╗");
                sonuc.AppendLine($"║ ROTA PLANLAMA RAPORU - {textBox1.Text} ({comboBoxYolcuTipi.SelectedItem}) ║");
                sonuc.AppendLine("╚════════════════════════════════════════════════════════════════════╝");
                sonuc.AppendLine($"🕒 Tarih: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                sonuc.AppendLine($"📍 Başlangıç: ({baslangic.Enlem:F6}, {baslangic.Boylam:F6})");
                sonuc.AppendLine($"📍 Bitiş: ({bitis.Enlem:F6}, {bitis.Boylam:F6})");

                var enYakinBaslangicDurak = rotaHesaplayici.EnYakinDuragiBul(baslangic);
                double baslangicDurakMesafe = rotaHesaplayici.MesafeHesapla(baslangic, new RotaHesaplayici.Konum { Enlem = enYakinBaslangicDurak.Enlem, Boylam = enYakinBaslangicDurak.Boylam });

                sonuc.AppendLine("\n📍 Kullanıcı Konumuna En Yakın Duraklar:");
                sonuc.AppendLine($"🔹 {enYakinBaslangicDurak.Ad} ({baslangicDurakMesafe:F2} km)");

                sonuc.AppendLine("╓───── TÜM SENARYOLAR KARŞILAŞTIRMA ÖZETİ ─────╖");
                sonuc.AppendLine("║ Rota Türü           Süre   Maliyet   Mesafe   Aktarma ║");
                sonuc.AppendLine("╟─────────────────────────────────────────────────╢");

                var rotaListesi = alternatifRotalar.ToList();
                foreach (var (aciklama, _, maliyet, sure, mesafe) in rotaListesi)
                {
                    int aktarmaSayisi = rotaListesi.First(r => r.Aciklama == aciklama).Rota.Count(r => r.Ucret > 0) - 1;
                    sonuc.AppendLine($"║ {aciklama,-20} {sure,3} dk {maliyet,6:F2} TL {mesafe,6:F2} km {aktarmaSayisi,3} ║");
                }
                sonuc.AppendLine("╙─────────────────────────────────────────────────╜\n");

                sonuc.AppendLine("🚏 DETAYLI ROTA PLANLARI");
                sonuc.AppendLine(new string('═', 70) + "\n");

                foreach (var (aciklama, rotaAdimlari, maliyet, sure, mesafe) in alternatifRotalar)
                {
                    string baslik = aciklama switch
                    {
                        "Sadece Taksi (Hızlı ama maliyetli)" => "🚖 Taksi ile Direkt (Hızlı, Konforlu ama Pahalı)",
                        "Sadece Otobüs (Uygun maliyetli)" => "🚍 Otobüs ile Ekonomik (Ucuz ama Daha Yavaş)",
                        "Sadece Tramvay (Rahat ve dengeli)" => "🚋 Tramvay ile Konforlu (Dengeli Fiyat ve Hız)",
                        "Otobüs-Tramvay Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)",
                        "Tramvay-Otobüs Aktarmalı" => "🔄 Hibrit Rota (Optimize Edilmiş Aktarma)",
                        _ => aciklama
                    };

                    sonuc.AppendLine($"【 {baslik} 】");
                    sonuc.AppendLine($"Toplam Süre: {sure} dk | Maliyet: {maliyet:F2} TL | Mesafe: {mesafe:F2} km");
                    sonuc.AppendLine(new string('─', 60));

                    DateTime geciciZaman = DateTime.Now;
                    int adimSayisi = 0;

                    foreach (var adim in rotaAdimlari)
                    {
                        adimSayisi++;
                        string aracIkoni = adim.KullanilanArac.AracTuru switch
                        {
                            "Taksi" => "🚖",
                            "Otobüs" => "🚍",
                            "Tramvay" => "🚋",
                            _ => "🚶"
                        };

                        sonuc.AppendLine($"Adım {adimSayisi}: {aracIkoni} {adim.BaslangicDurak} → {adim.BitisDurak}");
                        sonuc.AppendLine($"   ⏱️ Süre: {adim.Sure} dk (Varış: {geciciZaman.AddMinutes(adim.Sure):HH:mm:ss})");
                        sonuc.AppendLine($"   📏 Mesafe: {adim.Mesafe:F2} km");
                        sonuc.AppendLine($"   💰 Ücret: {adim.Ucret:F2} TL{(adim.KullanilanArac.AracTuru == "Yaya" ? "" : yolcu is Ogrenci ? " (Öğrenci %50 indirimli)" : "")}");
                        if (adimSayisi < rotaAdimlari.Count && adim.Ucret > 0)
                            sonuc.AppendLine("   🔄 Aktarma: Sonraki araca geçiş");
                        sonuc.AppendLine();

                        geciciZaman = geciciZaman.AddMinutes(adim.Sure);
                    }

                    string avantajlar = aciklama.Contains("Taksi") ? "Hızlı, direkt" :
                                      aciklama.Contains("Otobüs") ? "Ucuz, yaygın" :
                                      "Konforlu, düzenli";
                    string dezavantajlar = aciklama.Contains("Taksi") ? "Pahalı" :
                                           aciklama.Contains("Otobüs") ? "Kalabalık olabilir" :
                                           "Sınırlı rota";
                    sonuc.AppendLine($"✅ Avantajlar: {avantajlar}");
                    sonuc.AppendLine($"❌ Dezavantajlar: {dezavantajlar}");
                    sonuc.AppendLine(new string('═', 60) + "\n");
                }

                var enHizli = rotaListesi.OrderBy(r => r.Sure).First();
                var enUcuz = rotaListesi.OrderBy(r => r.Maliyet).First();
                sonuc.AppendLine("📌 ÖNERİLER:");
                sonuc.AppendLine($"🏃 En Hızlı Seçenek: {enHizli.Aciklama} ({enHizli.Sure} dk)");
                sonuc.AppendLine($"💸 En Ucuz Seçenek: {enUcuz.Aciklama} ({enUcuz.Maliyet:F2} TL)");

                richTextBoxSonuc.Text = sonuc.ToString();

                var selectedRoute = enUcuz;
                IOdeme odeme = comboBoxOdemeTipi.SelectedItem.ToString() switch
                {
                    "Nakit" => new NakitOdeme(),
                    "Kredi Kartı" => new KrediKartiOdeme(),
                    "Kentkart" => new KentkartOdeme(),
                    _ => new NakitOdeme()
                };
                odeme.OdemeYap(selectedRoute.Maliyet);

                gMapControl.Refresh();
            }
            catch (FormatException)
            {
                MessageBox.Show("Enlem ve boylam değerleri sayısal olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}