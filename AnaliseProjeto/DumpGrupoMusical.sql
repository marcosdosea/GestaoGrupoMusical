CREATE DATABASE  IF NOT EXISTS `grupomusical` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `grupomusical`;
-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: grupomusical
-- ------------------------------------------------------
-- Server version	8.0.37

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
  `idApresentacao` int NOT NULL,
  `idTipoInstrumento` int NOT NULL,
  `quantidadePlanejada` int NOT NULL DEFAULT '0',
  `quantidadeConfirmada` int NOT NULL DEFAULT '0',
  `quantidadeSolicitada` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`idApresentacao`,`idTipoInstrumento`),
  KEY `fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_ApresentacaoTipoInstrumento_Apresentacao1_idx` (`idApresentacao`),
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_Apresentacao1` FOREIGN KEY (`idApresentacao`) REFERENCES `evento` (`id`),
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
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
  `Id` int NOT NULL AUTO_INCREMENT,
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
INSERT INTO `aspnetroles` VALUES ('4e41763d-d54c-472a-ab46-dadabb2d8859','ADMINISTRADOR SISTEMA','ADMINISTRADOR SISTEMA','fac5a197-97f3-47e9-b29e-479fa1e5ac80'),('fd597ca8-7776-430c-8bce-c4ed9c20eadd','ADMINISTRADOR GRUPO','ADMINISTRADOR GRUPO',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
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
INSERT INTO `aspnetuserroles` VALUES ('0f900e08-881c-41e0-8387-b2f8ea9c7eba','4e41763d-d54c-472a-ab46-dadabb2d8859'),('d9e89786-e454-48e8-a575-b419db5f83a2','fd597ca8-7776-430c-8bce-c4ed9c20eadd');
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
  `AccessFailedCount` int NOT NULL,
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
INSERT INTO `aspnetusers` VALUES ('0f900e08-881c-41e0-8387-b2f8ea9c7eba','62870079079','62870079079',NULL,NULL,_binary '\0','AQAAAAEAACcQAAAAECfuURCq3/SlKumV6LoffVZ/PJ1E+pUg0ZkVLrPC8L2MTj5forpWexL8oTCEx3d1Ww==','U3ODHHNJ3VZU45UGMWA2AEOTBKWHODV3','b3896af2-a235-48ad-b81e-4feed9c28f65',NULL,_binary '\0',_binary '\0',NULL,_binary '',0),('d9e89786-e454-48e8-a575-b419db5f83a2','12345678901','12345678901','email.tests111@gmail.com','EMAIL.TESTS111@GMAIL.COM',_binary '\0','AQAAAAIAAYagAAAAENVpifZG1DzN69OhOXiwT5AaDx3inzXdU6wocfyuaCkscgElW/3SqCg8GQGB42TwMQ==','JXNF7LPR3MZGXSJN6II32YZPDPM7I42G','65dc4680-433e-4720-af3a-f14284162399',NULL,_binary '\0',_binary '\0',NULL,_binary '',0);
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
  `id` int NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int NOT NULL,
  `tipo` enum('FIXO','EXTRA') NOT NULL DEFAULT 'FIXO',
  `dataHoraInicio` datetime NOT NULL,
  `dataHoraFim` datetime NOT NULL,
  `presencaObrigatoria` tinyint NOT NULL DEFAULT '0',
  `local` varchar(100) DEFAULT NULL,
  `repertorio` varchar(1000) DEFAULT NULL,
  `idColaboradorResponsavel` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel`),
  KEY `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Ensaio_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_Ensaio_Pessoa1` FOREIGN KEY (`idColaboradorResponsavel`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ensaio`
--

LOCK TABLES `ensaio` WRITE;
/*!40000 ALTER TABLE `ensaio` DISABLE KEYS */;
INSERT INTO `ensaio` VALUES (1,3,'FIXO','2024-08-15 14:00:00','2024-08-15 16:00:00',1,'Auditório Principal','Sinfonia No. 5 de Beethoven, Abertura de Egmont, Concerto para Piano No. 2 de Rachmaninoff',1),(2,3,'EXTRA','2024-08-20 18:30:00','2024-08-20 20:00:00',0,'Sala de Concertos','Noite de Jazz com John Coltrane, Miles Davis, Herbie Hancock',1),(3,3,'FIXO','2024-09-01 10:00:00','2024-09-01 12:00:00',1,'Teatro Municipal','Abertura 1812 de Tchaikovsky, Suite do Quebra-Nozes, Concerto para Violino de Mendelssohn',1),(4,3,'EXTRA','2024-09-10 15:00:00','2024-09-10 17:00:00',0,'Parque Central','Concerto ao ar livre com temas de filmes, trilha sonora de Star Wars, Indiana Jones, Harry Potter',1);
/*!40000 ALTER TABLE `ensaio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ensaiopessoa`
--

DROP TABLE IF EXISTS `ensaiopessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ensaiopessoa` (
  `idPessoa` int NOT NULL,
  `idEnsaio` int NOT NULL,
  `presente` tinyint NOT NULL DEFAULT '0',
  `justificativaFalta` varchar(200) DEFAULT NULL,
  `justificativaAceita` tinyint NOT NULL DEFAULT '0',
  `idPapelGrupo` int NOT NULL,
  PRIMARY KEY (`idPessoa`,`idEnsaio`),
  KEY `fk_PessoaEnsaio_Ensaio1_idx` (`idEnsaio`),
  KEY `fk_PessoaEnsaio_Pessoa1_idx` (`idPessoa`),
  KEY `fk_EnsaioPessoa_PapelGrupo1_idx` (`idPapelGrupo`),
  CONSTRAINT `fk_EnsaioPessoa_PapelGrupo1` FOREIGN KEY (`idPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`),
  CONSTRAINT `fk_PessoaEnsaio_Ensaio1` FOREIGN KEY (`idEnsaio`) REFERENCES `ensaio` (`id`),
  CONSTRAINT `fk_PessoaEnsaio_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ensaiopessoa`
--

LOCK TABLES `ensaiopessoa` WRITE;
/*!40000 ALTER TABLE `ensaiopessoa` DISABLE KEYS */;
/*!40000 ALTER TABLE `ensaiopessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `evento`
--

DROP TABLE IF EXISTS `evento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `evento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int NOT NULL,
  `dataHoraInicio` datetime NOT NULL,
  `dataHoraFim` datetime NOT NULL,
  `local` varchar(100) DEFAULT NULL,
  `repertorio` varchar(1000) DEFAULT NULL,
  `idColaboradorResponsavel` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel`),
  KEY `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Ensaio_GrupoMusical10` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_Ensaio_Pessoa10` FOREIGN KEY (`idColaboradorResponsavel`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `evento`
--

LOCK TABLES `evento` WRITE;
/*!40000 ALTER TABLE `evento` DISABLE KEYS */;
INSERT INTO `evento` VALUES (1,3,'2024-08-15 16:00:00','2024-08-15 19:00:00','Sest','Forro',1),(2,3,'2024-08-15 16:00:00','2024-08-15 19:00:00','Sesc','Quadrilha',1);
/*!40000 ALTER TABLE `evento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eventopessoa`
--

DROP TABLE IF EXISTS `eventopessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eventopessoa` (
  `idEvento` int NOT NULL,
  `idPessoa` int NOT NULL,
  `idTipoInstrumento` int NOT NULL,
  `presente` tinyint NOT NULL DEFAULT '0',
  `justificativaFalta` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_bin DEFAULT NULL,
  `justificativaAceita` tinyint NOT NULL DEFAULT '0',
  `status` enum('INSCRITO','DEFERIDO','INDEFERIDO') NOT NULL DEFAULT 'INSCRITO',
  `idPapelGrupoPapelGrupo` int NOT NULL,
  PRIMARY KEY (`idEvento`,`idPessoa`),
  KEY `fk_ApresentacaoPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_ApresentacaoPessoa_Apresentacao1_idx` (`idEvento`),
  KEY `fk_ApresentacaoPessoa_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_EventoPessoa_PapelGrupo1_idx` (`idPapelGrupoPapelGrupo`),
  CONSTRAINT `fk_ApresentacaoPessoa_Apresentacao1` FOREIGN KEY (`idEvento`) REFERENCES `evento` (`id`),
  CONSTRAINT `fk_ApresentacaoPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`),
  CONSTRAINT `fk_ApresentacaoPessoa_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`),
  CONSTRAINT `fk_EventoPessoa_PapelGrupo1` FOREIGN KEY (`idPapelGrupoPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eventopessoa`
--

LOCK TABLES `eventopessoa` WRITE;
/*!40000 ALTER TABLE `eventopessoa` DISABLE KEYS */;
/*!40000 ALTER TABLE `eventopessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurino`
--

DROP TABLE IF EXISTS `figurino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurino` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(100) NOT NULL,
  `data` date DEFAULT NULL,
  `idGrupoMusical` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `fk_Figurino_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_Figurino_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurino`
--

LOCK TABLES `figurino` WRITE;
/*!40000 ALTER TABLE `figurino` DISABLE KEYS */;
INSERT INTO `figurino` VALUES (1,'Vestido','2023-05-24',3),(2,'Fantasia Dracula','2023-05-24',3);
/*!40000 ALTER TABLE `figurino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinoapresentacao`
--

DROP TABLE IF EXISTS `figurinoapresentacao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinoapresentacao` (
  `idFigurino` int NOT NULL,
  `idApresentacao` int NOT NULL,
  PRIMARY KEY (`idFigurino`,`idApresentacao`),
  KEY `fk_FigurinoApresentacao_Apresentacao1_idx` (`idApresentacao`),
  KEY `fk_FigurinoApresentacao_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoApresentacao_Apresentacao1` FOREIGN KEY (`idApresentacao`) REFERENCES `evento` (`id`),
  CONSTRAINT `fk_FigurinoApresentacao_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurinoapresentacao`
--

LOCK TABLES `figurinoapresentacao` WRITE;
/*!40000 ALTER TABLE `figurinoapresentacao` DISABLE KEYS */;
/*!40000 ALTER TABLE `figurinoapresentacao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinoensaio`
--

DROP TABLE IF EXISTS `figurinoensaio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinoensaio` (
  `idFigurino` int NOT NULL,
  `idEnsaio` int NOT NULL,
  PRIMARY KEY (`idFigurino`,`idEnsaio`),
  KEY `fk_FigurinoEnsaio_Ensaio1_idx` (`idEnsaio`),
  KEY `fk_FigurinoEnsaio_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoEnsaio_Ensaio1` FOREIGN KEY (`idEnsaio`) REFERENCES `ensaio` (`id`),
  CONSTRAINT `fk_FigurinoEnsaio_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `figurinoensaio`
--

LOCK TABLES `figurinoensaio` WRITE;
/*!40000 ALTER TABLE `figurinoensaio` DISABLE KEYS */;
/*!40000 ALTER TABLE `figurinoensaio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `figurinomanequim`
--

DROP TABLE IF EXISTS `figurinomanequim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `figurinomanequim` (
  `idFigurino` int NOT NULL,
  `idManequim` int NOT NULL,
  `quantidadeDisponivel` int NOT NULL DEFAULT '0',
  `quantidadeEntregue` int NOT NULL DEFAULT '0',
  `quantidadeDescartada` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`idFigurino`,`idManequim`),
  KEY `fk_FigurinoManequim_Manequim1_idx` (`idManequim`),
  KEY `fk_FigurinoManequim_Figurino1_idx` (`idFigurino`),
  CONSTRAINT `fk_FigurinoManequim_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`),
  CONSTRAINT `fk_FigurinoManequim_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
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
  `id` int NOT NULL AUTO_INCREMENT,
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
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupomusical`
--

LOCK TABLES `grupomusical` WRITE;
/*!40000 ALTER TABLE `grupomusical` DISABLE KEYS */;
INSERT INTO `grupomusical` VALUES (3,'Gamma Beats','Gamma Beats Music Ltda','22334455000167','98765000','Rua das Flores','Jardim das Acácias','Curitiba','PR','Brasil','contato@gammabeats.com.br','youtube.com/gammabeats','instagram.com/gammabeats','facebook.com/gammabeats','(41) 98765-4321','(41) 98765-6789','11987654325','9101','11987654326','gammabeats@pix.com','email'),(4,'Delta Harmony','Delta Harmony Ltda','33445566000178','87654000','Rua dos Pioneiros','Vila Nova','Porto Alegre','RS','Brasil','contato@deltaharmony.com.br','youtube.com/deltaharmony','instagram.com/deltaharmony','facebook.com/deltaharmony','(51) 97654-3210','(51) 97654-5678','11987654327','1121','11987654328','deltaharmony@pix.com','email'),(5,'Echo Rhythm','Echo Rhythm Ltda','44556677000189','76543000','Avenida Central','Centro','Belo Horizonte','MG','Brasil','contato@echorhythm.com.br','youtube.com/echorhythm','instagram.com/echorhythm','facebook.com/echorhythm','(31) 96543-2109','(31) 96543-5678','11987654329','3141','11987654330','echorhythm@pix.com','email'),(6,'Foxtrot Sound','Foxtrot Sound Ltda','55667788000190','65432000','Rua Primavera','Jardim Primavera','Florianópolis','SC','Brasil','contato@foxtrotsound.com.br','youtube.com/foxtrotsound','instagram.com/foxtrotsound','facebook.com/foxtrotsound','(48) 95432-1098','(48) 95432-5678','11987654331','5161','11987654332','foxtrotsound@pix.com','email'),(7,'Golf Harmony','Golf Harmony Ltda','66778899000121','54321000','Rua dos Pássaros','Vila das Aves','Salvador','BA','Brasil','contato@golfharmony.com.br','youtube.com/golfharmony','instagram.com/golfharmony','facebook.com/golfharmony','(71) 94321-0987','(71) 94321-5678','11987654333','7181','11987654334','golfharmony@pix.com','email'),(8,'Hotel Melody','Hotel Melody Ltda','77889900100132','43210000','Avenida das Américas','Zona Sul','Recife','PE','Brasil','contato@hotelmelody.com.br','youtube.com/hotelmelody','instagram.com/hotelmelody','facebook.com/hotelmelody','(81) 93210-9876','(81) 93210-5678','11987654335','9201','11987654336','hotelmelody@pix.com','email'),(9,'India Tune','India Tune Ltda','88990011200143','32100000','Rua das Palmeiras','Centro','Fortaleza','CE','Brasil','contato@indiatune.com.br','youtube.com/indiatune','instagram.com/indiatune','facebook.com/indiatune','(85) 92109-8765','(85) 92109-5678','11987654337','1221','11987654338','indiatune@pix.com','email'),(10,'Juliett Music','Juliett Music Ltda','99001122300154','21000000','Avenida dos Estados','Centro','Manaus','AM','Brasil','contato@juliettmusic.com.br','youtube.com/juliettmusic','instagram.com/juliettmusic','facebook.com/juliettmusic','(92) 91098-7654','(92) 91098-5678','11987654339','3241','11987654340','juliettmusic@pix.com','email'),(11,'Kilo Beat','Kilo Beat Ltda','10111233400165','10900000','Rua do Sol','Centro','Brasília','DF','Brasil','contato@kilobeat.com.br','youtube.com/kilobeat','instagram.com/kilobeat','facebook.com/kilobeat','(61) 90987-6543','(61) 90987-5678','11987654341','5261','11987654342','kilobeat@pix.com','email'),(12,'Lima Sound','Lima Sound Ltda','11222344500176','99880000','Avenida Oceânica','Zona Norte','Natal','RN','Brasil','contato@limasound.com.br','youtube.com/limasound','instagram.com/limasound','facebook.com/limasound','(84) 89876-5432','(84) 89876-5678','11987654343','7281','11987654344','limasound@pix.com','email'),(13,'Mike Melody','Mike Melody Ltda','22333455600187','88770000','Rua da Harmonia','Vila Harmonia','João Pessoa','PB','Brasil','contato@mikemelody.com.br','youtube.com/mikemelody','instagram.com/mikemelody','facebook.com/mikemelody','(83) 88765-4321','(83) 88765-5678','11987654345','8301','11987654346','mikemelody@pix.com','email'),(14,'November Harmony','November Harmony Ltda','33444566700198','77660000','Avenida Principal','Centro','Teresina','PI','Brasil','contato@novemberharmony.com.br','youtube.com/novemberharmony','instagram.com/novemberharmony','facebook.com/novemberharmony','(86) 87654-3210','(86) 87654-5678','11987654347','9321','11987654348','novemberharmony@pix.com','email'),(15,'Oscar Beat','Oscar Beat Ltda','44555677800209','66550000','Rua do Comércio','Centro','São Luís','MA','Brasil','contato@oscarbeat.com.br','youtube.com/oscarbeat','instagram.com/oscarbeat','facebook.com/oscarbeat','(98) 76543-2109','(98) 76543-5678','11987654349','1341','11987654350','oscarbeat@pix.com','email'),(16,'Papa Melody','Papa Melody Ltda','55666788900210','55440000','Avenida das Nações','Centro','Belém','PA','Brasil','contato@papamelody.com.br','youtube.com/papamelody','instagram.com/papamelody','facebook.com/papamelody','(91) 65432-1098','(91) 65432-5678','11987654351','2361','11987654352','papamelody@pix.com','email');
/*!40000 ALTER TABLE `grupomusical` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `informativo`
--

DROP TABLE IF EXISTS `informativo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `informativo` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` int NOT NULL,
  `idPessoa` int NOT NULL,
  `mensagem` varchar(2000) NOT NULL,
  `data` datetime NOT NULL,
  `entregarAssociadosAtivos` tinyint NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `fk_GrupoMusicalPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_GrupoMusicalPessoa_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_GrupoMusicalPessoa_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_GrupoMusicalPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `informativo`
--

LOCK TABLES `informativo` WRITE;
/*!40000 ALTER TABLE `informativo` DISABLE KEYS */;
INSERT INTO `informativo` VALUES (1,3,1,'Esta é uma mensagem de teste de informativo','2024-03-05 00:00:00',0),(2,3,1,'Esta é uma mensagem de teste de informativo','2024-02-05 00:00:00',0),(3,3,1,'Esta é uma mensagem de teste de informativo','2024-03-21 00:00:00',0),(4,3,1,'Esta é uma mensagem de teste de informativo','2024-02-05 00:00:00',0);
/*!40000 ALTER TABLE `informativo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `instrumentomusical`
--

DROP TABLE IF EXISTS `instrumentomusical`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `instrumentomusical` (
  `id` int NOT NULL AUTO_INCREMENT,
  `patrimonio` varchar(20) NOT NULL,
  `dataAquisicao` date NOT NULL,
  `status` enum('DISPONIVEL','EMPRESTADO','DANIFICADO') NOT NULL DEFAULT 'DISPONIVEL',
  `idTipoInstrumento` int NOT NULL,
  `idGrupoMusical` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_InstrumentoMusical_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_InstrumentoMusical_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_InstrumentoMusical_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_InstrumentoMusical_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `instrumentomusical`
--

LOCK TABLES `instrumentomusical` WRITE;
/*!40000 ALTER TABLE `instrumentomusical` DISABLE KEYS */;
INSERT INTO `instrumentomusical` VALUES (1,'123456789','2024-05-15','DISPONIVEL',1,3),(2,'993456789','2024-04-16','DISPONIVEL',2,3);
/*!40000 ALTER TABLE `instrumentomusical` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manequim`
--

DROP TABLE IF EXISTS `manequim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manequim` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tamanho` varchar(2) NOT NULL,
  `descricao` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manequim`
--

LOCK TABLES `manequim` WRITE;
/*!40000 ALTER TABLE `manequim` DISABLE KEYS */;
INSERT INTO `manequim` VALUES (5,'PP','Extra Pequeno'),(7,'G','Grande'),(9,'M','MEDIO');
/*!40000 ALTER TABLE `manequim` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materialestudo`
--

DROP TABLE IF EXISTS `materialestudo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materialestudo` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(200) NOT NULL,
  `link` varchar(500) NOT NULL,
  `data` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `idGrupoMusical` int NOT NULL,
  `idColaborador` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_MaterialEstudo_GrupoMusical1_idx` (`idGrupoMusical`),
  KEY `fk_MaterialEstudo_Pessoa1_idx` (`idColaborador`),
  CONSTRAINT `fk_MaterialEstudo_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_MaterialEstudo_Pessoa1` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materialestudo`
--

LOCK TABLES `materialestudo` WRITE;
/*!40000 ALTER TABLE `materialestudo` DISABLE KEYS */;
INSERT INTO `materialestudo` VALUES (1,'Frevo','drive.com','2024-04-15 00:00:00',3,1),(2,'Forro','drive.com','2024-07-15 00:00:00',3,1);
/*!40000 ALTER TABLE `materialestudo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimentacaofigurino`
--

DROP TABLE IF EXISTS `movimentacaofigurino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimentacaofigurino` (
  `id` int NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `idFigurino` int NOT NULL,
  `idManequim` int NOT NULL,
  `idAssociado` int NOT NULL,
  `idColaborador` int NOT NULL,
  `status` enum('DISPONIVEL','ENTREGUE','RECEBIDO','DANIFICADO','DEVOLVIDO') NOT NULL DEFAULT 'DISPONIVEL',
  `confirmacaoRecebimento` tinyint NOT NULL DEFAULT '0',
  `quantidade` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_EntregarFigurino_Figurino1_idx` (`idFigurino`),
  KEY `fk_EntregarFigurino_Manequim1_idx` (`idManequim`),
  KEY `fk_EntregarFigurino_Pessoa1_idx` (`idAssociado`),
  KEY `fk_EntregarFigurino_Pessoa2_idx` (`idColaborador`),
  CONSTRAINT `fk_EntregarFigurino_Figurino1` FOREIGN KEY (`idFigurino`) REFERENCES `figurino` (`id`),
  CONSTRAINT `fk_EntregarFigurino_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`),
  CONSTRAINT `fk_EntregarFigurino_Pessoa1` FOREIGN KEY (`idAssociado`) REFERENCES `pessoa` (`id`),
  CONSTRAINT `fk_EntregarFigurino_Pessoa2` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
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
  `id` int NOT NULL AUTO_INCREMENT,
  `data` datetime NOT NULL,
  `idInstrumentoMusical` int NOT NULL,
  `idAssociado` int NOT NULL,
  `idColaborador` int NOT NULL,
  `confirmacaoAssociado` tinyint NOT NULL DEFAULT '0',
  `tipoMovimento` enum('EMPRESTIMO','DEVOLUCAO') NOT NULL DEFAULT 'EMPRESTIMO',
  PRIMARY KEY (`id`),
  KEY `fk_DevolverInstrumento_InstrumentoMusical1_idx` (`idInstrumentoMusical`),
  KEY `fk_DevolverInstrumento_Pessoa1_idx` (`idAssociado`),
  KEY `fk_DevolverInstrumento_Pessoa2_idx` (`idColaborador`),
  CONSTRAINT `fk_DevolverInstrumento_InstrumentoMusical1` FOREIGN KEY (`idInstrumentoMusical`) REFERENCES `instrumentomusical` (`id`),
  CONSTRAINT `fk_DevolverInstrumento_Pessoa1` FOREIGN KEY (`idAssociado`) REFERENCES `pessoa` (`id`),
  CONSTRAINT `fk_DevolverInstrumento_Pessoa2` FOREIGN KEY (`idColaborador`) REFERENCES `pessoa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
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
  `idPapelGrupo` int NOT NULL,
  `nome` varchar(100) NOT NULL,
  PRIMARY KEY (`idPapelGrupo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `papelgrupo`
--

LOCK TABLES `papelgrupo` WRITE;
/*!40000 ALTER TABLE `papelgrupo` DISABLE KEYS */;
INSERT INTO `papelgrupo` VALUES (1,'ASSOCIADO'),(2,'COLABORADOR'),(3,'ADMINISTRADOR GRUPO'),(5,'REGENTE');
/*!40000 ALTER TABLE `papelgrupo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pessoa`
--

DROP TABLE IF EXISTS `pessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pessoa` (
  `id` int NOT NULL AUTO_INCREMENT,
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
  `ativo` tinyint NOT NULL DEFAULT '0',
  `isentoPagamento` tinyint NOT NULL DEFAULT '0',
  `idGrupoMusical` int NOT NULL,
  `idPapelGrupo` int NOT NULL,
  `idManequim` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`),
  KEY `fk_Pessoa_GrupoMusical1_idx` (`idGrupoMusical`),
  KEY `fk_Pessoa_PapelGrupoMusical1_idx` (`idPapelGrupo`),
  KEY `fk_Pessoa_Manequim1_idx` (`idManequim`),
  CONSTRAINT `fk_Pessoa_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`),
  CONSTRAINT `fk_Pessoa_Manequim1` FOREIGN KEY (`idManequim`) REFERENCES `manequim` (`id`),
  CONSTRAINT `fk_Pessoa_PapelGrupoMusical1` FOREIGN KEY (`idPapelGrupo`) REFERENCES `papelgrupo` (`idPapelGrupo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pessoa`
--

LOCK TABLES `pessoa` WRITE;
/*!40000 ALTER TABLE `pessoa` DISABLE KEYS */;
INSERT INTO `pessoa` VALUES (1,'12345678901','João Silva','M','12345678','Rua das Flores','Centro','São Paulo','SP','1980-05-15','11987654321','1187654321','email.tests111@gmail.com','2020-01-01',NULL,NULL,1,0,3,1,5),(2,'10987654321','Maria Souza','F','87654321','Avenida Paulista','Bela Vista','São Paulo','SP','1990-03-22','11912345678','1134567890','email.tests112@gmail.com','2020-02-15',NULL,NULL,1,0,3,1,5);
/*!40000 ALTER TABLE `pessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pessoatipoinstrumento`
--

DROP TABLE IF EXISTS `pessoatipoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pessoatipoinstrumento` (
  `idPessoa` int NOT NULL,
  `idTipoInstrumento` int NOT NULL,
  PRIMARY KEY (`idPessoa`,`idTipoInstrumento`),
  KEY `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento`),
  KEY `fk_Pessoa_has_TipoInstrumento_Pessoa_idx` (`idPessoa`),
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_Pessoa` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`),
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1` FOREIGN KEY (`idTipoInstrumento`) REFERENCES `tipoinstrumento` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
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
  `id` int NOT NULL AUTO_INCREMENT,
  `descricao` varchar(100) NOT NULL,
  `dataInicio` date NOT NULL,
  `dataFim` date NOT NULL,
  `valor` decimal(10,2) NOT NULL,
  `idGrupoMusical` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_ReceitaFinanceira_GrupoMusical1_idx` (`idGrupoMusical`),
  CONSTRAINT `fk_ReceitaFinanceira_GrupoMusical1` FOREIGN KEY (`idGrupoMusical`) REFERENCES `grupomusical` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receitafinanceira`
--

LOCK TABLES `receitafinanceira` WRITE;
/*!40000 ALTER TABLE `receitafinanceira` DISABLE KEYS */;
/*!40000 ALTER TABLE `receitafinanceira` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receitafinanceirapessoa`
--

DROP TABLE IF EXISTS `receitafinanceirapessoa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receitafinanceirapessoa` (
  `idReceitaFinanceira` int NOT NULL,
  `idPessoa` int NOT NULL,
  `valor` decimal(10,2) NOT NULL,
  `valorPago` decimal(10,2) NOT NULL DEFAULT '0.00',
  `dataPagamento` datetime NOT NULL,
  `observacoes` varchar(200) DEFAULT NULL,
  `status` enum('ABERTO','ENVIADO','PAGO','ISENTO') NOT NULL DEFAULT 'ABERTO',
  PRIMARY KEY (`idReceitaFinanceira`,`idPessoa`),
  KEY `fk_ReceitaFinanceiraPessoa_Pessoa1_idx` (`idPessoa`),
  KEY `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1_idx` (`idReceitaFinanceira`),
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_Pessoa1` FOREIGN KEY (`idPessoa`) REFERENCES `pessoa` (`id`),
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1` FOREIGN KEY (`idReceitaFinanceira`) REFERENCES `receitafinanceira` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receitafinanceirapessoa`
--

LOCK TABLES `receitafinanceirapessoa` WRITE;
/*!40000 ALTER TABLE `receitafinanceirapessoa` DISABLE KEYS */;
/*!40000 ALTER TABLE `receitafinanceirapessoa` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipoinstrumento`
--

DROP TABLE IF EXISTS `tipoinstrumento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipoinstrumento` (
  `id` int NOT NULL,
  `nome` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipoinstrumento`
--

LOCK TABLES `tipoinstrumento` WRITE;
/*!40000 ALTER TABLE `tipoinstrumento` DISABLE KEYS */;
INSERT INTO `tipoinstrumento` VALUES (1,'Sopro'),(2,'Corda');
/*!40000 ALTER TABLE `tipoinstrumento` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-08-21  1:12:33
