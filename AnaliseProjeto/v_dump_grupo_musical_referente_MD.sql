CREATE DATABASE  IF NOT EXISTS `grupomusical` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `grupomusical`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: grupomusical
-- ------------------------------------------------------
-- Server version	5.7.42-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `apresentacaotipoinstrumento`
--

DROP TABLE IF EXISTS `apresentacaotipoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `apresentacaotipoinstrumento` (
  `idApresentacao` int(11) NOT NULL,
  `idTipoInstrumento` int(11) NOT NULL,
  `quantidadePlanejada` int(11) NOT NULL DEFAULT '0',
  `quantidadeConfirmada` int(11) NOT NULL DEFAULT '0',
  `quantidadeSolicitada` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`idApresentacao`,`idTipoInstrumento`),
  KEY `fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_ApresentacaoTipoInstrumento_Apresentacao1_idx` (`idApresentacao`),
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_Apresentacao1` FOREIGN KEY (`idApresentacao`) REFERENCES `evento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `apresentacaotipoinstrumento`
--

LOCK TABLES `apresentacaotipoinstrumento` WRITE;
/*!40000 ALTER TABLE `apresentacaotipoinstrumento` DISABLE KEYS */;
/*!40000 ALTER TABLE `apresentacaotipoinstrumento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(767) NOT NULL,
  `ClaimType` text,
  `ClaimValue` text,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(767) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` text,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('245dbc59-f7ad-490e-90e3-d0fabfda91dc','ADMINISTRADOR GRUPO','ADMINISTRADOR GRUPO',NULL),('4e41763d-d54c-472a-ab46-dadabb2d8859','ADMINISTRADOR SISTEMA','ADMINISTRADOR SISTEMA','fac5a197-97f3-47e9-b29e-479fa1e5ac80'),('66a8639c-b17f-4fcc-8416-2266188635d6','REGENTE','REGENTE',NULL),('9d020009-99fd-48c8-b578-9deed019c83a','ASSOCIADO','ASSOCIADO',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(767) NOT NULL,
  `ClaimType` text,
  `ClaimValue` text,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `ProviderDisplayName` text,
  `UserId` varchar(767) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(767) NOT NULL,
  `RoleId` varchar(767) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('b22c175a-da30-4f23-9a8d-12cf466ce2ac','245dbc59-f7ad-490e-90e3-d0fabfda91dc'),('0f900e08-881c-41e0-8387-b2f8ea9c7eba','4e41763d-d54c-472a-ab46-dadabb2d8859'),('1f0af393-1367-436a-bcd3-dfe8b26fcaa8','66a8639c-b17f-4fcc-8416-2266188635d6'),('58b0cb55-17dd-45be-84b8-e456153a2623','66a8639c-b17f-4fcc-8416-2266188635d6'),('d90c9efb-9fec-4b52-aef9-2136526edacc','9d020009-99fd-48c8-b578-9deed019c83a');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(767) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` bit(1) NOT NULL,
  `PasswordHash` text,
  `SecurityStamp` text,
  `ConcurrencyStamp` text,
  `PhoneNumber` text,
  `PhoneNumberConfirmed` bit(1) NOT NULL,
  `TwoFactorEnabled` bit(1) NOT NULL,
  `LockoutEnd` timestamp NULL DEFAULT NULL,
  `LockoutEnabled` bit(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('0f900e08-881c-41e0-8387-b2f8ea9c7eba','62870079079','62870079079',NULL,NULL,_binary '\0','AQAAAAEAACcQAAAAECfuURCq3/SlKumV6LoffVZ/PJ1E+pUg0ZkVLrPC8L2MTj5forpWexL8oTCEx3d1Ww==','U3ODHHNJ3VZU45UGMWA2AEOTBKWHODV3','b3896af2-a235-48ad-b81e-4feed9c28f65',NULL,_binary '\0',_binary '\0',NULL,_binary '',0),('1f0af393-1367-436a-bcd3-dfe8b26fcaa8','99437057010','99437057010','karinaqueijo.g@gmail.com','KARINAQUEIJO.G@GMAIL.COM',_binary '\0','AQAAAAIAAYagAAAAEN6bveYJFq6HrPzIENPcf2zwiL71HlB1UGvvr9l6TWzfvzd7KPjutoka6QdukF9c6w==','DV4VYLDKESOKBUG5O24GHV3UPJZQQBT7','d089c3bf-edbb-4b91-8836-681cafd9ebf3',NULL,_binary '\0',_binary '\0',NULL,_binary '',0),('58b0cb55-17dd-45be-84b8-e456153a2623','01405380039','01405380039','kah24.ales@gmail.com','KAH24.ALES@GMAIL.COM',_binary '\0','AQAAAAIAAYagAAAAEPP5cOL7xFGZWOArZIqmymA8QWscLyNFngV1gOF/u381PWmPIG3W2ZjoPy7BE+IF/w==','CX5CNUQB4LBRQ2222MWGVRHUV3ITLCUV','e3a88300-4776-4418-8325-14335cc28afd',NULL,_binary '\0',_binary '\0',NULL,_binary '',0),('b22c175a-da30-4f23-9a8d-12cf466ce2ac','53649984016','53649984016','vereverynice.tester1@gmail.com','VEREVERYNICE.TESTER1@GMAIL.COM',_binary '\0','AQAAAAIAAYagAAAAEPN+ZZZdZ01hzhtePun3D0JqOE1J1hBh8o9qBqf62LVDAlQfxnwjupWe1CGN+L6rcw==','FJLP2IG3RMAKQTDUVJAXUI6HBAMG36ST','03ca82cf-c74e-4cdb-b804-f0d31fb4d7e6',NULL,_binary '\0',_binary '\0',NULL,_binary '',0),('d90c9efb-9fec-4b52-aef9-2136526edacc','57776907006','57776907006','caioteste949@gmail.com','CAIOTESTE949@GMAIL.COM',_binary '\0','AQAAAAIAAYagAAAAEMZJ/O6KvUkBXx23xvX+/H4LxM9l+zY6DqOgxRI3SzrOI/TsijIZhpxLNaSwEmjOpA==','CUIXSDZONBHE2JXOFH6F7RQZEDZ4XBXF','20115d7f-57bb-4d0e-a42e-2fdbb4668d3d',NULL,_binary '\0',_binary '\0',NULL,_binary '',0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(767) NOT NULL,
  `LoginProvider` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Value` text,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ensaio`
--

DROP TABLE IF EXISTS `ensaio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ensaio` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int(11) NOT NULL,
  `tipo` enum('FIXO','EXTRA') NOT NULL DEFAULT 'FIXO',
  `dataHoraInicio` datetime NOT NULL,
  `dataHoraFim` datetime NOT NULL,
  `presencaObrigatoria` tinyint(4) NOT NULL DEFAULT '0',
  `local` varchar(100) DEFAULT NULL,
  `repertorio` varchar(1000) DEFAULT NULL,
  `idColaboradorResponsavel` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel`),
  KEY `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Ensaio_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ensaio_Pessoa1` FOREIGN KEY (`idColaboradorResponsavel`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ensaio`
--

LOCK TABLES `ensaio` WRITE;
/*!40000 ALTER TABLE `ensaio` DISABLE KEYS */;
INSERT INTO `ensaio` VALUES (1,1,'FIXO','2024-08-10 00:00:00','2024-08-11 00:00:00',1,'Local A','Repertório A',2),(2,1,'FIXO','2024-08-29 20:59:00','2024-08-30 20:59:00',1,'Local B','Repertório B',2);
/*!40000 ALTER TABLE `ensaio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ensaiopessoa`
--

DROP TABLE IF EXISTS `ensaiopessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ensaiopessoa` (
  `idPessoa` int(11) NOT NULL,
  `idEnsaio` int(11) NOT NULL,
  `presente` tinyint(4) NOT NULL DEFAULT '0',
  `justificativaFalta` varchar(200) DEFAULT NULL,
  `justificativaAceita` tinyint(4) NOT NULL DEFAULT '0',
  `idPapelGrupo` int(11) NOT NULL,
  PRIMARY KEY (`idPessoa`,`idEnsaio`),
  KEY `fk_PessoaEnsaio_Ensaio1_idx` (`idEnsaio`),
  KEY `fk_PessoaEnsaio_Pessoa1_idx` (`idPessoa`),
  KEY `fk_EnsaioPessoa_PapelGrupo1_idx` (`idPapelGrupo`),
  CONSTRAINT `fk_EnsaioPessoa_PapelGrupo1` FOREIGN KEY (`idPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PessoaEnsaio_Ensaio1` FOREIGN KEY (`idEnsaio`) REFERENCES `ensaio` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PessoaEnsaio_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ensaiopessoa`
--

LOCK TABLES `ensaiopessoa` WRITE;
/*!40000 ALTER TABLE `ensaiopessoa` DISABLE KEYS */;
INSERT INTO `ensaiopessoa` VALUES (3,1,0,NULL,0,5),(4,2,0,NULL,0,5),(5,1,0,NULL,0,1);
/*!40000 ALTER TABLE `ensaiopessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `evento`
--

DROP TABLE IF EXISTS `evento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `evento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int(11) NOT NULL,
  `dataHoraInicio` datetime NOT NULL,
  `dataHoraFim` datetime NOT NULL,
  `local` varchar(100) DEFAULT NULL,
  `repertorio` varchar(1000) DEFAULT NULL,
  `idColaboradorResponsavel` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel`),
  KEY `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Ensaio_GrupoMusical10` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ensaio_Pessoa10` FOREIGN KEY (`idColaboradorResponsavel`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `evento`
--

LOCK TABLES `evento` WRITE;
/*!40000 ALTER TABLE `evento` DISABLE KEYS */;
INSERT INTO `evento` VALUES (1,1,'2024-08-22 20:45:00','2024-08-23 20:45:00','Shopping Socorro','Repertório 1',2),(2,1,'2024-08-24 20:45:00','2024-08-27 20:45:00','Shopping Rio Mar','Repertório 2',2);
/*!40000 ALTER TABLE `evento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eventopessoa`
--

DROP TABLE IF EXISTS `eventopessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eventopessoa` (
  `idEvento` int(11) NOT NULL,
  `idPessoa` int(11) NOT NULL,
  `idTipoInstrumento` int(11) NOT NULL,
  `presente` tinyint(4) NOT NULL DEFAULT '0',
  `justificativaFalta` varchar(200) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `justificativaAceita` tinyint(4) NOT NULL DEFAULT '0',
  `status` enum('INSCRITO','DEFERIDO','INDEFERIDO') NOT NULL DEFAULT 'INSCRITO',
  `idPapelGrupoPapelGrupo` int(11) NOT NULL,
  PRIMARY KEY (`idEvento`,`idPessoa`),
  KEY `fk_ApresentacaoPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_ApresentacaoPessoa_Apresentacao1_idx` (`idEvento`),
  KEY `fk_ApresentacaoPessoa_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_EventoPessoa_PapelGrupo1_idx` (`idPapelGrupoPapelGrupo`),
  CONSTRAINT `fk_ApresentacaoPessoa_Apresentacao1` FOREIGN KEY (`idEvento`) REFERENCES `evento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoPessoa_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EventoPessoa_PapelGrupo1` FOREIGN KEY (`idPapelGrupoPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eventopessoa`
--

LOCK TABLES `eventopessoa` WRITE;
/*!40000 ALTER TABLE `eventopessoa` DISABLE KEYS */;
INSERT INTO `eventopessoa` VALUES (1,3,0,0,NULL,0,'INSCRITO',5),(1,4,0,0,NULL,0,'INSCRITO',5),(1,5,3,0,NULL,0,'INSCRITO',1),(2,3,0,0,NULL,0,'INSCRITO',5);
/*!40000 ALTER TABLE `eventopessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurino`
--

DROP TABLE IF EXISTS `figurino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurino` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) NOT NULL,
  `data` date DEFAULT NULL,
  `idGrupoMusical` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `fk_Figurino_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Figurino_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurino`
--

LOCK TABLES `figurino` WRITE;
/*!40000 ALTER TABLE `figurino` DISABLE KEYS */;
INSERT INTO `figurino` VALUES (1,'Sao joão 2020','2024-08-21',1),(2,'Maria Jiquinha 1999','1999-10-10',1),(3,'Arrastão','2000-10-10',1);
/*!40000 ALTER TABLE `figurino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinoapresentacao`
--

DROP TABLE IF EXISTS `figurinoapresentacao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinoapresentacao` (
  `idFigurino` int(11) NOT NULL,
  `idApresentacao` int(11) NOT NULL,
  PRIMARY KEY (`idFigurino`,`idApresentacao`),
  KEY `fk_FigurinoApresentacao_Apresentacao1_idx` (`idApresentacao`),
  KEY `fk_FigurinoApresentacao_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoApresentacao_Apresentacao1` FOREIGN KEY (`idApresentacao`) REFERENCES `evento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoApresentacao_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurinoapresentacao`
--

LOCK TABLES `figurinoapresentacao` WRITE;
/*!40000 ALTER TABLE `figurinoapresentacao` DISABLE KEYS */;
INSERT INTO `figurinoapresentacao` VALUES (2,1),(3,2);
/*!40000 ALTER TABLE `figurinoapresentacao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinoensaio`
--

DROP TABLE IF EXISTS `figurinoensaio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinoensaio` (
  `idFigurino` int(11) NOT NULL,
  `idEnsaio` int(11) NOT NULL,
  PRIMARY KEY (`idFigurino`,`idEnsaio`),
  KEY `fk_FigurinoEnsaio_Ensaio1_idx` (`idEnsaio`),
  KEY `fk_FigurinoEnsaio_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoEnsaio_Ensaio1` FOREIGN KEY (`idEnsaio`) REFERENCES `ensaio` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoEnsaio_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurinoensaio`
--

LOCK TABLES `figurinoensaio` WRITE;
/*!40000 ALTER TABLE `figurinoensaio` DISABLE KEYS */;
INSERT INTO `figurinoensaio` VALUES (3,1),(1,2);
/*!40000 ALTER TABLE `figurinoensaio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinomanequim`
--

DROP TABLE IF EXISTS `figurinomanequim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinomanequim` (
  `idFigurino` int(11) NOT NULL,
  `idManequim` int(11) NOT NULL,
  `quantidadeDisponivel` int(11) NOT NULL DEFAULT '0',
  `quantidadeEntregue` int(11) NOT NULL DEFAULT '0',
  `quantidadeDescartada` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`idFigurino`,`idManequim`),
  KEY `fk_FigurinoManequim_Manequim1_idx` (`idManequim`),
  KEY `fk_FigurinoManequim_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoManequim_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoManequim_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurinomanequim`
--

LOCK TABLES `figurinomanequim` WRITE;
/*!40000 ALTER TABLE `figurinomanequim` DISABLE KEYS */;
/*!40000 ALTER TABLE `figurinomanequim` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grupomusical`
--

DROP TABLE IF EXISTS `grupomusical`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `grupomusical` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) NOT NULL,
  `razaoSocial` varchar(100) NOT NULL,
  `cnpj` varchar(14) NOT NULL,
  `cep` varchar(8) DEFAULT NULL,
  `rua` varchar(100) DEFAULT NULL,
  `bairro` varchar(100) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `estado` char(2) NOT NULL DEFAULT 'SE',
  `pais` varchar(50) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `youtube` varchar(100) DEFAULT NULL,
  `instagram` varchar(100) DEFAULT NULL,
  `facebook` varchar(100) DEFAULT NULL,
  `telefone1` varchar(20) DEFAULT NULL,
  `telefone2` varchar(20) DEFAULT NULL,
  `banco` varchar(100) DEFAULT NULL,
  `agencia` varchar(15) DEFAULT NULL,
  `numeroContaBanco` varchar(15) DEFAULT NULL,
  `chavePIX` varchar(100) DEFAULT NULL,
  `chavePIXTipo` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `cnpj_UNIQUE` (`cnpj`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupomusical`
--

LOCK TABLES `grupomusical` WRITE;
/*!40000 ALTER TABLE `grupomusical` DISABLE KEYS */;
INSERT INTO `grupomusical` VALUES (1,'Batalá - SE','Batala Group','22884507000198','69309215','Travessa dos Imigrantes','Buritis','Boa Vista','RR','Brasil','batalateste@gmail.com','/batalaRR','@batalaRR','/batalaRR','(27)24637172','(82)2399-2069','do Brasiiiil','9999','17393065','39446457076','CPF');
/*!40000 ALTER TABLE `grupomusical` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `informativo`
--

DROP TABLE IF EXISTS `informativo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `informativo` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int(11) NOT NULL,
  `idPessoa` int(11) NOT NULL,
  `mensagem` varchar(2000) NOT NULL,
  `data` datetime NOT NULL,
  `entregarAssociadosAtivos` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `fk_GrupoMusicalPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_GrupoMusicalPessoa_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_GrupoMusicalPessoa_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_GrupoMusicalPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `informativo`
--

LOCK TABLES `informativo` WRITE;
/*!40000 ALTER TABLE `informativo` DISABLE KEYS */;
/*!40000 ALTER TABLE `informativo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `instrumentomusical`
--

DROP TABLE IF EXISTS `instrumentomusical`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `instrumentomusical` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `patrimonio` varchar(20) NOT NULL,
  `dataAquisicao` date NOT NULL,
  `status` enum('DISPONIVEL','EMPRESTADO','DANIFICADO') NOT NULL DEFAULT 'DISPONIVEL',
  `idTipoInstrumento` int(11) NOT NULL,
  `idGrupoMusical` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_InstrumentoMusical_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_InstrumentoMusical_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_InstrumentoMusical_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_InstrumentoMusical_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `instrumentomusical`
--

LOCK TABLES `instrumentomusical` WRITE;
/*!40000 ALTER TABLE `instrumentomusical` DISABLE KEYS */;
/*!40000 ALTER TABLE `instrumentomusical` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manequim`
--

DROP TABLE IF EXISTS `manequim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manequim` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tamanho` varchar(2) NOT NULL,
  `descricao` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manequim`
--

LOCK TABLES `manequim` WRITE;
/*!40000 ALTER TABLE `manequim` DISABLE KEYS */;
INSERT INTO `manequim` VALUES (1,'PP','Extra Pequeno'),(2,'P','Pequeno'),(3,'M','MEDIO'),(4,'G','Grande');
/*!40000 ALTER TABLE `manequim` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materialestudo`
--

DROP TABLE IF EXISTS `materialestudo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materialestudo` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nome` varchar(200) NOT NULL,
  `link` varchar(500) NOT NULL,
  `data` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `idGrupoMusical` int(11) NOT NULL,
  `idColaborador` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_MaterialEstudo_GrupoMusical1_idx` (`idGrupoMusical`),
  KEY `fk_MaterialEstudo_Pessoa1_idx` (`idColaborador`),
  CONSTRAINT `fk_MaterialEstudo_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_MaterialEstudo_Pessoa1` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materialestudo`
--

LOCK TABLES `materialestudo` WRITE;
/*!40000 ALTER TABLE `materialestudo` DISABLE KEYS */;
/*!40000 ALTER TABLE `materialestudo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimentacaofigurino`
--

DROP TABLE IF EXISTS `movimentacaofigurino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimentacaofigurino` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `idFigurino` int(11) NOT NULL,
  `idManequim` int(11) NOT NULL,
  `idAssociado` int(11) NOT NULL,
  `idColaborador` int(11) NOT NULL,
  `status` enum('DISPONIVEL','ENTREGUE','RECEBIDO','DANIFICADO','DEVOLVIDO') NOT NULL DEFAULT 'DISPONIVEL',
  `confirmacaoRecebimento` tinyint(4) NOT NULL DEFAULT '0',
  `quantidade` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_EntregarFigurino_Figurino1_idx` (`idFigurino`),
  KEY `fk_EntregarFigurino_Manequim1_idx` (`idManequim`),
  KEY `fk_EntregarFigurino_Pessoa1_idx` (`idAssociado`),
  KEY `fk_EntregarFigurino_Pessoa2_idx` (`idColaborador`),
  CONSTRAINT `fk_EntregarFigurino_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Pessoa1` FOREIGN KEY (`idAssociado`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Pessoa2` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimentacaofigurino`
--

LOCK TABLES `movimentacaofigurino` WRITE;
/*!40000 ALTER TABLE `movimentacaofigurino` DISABLE KEYS */;
/*!40000 ALTER TABLE `movimentacaofigurino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimentacaoinstrumento`
--

DROP TABLE IF EXISTS `movimentacaoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimentacaoinstrumento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `idInstrumentoMusical` int(11) NOT NULL,
  `idAssociado` int(11) NOT NULL,
  `idColaborador` int(11) NOT NULL,
  `confirmacaoAssociado` tinyint(4) NOT NULL DEFAULT '0',
  `tipoMovimento` enum('EMPRESTIMO','DEVOLUCAO') NOT NULL DEFAULT 'EMPRESTIMO',
  PRIMARY KEY (`id`),
  KEY `fk_DevolverInstrumento_InstrumentoMusical1_idx` (`idInstrumentoMusical`),
  KEY `fk_DevolverInstrumento_Pessoa1_idx` (`idAssociado`),
  KEY `fk_DevolverInstrumento_Pessoa2_idx` (`idColaborador`),
  CONSTRAINT `fk_DevolverInstrumento_InstrumentoMusical1` FOREIGN KEY (`idInstrumentoMusical`) REFERENCES `instrumentomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_DevolverInstrumento_Pessoa1` FOREIGN KEY (`idAssociado`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_DevolverInstrumento_Pessoa2` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimentacaoinstrumento`
--

LOCK TABLES `movimentacaoinstrumento` WRITE;
/*!40000 ALTER TABLE `movimentacaoinstrumento` DISABLE KEYS */;
/*!40000 ALTER TABLE `movimentacaoinstrumento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `papelgrupo`
--

DROP TABLE IF EXISTS `papelgrupo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `papelgrupo` (
  `idPapelGrupo` int(11) NOT NULL,
  `nome` varchar(100) NOT NULL,
  PRIMARY KEY (`idPapelGrupo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `papelgrupo`
--

LOCK TABLES `papelgrupo` WRITE;
/*!40000 ALTER TABLE `papelgrupo` DISABLE KEYS */;
INSERT INTO `papelgrupo` VALUES (1,'associado'),(2,'colaborador'),(3,'Administrador do Grupo Musical'),(4,'Administrador do Sistema'),(5,'regente');
/*!40000 ALTER TABLE `papelgrupo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pessoa`
--

DROP TABLE IF EXISTS `pessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pessoa` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cpf` varchar(11) NOT NULL,
  `nome` varchar(100) NOT NULL,
  `sexo` enum('M','F') NOT NULL DEFAULT 'F',
  `cep` varchar(8) NOT NULL,
  `rua` varchar(100) DEFAULT NULL,
  `bairro` varchar(100) DEFAULT NULL,
  `cidade` varchar(100) DEFAULT NULL,
  `estado` char(2) NOT NULL DEFAULT 'SE',
  `dataNascimento` date DEFAULT NULL,
  `telefone1` varchar(20) NOT NULL,
  `telefone2` varchar(20) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `dataEntrada` date DEFAULT NULL,
  `dataSaida` date DEFAULT NULL,
  `motivoSaida` varchar(100) DEFAULT NULL,
  `ativo` tinyint(4) NOT NULL DEFAULT '0',
  `isentoPagamento` tinyint(4) NOT NULL DEFAULT '0',
  `idGrupoMusical` int(11) NOT NULL,
  `idPapelGrupo` int(11) NOT NULL,
  `idManequim` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`),
  KEY `fk_Pessoa_GrupoMusical1_idx` (`idGrupoMusical`),
  KEY `fk_Pessoa_PapelGrupoMusical1_idx` (`idPapelGrupo`),
  KEY `fk_Pessoa_Manequim1_idx` (`idManequim`),
  CONSTRAINT `fk_Pessoa_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_PapelGrupoMusical1` FOREIGN KEY (`idPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pessoa`
--

LOCK TABLES `pessoa` WRITE;
/*!40000 ALTER TABLE `pessoa` DISABLE KEYS */;
INSERT INTO `pessoa` VALUES (2,'53649984016','Verynice ADM','M','',NULL,NULL,NULL,'',NULL,'',NULL,'vereverynice.tester1@gmail.com',NULL,NULL,NULL,1,1,1,3,1),(3,'99437057010','Karina Key','F','49509146','Rua José Vicente de Oliveira','Mamede Paes Mendonça','Itabaiana','SE','2000-08-21','(79)99629-1292',NULL,'karinaqueijo.g@gmail.com','2024-08-20',NULL,NULL,1,0,1,5,2),(4,'01405380039','Kah Ales','M','49509146','Rua José Vicente de Oliveira','Mamede Paes Mendonça','Itabaiana','SE','2001-08-21','(79)99629-1292',NULL,'kah24.ales@gmail.com','2024-08-20',NULL,NULL,1,0,1,5,3),(5,'57776907006','Caio Figueredo Amorim','M','49509146','Rua José Vicente de Oliveira','Mamede Paes Mendonça','Itabaiana','SE','1992-10-10','(79)99629-1292',NULL,'caioteste949@gmail.com','2024-08-15',NULL,NULL,1,0,1,1,3);
/*!40000 ALTER TABLE `pessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pessoatipoinstrumento`
--

DROP TABLE IF EXISTS `pessoatipoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pessoatipoinstrumento` (
  `idPessoa` int(11) NOT NULL,
  `idTipoInstrumento` int(11) NOT NULL,
  PRIMARY KEY (`idPessoa`,`idTipoInstrumento`),
  KEY `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_Pessoa_has_TipoInstrumento_Pessoa_idx` (`idPessoa`),
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_Pessoa` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pessoatipoinstrumento`
--

LOCK TABLES `pessoatipoinstrumento` WRITE;
/*!40000 ALTER TABLE `pessoatipoinstrumento` DISABLE KEYS */;
/*!40000 ALTER TABLE `pessoatipoinstrumento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receitafinanceira`
--

DROP TABLE IF EXISTS `receitafinanceira`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receitafinanceira` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `descricao` varchar(100) NOT NULL,
  `dataInicio` date NOT NULL,
  `dataFim` date NOT NULL,
  `valor` decimal(10,2) NOT NULL,
  `idGrupoMusical` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_ReceitaFinanceira_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_ReceitaFinanceira_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receitafinanceira`
--

LOCK TABLES `receitafinanceira` WRITE;
/*!40000 ALTER TABLE `receitafinanceira` DISABLE KEYS */;
INSERT INTO `receitafinanceira` VALUES (1,'Paguem 1','2024-05-10','2024-08-19',500.00,1),(2,'Paguem 2','2024-05-10','2025-08-11',500.00,1);
/*!40000 ALTER TABLE `receitafinanceira` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receitafinanceirapessoa`
--

DROP TABLE IF EXISTS `receitafinanceirapessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receitafinanceirapessoa` (
  `idReceitaFinanceira` int(11) NOT NULL,
  `idPessoa` int(11) NOT NULL,
  `valor` decimal(10,2) NOT NULL,
  `valorPago` decimal(10,2) NOT NULL DEFAULT '0.00',
  `dataPagamento` datetime NOT NULL,
  `observacoes` varchar(200) DEFAULT NULL,
  `status` enum('ABERTO','ENVIADO','PAGO','ISENTO') NOT NULL DEFAULT 'ABERTO',
  PRIMARY KEY (`idReceitaFinanceira`,`idPessoa`),
  KEY `fk_ReceitaFinanceiraPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1_idx` (`idReceitaFinanceira`),
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1` FOREIGN KEY (`idReceitaFinanceira`) REFERENCES `receitafinanceira` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receitafinanceirapessoa`
--

LOCK TABLES `receitafinanceirapessoa` WRITE;
/*!40000 ALTER TABLE `receitafinanceirapessoa` DISABLE KEYS */;
INSERT INTO `receitafinanceirapessoa` VALUES (1,5,500.00,0.00,'2024-05-10 00:00:00',NULL,'ABERTO'),(2,5,500.00,0.00,'2024-05-10 00:00:00',NULL,'ABERTO');
/*!40000 ALTER TABLE `receitafinanceirapessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipoinstrumento`
--

DROP TABLE IF EXISTS `tipoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipoinstrumento` (
  `id` int(11) NOT NULL,
  `nome` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipoinstrumento`
--

LOCK TABLES `tipoinstrumento` WRITE;
/*!40000 ALTER TABLE `tipoinstrumento` DISABLE KEYS */;
INSERT INTO `tipoinstrumento` VALUES (0,'NENHUM'),(1,'Tambor'),(2,'Flauta'),(3,'Violão'),(4,'Xilofone'),(5,'Guitarra');
/*!40000 ALTER TABLE `tipoinstrumento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'grupomusical'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-08-21 21:35:22
