-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 26, 2026 at 03:39 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `jogsifoglalo`
--
CREATE DATABASE IF NOT EXISTS `jogsifoglalo` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `jogsifoglalo`;

-- --------------------------------------------------------

--
-- Table structure for table `felhasznalok`
--

CREATE TABLE `felhasznalok` (
  `Felhasznalo_Id` int(11) NOT NULL,
  `Nev` varchar(100) NOT NULL,
  `Cim` varchar(100) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Jelszo` varchar(255) NOT NULL,
  `Telefonszam` varchar(20) NOT NULL,
  `Szerepkor_Nev` enum('tanulo','oktato','admin') NOT NULL DEFAULT 'tanulo'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `felhasznalok`
--

INSERT INTO `felhasznalok` (`Felhasznalo_Id`, `Nev`, `Cim`, `Email`, `Jelszo`, `Telefonszam`, `Szerepkor_Nev`) VALUES
(1, 'Csaba Bence', 'Szombathely, Országh László Utca 15/1', 'csaba.bence@gmail.com', '$2a$11$SzTcF1nyav6tDwPzDLT..OBzTYdYnf/YjY7bi4XqKDBrxKFWf01vG', '06307249445', 'tanulo'),
(2, 'Takács Barnabás', 'Szombathely, Nyárfa Utca 8', 'takacs.barnabas@gmail.com', '$2a$11$Gn/fLQ0NGSGFJEqL5yIxIeK4mLGk8n93sXAw7huraFH4bKw1PPmdu', '0670257717', 'tanulo'),
(3, 'Kovács Mária', 'Budapest, Fő utca 12.', 'kovacs.maria@jogsifoglalo.com', '$2a$11$9K4yjXIi0PAde1S9vuSHPu7jyq/SN8UEboi0/yGTTATZ/9tyOULum', '06201234567', 'oktato'),
(4, 'Nagy Gábor', 'Szeged, Kárász utca 5.', 'nagy.gabor@jogsifoglalo.com', '$2a$11$cvln9TdppUyoNC0tBIQQvuvse4RUz1xHOJlrNkC5bCr1Z8wTsyBh6', '06309876543', 'oktato'),
(5, 'Horváth Tibor', 'Pécs, Király utca 21.', 'horvath.tibor@jogsifoglalo.com', '$2a$11$g51fHX8S0w01bDH0SjJ8zOUVwsdS6jyfFOE6pAJtLTNspGAfWvtTO', '06701122334', 'oktato'),
(6, 'Admin Főnök', 'Központ', 'admin@jogsifoglalo.com', '$2a$11$GVZWcxxm8H9NTHwxfew4/.gdaiCAQ5B2Jz2co8Q4gWiUUZ17jMita', '0610000001', 'admin'),
(7, 'Admin Zsófia', 'Központ', 'zsofia.admin@jogsifoglalo.com', '$2a$11$tOQxaHISwdw7lCe6beDdkeNO3pPCiRYqo4vpwQeKnEYaqJOsSO8y6', '0610000002', 'admin');

-- --------------------------------------------------------

--
-- Table structure for table `fizetesek`
--

CREATE TABLE `fizetesek` (
  `Fizetes_Id` int(11) NOT NULL,
  `Felhasznalo_Id` int(11) NOT NULL,
  `Idopont_Id` int(11) NOT NULL,
  `Osszeg` int(11) NOT NULL,
  `Datum` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `fizetesek`
--

INSERT INTO `fizetesek` (`Fizetes_Id`, `Felhasznalo_Id`, `Idopont_Id`, `Osszeg`, `Datum`) VALUES
(1, 1, 13, 18000, '2026-04-20 14:15:00'),
(2, 1, 14, 18000, '2026-04-20 14:16:00'),
(3, 2, 15, 20000, '2026-04-21 09:30:00'),
(4, 2, 16, 20000, '2026-04-21 09:30:00');

--
-- Triggers `fizetesek`
--
DELIMITER $$
CREATE TRIGGER `trg_fizetes_elott_csak_szabad` BEFORE INSERT ON `fizetesek` FOR EACH ROW BEGIN
    IF (SELECT Allapot FROM idopontok WHERE Idopont_Id = NEW.Idopont_Id) <> 'szabad' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Az idopont nem szabad, nem fizetheto ki.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `idopontok`
--

CREATE TABLE `idopontok` (
  `Idopont_Id` int(11) NOT NULL,
  `Oktat_Id` int(11) NOT NULL,
  `Tanulo_Id` int(11) DEFAULT NULL,
  `Kezdes_Dt` datetime NOT NULL,
  `Idotartam` smallint(5) UNSIGNED NOT NULL DEFAULT 120,
  `Ar` int(11) NOT NULL,
  `Megjegyzes` varchar(255) DEFAULT NULL,
  `Allapot` enum('szabad','foglalt','lemondva','teljesitve') NOT NULL DEFAULT 'szabad'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `idopontok`
--

INSERT INTO `idopontok` (`Idopont_Id`, `Oktat_Id`, `Tanulo_Id`, `Kezdes_Dt`, `Idotartam`, `Ar`, `Megjegyzes`, `Allapot`) VALUES
(1, 1, 1, '2026-04-10 08:00:00', 120, 18000, 'Első vezetés, rutin', 'teljesitve'),
(2, 1, 1, '2026-04-12 10:00:00', 120, 18000, 'Városi vezetés alapok', 'teljesitve'),
(3, 2, 2, '2026-04-15 14:00:00', 120, 20000, 'Motoros rutin, egyensúly', 'teljesitve'),
(4, 1, NULL, '2026-05-10 08:00:00', 120, 18000, 'Szabad városi óra', 'szabad'),
(5, 1, NULL, '2026-05-10 10:30:00', 120, 18000, 'Rutin pálya gyakorlás', 'szabad'),
(6, 1, NULL, '2026-05-11 14:00:00', 120, 18000, 'Délutáni szabad óra', 'szabad'),
(7, 2, NULL, '2026-05-12 09:00:00', 120, 20000, 'Motoros forgalmi', 'szabad'),
(8, 2, NULL, '2026-05-13 11:00:00', 120, 20000, 'Vizsgafelkészítő', 'szabad'),
(9, 3, NULL, '2026-05-15 08:00:00', 120, 28000, 'Teherautó manőverezés', 'szabad'),
(10, 3, NULL, '2026-05-16 13:00:00', 120, 28000, 'Országúti vezetés', 'szabad'),
(11, 1, NULL, '2026-06-01 08:00:00', 120, 18000, 'Kora reggeli óra', 'szabad'),
(12, 2, NULL, '2026-06-02 10:00:00', 120, 20000, 'Motoros rutin pálya', 'szabad'),
(13, 1, 1, '2026-05-20 08:00:00', 120, 18000, 'Parkolási gyakorlatok', 'foglalt'),
(14, 1, 1, '2026-05-22 10:30:00', 120, 18000, 'Országúti vezetés, előzés', 'foglalt'),
(15, 2, 2, '2026-05-25 14:00:00', 120, 20000, 'Forgalmi gyakorlás', 'foglalt'),
(16, 2, 2, '2026-05-27 16:30:00', 120, 20000, 'Éjszakai vezetés', 'foglalt'),
(17, 3, 1, '2026-05-29 08:00:00', 120, 28000, 'Teherautós gyakorlás', 'foglalt'),
(18, 1, 2, '2026-06-05 08:00:00', 120, 18000, 'Vizsga előtti utolsó óra', 'foglalt'),
(19, 2, 1, '2026-06-08 14:00:00', 120, 20000, 'Motoros rutin pótlás', 'foglalt'),
(20, 1, 1, '2026-05-18 08:00:00', 120, 18000, 'Betegség miatt elmarad', 'lemondva'),
(21, 2, 2, '2026-05-19 10:00:00', 120, 20000, 'Családi okok', 'lemondva'),
(22, 3, 1, '2026-06-10 08:00:00', 120, 28000, 'Műszaki hiba', 'lemondva'),
(23, 1, NULL, '2026-06-15 08:00:00', 120, 18000, 'Városi vezetés', 'szabad'),
(24, 2, NULL, '2026-06-16 09:00:00', 120, 20000, 'Motoros forgalmi', 'szabad'),
(25, 3, NULL, '2026-06-17 14:00:00', 120, 28000, 'Teherautó manőverezés', 'szabad'),
(26, 1, NULL, '2026-05-21 08:00:00', 120, 18000, 'Hegyi menet gyakorlása', 'szabad'),
(27, 2, NULL, '2026-05-21 10:00:00', 120, 20000, 'Motoros vészfékezés', 'szabad'),
(28, 3, NULL, '2026-05-21 13:00:00', 120, 28000, 'Raksúlykezelés elmélet', 'szabad'),
(29, 1, 1, '2026-05-23 09:00:00', 120, 18000, 'Autópálya tempó', 'foglalt'),
(30, 2, 2, '2026-05-23 11:30:00', 120, 20000, 'Szlalom gyakorlatok', 'foglalt'),
(31, 3, NULL, '2026-05-23 15:00:00', 120, 28000, 'Tolatási manőverek', 'szabad'),
(32, 1, NULL, '2026-05-24 08:00:00', 120, 18000, 'Műszaki átnézés alapok', 'szabad'),
(33, 2, NULL, '2026-05-24 10:30:00', 120, 20000, 'Városi sűrű forgalom', 'szabad'),
(34, 1, NULL, '2026-05-26 14:00:00', 120, 18000, 'Parkolás hátramenetben', 'szabad'),
(35, 2, NULL, '2026-05-26 16:30:00', 120, 20000, 'Éjszakai távolsági fény', 'szabad'),
(36, 3, 2, '2026-05-27 08:00:00', 120, 28000, 'Pótkocsi csatlakoztatás', 'foglalt'),
(37, 1, NULL, '2026-05-28 10:00:00', 120, 18000, 'Defektjavítás elmélet', 'szabad'),
(38, 2, NULL, '2026-05-28 12:30:00', 120, 20000, 'Motoros kanyarív', 'szabad'),
(39, 1, NULL, '2026-05-30 08:00:00', 120, 18000, 'Gyalogosátkelőhelyek', 'szabad'),
(40, 3, NULL, '2026-05-30 11:00:00', 120, 28000, 'Tachográf használata', 'szabad'),
(41, 1, NULL, '2026-05-31 09:00:00', 120, 18000, 'Hétvégi forgalom', 'lemondva'),
(42, 2, NULL, '2026-05-31 11:30:00', 120, 20000, 'Esős időjárás vezetés', 'lemondva'),
(43, 3, NULL, '2026-05-31 14:00:00', 120, 28000, 'Emelkedőn elindulás', 'lemondva'),
(44, 1, NULL, '2026-06-02 08:00:00', 120, 18000, 'Kereszteződések típusai', 'szabad'),
(45, 2, NULL, '2026-06-02 15:00:00', 120, 20000, 'Motoros öltözék ellenőrzés', 'szabad'),
(46, 3, NULL, '2026-06-03 10:00:00', 120, 28000, 'Üzemanyag-takarékos vezetés', 'szabad'),
(47, 1, 1, '2026-06-03 13:00:00', 120, 18000, 'Vizsgaútvonal bejárás', 'foglalt'),
(48, 2, NULL, '2026-06-04 09:00:00', 120, 20000, 'Rendőri forgalomirányítás', 'szabad'),
(49, 3, NULL, '2026-06-04 12:00:00', 120, 28000, 'Biztonsági ellenőrzés', 'szabad'),
(50, 1, NULL, '2026-06-04 15:00:00', 120, 18000, 'Vészfékezés ABS-szel', 'szabad');

-- --------------------------------------------------------

--
-- Table structure for table `kategoriak`
--

CREATE TABLE `kategoriak` (
  `Kategoria_Id` int(11) NOT NULL,
  `Kategoria_Kod` varchar(10) NOT NULL,
  `Kategoria_Nev` varchar(100) NOT NULL,
  `Oradij` int(11) NOT NULL,
  `Leiras` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `kategoriak`
--

INSERT INTO `kategoriak` (`Kategoria_Id`, `Kategoria_Kod`, `Kategoria_Nev`, `Oradij`, `Leiras`) VALUES
(1, 'A', 'Motor jogosítvány', 10000, 'Korlátlan motorkerékpár jogosítvány.'),
(2, 'B', 'Személygépkocsi jogosítvány', 9000, 'Maximum 3500 kg-os gépkocsi.'),
(3, 'C', 'Teherautó jogosítvány', 14000, '3500 kg-ot meghaladó gépkocsi.');

-- --------------------------------------------------------

--
-- Table structure for table `oktat`
--

CREATE TABLE `oktat` (
  `Oktat_Id` int(11) NOT NULL,
  `Oktato_Id` int(11) NOT NULL,
  `Kategoria_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `oktat`
--

INSERT INTO `oktat` (`Oktat_Id`, `Oktato_Id`, `Kategoria_Id`) VALUES
(1, 1, 1),
(2, 2, 2),
(3, 3, 3);

-- --------------------------------------------------------

--
-- Table structure for table `oktatok`
--

CREATE TABLE `oktatok` (
  `Oktato_Id` int(11) NOT NULL,
  `Felhasznalo_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `oktatok`
--

INSERT INTO `oktatok` (`Oktato_Id`, `Felhasznalo_Id`) VALUES
(1, 3),
(2, 4),
(3, 5);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `felhasznalok`
--
ALTER TABLE `felhasznalok`
  ADD PRIMARY KEY (`Felhasznalo_Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indexes for table `fizetesek`
--
ALTER TABLE `fizetesek`
  ADD PRIMARY KEY (`Fizetes_Id`),
  ADD UNIQUE KEY `Idopont_Id` (`Idopont_Id`),
  ADD KEY `Felhasznalo_Id` (`Felhasznalo_Id`);

--
-- Indexes for table `idopontok`
--
ALTER TABLE `idopontok`
  ADD PRIMARY KEY (`Idopont_Id`),
  ADD KEY `idx_oktat_kezdes` (`Oktat_Id`,`Kezdes_Dt`),
  ADD KEY `FK_Idopont_Tanulo` (`Tanulo_Id`);

--
-- Indexes for table `kategoriak`
--
ALTER TABLE `kategoriak`
  ADD PRIMARY KEY (`Kategoria_Id`);

--
-- Indexes for table `oktat`
--
ALTER TABLE `oktat`
  ADD PRIMARY KEY (`Oktat_Id`),
  ADD UNIQUE KEY `Oktato_Kategoria` (`Oktato_Id`,`Kategoria_Id`),
  ADD KEY `fk_oktat_kateg` (`Kategoria_Id`);

--
-- Indexes for table `oktatok`
--
ALTER TABLE `oktatok`
  ADD PRIMARY KEY (`Oktato_Id`),
  ADD UNIQUE KEY `Felhasznalo_Id` (`Felhasznalo_Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `felhasznalok`
--
ALTER TABLE `felhasznalok`
  MODIFY `Felhasznalo_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `fizetesek`
--
ALTER TABLE `fizetesek`
  MODIFY `Fizetes_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `idopontok`
--
ALTER TABLE `idopontok`
  MODIFY `Idopont_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `kategoriak`
--
ALTER TABLE `kategoriak`
  MODIFY `Kategoria_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `oktat`
--
ALTER TABLE `oktat`
  MODIFY `Oktat_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `oktatok`
--
ALTER TABLE `oktatok`
  MODIFY `Oktato_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `fizetesek`
--
ALTER TABLE `fizetesek`
  ADD CONSTRAINT `fk_fizet_idopont` FOREIGN KEY (`Idopont_Id`) REFERENCES `idopontok` (`Idopont_Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_fizet_user` FOREIGN KEY (`Felhasznalo_Id`) REFERENCES `felhasznalok` (`Felhasznalo_Id`) ON DELETE CASCADE;

--
-- Constraints for table `idopontok`
--
ALTER TABLE `idopontok`
  ADD CONSTRAINT `FK_Idopont_Tanulo` FOREIGN KEY (`Tanulo_Id`) REFERENCES `felhasznalok` (`Felhasznalo_Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_idopont_oktat` FOREIGN KEY (`Oktat_Id`) REFERENCES `oktat` (`Oktat_Id`) ON DELETE CASCADE;

--
-- Constraints for table `oktat`
--
ALTER TABLE `oktat`
  ADD CONSTRAINT `fk_oktat_kateg` FOREIGN KEY (`Kategoria_Id`) REFERENCES `kategoriak` (`Kategoria_Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_oktat_oktato` FOREIGN KEY (`Oktato_Id`) REFERENCES `oktatok` (`Oktato_Id`) ON DELETE CASCADE;

--
-- Constraints for table `oktatok`
--
ALTER TABLE `oktatok`
  ADD CONSTRAINT `fk_oktato_user` FOREIGN KEY (`Felhasznalo_Id`) REFERENCES `felhasznalok` (`Felhasznalo_Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
