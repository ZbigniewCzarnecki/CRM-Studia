# SalonCRM — System zarządzania salonem kosmetologicznym

Projekt zaliczeniowy z przedmiotu **Programowanie zaawansowane**.  
Prowadzący: dr inż. Piotr Bobiński | Termin: 26 czerwca 2026

---

## Opis projektu

Aplikacja klient-serwer zaimplementowana w C# (.NET 8) przy użyciu czystych socketów TCP.  
System umożliwia zarządzanie danymi salonu kosmetologicznego: klientami, usługami i rezerwacjami.

- **Serwer** — nasłuchuje na porcie 5000, obsługuje maksymalnie 3 równoczesnych klientów (Semaphore), każdego w osobnym wątku
- **Klient** — łączy się z serwerem, pobiera dane różnych typów, przetwarza je za pomocą LINQ, obsługuje błędy deserializacji
- **Protokół** — tekstowy, linia po linii: `CONNECT:<id>` / `GET:<klasa>` / `DATA:<json>` / `EXIT`

### Modele danych

| Klasa | Pola | Equals po |
|-------|------|-----------|
| `Client` | FirstName, LastName, PhoneNumber, Email | Email |
| `Service` | Name, Description, DurationMinutes, Price | Name |
| `Appointment` | ClientName, ServiceName, Date, TotalAmount | ClientName + Date |

---

## Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Kompilacja

```bash
dotnet build
```

## Uruchomienie

**Serwer** (w pierwszym terminalu):
```bash
dotnet run --project src/SalonCRM.Server
```

**Klient** (w drugim terminalu, opcjonalnie z ID klienta):
```bash
dotnet run --project src/SalonCRM.Client -- 1
```

Można uruchomić kilku klientów jednocześnie (maks. 3 jednocześnie obsługiwanych).

## Testy

```bash
dotnet test
```

Projekt zawiera 38 testów: jednostkowe (Unit), integracyjne (Integration) oraz end-to-end (E2E) z prawdziwym serwerem TCP.

---

## Skład zespołu

<!-- UZUPEŁNIJ: imię i nazwisko, zakres odpowiedzialności -->

---

## Deklaracja użycia AI

<!-- UZUPEŁNIJ zgodnie z wymaganiami prowadzącego i plikiem AI_USAGE_LOG.md -->

Projekt był wspomagany narzędziem AI (Claude Code) w zakresie:
- generowania szkieletu kodu i struktury projektu
- implementacji protokołu TCP i logiki wielowątkowej
- pisania testów jednostkowych, integracyjnych i E2E

Cała logika biznesowa, architektura i decyzje projektowe zostały podjęte przez autora projektu.
