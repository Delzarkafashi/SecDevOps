# SecDevOps – Demoapp med säkerhet genom hela kedjan

## Kursmål
Bygga en enkel applikation med inloggning och inlägg där **säkerhet är inbyggd i varje lager**: databas, backend, frontend och CI/CD. Fokus ligger på *förståelse först, implementation sen*.

---

## Grundidé
En enkel app där användare loggar in och hanterar inlägg.

### Roller
- **Reader**
  - Kan logga in
  - Kan läsa inlägg

- **Editor/Admin**
  - Kan läsa inlägg
  - Kan skapa inlägg
  - Kan radera inlägg

Rollerna används för att visa autentisering och auktorisering i praktiken.

---

## Kursupplägg (ordning är viktig)

1. Förstå systemet och hotbilden  
2. Bygga och säkra databasen  
3. Bygga säker backend  
4. Koppla enkel frontend  
5. Testa säkerhet automatiskt  
6. Säker leverans med CI/CD  

---

## Vecka 1 – Systemförståelse och hotbild
**Fokus:** Förstå vad vi bygger och vad som kan gå fel.

### Innehåll
- Vad är SecDevOps
- CIA-triaden
- Systemets delar:
  - Databas
  - Backend
  - Frontend
  - CI/CD
- Vanliga angreppsvägar
- Varför frontend aldrig är säker
- Backend som enda beslutsfattare

### Resultat
Deltagaren förstår *varför* säkerhet måste byggas in i varje lager.

---

## Vecka 2 – Databassäkerhet
**Fokus:** Databasen som första skyddsnivå.

### Innehåll
- Tabell för användare (users)
  - Unik email (case-insensitive med citext)
  - Unikt username
  - Roll i users.role (viewer, staff, admin) låst med CHECK
  - Hashade lösenord (password_hash, aldrig klartext)
  - Kontoskydd
    - failed_login_count
    - locked_until
  - Status och logg
    - is_active
    - created_at
    - last_login_at

- Tabell för inlägg (posts)
  - title och content (NOT NULL + CHECK mot tomma strängar)
  - created_at, updated_at
  - created_by och updated_by (FK till users.id)

- Constraints och skydd i databasen
  - NOT NULL
  - CHECK
  - UNIQUE
  - Foreign keys (FK)

- Principen om minsta privilegium
  - Viewer: SELECT
  - Staff: SELECT, INSERT
  - Admin: SELECT, INSERT, UPDATE, DELETE
  - Separata databaskonton:
    - viewer_db
    - staff_db
    - admin_db

- Ingen publik databasåtkomst
  - Databasen nås endast via backend, aldrig direkt från internet

### Praktiskt
Bygga en säker databasstruktur som stoppar fel data, dubbletter, trasiga relationer och otillåtna operationer.

---

## Vecka 3–4 – Backend: autentisering och auktorisering (2 veckor)
**Fokus:** Backend bestämmer alltid. Ingen tillit till frontend.

---

## Övergripande mål
Deltagaren ska förstå och implementera säker autentisering och auktorisering i backend samt kunna visa hur backend stoppar otillåtna anrop oavsett frontend.

---

## Vecka 3 – Autentisering (vem är du?)

### Innehåll
- Säker inloggning
- Login endpoint (`POST /auth/login`)
- Läsa användare från databasen
- Verifiera lösenord med hash (BCrypt)
- Hantera:
  - fel lösenord
  - inaktivt konto
  - låst konto (failed_login_count, locked_until)
- Skapa JWT med:
  - userId
  - role
  - giltighetstid (expiry)
- Grundläggande loggning:
  - lyckad inloggning
  - misslyckade försök

### Praktiskt
- Bygga login-endpoint
- Testa rätt/fel lösenord
- Visa att låsta konton inte kan logga in

---

## Vecka 4 – Auktorisering (vad får du göra?)

### Innehåll
- JWT-validering i middleware
- Rollkontroll server-side
- Skyddade endpoints:
  - `GET /posts` → viewer, staff, admin
  - `POST /posts` → staff, admin
  - `DELETE /posts/{id}` → admin
- Input-validering
- Utökad loggning:
  - nekade anrop
  - skapande av inlägg
  - radering av inlägg

### Säkerhetsprinciper
- Ingen affärslogik i frontend
- Ingen rollkontroll i frontend
- Backend litar aldrig på klientdata

### Praktiskt
- Anropa endpoints med fel roll
- Manipulera requests
- Visa att backend stoppar alla otillåtna försök

---

## Resultat efter 4 veckor
Efter vecka 3–4 kan deltagaren:
- Förklara autentisering vs auktorisering
- Implementera JWT-baserad inloggning
- Skydda endpoints med roller
- Visa hur backend är systemets säkerhetsmotor
---

## Vecka 5 – Frontend: osäker klient
**Fokus:** Visa frontendens begränsningar.

### Innehåll
- Enkel inloggningssida
- Visa innehåll endast vid inloggning
- Visa funktioner baserat på roll
- Ingen säkerhetslogik i frontend
- Demonstration av manipulation via devtools

### Resultat
Deltagaren förstår varför backend aldrig får lita på klienten.

---

## Vecka 6 – Testning av säkerhet
**Fokus:** Säkerhet ska testas automatiskt.

### Innehåll
- Unit tests:
  - validering
  - auth-regler
- Integration tests:
  - endpoint kräver rätt roll
- Tester som bevisar säkerhet

### Resultat
Säkerhet verifieras kontinuerligt, inte manuellt.

---

## Vecka 7 – CI/CD och DevOps
**Fokus:** Säker leverans hela vägen till drift.

### Innehåll
- CI/CD-pipeline:
  - build
  - test
  - deploy
- Deploy stoppas vid test-fail
- Secrets via miljövariabler
- Ingen hemlig data i kod
- Tydlig separation av miljöer:
  - dev
  - prod

### Extra säkerhet (låg insats, hög effekt)
- Loggning av:
  - inloggningar
  - misslyckade försök
  - radering av data
- Enkel rate limiting eller kontolåsning

---

## Slutresultat
Efter kursen har deltagaren:
- Byggt en komplett demoapp
- Förstått säkerhet i varje lager
- Implementerat autentisering och roller
- Testat säkerhet automatiskt
- Levererat via säker CI/CD-pipeline

Detta är en komplett grund i SecDevOps med fokus på praktik och förståelse.