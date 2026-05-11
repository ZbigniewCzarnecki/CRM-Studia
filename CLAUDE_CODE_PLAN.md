# PLAN PROJEKTU: SalonCRM — Aplikacja Klient-Serwer (C# / .NET)

## KONTEKST

Projekt zaliczeniowy z przedmiotu "Programowanie zaawansowane" (50% oceny).
Temat: **CRM Salonu Kosmetologicznego** — aplikacja klient-serwer z trzema klasami modeli.
Prowadzący: dr inż. Piotr Bobiński.
Termin: 26 czerwca 2026, 23:59.

**WAŻNE OGRANICZENIA:**
- Technologia: **C# (.NET)**, czyste sockety TCP — BEZ frameworków (ASP.NET, gRPC, SignalR itp.)
- Prowadzący wymaga podejścia "niskopoziomowego" — ręczne sockety, wątki, serializacja
- Dozwolone: `System.Net.Sockets`, `System.Threading`, `System.Text.Json`, `xUnit`

---

## STRUKTURA PROJEKTU

```
SalonCRM/
├── SalonCRM.sln
├── README.md
├── src/
│   ├── SalonCRM.Models/          ← Modele danych (Class Library)
│   │   ├── SalonCRM.Models.csproj
│   │   ├── Client.cs
│   │   ├── Service.cs
│   │   └── Appointment.cs
│   ├── SalonCRM.Server/          ← Serwer TCP (Console App)
│   │   ├── SalonCRM.Server.csproj
│   │   ├── Program.cs
│   │   ├── SalonServer.cs
│   │   └── ClientHandler.cs
│   └── SalonCRM.Client/          ← Klient TCP (Console App)
│       ├── SalonCRM.Client.csproj
│       ├── Program.cs
│       └── SalonClient.cs
└── tests/
    └── SalonCRM.Tests/           ← Testy (xUnit)
        ├── SalonCRM.Tests.csproj
        ├── Unit/
        │   ├── ClientModelTests.cs
        │   ├── ServiceModelTests.cs
        │   ├── AppointmentModelTests.cs
        │   └── DataStoreTests.cs
        ├── Integration/
        │   ├── SerializationTests.cs
        │   └── CommunicationTests.cs
        └── E2E/
            └── FullSystemTests.cs
```

---

## FAZA 1: MODELE DANYCH (`SalonCRM.Models`)

### 1.1 Klasa `Client` (klient salonu)
```
Pola:
- string FirstName       (imię)
- string LastName         (nazwisko)
- string PhoneNumber      (telefon)
- string Email            (email)

Metody:
- Konstruktor parametryczny
- ToString() → "Client { FirstName, LastName, Phone, Email }"
- Equals(object?) + GetHashCode() → porównanie po Email (unikalny)
- Implementuje interfejs ISerializable LUB używa atrybutu [Serializable]
```

### 1.2 Klasa `Service` (usługa kosmetologiczna)
```
Pola:
- string Name             (nazwa usługi, np. "Manicure hybrydowy")
- string Description      (opis)
- int DurationMinutes     (czas trwania w minutach)
- decimal Price           (cena w PLN)

Metody:
- Konstruktor parametryczny
- ToString() → "Service { Name, Duration min, Price PLN }"
- Equals(object?) + GetHashCode() → porównanie po Name
```

### 1.3 Klasa `Appointment` (wizyta / rezerwacja)
```
Pola:
- string ClientName       (imię i nazwisko klienta)
- string ServiceName      (nazwa usługi)
- DateTime Date           (data i godzina wizyty)
- decimal TotalAmount     (kwota do zapłaty)

Metody:
- Konstruktor parametryczny
- ToString() → "Appointment { ClientName, ServiceName, Date, Amount PLN }"
- Equals(object?) + GetHashCode() → porównanie po ClientName + Date
```

### Serializacja
- Użyj `System.Text.Json` (JsonSerializer) do serializacji/deserializacji obiektów
- Każda klasa musi mieć bezparametrowy konstruktor (dla deserializacji JSON) LUB odpowiednie atrybuty
- Alternatywnie: BinaryFormatter (ale jest deprecated od .NET 8+), więc lepiej JSON

---

## FAZA 2: SERWER (`SalonCRM.Server`)

### 2.1 Inicjalizacja danych (`SalonServer.cs`)
```
- Utwórz Dictionary<string, object> _dataStore
- Dodaj 4 obiekty Client:   client_1, client_2, client_3, client_4
- Dodaj 4 obiekty Service:  service_1, service_2, service_3, service_4
- Dodaj 4 obiekty Appointment: appointment_1, appointment_2, appointment_3, appointment_4
- Dane powinny być realistyczne (polskie imiona, prawdziwe usługi kosmetologiczne)
```

