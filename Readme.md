# Zawartość projektu

## Controllers 
- `AuthController` – zawiera funkcjonalności odpowiedzialne za:
  - rejestrację użytkownika,
  - logowanie,
  - wylogowywanie,
  - odświeżanie access tokenu użytkownika.
- `AdminController` – zawiera endpoint typu GET, dostępny wyłącznie dla użytkowników posiadających rolę Admin.
- `UserController` – zawiera endpoint typu GET, dostępny dla wszystkich poprawnie uwierzytelnionych użytkowników.

## Services
- `IPasswordService` oraz `PasswordService` – serwis odpowiedzialny za operacje związane z hashowaniem haseł. Wykorzystuje mechanizm PasswordHasher dostępny w platformie Identity.
- `ITokenService` oraz `JwtTokenService` – serwis odpowiedzialny za generowanie:
  - access tokenów,
  - refresh tokenów użytkowników aplikacji.

## Plik `Program.cs`
- W liniach `15–27` została dodana konfiguracja autentykacji z użyciem tokenów JWT.
- W linii `41` dodano uruchomienie middleware odpowiedzialnego za obsługę autentykacji.

## Plik `API.http`
Plik wbudowanego w Ridera mini-klienta HTTP, zawierający przykładowe zapytania do endpointów udostępnionych przez kontrolery.

# Zainstalowane pakiety:

- `Microsoft.AspNetCore.Authentication.JwtBearer` - pakiet dodający elementy potrzebne do implementacji autentykacji za pomocą tokenów JWT w aplikacji .net.

---

# Słownik pojęć

## Hashowanie
Hashowanie to proces przekształcania podanej wartości lub frazy w ciąg znaków o określonej długości przy użyciu algorytmu haszującego (np. Argon2, bcrypt, scrypt).
Używane do bezpiecznego przechowywania haseł w bazach danych.

Najważniejsze cechy hashowania:

- jest to operacja jednokierunkowa,
- zahashowanej wartości nie da się „odszyfrować”


### Hashowanie tej samej wartości
Hashowanie tej samej frazy przy użyciu tego samego algorytmu bez dodatkowych zabezpieczeń zawsze daje identyczny wynik.
```
"myPasswd" -> AQAAAAIAAYagAAAAED9JAjrsjll4yfeDa0V0El0leiRYoUGYURmZ124w1PVkNAz+d4DkV/BsdsFVPACKqw==
"myPasswd" -> AQAAAAIAAYagAAAAED9JAjrsjll4yfeDa0V0El0leiRYoUGYURmZ124w1PVkNAz+d4DkV/BsdsFVPACKqw==
```

Aby rozwiązać ten problem, przed hashowaniem hasła stosuje się tzw. sól (salt).

Sól to losowy ciąg znaków dodawany do hasła przed wykonaniem operacji hashowania.

Przykład:

```myPasswd + xasjn24b2nincnz = myPasswdxasjn24b2nincnz```

Dodanie soli powoduje, że nawet identyczne hasła różnych użytkowników generują różne hashe.

Wartość soli może być:

- przechowywana w osobnej kolumnie bazy danych,
- dołączana bezpośrednio do wygenerowanego hasha.

### Sprawdzanie poprawności hasła
Ponieważ hashowanie jest operacją jednokierunkową, weryfikacja hasła polega na:

1. ponownym zahashowaniu podanego hasła,
2. porównaniu wyniku z hashem zapisanym w bazie danych.

Aby wygenerować identyczny hash, konieczne jest użycie tych samych parametrów oraz tej samej soli, która została wykorzystana podczas tworzenia oryginalnego hasha.

---

## Autentykacja oraz autoryzacja

### Autentykacja
Autentykacja to proces potwierdzania tożsamości użytkownika poprzez przekazanie poświadczeń.

Przykład z życia codziennego:
Pokazuję identyfikator przed wejściem do biura.

### Autoryzacja
Autoryzacja to proces sprawdzania, czy użytkownik posiada uprawnienia do wykonania określonej operacji.

Przykład z życia codziennego:
Próbuję wejść kartą do pomieszczenia, do którego nie mam dostępu. System analizuje moje uprawnienia i odmawia wejścia.

---

## JWT
JWT (`JSON Web Token`) to jeden z najpopularniejszych sposobów uwierzytelniania użytkowników w aplikacjach webowych.

Token JWT składa się z trzech części oddzielonych kropkami:

```header.payload.signature```


### Header
Header zawiera informacje:

- o typie tokenu,
- o algorytmie użytym do podpisu kryptograficznego.

