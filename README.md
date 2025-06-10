# EtheriumNotifier

Bu proje, Ethereum ağındaki işlemleri Hangfire ile periyodik olarak takip edip kaydeden,
Redis & In-Memory DB ile idempotency ve cache yöneten ve tanımlı bildirim kanallarına simülasyon yoluyla bildirimler gönderen modüler bir .NET uygulamasıdır.

**Transaction Okuma & Kaydetme
Hangfire kullanarak her dakika tetiklenen job, Nethereum ile son N bloktan transferleri çeker ve In-Memory veritabanına kaydeder.

**Idempotency & Cache (Redis)
Redis ile transaction hash’leri hem tekil key (Tx-Hash:{hash}), hem dakika bazlı set (Tx-Minute:{yyyy-MM-dd-HH:mm}) olarak saklanır.
Aynı hash ikinci kez işlenmez.

**Notification Channel Yönetimi
CRUD endpoint’leri üzerinden kullanıcı bazlı bildirim kanalları oluşturma/güncelleme/silme/listeleme.
Her kullanıcı-tip kombinasyonu için tekil kayıt garantisi.

**Bildirim Simülasyonu & Log
appsettings.json üzerinden gelen MinEthAmount (ör. 0.25 ETH) eşik değerine göre yalnızca başarılı ve minimum tutar üstündeki işlemler bildirilir.
Simülasyon: Console çıktısı ve Notification entity’sine log kaydı.

**Notification Log Listeleme
GET /api/notificationlog endpoint’i ile userId, kanal tipi, tarih aralığı (max 30 gün) filtrelerine göre geçmiş bildirimler sorgulanabilir.

**Filtreleme & Pagination
Transaction listelerini adres, hash, blok numarası, tutar, tarih aralığı gibi kriterlerle süzebilir ve sayfalayabilirsiniz.

**Structured Logging (Serilog)
Konsol ve dosya üzerinde structured log desteği.

# Uygulamayı Başlatmak İçin Gerekenler

Uygulama Redis ile entegre bir şekilde çalıştığı için , önce Redis'i çalıştırmamız gerekir . Daha sonrada gerekli connection adresi ile Redis'i uygulamaya bağlamanız gerekmektedir.
Redis indirmek için gerekli URL aşağıdadır. Kullanılan versiyon 5.0.14.1
https://github.com/tporadowski/redis/releases
Gerekli Bağlantı Stringi
"Redis": {
    "Host": "localhost",
    "Port": 6379,
    "InstanceName": "Localhost-Redis",
},

Redis Desktop Manager ile de bu ayarlamaları kolayca yapabilmek için Gerekli URL bağlantısını paylaşıyorum . Buradan AnotherRedisDesktopManager 1.7.7 versiyonunu açıp UI kısmına erişebilirsiniz.
https://github.com/qishibo/AnotherRedisDesktopManager/releases

# Teknolojiler
.NET 9.0
Nethereum.Web3
Entity Framework Core (InMemory)
StackExchange.Redis
Hangfire (MemoryStorage)
Serilog
Swagger
AutoMapper


# Başlangıç için : 
Redis'i başlatın
Uygulamayı klonlayıp açın . VisualStudio üzerinden .sln dosyasını kullanın.
Veya doğrudan `EthereumNotifier.exe` çalıştırın.

**Hangfire Dashboard & Swagger**  
- `http://localhost:5000/hangfire`  
- `http://localhost:5000/swagger`

## API Endpoints

### Transaction
- **GET** `/api/transaction/filter` – Dinamik filtre & pagination  
- **GET** `/api/transaction/by-hash/{hash}` – Tekil transaction  
- **GET** `/api/transaction/all` – Tüm kaydedilmiş transactionlar

### Notification Channel
- **POST** `/api/notificationchannel` – Yeni kanal oluştur  
- **GET** `/api/notificationchannel` – Kanal listele (query: userId, channelType)  
- **PUT** `/api/notificationchannel` – Kanal güncelle  
- **DELETE** `/api/notificationchannel/{id}` – Kanal sil

### Notification Log
- **GET** `/api/notificationlog` – Bildirim geçmişi listele (query: userId, channels[], fromDate, toDate)


