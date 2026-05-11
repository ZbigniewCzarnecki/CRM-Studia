# SalonCRM — Dziennik postępu wdrożenia

---

## Etap 1: Inicjalizacja projektu i struktury

**Status:** ✅ Ukończony

- Zainicjalizowano repozytorium git
- Utworzono solution `SalonCRM.sln`
- Dodano 4 projekty:
  - `src/SalonCRM.Models` — Class Library (modele danych)
  - `src/SalonCRM.Server` — Console App (serwer TCP)
  - `src/SalonCRM.Client` — Console App (klient TCP)
  - `tests/SalonCRM.Tests` — xUnit (testy)
- Dodano referencje między projektami (Server → Models, Client → Models, Tests → wszystkie)
- Dodano `.gitignore` dla .NET

**Decyzje:**
- Targety .NET 8 we wszystkich projektach — najnowszy LTS, wymaganie prowadzącego
- `nullable enable` + `implicit usings` we wszystkich projektach — standard C# 12
- Projekt Tests referencjonuje wszystkie 3 projekty, aby pokryć testy jednostkowe, integracyjne i E2E

**Commit:** `feat: initialize SalonCRM solution structure with 4 projects`

---

## Etap 2: Modele danych (`SalonCRM.Models`)

**Status:** ✅ Ukończony

- Zaimplementowano `Client.cs` — pola: FirstName, LastName, PhoneNumber, Email; Equals po Email
- Zaimplementowano `Service.cs` — pola: Name, Description, DurationMinutes, Price; Equals po Name
- Zaimplementowano `Appointment.cs` — pola: ClientName, ServiceName, Date, TotalAmount; Equals po ClientName+Date
- Każda klasa ma:
  - Konstruktor parametryczny + bezparametrowy (wymagany przez JSON deserializację)
  - `ToString()` w formacie określonym w planie
  - `Equals()` + `GetHashCode()` zgodnie ze specyfikacją
- Serializacja przez `System.Text.Json` (bez deprecated BinaryFormatter)

**Decyzje:**
- Bezparametrowy konstruktor zamiast `[Serializable]` — JSON deserializacja wymaga pustego konstruktora lub `[JsonConstructor]`; wybrano prostsze podejście
- `GetHashCode()` oparte na tym samym polu co `Equals()` — spójność kontraktu equals/hashcode

**Commit:** `feat: implement Client, Service, Appointment models with JSON-compatible constructors`

---

## Etap 3: Serwer — DataStore i inicjalizacja danych

**Status:** ✅ Ukończony

- Zaimplementowano `DataStore.cs` z `Dictionary<string, object>` (12 obiektów: 4 Client, 4 Service, 4 Appointment)
- Polskie imiona i realistyczne usługi kosmetologiczne zgodnie z planem
- Metoda `GetByClassName(string)` zwraca `List<object>` odpowiednich obiektów
- Dla nieznanej klasy: celowo zwraca obiekty `Service` (wymóg prowadzącego — symulacja błędu deserializacji)

**Commit:** `feat: implement DataStore with 12 sample objects and GetByClassName fallback`

---

## Etap 4 & 5: Serwer TCP — nasłuchiwanie i obsługa klientów

**Status:** ✅ Ukończony

- `SalonServer.cs` — TcpListener na porcie 5000, MAX_CLIENTS=3, Semaphore(3,3) do zarządzania limitem
- Pętla `AcceptTcpClient()` + nowy `Thread` per klient
- `ClientHandler.cs` — obsługa protokołu:
  - Parsowanie `CONNECT:<id>` → sprawdzenie limitu → `OK` / `REFUSED`
  - Pętla `GET:<className>` → serializacja JSON → `DATA:<json>`
  - `Thread.Sleep(Random.Next(500, 2000))` — symulacja obciążenia
  - Obsługa `EXIT` — zwolnienie semafora, zamknięcie połączenia
- Logowanie na konsolę: `[Serwer] Klient #id...`

**Decyzje:**
- `Semaphore` zamiast prostego licznika z `lock` — Semaphore lepiej nadaje się do ograniczania współbieżnego dostępu, kod jest czytelniejszy i thread-safe bez dodatkowej synchronizacji
- `StreamReader`/`StreamWriter` z `AutoFlush = true` — wymaganie planu, zapobiega zawieszeniu na buforowaniu

**Commit:** `feat: implement TCP server with Semaphore-based connection limiting and ClientHandler`

---

## Etap 6: Klient TCP (`SalonCRM.Client`)

**Status:** ✅ Ukończony

- `SalonClient.cs` — TcpClient łączący się z localhost:5000
- Wysyła `CONNECT:<id>`, obsługuje `OK`/`REFUSED`
- Pętla 4 żądań: `GET:Client`, `GET:Service`, `GET:Appointment`, `GET:Pet`
- Deserializacja JSON do odpowiednich typów, przetwarzanie LINQ:
  - Clients: `Where(c => c.LastName.StartsWith("K"))`  
  - Services: `OrderByDescending(s => s.Price)`
  - Appointments: `Where(a => a.Date > DateTime.Now).Count()`
  - Pet: celowy błąd — łapany `JsonException` i wypisywany jako błąd rzutowania
- Logowanie: `[Klient #id] ...`

**Commit:** `feat: implement TCP client with LINQ processing and cast-error handling`

---

## Etap 7–9: Testy (`SalonCRM.Tests`)

**Status:** ✅ Ukończony

**Testy jednostkowe (`Unit/`):**
- `ClientModelTests` — Equals po Email, ToString, konstruktor
- `ServiceModelTests` — Equals po Name, ToString z ceną i czasem
- `AppointmentModelTests` — Equals po ClientName+Date, ToString
- `DataStoreTests` — inicjalizacja 12 obiektów, GetByClassName, fallback dla nieznanych klas

**Testy integracyjne (`Integration/`):**
- `SerializationTests` — serialize/deserialize Client, Service, Appointment; błąd przy złym typie
- `CommunicationTests` — parsing protokołu CONNECT/GET/EXIT

**Testy E2E (`E2E/`):**
- `FullSystemTests` — uruchomienie prawdziwego serwera na losowym porcie, N klientów ≤ MAX_CLIENTS dostaje OK, MAX_CLIENTS+1 dostaje REFUSED, żądanie "Pet" wywołuje błąd deserializacji

**Decyzje:**
- Testy E2E używają losowego portu (`TcpListener(IPAddress.Loopback, 0)`) — unika konfliktów portów przy równoległym uruchomieniu testów
- `Task.Delay` zamiast `Thread.Sleep` w testach async — nie blokuje wątku testowego

**Commit:** `test: add unit, integration and E2E tests with real TCP server`

---

## Etap 10: README.md

**Status:** ✅ Ukończony

- Tytuł projektu i skład zespołu
- Opis działania aplikacji
- Instrukcja techniczna (build, run server, run client, run tests)
- Deklaracja użycia AI (placeholder do uzupełnienia)

**Commit:** `docs: add README with setup instructions and AI usage declaration placeholder`

---

## Podsumowanie

| Etap | Opis | Status |
|------|------|--------|
| 1 | Inicjalizacja solution | ✅ |
| 2 | Modele danych | ✅ |
| 3 | DataStore | ✅ |
| 4–5 | Serwer TCP | ✅ |
| 6 | Klient TCP | ✅ |
| 7–9 | Testy (Unit/Integration/E2E) | ✅ |
| 10 | README | ✅ |