Przykład:
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```


### Payload
Payload zawiera dane dotyczące użytkownika oraz samego tokenu, np.:

- identyfikator użytkownika,
- nazwę użytkownika,
- rolę,
- czas wygaśnięcia tokenu,
- informacje o wystawcy tokenu.

Przykład:
```json
{
  "sub": "1",
  "unique_name": "kacper",
  "role": "admin",
  "nbf": 1779519085,
  "exp": 1779519685,
  "iat": 1779519085,
  "iss": "http://localhost:5192"
}
```

### Sygnatura
Sygnatura (signature) zawiera podpis kryptograficzny tokenu wygenerowany:

- na podstawie headera i payloadu,
- przy użyciu określonego algorytmu,
- oraz sekretnego klucza przechowywanego przez serwer.
- 
---

Przykład zdekodowanego tokenu JWT można zobaczyć tutaj:

https://jwt.io

---

JWT jest przykładem bezstanowego (stateless) podejścia do autentykacji użytkowników.

Oznacza to, że serwer zazwyczaj nie przechowuje informacji o wydanych access tokenach.

Token podczas generowania jest podpisywany kryptograficznie, dzięki czemu jego zawartość nie może zostać zmodyfikowana bez wykrycia.

Payload tokenu JWT jest domyślnie jawny, jednak każda próba zmiany jego zawartości (np. zmiany roli użytkownika na admin) spowoduje unieważnienie podpisu kryptograficznego i odrzucenie tokenu przez serwer.

**UWAGA: w ramach payloadu tokenu JWT nie powinny być przechowywane żadne dane wrażliwe typu e-maile, numery kont bankowych, numery pesel etc.**

---

Przykład tokenu JWT:

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidW5pcXVlX25hbWUiOiJrYWNwZXIiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3Nzk1MTkwODUsImV4cCI6MTc3OTUxOTY4NSwiaWF0IjoxNzc5NTE5MDg1LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUxOTIifQ.zlJXnAQUUK3jf3mQ3sl-T2w8mmW_zU3F18uU4dHyLus
```

---

### Wysyłanie tokenu w zapytaniu HTTP

Aby uzyskać dostęp do zabezpieczonego endpointu, token JWT musi zostać przesłany w nagłówku `Authorization`.

Przykład:

`Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

Po otrzymaniu tokenu serwer:

1. sprawdza poprawność podpisu kryptograficznego,
2. odczytuje dane z payloadu,
3. podejmuje decyzję o przyznaniu lub odmowie dostępu.

Przykładowo można odczytać rolę użytkownika i sprawdzić, czy posiada dostęp do danego endpointu.

---

## Refresh token

### Czym jest refresh token?

Refresh token to bezpieczny, losowo wygenerowany ciąg znaków służący do uzyskania nowego access tokenu JWT bez konieczności ponownego logowania użytkownika.

### Dlaczego stosujemy refresh tokeny?

Ze względu na bezpieczeństwo access tokeny JWT mają zazwyczaj krótki czas życia — najczęściej od 10 do 15 minut.

Dzięki temu, nawet jeśli token wycieknie, osoba atakująca ma niewiele czasu na jego wykorzystanie.

Krótki czas życia tokenu pogarsza jednak wygodę użytkownika, ponieważ wymagałby częstego ponownego logowania.
Aby temu zapobiec, stosuje się refresh tokeny.
Pozwalają one użytkownikowi uzyskać nowy access token bez ponownego podawania loginu i hasła.

Refresh tokeny mają znacznie dłuższy czas życia, np. 7 dni.

Oznacza to, że użytkownik może przez ten czas odnawiać swoją sesję bez ponownego logowania.

### Przechowywanie refresh tokenu

Refresh tokeny mają krytyczne znaczenie dla bezpieczeństwa aplikacji, ponieważ umożliwiają uzyskanie nowych access tokenów.

Z tego powodu powinny być przechowywane w możliwie najbezpieczniejszy sposób.

W aplikacjach webowych dobrą praktyką jest przechowywanie refresh tokenu w:

`HttpOnly + Secure + SameSite=Strict Cookie`

---

## Strict HttpOnly Cookie

### Czym jest ciasteczko?

Ciasteczko (`cookie`) to niewielki fragment danych zapisywany na urządzeniu użytkownika przez przeglądarkę internetową.

Cookies mogą służyć m.in. do:

- przechowywania danych sesji,
- utrzymywania stanu zalogowania,
- personalizacji zawartości strony,
- zbierania statystyk.

### HttpOnly
`HttpOnly` to atrybut zabezpieczający ciasteczka HTTP.

Bez tego atrybutu zawartość cookies może zostać odczytana przez JavaScript działający w przeglądarce.

Może to prowadzić do kradzieży tokenów w przypadku ataku typu XSS (`Cross-Site Scripting`).

Ustawienie HttpOnly powoduje, że:

- JavaScript nie ma dostępu do ciasteczka,
- ciasteczko jest przesyłane wyłącznie w żądaniach HTTP.

### SameSite=Strict

`SameSite=Strict` to kolejny mechanizm zabezpieczający.

Chroni przed atakami typu CSRF (`Cross-Site Request Forgery`) poprzez ograniczenie wysyłania ciasteczka wyłącznie do żądań pochodzących z tej samej domeny.

Dzięki temu przeglądarka nie wyśle ciasteczka podczas żądań wykonywanych z innych stron internetowych.
