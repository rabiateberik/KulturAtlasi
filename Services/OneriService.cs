using KulturAtlasi.Data;
using KulturAtlasi.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace KulturAtlasi.Services
{
    public class OneriService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OneriService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _httpClient = new HttpClient();
            // API anahtarını appsettings.json dosyasından çekiyoruz
            _apiKey = configuration["Groq:ApiKey"];
        }

        // Öneri metodu
        public async Task<List<OneriItem>> GetOneriAsync(string userId)
        {
            var sonKitap = await _context.Kitaplar.Where(k => k.KullaniciID == userId).OrderByDescending(k => k.KayitTarihi).FirstOrDefaultAsync();
            var sonFilm = await _context.Filmler.Where(f => f.KullaniciID == userId).OrderByDescending(f => f.KayitTarihi).FirstOrDefaultAsync();

            string kullaniciDurumu = "";
            string turHedefi = "Kitap";

            if (sonKitap != null) kullaniciDurumu += $"Okuduğu son kitap: '{sonKitap.Baslik}' ({sonKitap.Yazar}). ";
            if (sonFilm != null)
            {
                kullaniciDurumu += $"İzlediği son film: '{sonFilm.Baslik}' ({sonFilm.Yonetmen}). ";
                if (sonKitap == null || sonFilm.KayitTarihi > sonKitap.KayitTarihi) turHedefi = "Film";
            }

            if (string.IsNullOrEmpty(kullaniciDurumu)) kullaniciDurumu = "Yeni kullanıcı. Popüler klasikleri öner.";

            string prompt = $@"
                Durum: {kullaniciDurumu}
                Görev: Bu kullanıcı için 3 tane {turHedefi} öner.
                Çıktıyı şu JSON formatında ver:
                {{
                    ""oneriler"": [
                        {{ ""Baslik"": ""Eser Adı"", ""Aciklama"": ""Kısa açıklama"" }},
                        {{ ""Baslik"": ""Eser Adı"", ""Aciklama"": ""Kısa açıklama"" }}
                    ]
                }}
            ";

            var url = "https://api.groq.com/openai/v1/chat/completions";

            var bodyObj = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen JSON formatında cevap veren bir kültür asistanısın. Sadece JSON döndür." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" },
                temperature = 0.5
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<OneriItem> { new OneriItem { Baslik = "Hata", Aciklama = $"API Hatası: {response.StatusCode}" } };
                }

                dynamic data = JsonConvert.DeserializeObject(responseString);
                string aiText = data.choices[0].message.content;
                aiText = aiText.Replace("```json", "").Replace("```", "").Trim();

                try
                {
                    return JsonConvert.DeserializeObject<List<OneriItem>>(aiText);
                }
                catch
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject(aiText);
                    string innerJson = JsonConvert.SerializeObject(jsonObject.oneriler);
                    return JsonConvert.DeserializeObject<List<OneriItem>>(innerJson);
                }
            }
            catch (Exception ex)
            {
                return new List<OneriItem> { new OneriItem { Baslik = "Sistem Hatası", Aciklama = ex.Message } };
            }
        }

        // Kültür analizi metodu
        public async Task<AnalizSonucu> KulturAnaliziYapAsync(string userId)
        {
            var kitaplar = await _context.Kitaplar.Where(k => k.KullaniciID == userId).Select(k => k.Baslik + " (" + k.Yazar + ")").ToListAsync();
            var filmler = await _context.Filmler.Where(f => f.KullaniciID == userId).Select(f => f.Baslik + " (" + f.Yonetmen + ")").ToListAsync();

            if (kitaplar.Count == 0 && filmler.Count == 0)
            {
                return new AnalizSonucu
                {
                    Unvan = "Gizemli Yolcu",
                    Aciklama = "Henüz çantasında hiç kitap veya film yok. Kültür yolculuğuna yeni başlıyor.",
                    RuhIkizi = "Bilinmiyor"
                };
            }

            string veriListesi = "Kitaplar: " + string.Join(", ", kitaplar) + ". Filmler: " + string.Join(", ", filmler);

            string prompt = $@"
                Sen eğlenceli ve derinlikli bir psikologsun.
                Aşağıdaki kitap ve film listesine sahip birinin karakter analizini yap.
                
                LİSTE: {veriListesi}

                GÖREV:
                1. Ona havalı bir 'Kültür Ünvanı' ver (Örn: Melankolik Filozof, Galaktik Gezgin, Romantik Şair vb.)
                2. Karakterini 2-3 cümleyle eğlenceli şekilde analiz et.
                3. Tarihten veya kurgusal dünyadan 'Ruh İkizi'ni bul.

                ÇOK ÖNEMLİ: Cevabı SADECE şu JSON formatında ver:
                {{
                    ""Unvan"": ""..."",
                    ""Aciklama"": ""..."",
                    ""RuhIkizi"": ""...""
                }}
            ";

            var url = "https://api.groq.com/openai/v1/chat/completions";
            var bodyObj = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen JSON döndüren bir sistemsin. Türkçe cevap ver." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" },
                temperature = 0.7
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseString);
                string aiText = data.choices[0].message.content;

                return JsonConvert.DeserializeObject<AnalizSonucu>(aiText);
            }
            catch
            {
                return new AnalizSonucu { Unvan = "Hata", Aciklama = "Yapay zeka şu an derin düşüncelere daldı.", RuhIkizi = "-" };
            }
        }

        // İkna et metodu
        public async Task<string> IknaEtAsync(string userId, int icerikId, string tur)
        {
            var gecmisKitaplar = await _context.Kitaplar
                .Where(k => k.KullaniciID == userId)
                .OrderByDescending(k => k.KayitTarihi)
                .Take(3)
                .Select(k => k.Baslik + " (" + k.Yazar + ")")
                .ToListAsync();

            var gecmisFilmler = await _context.Filmler
                .Where(f => f.KullaniciID == userId)
                .OrderByDescending(f => f.KayitTarihi)
                .Take(3)
                .Select(f => f.Baslik + " (" + f.Yonetmen + ")")
                .ToListAsync();

            var gecmisDiziler = await _context.Diziler
                .Where(d => d.KullaniciID == userId)
                .OrderByDescending(d => d.KayitTarihi)
                .Take(3)
                .Select(d => d.Baslik)
                .ToListAsync();

            var gecmisMuzikler = await _context.Muzikler
                .Where(m => m.KullaniciID == userId)
                .OrderByDescending(m => m.KayitTarihi)
                .Take(3)
                .Select(m => m.Baslik + " - " + m.Sanatci)
                .ToListAsync();

            var gecmisSeyahatler = await _context.Seyahatler
                .Where(s => s.KullaniciID == userId)
                .OrderByDescending(s => s.ZiyaretTarihi)
                .Take(3)
                .Select(s => s.Sehir + "/" + s.Ulke)
                .ToListAsync();

            List<string> tumZevkler = new List<string>();
            if (gecmisKitaplar.Any()) tumZevkler.Add($"Kitaplar: {string.Join(", ", gecmisKitaplar)}");
            if (gecmisFilmler.Any()) tumZevkler.Add($"Filmler: {string.Join(", ", gecmisFilmler)}");
            if (gecmisDiziler.Any()) tumZevkler.Add($"Diziler: {string.Join(", ", gecmisDiziler)}");
            if (gecmisMuzikler.Any()) tumZevkler.Add($"Müzikler: {string.Join(", ", gecmisMuzikler)}");
            if (gecmisSeyahatler.Any()) tumZevkler.Add($"Gezdiği Yerler: {string.Join(", ", gecmisSeyahatler)}");

            string kullaniciZevki = "Henüz fazla verisi yok, genel popüler kültürü seviyor varsay.";
            if (tumZevkler.Any())
            {
                kullaniciZevki = string.Join(". ", tumZevkler);
            }

            string hedefIcerik = "";
            string prompt = "";

            if (tur == "Kitap")
            {
                var k = await _context.Kitaplar.FindAsync(icerikId);
                hedefIcerik = (k != null) ? $"Kitap: {k.Baslik}, Yazar: {k.Yazar}" : "Bilinmeyen Eser";
            }
            else if (tur == "Film")
            {
                var f = await _context.Filmler.FindAsync(icerikId);
                hedefIcerik = (f != null) ? $"Film: {f.Baslik}, Yönetmen: {f.Yonetmen}" : "Bilinmeyen Yapım";
            }
            else if (tur == "Dizi")
            {
                var d = await _context.Diziler.FindAsync(icerikId);
                hedefIcerik = (d != null) ? $"Dizi: {d.Baslik}, Yönetmen: {d.Yonetmen}" : "Bilinmeyen Yapım";
            }
            else if (tur == "Müzik")
            {
                var m = await _context.Muzikler.FindAsync(icerikId);
                hedefIcerik = (m != null) ? $"Şarkı/Albüm: {m.Baslik}, Sanatçı: {m.Sanatci ?? "Bilinmiyor"}, Tür: {m.Tur ?? "Genel"}" : "Bilinmeyen Müzik";
            }
            else if (tur == "Seyahat")
            {
                var s = await _context.Seyahatler.FindAsync(icerikId);
                string yerBilgisi = (s != null) ? $"{s.Sehir}/{s.Ulke}" : "Bilinmiyor";
                hedefIcerik = (s != null) ? $"Başlık: {s.Baslik}, Konum: {yerBilgisi}, Tür: {s.Tur ?? "Genel Gezi"}" : "Bilinmeyen Rota";

                prompt = $@"
                    Sen enerjik ve gezgin bir seyahat gurususun.
                    KULLANICI PROFİLİ: {kullaniciZevki}
                    ŞU AN BAKTIĞI YER: {hedefIcerik}
                    
                    GÖREV:
                    Kullanıcı buraya gitmekte kararsız. Ona buranın atmosferini ve neden gitmesi gerektiğini anlat.
                    'Orada şunu yemelisin' veya 'Şu manzarayı kaçırma' gibi spesifik, iştah kabartan şeyler söyle.
                    Kısa ve vurucu olsun (Maks 3 cümle).
                ";
            }

            if (string.IsNullOrEmpty(prompt))
            {
                prompt = $@"
                    Sen samimi, esprili ve ikna kabiliyeti yüksek bir arkadaşsın.
                    KULLANICI PROFİLİ: {kullaniciZevki}
                    ŞU AN BAKTIĞI İÇERİK: {hedefIcerik}
                    
                    GÖREV:
                    Kullanıcı bu içeriğe bakıyor ama kararsız. Geçmiş zevklerine göre onu ikna et.
                    
                    KURALLAR:
                    1. 'Kanka', 'Hocam' gibi samimi hitaplar kullan.
                    2. Kısa tut (maksimum 3 cümle).
                    3. Dürüst ol ama merak uyandır.
                ";
            }

            var url = "https://api.groq.com/openai/v1/chat/completions";
            var bodyObj = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen Türkçe konuşan samimi bir arkadaşsın." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(responseString);
                return data.choices[0].message.content;
            }
            catch
            {
                return "Şu an ilham perilerim grevde ama bence buna bir şans vermelisin!";
            }
        }

        // Benzerini öner metodu
        public async Task<string> BenzeriniOnerAsync(string eserAdi, string tur)
        {
            string prompt = "";

            if (tur == "Seyahat")
            {
                prompt = $@"
                    Kullanıcı '{eserAdi}' isimli yere/şehre seyahat etti ve burayı çok sevdi.
                    
                    GÖREV:
                    Ona atmosferi, kültürü veya hissettirdiği duygular bakımından benzer 3 tane başka 'GEZİ ROTASI' (Şehir, Ülke veya Mekan) öner.
                    
                    KURALLAR:
                    1. ASLA kitap, film veya şarkı önerme. Sadece gidilecek FİZİKSEL YERLER öner.
                    2. Samimi bir rehber gibi konuş.
                    3. Format şöyle olsun: 
                       - Rota 1: [Yer Adı] -> Neden benziyor?
                       - Rota 2: [Yer Adı] -> Neden benziyor?
                       - Rota 3: [Yer Adı] -> Neden benziyor?
                ";
            }
            else if (tur == "Müzik" || tur == "Albüm")
            {
                prompt = $@"
                    Kullanıcı '{eserAdi}' isimli şarkıyı/albümü dinlemeyi çok seviyor.
                    
                    GÖREV:
                    Ona müzikal tarzı, ritmi ve duygusu benzer 3 tane başka şarkı veya albüm öner.
                    
                    KURALLAR:
                    1. Müzik gurusu gibi konuş.
                    2. Format:
                       - Öneri 1: [Şarkı - Sanatçı] -> Neden benziyor?
                       - Öneri 2: [Şarkı - Sanatçı] -> Neden benziyor?
                       - Öneri 3: [Şarkı - Sanatçı] -> Neden benziyor?
                ";
            }
            else
            {
                prompt = $@"
                    Kullanıcı '{eserAdi}' isimli {tur} eserini çok sevdi.
                    
                    GÖREV:
                    Ona tam olarak bu eserin tadında, atmosferi ve konusu benzer 3 tane başka {tur} öner.
                    
                    KURALLAR:
                    1. Samimi bir arkadaş gibi konuş.
                    2. Önerdiğin eserlerin neden benzediğini tek cümleyle açıkla.
                    3. Format şöyle olsun: 
                       - Eser 1: Neden benziyor?
                       - Eser 2: Neden benziyor?
                       - Eser 3: Neden benziyor?
                ";
            }

            var url = "https://api.groq.com/openai/v1/chat/completions";
            var bodyObj = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen Türkçe konuşan bir kültür ve seyahat uzmanısın." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(bodyObj), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(responseString);
                return data.choices[0].message.content;
            }
            catch
            {
                return "Şu an benzerlerini hatırlayamadım ama keşfetmeye devam et!";
            }
        }
    }

    public class OneriItem
    {
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
    }
}