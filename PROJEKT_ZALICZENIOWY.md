# Pliki wymagane na zaliczenie

Projekt zaliczeniowy: **aplikacja klient-serwer TCP w C# .NET 8**

> Pliki poniżej są **zamrożone** — nie powinny być zmieniane po oddaniu.

---

## Wymagane projekty

### `src/SalonCRM.Models/` — modele danych
| Plik | Opis |
|------|------|
| `Client.cs` | Klient salonu — Equals po Email |
| `Service.cs` | Usługa — Equals po Name |
| `Appointment.cs` | Wizyta — Equals po ClientName+Date |

### `src/SalonCRM.Server/` — serwer TCP (port 5000)
| Plik | Opis |
|------|------|
| `SalonServer.cs` | TcpListener, MAX_CLIENTS=3, SemaphoreSlim |
| `ClientHandler.cs` | Protokół CONNECT/GET/EXIT, Thread per klient, Thread.Sleep(500-2000ms) |
| `DataStore.cs` | 12 obiektów (4×Client, 4×Service, 4×Appointment), fallback na Service |
| `Program.cs` | Uruchomienie serwera |

### `src/SalonCRM.Client/` — klient TCP
| Plik | Opis |
|------|------|
| `SalonClient.cs` | TcpClient, CONNECT, 4× GET (Client/Service/Appointment/Pet), LINQ, JsonException |
| `Program.cs` | Uruchomienie klienta (opcjonalne ID jako arg) |

### `tests/SalonCRM.Tests/` — testy (38/38 zielonych)
| Plik | Typ | Testów |
|------|-----|--------|
| `Unit/ClientModelTests.cs` | Unit | 4 |
| `Unit/ServiceModelTests.cs` | Unit | 4 |
| `Unit/AppointmentModelTests.cs` | Unit | 4 |
| `Unit/DataStoreTests.cs` | Unit | 2 |
| `Integration/SerializationTests.cs` | Integration | 4 |
| `Integration/CommunicationTests.cs` | Integration | 4 |
| `E2E/FullSystemTests.cs` | E2E | 5 |

---

## Uruchomienie na zaliczenie

```bash
# Terminal 1 — serwer
dotnet run --project src/SalonCRM.Server

# Terminal 2 — klient (ID opcjonalne)
dotnet run --project src/SalonCRM.Client -- 1

# Testy
dotnet test tests/SalonCRM.Tests
```

---

## Czego NIE dotykać

Projekty `SalonCRM.Web` i wszystkie pliki poza listą powyżej to **część rozwijana** (aplikacja webowa CRM) — nie należą do wymagań zaliczeniowych.
