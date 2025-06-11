# EtheriumNotifier

> **Ethereum ağındaki işlemleri Hangfire ile periyodik takip eden,  
Redis & In-Memory DB ile idempotency ve cache yöneten,  
bildirim kanallarına simülasyon bildirimleri gönderen modüler bir .NET uygulaması.**

---

## Özellikler

### Transaction Okuma & Kaydetme
- Hangfire ile her dakika tetiklenen job, Nethereum ile son N bloktan transferleri çeker ve In-Memory veritabanına kaydeder.

### Idempotency & Cache (Redis)
- Redis ile transaction hash’leri hem tekil key (Tx-Hash:{hash}), hem dakika bazlı set (Tx-Minute:{yyyy-MM-dd-HH:mm}) olarak saklanır.
- Aynı hash ikinci kez işlenmez.

### Notification Channel Yönetimi
- CRUD endpoint’leri ile kullanıcı bazlı bildirim kanalları oluşturma/güncelleme/silme/listeleme.
- Her kullanıcı-tip kombinasyonu için tekil kayıt garantisi.

### Bildirim Simülasyonu & Log
- `appsettings.json` üzerinden gelen MinEthAmount (ör: 0.25 ETH) eşik değerine göre, başarılı ve minimum tutar üstündeki işlemler bildirilir.
- Simülasyon: Console çıktısı ve Notification entity’sine log kaydı.

### Notification Log Listeleme
- `/api/notificationlog` endpoint’i ile userId, kanal tipi, tarih aralığı (max 30 gün) filtreleriyle geçmiş bildirimler sorgulanabilir.

### Filtreleme & Pagination
- Transaction listeleri adres, hash, blok numarası, tutar, tarih aralığı gibi kriterlerle süzülebilir ve sayfalanabilir.

### Structured Logging (Serilog)
- Konsol ve dosya üzerinde structured log desteği.

---

##  Uygulamayı Çalıştırmanın 2 Yolu

### **1️⃣ Docker ile Hızlı Kurulum (Tavsiye Edilen Yol)**

> **Hiçbir ek kurulum gerekmez!  
.NET ve Redis otomatik kurulur, tek komutla çalışır.**

#### **Adımlar:**

1. **Projeyi bir klasöre çıkarın.**  
   (Tüm solution klasörleri, `Dockerfile` ve `docker-compose.yml` aynı yerde olmalı.)

2. **Docker Desktop uygulamasını başlatın.**

3. **Terminal/Komut İstemcisi açın.**  
   Proje ana dizinine (Dockerfile ve docker-compose.yml’nin olduğu klasör) gidin.

4. **Aşağıdaki komutu çalıştırın:**
    ```terminalden
    docker-compose up
    ```

5. **Arayüzlere erişim:**
   - Swagger: [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - Hangfire Dashboard: [http://localhost:5000/hangfire](http://localhost:5000/hangfire)

6. **Servisleri durdurmak için:**
    ```terminalden
    Ctrl + C
    ```
    veya yeni bir terminalde:
    ```
    docker-compose down
    ```

> **Not:** Redis otomatik başlatılır, ek işlem gerekmez. Port değiştirmek için `docker-compose.yml`'de portları düzenleyebilirsiniz.

---

### **2️⃣ Klasik Kurulum (Elle - Docker’sız)**

> **Bu yöntemde, Redis ve .NET 9.0 SDK'yı makinenize kurmanız gerekir.**

#### **Adımlar:**

1. [Redis’i indirin ve kurun](https://github.com/tporadowski/redis/releases) (kullanılan versiyon: 5.0.14.1).
   - UI için: [AnotherRedisDesktopManager](https://github.com/qishibo/AnotherRedisDesktopManager/releases)
2. Redis’i başlatın.
3. Proje klasörünü açın, Visual Studio ile `EtheriumNotifier.sln` dosyasını açın.
4. NuGet paketlerini yükleyin (`Restore`).
5. `appsettings.json` dosyasında Redis bağlantısının şöyle olduğuna emin olun:
    ```json
    "Redis": {
        "Host": "localhost",
        "Port": 6379,
        "InstanceName": "Localhost-Redis"
    }
    ```
6. Projeyi başlatın (`F5` veya `dotnet run`).
7. Swagger ve Hangfire’a erişim:
   - [http://localhost:5000/swagger](http://localhost:5000/swagger)
   - [http://localhost:5000/hangfire](http://localhost:5000/hangfire)
   (Portlar launchSettings.json’a göre farklı olabilir.)

---

## 🛰️ API Endpoints

### 🔹 Transaction
- **GET** `/api/transaction/filter` – Dinamik filtre & pagination  
- **GET** `/api/transaction/by-hash/{hash}` – Tekil transaction  
- **GET** `/api/transaction/all` – Tüm kaydedilmiş transactionlar

### 🔹 Notification Channel
- **POST** `/api/notificationchannel` – Yeni kanal oluştur  
- **GET** `/api/notificationchannel` – Kanal listele (query: userId, channelType)  
- **PUT** `/api/notificationchannel` – Kanal güncelle  
- **DELETE** `/api/notificationchannel/{id}` – Kanal sil

### 🔹 Notification Log
- **GET** `/api/notificationlog` – Bildirim geçmişi listele (query: userId, channels[], fromDate, toDate)

---

## 🧩 Kullanılan Teknolojiler

- .NET 9.0 SDK
- Docker Desktop (opsiyonel)
- Nethereum.Web3
- Entity Framework Core (InMemory)
- StackExchange.Redis
- Hangfire (MemoryStorage)
- Serilog
- Swagger
- AutoMapper

---

## 📄 Notlar

- Tüm temel ve ileri seviye özellikler, kurulum ve kullanım README’de detaylı olarak anlatılmıştır.
- Sürdürmesi ve genişletmesi kolay, **SOLID prensiplerine uygun mimari**.
- Docker ile dakikalar içinde çalışır.  
- Yardım için [issue açabilir](https://github.com/) veya proje sahibine ulaşabilirsiniz.

---


