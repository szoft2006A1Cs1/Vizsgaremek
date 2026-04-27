Jogsifoglaló Alkalmazás – Vizsgaremek
Ez a projekt egy autósiskolai órafoglaló rendszer, amely a tanulók, oktatók és adminisztrátorok közötti folyamatokat digitalizálja és teszi egyszerűbbé.

Funkciók
Tanulóknak: Regisztráció, interaktív naptár alapú órafoglalás, profilkezelés.

Oktatóknak: Saját órarend kezelése, oktatói profil karbantartása.

Adminisztrátoroknak: Teljes körű felhasználókezelés, rendszerstatisztikák és moderáció.

Technológiai háttér
Backend: .NET 8 Web API (C#)

Frontend: React (Vite, Axios, SweetAlert2)

Adatbázis: MySQL / Entity Framework Core (Code First)

Tesztelés: xUnit, Moq (egység- és folyamattesztek)

Projektstruktúra
/backend: A szerveroldali kód, kontrollerek és modellek.

/frontend: A React alapú kliensoldali kód és stíluslapok.

/database: Az adatbázis létrehozásához szükséges SQL fájl és az ER-modell.

/documentation: A teljes vizsgadokumentáció.

Telepítés és futtatás
1. Adatbázis előkészítése
Indítsa el a XAMPP alkalmazásban az Apache webszervert és a MySQL modult.

Nyissa meg a phpMyAdmin felületét, és importálja a jogsifoglalo.sql fájlt.

2. Backend indítása
Navigáljon a /backend mappába.

Ellenőrizze a MySQL portot (alapértelmezett: 3306) az appsettings.json fájl kapcsolati karakterláncában (Connection String).

Indítsa el a szervert a következő paranccsal:

dotnet run

3. Frontend indítása
Navigáljon a /frontend mappába.

Telepítse a szükséges függőségeket (a parancs a package.json alapján minden modult telepít):

npm install

Indítsa el a fejlesztői szervert:

npm run dev

Készítők
Csaba Bence

Takács Barnabás
