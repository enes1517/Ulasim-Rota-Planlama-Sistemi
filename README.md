Ulaşım Rota Planlama Sistemi

Bu proje, C# Windows Forms ve Dijkstra'nın graf algoritması kullanılarak geliştirilmiş bir masaüstü rota optimizasyon sistemidir. Toplu taşıma veya küçük ölçekli lojistik ağları için duraklar arasındaki en kısa veya en verimli rotayı hesaplamak üzere tasarlanmıştır.

Sistem, tüm durak ve bağlantı verilerini harici bir duraklar.json dosyasından dinamik olarak yükler. Bu, veri yönetimini kolaylaştırır ve sistemi esnek kılar.


✨ Temel Özellikler
Optimize Edilmiş Rota Hesaplama: Dijkstra algoritmasını kullanarak başlangıç ve bitiş durakları arasındaki en verimli rotayı (en kısa mesafe/süre) hesaplar.

Dinamik Veri Yükleme: Tüm durak ve rota bilgilerini duraklar.json dosyasından okuyarak sistemi dinamik ve kolayca güncellenebilir hale getirir.

Modüler Mimari: Kod yapısı, veri işleme (siniflar.cs içindeki algoritmalar) ve kullanıcı arayüzünü (Form1.cs) ayırarak modüler bir yaklaşım sergiler.

Basit Kullanıcı Arayüzü: Windows Forms üzerinden başlangıç ve bitiş duraklarının seçilmesine ve sonucun net bir şekilde görüntülenmesine olanak tanır.

Genişletilebilirlik: Altyapısı, ileride trafik verisi entegrasyonu, maliyet hesabı veya harita servisleriyle görselleştirme gibi gelişmiş özellikler eklemeye uygundur.

Hata Yönetimi: Geçersiz durak girişleri veya rota bulunamaması gibi durumlar için temel hata yakalama mekanizmalarına sahiptir.

🛠️ Kullanılan Teknolojiler
Programlama Dili: C#

Çerçeve (Framework): .NET Framework

Kullanıcı Arayüzü (UI): Windows Forms

Temel Algoritma: Dijkstra Graf Algoritması

Veri Depolama: JSON (Veri kaynağı olarak duraklar.json)

Geliştirme Ortamı: Visual Studio (.sln, .csproj)

🏁 Kurulum ve Çalıştırma
Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyebilirsiniz.

Gereksinimler
.NET Framework (Projenin .csproj dosyasında belirtilen sürümle uyumlu)


Adımlar
Öncelikle bu depoyu makinenize klonlayın:

git clone https://github.com/enes1517/Ulasim-Rota-Planlama-Sistemi.git
Klonlanan klasörün içine girin:


cd Ulasim-Rota-Planlama-Sistemi
rotaSİstemi.sln çözüm (solution) dosyasını Visual Studio ile açın.

Visual Studio'nun gerekli bağımlılıkları (örn: JSON.NET kütüphanesi) otomatik olarak geri yüklemesini bekleyin.

Projeyi Start (Başlat) butonuna basarak (veya F5 tuşu ile) derleyin ve çalıştırın.

📂 Önemli Proje Dosyaları
Program.cs: Uygulamanın ana giriş noktasıdır (Entry Point). Form1'i başlatır.

Form1.cs: Ana kullanıcı arayüzü formudur. Kullanıcıdan girdileri (başlangıç/bitiş durakları) alır ve rota sonuçlarını gösterir.

siniflar.cs: Projenin iş mantığını ve çekirdek algoritmalarını (Dijkstra) içeren sınıfları barındırır.

duraklar.json: Durakları, aralarındaki mesafeleri ve bağlantıları içeren ana veri kaynağıdır.

ClassDiagram.cd: Projenin sınıf mimarisini görselleştiren sınıf diyagramı dosyasıdır.

duraklar.json: Durakları, aralarındaki mesafeleri ve bağlantıları içeren ana veri kaynağıdır.

ClassDiagram.cd: Projenin sınıf mimarisini görselleştiren sınıf diyagramı dosyasıdır.