Przykładowe dane:
```
client_1: Anna Kowalska, 500-100-200, anna@email.pl
client_2: Maria Nowak, 600-300-400, maria@email.pl
client_3: Katarzyna Wiśniewska, 700-500-600, kasia@email.pl
client_4: Joanna Zielińska, 800-700-800, joanna@email.pl

service_1: Manicure hybrydowy, 60 min, 120 PLN
service_2: Pedicure klasyczny, 45 min, 90 PLN
service_3: Mikrodermabrazja, 30 min, 150 PLN
service_4: Depilacja laserowa, 40 min, 200 PLN

appointment_1: Anna Kowalska, Manicure hybrydowy, 2026-06-15 10:00, 120 PLN
appointment_2: Maria Nowak, Pedicure klasyczny, 2026-06-15 12:00, 90 PLN
appointment_3: Katarzyna Wiśniewska, Mikrodermabrazja, 2026-06-16 09:00, 150 PLN
appointment_4: Joanna Zielińska, Depilacja laserowa, 2026-06-16 14:00, 200 PLN
```

### 2.2 Nasłuchiwanie (`SalonServer.cs`)
```
- TcpListener na porcie 5000 (lub konfigurowalnym)
- Stała MAX_CLIENTS = 3 (lub inna sensowna wartość)
- Licznik aktywnych klientów (int z lock/Interlocked/Semaphore)
- Pętla Accept → sprawdź limit → nowy Thread/Task dla klienta
```

### 2.3 Obsługa klienta (`ClientHandler.cs`)
```
Algorytm:
1. Odbierz ID klienta (liczba) ze strumienia
2. Sprawdź limit MAX_CLIENTS:
   - Jeśli przekroczony → wyślij "REFUSED", zamknij połączenie, wypisz na konsoli
   - Jeśli OK → wyślij "OK", wypisz na konsoli
3. Odbierz żądanie (nazwa klasy, np. "Client", "Service", "Appointment")
4. Sprawdź czy klasa istnieje w mapie:
   - TAK → pobierz obiekty tej klasy, serializuj do JSON, wyślij
   - NIE → wyślij celowo obiekty INNEJ klasy (np. zamiast "Pet" wyślij obiekty Service)
5. Dodaj losowe opóźnienie Thread.Sleep(Random.Next(500, 2000)) — symulacja obciążenia
6. Powtarzaj od kroku 3 aż klient zakończy (wyśle np. "EXIT")
7. Zmniejsz licznik klientów, zamknij połączenie
```

### 2.4 Protokół komunikacji (tekstowy, linia po linii)
```
Format wiadomości przez NetworkStream + StreamReader/StreamWriter:

Klient → Serwer:  "CONNECT:<clientId>"       np. "CONNECT:42"
Serwer → Klient:  "OK" lub "REFUSED"

Klient → Serwer:  "GET:<className>"           np. "GET:Client"
Serwer → Klient:  "DATA:<json>"               np. "DATA:[{...},{...}]"
                   lub "DATA:<json_innego_typu>"  (celowy zły typ)

Klient → Serwer:  "EXIT"
Serwer → Klient:  (zamknięcie połączenia)
```

---

## FAZA 3: KLIENT (`SalonCRM.Client`)

### 3.1 Logika klienta (`SalonClient.cs`)
```
Algorytm:
1. Połącz się z serwerem przez TcpClient (localhost:5000)
2. Wyślij "CONNECT:<id>"
3. Odbierz odpowiedź:
   - "REFUSED" → wypisz info, zakończ
   - "OK" → kontynuuj
4. Pętla (np. 4 iteracje, różne klasy + 1 nieistniejąca):
   a. Wyślij "GET:Client" / "GET:Service" / "GET:Appointment" / "GET:Pet"
   b. Odbierz "DATA:<json>"
   c. Spróbuj deserializować jako List<ŻądanaKlasa>
   d. Jeśli się uda → przetwórz LINQ/Stream:
      - np. .Where(x => x.Price > 100).Select(x => x.ToString())
      - Wypisz na konsoli z prefixem "[Klient #id]"
   e. Jeśli InvalidCastException / JsonException → złap wyjątek,
      wypisz "[Klient #id] Błąd rzutowania: oczekiwano Client, otrzymano inny typ"
5. Wyślij "EXIT", zamknij połączenie
```

### 3.2 Przetwarzanie strumieniowe (LINQ)
```
Przykłady operacji LINQ na odebranych kolekcjach:
- clients.Where(c => c.LastName.StartsWith("K")).ToList()
- services.OrderByDescending(s => s.Price).Select(s => s.ToString())
- appointments.Where(a => a.Date > DateTime.Now).Count()
```

---

## FAZA 4: TESTY (`SalonCRM.Tests`)

### 4.1 Testy jednostkowe (`Unit/`)
```
ClientModelTests.cs:
- Equals_SameEmail_ReturnsTrue()
- Equals_DifferentEmail_ReturnsFalse()
- ToString_ReturnsFormattedString()
- Constructor_SetsAllProperties()

ServiceModelTests.cs:
- Equals_SameName_ReturnsTrue()
- ToString_ContainsPriceAndDuration()

AppointmentModelTests.cs:
- Equals_SameClientAndDate_ReturnsTrue()
- ToString_ContainsAllInfo()

DataStoreTests.cs:
- InitializeStore_Contains12Objects()
- GetObjectsByClass_ReturnsCorrectType("Client" → 4 obiekty Client)
- GetObjectsByClass_UnknownClass_ReturnsFallbackObjects()
- KeyFormat_IsCorrect("client_1", "service_2" itp.)
```

