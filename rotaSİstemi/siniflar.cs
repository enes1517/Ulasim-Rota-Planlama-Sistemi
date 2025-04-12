using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace rotaSİstemi
{
    namespace rotaSİstemi
    {
        // Yolcu Sınıfı ve Kalıtım
        public abstract class Yolcu
        {
            public string Ad { get; set; }
            protected double IndirimOrani { get; set; }

            public Yolcu(string ad)
            {
                Ad = ad;
            }

            public abstract double UcretHesapla(double temelUcret);
        }

        public class Ogrenci : Yolcu
        {
            public Ogrenci(string ad) : base(ad)
            {
                IndirimOrani = 0.5; 
            }

            public override double UcretHesapla(double temelUcret)
            {
                return temelUcret * (1 - IndirimOrani);
            }
        }

        public class Yasli : Yolcu
        {
            public Yasli(string ad) : base(ad)
            {
                IndirimOrani = 0.75; 
            }

            public override double UcretHesapla(double temelUcret)
            {
                return temelUcret * (1 - IndirimOrani);
            }
        }

        public class Genel : Yolcu
        {
            public Genel(string ad) : base(ad)
            {
                IndirimOrani = 0; 
            }

            public override double UcretHesapla(double temelUcret)
            {
                return temelUcret;
            }
        }

        // Araç Sınıfı ve Kalıtım
        public abstract class Arac
        {
            public string AracTuru { get; protected set; }
            public double TemelUcret { get; protected set; }

            public abstract double MaliyetHesapla(double mesafe);
        }

        public class Otobus : Arac
        {
            public Otobus()
            {
                AracTuru = "Otobüs";
                TemelUcret = 0; 
            }

            public override double MaliyetHesapla(double mesafe)
            {
                return TemelUcret; // Ücret JSON'dan dinamik olarak atanacak
            }
        }
        public class Yaya : Arac
        {
            public Yaya()
            {
                AracTuru = "Yaya";
                TemelUcret = 0; 
            }

            public override double MaliyetHesapla(double mesafe)
            {
                return 0; 
            }
        }

        public class Tramvay : Arac
        {
            public Tramvay()
            {
                AracTuru = "Tramvay";
                TemelUcret = 0; // Ücret JSON'dan gelecek
            }

            public override double MaliyetHesapla(double mesafe)
            {
                return TemelUcret;
            }
        }

        public class Taksi : Arac
        {
            private double acilisUcreti;
            private double kmBasiUcret;

            public Taksi(double acilisUcreti, double kmBasiUcret)
            {
                AracTuru = "Taksi";
                this.acilisUcreti = acilisUcreti;
                this.kmBasiUcret = kmBasiUcret;
                TemelUcret = acilisUcreti;
            }

            public override double MaliyetHesapla(double mesafe)
            {
                return TemelUcret + (mesafe * kmBasiUcret);
            }
        }

        // Ödeme Arayüzü ve Sınıfları
        public interface IOdeme
        {
            void OdemeYap(double tutar);
        }

        public class NakitOdeme : IOdeme
        {
            public void OdemeYap(double tutar)
            {
                MessageBox.Show($"Nakit ödeme: {tutar:F2} TL");
            }
        }

        public class KrediKartiOdeme : IOdeme
        {
            public void OdemeYap(double tutar)
            {
                MessageBox.Show($"Kredi kartı ile ödeme: {tutar:F2} TL");
            }
        }

        public class KentkartOdeme : IOdeme
        {
            public void OdemeYap(double tutar)
            {
                MessageBox.Show($"Kentkart ile ödeme: {tutar:F2} TL");
            }
        }

        // JSON Veri Modeli
        public class SehirVerisi
        {
            [JsonProperty("city")]
            public string Sehir { get; set; }

            [JsonProperty("taxi")]
            public TaksiBilgisi Taksi { get; set; }

            [JsonProperty("duraklar")]
            public List<Durak> Duraklar { get; set; }
        }

        public class TaksiBilgisi
        {
            [JsonProperty("openingFee")]
            public double AcilisUcreti { get; set; }

            [JsonProperty("costPerKm")]
            public double KmBasiUcret { get; set; }
        }

        public class Durak
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Ad { get; set; }

            [JsonProperty("type")]
            public string Tur { get; set; }

            [JsonProperty("lat")]
            public double Enlem { get; set; }

            [JsonProperty("lon")]
            public double Boylam { get; set; }

            [JsonProperty("sonDurak")]
            public bool SonDurak { get; set; }

            [JsonProperty("nextStops")]
            public List<SonrakiDurak> SonrakiDuraklar { get; set; }

            [JsonProperty("transfer")]
            public AktarmaBilgisi Aktarma { get; set; }
        }

        public class SonrakiDurak
        {
            [JsonProperty("stopId")]
            public string DurakId { get; set; }

            [JsonProperty("mesafe")]
            public double Mesafe { get; set; }

            [JsonProperty("sure")]
            public int Sure { get; set; }

            [JsonProperty("ucret")]
            public double Ucret { get; set; }
        }

        public class AktarmaBilgisi
        {
            [JsonProperty("transferStopId")]
            public string AktarmaDurakId { get; set; }

            [JsonProperty("transferSure")]
            public int AktarmaSure { get; set; }

            [JsonProperty("transferUcret")]
            public double AktarmaUcret { get; set; }
        }

        // Rota Adımı Modeli
        public class RotaAdimi
        {
            public string BaslangicDurak { get; set; }
            public string BitisDurak { get; set; }
            public Arac KullanilanArac { get; set; }
            public double Mesafe { get; set; }
            public int Sure { get; set; }
            public double Ucret { get; set; }
        }

        // Rota Hesaplayıcı
        public class RotaHesaplayici
        {
            public class Konum
            {
                public double Enlem { get; set; }
                public double Boylam { get; set; }
            }

            private List<Durak> duraklar;
            private TaksiBilgisi taksiBilgisi;
            private const double TaksiEsikDegeri = 3.0;
            private const double AktarmaIndirimi = 0.5; 

            public RotaHesaplayici()
            {
                DuraklariYukle();
            }

            private void DuraklariYukle()
            {
                try
                {
                    string jsonDosyaYolu = "duraklar.json";
                    string jsonIcerik = System.IO.File.ReadAllText(jsonDosyaYolu);
                    SehirVerisi sehirVerisi = JsonConvert.DeserializeObject<SehirVerisi>(jsonIcerik);

                    duraklar = sehirVerisi.Duraklar;
                    taksiBilgisi = sehirVerisi.Taksi;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"JSON dosyası yüklenirken hata oluştu: {ex.Message}");
                }
            }

            public double MesafeHesapla(Konum konum1, Konum konum2)
            {
                const double R = 6371; // Dünya yarıçapı (km)
                double enlemFark = DereceyiRadyanaCevir(konum2.Enlem - konum1.Enlem);
                double boylamFark = DereceyiRadyanaCevir(konum2.Boylam - konum1.Boylam);
                double a = Math.Sin(enlemFark / 2) * Math.Sin(enlemFark / 2) +
                           Math.Cos(DereceyiRadyanaCevir(konum1.Enlem)) * Math.Cos(DereceyiRadyanaCevir(konum2.Enlem)) *
                           Math.Sin(boylamFark / 2) * Math.Sin(boylamFark / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return R * c;
            }

            private double DereceyiRadyanaCevir(double derece)
            {
                return derece * Math.PI / 180;
            }

            public Durak EnYakinDuragiBul(Konum konum)
            {
                return duraklar.OrderBy(d => MesafeHesapla(konum, new Konum { Enlem = d.Enlem, Boylam = d.Boylam })).First();
            }

            // Dijkstra Algoritması ile En Kısa Rota
            private (List<RotaAdimi>, double) DijkstraRotaBul(Durak baslangicDurak, Durak bitisDurak, Yolcu yolcu, string tercih = "")
            {
                Dictionary<string, double> mesafeler = new Dictionary<string, double>();
                Dictionary<string, string> onceki = new Dictionary<string, string>();
                Dictionary<string, RotaAdimi> adimlar = new Dictionary<string, RotaAdimi>();
                List<string> ziyaretEdilmemis = new List<string>();

                var filtrelenmisDuraklar = duraklar.Where(d => string.IsNullOrEmpty(tercih) || d.Tur == tercih).ToList();
                foreach (var durak in filtrelenmisDuraklar)
                {
                    mesafeler[durak.Id] = double.MaxValue;
                    onceki[durak.Id] = null;
                    ziyaretEdilmemis.Add(durak.Id);
                }

                mesafeler[baslangicDurak.Id] = 0;

                while (ziyaretEdilmemis.Count > 0)
                {
                    string mevcutDurakId = ziyaretEdilmemis.OrderBy(d => mesafeler[d]).First();
                    Durak mevcutDurak = filtrelenmisDuraklar.First(d => d.Id == mevcutDurakId);

                    if (mevcutDurakId == bitisDurak.Id)
                        break;

                    ziyaretEdilmemis.Remove(mevcutDurakId);

                    foreach (var sonraki in mevcutDurak.SonrakiDuraklar)
                    {
                        if (ziyaretEdilmemis.Contains(sonraki.DurakId))
                        {
                            Durak sonrakiDurak = filtrelenmisDuraklar.First(d => d.Id == sonraki.DurakId);
                            if (!string.IsNullOrEmpty(tercih) && (mevcutDurak.Tur != tercih || sonrakiDurak.Tur != tercih)) continue;

                            double yeniUcret = mesafeler[mevcutDurakId] + yolcu.UcretHesapla(sonraki.Ucret);
                            if (yeniUcret < mesafeler[sonraki.DurakId])
                            {
                                mesafeler[sonraki.DurakId] = yeniUcret;
                                onceki[sonraki.DurakId] = mevcutDurakId;
                                adimlar[sonraki.DurakId] = new RotaAdimi
                                {
                                    BaslangicDurak = mevcutDurak.Ad,
                                    BitisDurak = sonrakiDurak.Ad,
                                    KullanilanArac = tercih == "bus" ? (Arac)new Otobus() : new Tramvay(),
                                    Mesafe = sonraki.Mesafe,
                                    Sure = sonraki.Sure,
                                    Ucret = yolcu.UcretHesapla(sonraki.Ucret)
                                };
                            }
                        }
                    }

                    if (mevcutDurak.Aktarma != null && ziyaretEdilmemis.Contains(mevcutDurak.Aktarma.AktarmaDurakId))
                    {
                        string aktarmaId = mevcutDurak.Aktarma.AktarmaDurakId;
                        Durak aktarmaDurak = filtrelenmisDuraklar.First(d => d.Id == aktarmaId);
                        if (!string.IsNullOrEmpty(tercih) && (mevcutDurak.Tur != tercih || aktarmaDurak.Tur != tercih)) continue;

                        double aktarmaUcret = yolcu.UcretHesapla(mevcutDurak.Aktarma.AktarmaUcret) * AktarmaIndirimi;
                        double yeniUcret = mesafeler[mevcutDurakId] + aktarmaUcret;
                        if (yeniUcret < mesafeler[aktarmaId])
                        {
                            mesafeler[aktarmaId] = yeniUcret;
                            onceki[aktarmaId] = mevcutDurakId;
                            adimlar[aktarmaId] = new RotaAdimi
                            {
                                BaslangicDurak = mevcutDurak.Ad,
                                BitisDurak = aktarmaDurak.Ad,
                                KullanilanArac = tercih == "bus" ? (Arac)new Otobus() : new Tramvay(),
                                Mesafe = 0,
                                Sure = mevcutDurak.Aktarma.AktarmaSure,
                                Ucret = aktarmaUcret
                            };
                        }
                    }
                }

                List<RotaAdimi> rota = new List<RotaAdimi>();
                string gecerliDurak = bitisDurak.Id;
                while (gecerliDurak != null)
                {
                    if (adimlar.ContainsKey(gecerliDurak))
                    {
                        rota.Insert(0, adimlar[gecerliDurak]);
                    }
                    gecerliDurak = onceki[gecerliDurak];
                }

                return (rota, mesafeler[bitisDurak.Id]);
            }

            public List<(string Aciklama, List<RotaAdimi> Rota, double Maliyet, int Sure, double Mesafe)> RotaPlanla(Yolcu yolcu, Konum baslangic, Konum bitis)
            {
                const double YURUYUS_Esik_Degeri = 3.0; 
                const double YURUYUS_HIZI = 0.083; 
                const double AktarmaIndirimi = 0.5; 

                List<(string, List<RotaAdimi>, double, int, double)> alternatifRotalar = new List<(string, List<RotaAdimi>, double, int, double)>();
                Taksi taksi = new Taksi(taksiBilgisi.AcilisUcreti, taksiBilgisi.KmBasiUcret);
                bool taksiVeyaYayaKullanildi = false; // Taksi veya yürüme sadece başta kullanılabilir

                // En yakın durakları bul
                Durak baslangicOtobusDurak = duraklar.Where(d => d.Tur == "bus")
                    .OrderBy(d => MesafeHesapla(baslangic, new Konum { Enlem = d.Enlem, Boylam = d.Boylam }))
                    .FirstOrDefault();
                Durak bitisOtobusDurak = duraklar.Where(d => d.Tur == "bus")
                    .OrderBy(d => MesafeHesapla(bitis, new Konum { Enlem = d.Enlem, Boylam = d.Boylam }))
                    .FirstOrDefault();
                Durak baslangicTramDurak = duraklar.Where(d => d.Tur == "tram")
                    .OrderBy(d => MesafeHesapla(baslangic, new Konum { Enlem = d.Enlem, Boylam = d.Boylam }))
                    .FirstOrDefault();
                Durak bitisTramDurak = duraklar.Where(d => d.Tur == "tram")
                    .OrderBy(d => MesafeHesapla(bitis, new Konum { Enlem = d.Enlem, Boylam = d.Boylam }))
                    .FirstOrDefault();

                List<RotaAdimi> AddInitialAndFinalSteps(Durak baslangicDurak, Durak bitisDurak, List<RotaAdimi> rota,
                    Konum baslangicKonum, Konum bitisKonum, ref double maliyetEk, ref int sureEk, ref double mesafeEk, ref bool taksiVeyaYayaKullanildiFlag)
                {
                    List<RotaAdimi> yeniRota = new List<RotaAdimi>();
                    double baslangicMesafesi = MesafeHesapla(baslangicKonum, new Konum { Enlem = baslangicDurak.Enlem, Boylam = baslangicDurak.Boylam });
                    double bitisMesafesi = MesafeHesapla(new Konum { Enlem = bitisDurak.Enlem, Boylam = bitisDurak.Boylam }, bitisKonum);

                    if (baslangicMesafesi > 0 && !taksiVeyaYayaKullanildiFlag)
                    {
                        if (baslangicMesafesi <= YURUYUS_Esik_Degeri)
                        {
                            int yuruyusSuresi = (int)(baslangicMesafesi / YURUYUS_HIZI);
                            yeniRota.Add(new RotaAdimi
                            {
                                BaslangicDurak = "Başlangıç Noktası",
                                BitisDurak = baslangicDurak.Ad,
                                KullanilanArac = new Yaya(),
                                Mesafe = baslangicMesafesi,
                                Sure = yuruyusSuresi,
                                Ucret = 0
                            });
                            sureEk += yuruyusSuresi;
                            mesafeEk += baslangicMesafesi;
                            taksiVeyaYayaKullanildiFlag = true; // Yürüme kullanıldı
                        }
                        else
                        {
                            double taksiUcret = yolcu.UcretHesapla(taksi.MaliyetHesapla(baslangicMesafesi));
                            yeniRota.Add(new RotaAdimi
                            {
                                BaslangicDurak = "Başlangıç Noktası",
                                BitisDurak = baslangicDurak.Ad,
                                KullanilanArac = taksi,
                                Mesafe = baslangicMesafesi,
                                Sure = (int)(baslangicMesafesi * 2),
                                Ucret = taksiUcret
                            });
                            maliyetEk += taksiUcret;
                            sureEk += (int)(baslangicMesafesi * 2);
                            mesafeEk += baslangicMesafesi;
                            taksiVeyaYayaKullanildiFlag = true; 
                        }
                    }
                    else if (baslangicMesafesi > 0 && taksiVeyaYayaKullanildiFlag)
                    {
                        return new List<RotaAdimi>(); 
                    }

                    yeniRota.AddRange(rota);

                  
                     if (bitisMesafesi > 0 && taksiVeyaYayaKullanildiFlag)
                    {
                        return new List<RotaAdimi>(); 
                    }

                    return yeniRota;
                }

                // 1. Sadece Taksi (Hızlı ama maliyetli)
                double taksiMesafe = MesafeHesapla(baslangic, bitis);
                double taksiUcret = yolcu.UcretHesapla(taksi.MaliyetHesapla(taksiMesafe));
                alternatifRotalar.Add(("Sadece Taksi (Hızlı ama maliyetli)", new List<RotaAdimi>
    {
        new RotaAdimi
        {
            BaslangicDurak = "Başlangıç Noktası",
            BitisDurak = "Bitiş Noktası",
            KullanilanArac = taksi,
            Mesafe = taksiMesafe,
            Sure = (int)(taksiMesafe * 2),
            Ucret = taksiUcret
        }
    }, taksiUcret, (int)(taksiMesafe * 2), taksiMesafe));

                // 2. Sadece Otobüs (Uygun maliyetli)
                if (baslangicOtobusDurak != null && bitisOtobusDurak != null)
                {
                    var (busRota, busMaliyet) = DijkstraRotaBul(baslangicOtobusDurak, bitisOtobusDurak, yolcu, "bus");
                    if (busRota.Any())
                    {
                        int busSure = busRota.Sum(r => r.Sure);
                        double busMesafe = busRota.Sum(r => r.Mesafe);
                        double toplamBusMaliyet = busMaliyet;
                        taksiVeyaYayaKullanildi = false; // Her rota için sıfırla
                        var busRotaWithSteps = AddInitialAndFinalSteps(baslangicOtobusDurak, bitisOtobusDurak, busRota, baslangic, bitis, ref toplamBusMaliyet, ref busSure, ref busMesafe, ref taksiVeyaYayaKullanildi);
                        if (busRotaWithSteps.Any())
                        {
                            alternatifRotalar.Add(("Sadece Otobüs (Uygun maliyetli)", busRotaWithSteps, toplamBusMaliyet, busSure, busMesafe));
                        }
                    }
                }

                // 3. Sadece Tramvay (Rahat ve dengeli)
                if (baslangicTramDurak != null && bitisTramDurak != null)
                {
                    var (tramRota, tramMaliyet) = DijkstraRotaBul(baslangicTramDurak, bitisTramDurak, yolcu, "tram");
                    if (tramRota.Any())
                    {
                        int tramSure = tramRota.Sum(r => r.Sure);
                        double tramMesafe = tramRota.Sum(r => r.Mesafe);
                        double toplamTramMaliyet = tramMaliyet;
                        taksiVeyaYayaKullanildi = false; // Her rota için sıfırla
                        var tramRotaWithSteps = AddInitialAndFinalSteps(baslangicTramDurak, bitisTramDurak, tramRota, baslangic, bitis, ref toplamTramMaliyet, ref tramSure, ref tramMesafe, ref taksiVeyaYayaKullanildi);
                        if (tramRotaWithSteps.Any())
                        {
                            alternatifRotalar.Add(("Sadece Tramvay (Rahat ve dengeli)", tramRotaWithSteps, toplamTramMaliyet, tramSure, tramMesafe));
                        }
                    }
                }

                // 4. Otobüs-Tramvay Aktarmalı (En az maliyetli ve en az süreli için optimize edilmiş)
                var aktarmaDuraklari = duraklar.Where(d => d.Aktarma != null &&
                    ((d.Tur == "bus" && duraklar.Any(d2 => d2.Id == d.Aktarma.AktarmaDurakId && d2.Tur == "tram")) ||
                     (d.Tur == "tram" && duraklar.Any(d2 => d2.Id == d.Aktarma.AktarmaDurakId && d2.Tur == "bus"))))
                    .ToList();

                foreach (var aktarmaDurak in aktarmaDuraklari)
                {
                    Durak aktarmaHedefDurak = duraklar.First(d => d.Id == aktarmaDurak.Aktarma.AktarmaDurakId);
                    if (aktarmaDurak.Tur == "bus" && baslangicOtobusDurak != null && bitisTramDurak != null)
                    {
                        var (busKismiRota, busKismiMaliyet) = DijkstraRotaBul(baslangicOtobusDurak, aktarmaDurak, yolcu, "bus");
                        var (tramKismiRota, tramKismiMaliyet) = DijkstraRotaBul(aktarmaHedefDurak, bitisTramDurak, yolcu, "tram");

                        if (busKismiRota.Any() && tramKismiRota.Any())
                        {
                            double aktarmaUcret = yolcu.UcretHesapla(aktarmaDurak.Aktarma.AktarmaUcret) * AktarmaIndirimi;
                            List<RotaAdimi> birlesikRota = new List<RotaAdimi>(busKismiRota);
                            birlesikRota.Add(new RotaAdimi
                            {
                                BaslangicDurak = aktarmaDurak.Ad,
                                BitisDurak = aktarmaHedefDurak.Ad,
                                KullanilanArac = aktarmaDurak.Tur == "bus" ? (Arac)new Otobus() : new Tramvay(),
                                Mesafe = 0,
                                Sure = aktarmaDurak.Aktarma.AktarmaSure,
                                Ucret = aktarmaUcret
                            });
                            birlesikRota.AddRange(tramKismiRota);

                            double toplamMaliyet = busKismiMaliyet + aktarmaUcret + tramKismiMaliyet;
                            int toplamSure = birlesikRota.Sum(r => r.Sure);
                            double toplamMesafe = birlesikRota.Sum(r => r.Mesafe);
                            taksiVeyaYayaKullanildi = false; // Her rota için sıfırla
                            var birlesikRotaWithSteps = AddInitialAndFinalSteps(baslangicOtobusDurak, bitisTramDurak, birlesikRota, baslangic, bitis, ref toplamMaliyet, ref toplamSure, ref toplamMesafe, ref taksiVeyaYayaKullanildi);
                            if (birlesikRotaWithSteps.Any())
                            {
                                alternatifRotalar.Add(("Otobüs-Tramvay Aktarmalı", birlesikRotaWithSteps, toplamMaliyet, toplamSure, toplamMesafe));
                            }
                        }
                    }
                    else if (aktarmaDurak.Tur == "tram" && baslangicTramDurak != null && bitisOtobusDurak != null)
                    {
                        var (tramKismiRota, tramKismiMaliyet) = DijkstraRotaBul(baslangicTramDurak, aktarmaDurak, yolcu, "tram");
                        var (busKismiRota, busKismiMaliyet) = DijkstraRotaBul(aktarmaHedefDurak, bitisOtobusDurak, yolcu, "bus");

                        if (tramKismiRota.Any() && busKismiRota.Any())
                        {
                            double aktarmaUcret = yolcu.UcretHesapla(aktarmaDurak.Aktarma.AktarmaUcret) * AktarmaIndirimi;
                            List<RotaAdimi> birlesikRota = new List<RotaAdimi>(tramKismiRota);
                            birlesikRota.Add(new RotaAdimi
                            {
                                BaslangicDurak = aktarmaDurak.Ad,
                                BitisDurak = aktarmaHedefDurak.Ad,
                                KullanilanArac = aktarmaDurak.Tur == "bus" ? (Arac)new Otobus() : new Tramvay(),
                                Mesafe = 0,
                                Sure = aktarmaDurak.Aktarma.AktarmaSure,
                                Ucret = aktarmaUcret
                            });
                            birlesikRota.AddRange(busKismiRota);

                            double toplamMaliyet = tramKismiMaliyet + aktarmaUcret + busKismiMaliyet;
                            int toplamSure = birlesikRota.Sum(r => r.Sure);
                            double toplamMesafe = birlesikRota.Sum(r => r.Mesafe);
                            taksiVeyaYayaKullanildi = false; // Her rota için sıfırla
                            var birlesikRotaWithSteps = AddInitialAndFinalSteps(baslangicTramDurak, bitisOtobusDurak, birlesikRota, baslangic, bitis, ref toplamMaliyet, ref toplamSure, ref toplamMesafe, ref taksiVeyaYayaKullanildi);
                            if (birlesikRotaWithSteps.Any())
                            {
                                alternatifRotalar.Add(("Tramvay-Otobüs Aktarmalı", birlesikRotaWithSteps, toplamMaliyet, toplamSure, toplamMesafe));
                            }
                        }
                    }
                }

                // Tüm alternatif rotaları döndür
                return alternatifRotalar
                    .Where(r => r.Item2 != null && r.Item2.Any()) 
                    .Distinct()
                    .ToList();
            }
        }
    }
}

