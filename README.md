Bu proje, toplu taşıma veya lojistik için rota planlama sistemi olarak geliştirilmiştir. Duraklar arasında en kısa veya en uygun rotaları hesaplar, JSON verileriyle dinamik veri yükler. C# ve Windows Forms ile masaüstü uygulamasıdır, graf algoritmaları (örneğin Dijkstra veya A* gibi) kullanarak rota optimizasyonu yapar. Eğitim veya küçük ölçekli ulaşım planlaması için uygundur. Proje, durak verilerini JSON'dan okuyarak gerçek zamanlı rota önerileri sunar ve potansiyel olarak harita entegrasyonu ile genişletilebilir.
Kod yapısı, sınıf tabanlı bir yaklaşım gösterir ve veri işleme ile kullanıcı arayüzünü ayırır. Bu, sistemi modüler kılar ve yeni algoritmalar eklemeyi kolaylaştırır.
Özellikler
Rota Hesaplama: Başlangıç ve bitiş durakları girilerek rota önerisi, mesafe/zaman hesabı, alternatif rotalar. (siniflar.cs'te algoritma sınıfları tanımlanır)
Durak Yönetimi: duraklar.json'dan veri yükleme, durak ekleme/güncelleme/silme işlemleri.
Kullanıcı Arayüzü: Form1 ile giriş/çıkış seçimleri (ComboBox veya TextBox), rota görüntüleme (ListBox veya metin tabanlı harita).
Veri Depolama: JSON dosyaları ile hafif ve taşınabilir veri yönetimi.
Tasarım Araçları: ClassDiagram.cd ile sınıf diyagramı, mimari görselleştirme ve planlama.
Giriş Noktası: Program.cs ile uygulama başlatma ve ana form yükleme.
Gelişmiş Özellikler: Potansiyel olarak trafik verisi entegrasyonu, maliyet hesabı ve rota görselleştirme .
Hata İşleme: Geçersiz durak veya rota için uyarılar, JSON parse hataları yakalama.
Kullanılan Teknolojiler  
Dil ve Çerçeve: C# .NET Framework
UI: Windows Forms (Form1.cs ve tasarımı).
Veri: JSON dosyaları (duraklar.json için veri kaynağı).
Araçlar: Visual Studio (.sln, .csproj ile).
Bağımlılıklar: Temel .NET kütüphaneleri, JSON.NET gibi
Kurulum Gereklilikleri
.NET Framework  
Visual Studio.
Nasıl Çalıştırılır
Depoyu klonlayın: git clone https://github.com/enes1517/Ulasim-Rota-Planlama-Sistemi.git.
rotaSİstemi.sln'i Visual Studio'da açın.