### 4.2 Testy integracyjne (`Integration/`)
```
SerializationTests.cs:
- Serialize_ClientList_DeserializesCorrectly()
- Serialize_ServiceList_DeserializesCorrectly()
- Deserialize_WrongType_ThrowsException()
  (serializuj List<Service>, deserializuj jako List<Client> → wyjątek)

CommunicationTests.cs:
- MockSocket_SendAndReceive_ProtocolMessages()
  (testuj parsing "CONNECT:42" → id=42, "GET:Client" → className="Client")
- MockSocket_DataExchange_SerializedObjects()
```

### 4.3 Testy E2E / akceptacyjne (`E2E/`)
```
FullSystemTests.cs:
- StartServer_ConnectNClients_AllGetOK()
  (N <= MAX_CLIENTS, wszystkie połączenia zaakceptowane)
- StartServer_ExceedMaxClients_ExtraClientGetRefused()
  (MAX_CLIENTS+1 klient dostaje REFUSED)
- Client_RequestUnknownClass_HandlesCastError()
  (klient prosi o "Pet", dostaje dane innego typu, łapie wyjątek)
- Client_RequestKnownClass_ReceivesCorrectData()
  (klient prosi o "Service", dostaje 4 obiekty Service)

Uwaga: Testy E2E uruchamiają prawdziwy serwer na losowym porcie,
łączą prawdziwych klientów i weryfikują wyniki.
```

---

## FAZA 5: README.md

Plik README.md musi zawierać:

1. **Tytuł projektu** — SalonCRM: System zarządzania salonem kosmetologicznym
2. **Skład zespołu** — imiona, nazwiska, przypisanie odpowiedzialności
3. **Opis projektu** — krótki opis co robi aplikacja
4. **Instrukcja techniczna:**
   - Wymagania: .NET 8+ SDK
   - Kompilacja: `dotnet build`
   - Uruchomienie serwera: `dotnet run --project src/SalonCRM.Server`
   - Uruchomienie klienta: `dotnet run --project src/SalonCRM.Client`
   - Uruchomienie testów: `dotnet test`
5. **Deklaracja użycia AI** — UZUPEŁNIJ (patrz plik AI_USAGE_LOG.md)

---

## KOLEJNOŚĆ IMPLEMENTACJI

```
KROK 1: Inicjalizacja projektu
  - dotnet new sln -n SalonCRM
  - dotnet new classlib -n SalonCRM.Models -o src/SalonCRM.Models
  - dotnet new console -n SalonCRM.Server -o src/SalonCRM.Server
  - dotnet new console -n SalonCRM.Client -o src/SalonCRM.Client
  - dotnet new xunit -n SalonCRM.Tests -o tests/SalonCRM.Tests
  - Dodaj projekty do solution, dodaj referencje między projektami

KROK 2: Modele (Client.cs, Service.cs, Appointment.cs)
  - Implementuj pola, konstruktory, ToString(), Equals(), GetHashCode()
  - Upewnij się, że klasy mają bezparametrowe konstruktory (dla JSON)

KROK 3: Serwer — DataStore
  - Klasa/metoda inicjalizująca Dictionary<string, object> z 12 obiektami
  - Metoda GetByClassName(string className) → List<object>

KROK 4: Serwer — nasłuchiwanie TCP + wielowątkowość
  - TcpListener, MAX_CLIENTS, Semaphore/lock
  - Osobny wątek per klient

KROK 5: Serwer — ClientHandler (obsługa protokołu)
  - Parsing CONNECT/GET/EXIT
  - Serializacja JSON i wysyłanie
  - Losowe opóźnienia
  - Celowy zły typ dla nieznanych klas

KROK 6: Klient — połączenie i protokół
  - TcpClient, wysyłanie CONNECT, odbieranie OK/REFUSED
  - Pętla GET z różnymi klasami
  - Deserializacja, przetwarzanie LINQ, obsługa wyjątków

KROK 7: Testy jednostkowe
KROK 8: Testy integracyjne
KROK 9: Testy E2E
KROK 10: README.md — dokumentacja finalna
```

---

## UWAGI TECHNICZNE

- Używaj `StreamReader` / `StreamWriter` z `AutoFlush = true` na `NetworkStream`
- JSON: `System.Text.Json.JsonSerializer.Serialize()` / `Deserialize()`
- Do testów E2E: uruchom serwer na `port 0` (system przydzieli wolny port) lub losowy port 10000+
- Pamiętaj o `Dispose` / `using` na TcpClient, NetworkStream itp.
- Logowanie na konsolę serwera: "[Serwer] Klient #42 połączony", "[Serwer] Wysłano 4x Client do klienta #42"
- Logowanie na konsolę klienta: "[Klient #42] Otrzymano 4 obiekty typu Client"
