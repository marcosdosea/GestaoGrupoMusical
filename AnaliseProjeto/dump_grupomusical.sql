-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema GrupoMusical
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema GrupoMusical
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `GrupoMusical` DEFAULT CHARACTER SET utf8 ;
USE `GrupoMusical` ;

-- -----------------------------------------------------
-- Table `GrupoMusical`.`GrupoMusical`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`GrupoMusical` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `razaoSocial` VARCHAR(100) NOT NULL,
  `cnpj` VARCHAR(14) NOT NULL,
  `cep` VARCHAR(8) NULL,
  `rua` VARCHAR(100) NULL,
  `bairro` VARCHAR(100) NULL,
  `cidade` VARCHAR(100) NULL,
  `estado` CHAR(2) NOT NULL DEFAULT 'SE',
  `pais` VARCHAR(50) NULL,
  `email` VARCHAR(100) NULL,
  `youtube` VARCHAR(100) NULL,
  `instagram` VARCHAR(100) NULL,
  `facebook` VARCHAR(100) NULL,
  `telefone1` VARCHAR(20) NULL,
  `telefone2` VARCHAR(20) NULL,
  `banco` VARCHAR(100) NULL,
  `agencia` VARCHAR(15) NULL,
  `numeroContaBanco` VARCHAR(15) NULL,
  `chavePIX` VARCHAR(100) NULL,
  `chavePIXTipo` VARCHAR(15) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cnpj_UNIQUE` (`cnpj` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`PapelGrupo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`PapelGrupo` (
  `idPapelGrupo` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`idPapelGrupo`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Manequim`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Manequim` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `tamanho` VARCHAR(2) NOT NULL,
  `descricao` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Pessoa`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Pessoa` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `cpf` VARCHAR(11) NOT NULL,
  `nome` VARCHAR(100) NOT NULL,
  `sexo` ENUM('M', 'F') NOT NULL DEFAULT 'F',
  `cep` VARCHAR(8) NOT NULL,
  `rua` VARCHAR(100) NULL,
  `bairro` VARCHAR(100) NULL,
  `cidade` VARCHAR(100) NULL,
  `estado` CHAR(2) NOT NULL DEFAULT 'SE',
  `dataNascimento` DATE NULL,
  `telefone1` VARCHAR(20) NOT NULL,
  `telefone2` VARCHAR(20) NULL,
  `email` VARCHAR(100) NOT NULL,
  `dataEntrada` DATE NULL,
  `dataSaida` DATE NULL,
  `motivoSaida` VARCHAR(100) NULL,
  `ativo` TINYINT NOT NULL DEFAULT 0,
  `isentoPagamento` TINYINT NOT NULL DEFAULT 0,
  `idGrupoMusical` INT NOT NULL,
  `idPapelGrupo` INT NOT NULL,
  `idManequim` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cpf_UNIQUE` (`cpf` ASC),
  INDEX `fk_Pessoa_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  INDEX `fk_Pessoa_PapelGrupoMusical1_idx` (`idPapelGrupo` ASC),
  INDEX `fk_Pessoa_Manequim1_idx` (`idManequim` ASC),
  CONSTRAINT `fk_Pessoa_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_PapelGrupoMusical1`
    FOREIGN KEY (`idPapelGrupo`)
    REFERENCES `GrupoMusical`.`PapelGrupo` (`idPapelGrupo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_Manequim1`
    FOREIGN KEY (`idManequim`)
    REFERENCES `GrupoMusical`.`Manequim` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`TipoInstrumento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`TipoInstrumento` (
  `id` INT NOT NULL,
  `nome` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`PessoaTipoInstrumento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`PessoaTipoInstrumento` (
  `idPessoa` INT NOT NULL,
  `idTipoInstrumento` INT NOT NULL,
  PRIMARY KEY (`idPessoa`, `idTipoInstrumento`),
  INDEX `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento` ASC),
  INDEX `fk_Pessoa_has_TipoInstrumento_Pessoa_idx` (`idPessoa` ASC),
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_Pessoa`
    FOREIGN KEY (`idPessoa`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Pessoa_has_TipoInstrumento_TipoInstrumento1`
    FOREIGN KEY (`idTipoInstrumento`)
    REFERENCES `GrupoMusical`.`TipoInstrumento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`InstrumentoMusical`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`InstrumentoMusical` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `patrimonio` VARCHAR(20) NOT NULL,
  `dataAquisicao` DATE NOT NULL,
  `status` ENUM('DISPONIVEL', 'EMPRESTADO', 'DANIFICADO') NOT NULL DEFAULT 'DISPONIVEL',
  `idTipoInstrumento` INT NOT NULL,
  `idGrupoMusical` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_InstrumentoMusical_TipoInstrumento1_idx` (`idTipoInstrumento` ASC),
  INDEX `fk_InstrumentoMusical_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_InstrumentoMusical_TipoInstrumento1`
    FOREIGN KEY (`idTipoInstrumento`)
    REFERENCES `GrupoMusical`.`TipoInstrumento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InstrumentoMusical_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`MovimentacaoInstrumento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`MovimentacaoInstrumento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `data` DATETIME NOT NULL,
  `idInstrumentoMusical` INT NOT NULL,
  `idAssociado` INT NOT NULL,
  `idColaborador` INT NOT NULL,
  `confirmacaoAssociado` TINYINT NOT NULL DEFAULT 0,
  `tipoMovimento` ENUM('EMPRESTIMO', 'DEVOLUCAO') NOT NULL DEFAULT 'EMPRESTIMO',
  PRIMARY KEY (`id`),
  INDEX `fk_DevolverInstrumento_InstrumentoMusical1_idx` (`idInstrumentoMusical` ASC),
  INDEX `fk_DevolverInstrumento_Pessoa1_idx` (`idAssociado` ASC),
  INDEX `fk_DevolverInstrumento_Pessoa2_idx` (`idColaborador` ASC),
  CONSTRAINT `fk_DevolverInstrumento_InstrumentoMusical1`
    FOREIGN KEY (`idInstrumentoMusical`)
    REFERENCES `GrupoMusical`.`InstrumentoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_DevolverInstrumento_Pessoa1`
    FOREIGN KEY (`idAssociado`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_DevolverInstrumento_Pessoa2`
    FOREIGN KEY (`idColaborador`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Figurino`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Figurino` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `data` DATE NULL,
  `idGrupoMusical` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_Figurino_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_Figurino_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`FigurinoManequim`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`FigurinoManequim` (
  `idFigurino` INT NOT NULL,
  `idManequim` INT NOT NULL,
  `quantidadeDisponivel` INT NOT NULL DEFAULT 0,
  `quantidadeEntregue` INT NOT NULL DEFAULT 0,
  `quantidadeDescartada` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`idFigurino`, `idManequim`),
  INDEX `fk_FigurinoManequim_Manequim1_idx` (`idManequim` ASC),
  INDEX `fk_FigurinoManequim_Figurino1_idx` (`idFigurino` ASC),
  CONSTRAINT `fk_FigurinoManequim_Figurino1`
    FOREIGN KEY (`idFigurino`)
    REFERENCES `GrupoMusical`.`Figurino` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoManequim_Manequim1`
    FOREIGN KEY (`idManequim`)
    REFERENCES `GrupoMusical`.`Manequim` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`MovimentacaoFigurino`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`MovimentacaoFigurino` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `data` DATETIME NOT NULL,
  `idFigurino` INT NOT NULL,
  `idManequim` INT NOT NULL,
  `idAssociado` INT NOT NULL,
  `idColaborador` INT NOT NULL,
  `status` ENUM('DISPONIVEL', 'ENTREGUE', 'RECEBIDO', 'DANIFICADO', 'DEVOLVIDO') NOT NULL DEFAULT 'DISPONIVEL',
  `confirmacaoRecebimento` TINYINT NOT NULL DEFAULT 0,
  `quantidade` INT NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  INDEX `fk_EntregarFigurino_Figurino1_idx` (`idFigurino` ASC),
  INDEX `fk_EntregarFigurino_Manequim1_idx` (`idManequim` ASC),
  INDEX `fk_EntregarFigurino_Pessoa1_idx` (`idAssociado` ASC),
  INDEX `fk_EntregarFigurino_Pessoa2_idx` (`idColaborador` ASC),
  CONSTRAINT `fk_EntregarFigurino_Figurino1`
    FOREIGN KEY (`idFigurino`)
    REFERENCES `GrupoMusical`.`Figurino` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Manequim1`
    FOREIGN KEY (`idManequim`)
    REFERENCES `GrupoMusical`.`Manequim` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Pessoa1`
    FOREIGN KEY (`idAssociado`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_EntregarFigurino_Pessoa2`
    FOREIGN KEY (`idColaborador`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Ensaio`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Ensaio` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` INT NOT NULL,
  `tipo` ENUM('FIXO', 'EXTRA') NOT NULL DEFAULT 'FIXO',
  `dataHoraInicio` DATETIME NOT NULL,
  `dataHoraFim` DATETIME NOT NULL,
  `presencaObrigatoria` TINYINT NOT NULL DEFAULT 0,
  `local` VARCHAR(100) NULL,
  `repertorio` VARCHAR(1000) NULL,
  `idColaboradorResponsavel` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel` ASC),
  INDEX `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_Ensaio_Pessoa1`
    FOREIGN KEY (`idColaboradorResponsavel`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ensaio_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`FigurinoEnsaio`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`FigurinoEnsaio` (
  `idFigurino` INT NOT NULL,
  `idEnsaio` INT NOT NULL,
  PRIMARY KEY (`idFigurino`, `idEnsaio`),
  INDEX `fk_FigurinoEnsaio_Ensaio1_idx` (`idEnsaio` ASC),
  INDEX `fk_FigurinoEnsaio_Figurino1_idx` (`idFigurino` ASC),
  CONSTRAINT `fk_FigurinoEnsaio_Figurino1`
    FOREIGN KEY (`idFigurino`)
    REFERENCES `GrupoMusical`.`Figurino` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoEnsaio_Ensaio1`
    FOREIGN KEY (`idEnsaio`)
    REFERENCES `GrupoMusical`.`Ensaio` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Evento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Evento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` INT NOT NULL,
  `dataHoraInicio` DATETIME NOT NULL,
  `dataHoraFim` DATETIME NOT NULL,
  `local` VARCHAR(100) NULL,
  `repertorio` VARCHAR(1000) NULL,
  `idColaboradorResponsavel` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Ensaio_Pessoa1_idx` (`idColaboradorResponsavel` ASC),
  INDEX `fk_Ensaio_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_Ensaio_Pessoa10`
    FOREIGN KEY (`idColaboradorResponsavel`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Ensaio_GrupoMusical10`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`FigurinoApresentacao`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`FigurinoApresentacao` (
  `idFigurino` INT NOT NULL,
  `idApresentacao` INT NOT NULL,
  PRIMARY KEY (`idFigurino`, `idApresentacao`),
  INDEX `fk_FigurinoApresentacao_Apresentacao1_idx` (`idApresentacao` ASC),
  INDEX `fk_FigurinoApresentacao_Figurino1_idx` (`idFigurino` ASC),
  CONSTRAINT `fk_FigurinoApresentacao_Figurino1`
    FOREIGN KEY (`idFigurino`)
    REFERENCES `GrupoMusical`.`Figurino` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_FigurinoApresentacao_Apresentacao1`
    FOREIGN KEY (`idApresentacao`)
    REFERENCES `GrupoMusical`.`Evento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`EnsaioPessoa`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`EnsaioPessoa` (
  `idPessoa` INT NOT NULL,
  `idEnsaio` INT NOT NULL,
  `presente` TINYINT NOT NULL DEFAULT 0,
  `justificativaFalta` VARCHAR(200) NULL,
  `justificativaAceita` TINYINT NOT NULL DEFAULT 0,
  `idPapelGrupoPapelGrupo` INT NOT NULL,
  PRIMARY KEY (`idPessoa`, `idEnsaio`),
  INDEX `fk_PessoaEnsaio_Ensaio1_idx` (`idEnsaio` ASC),
  INDEX `fk_PessoaEnsaio_Pessoa1_idx` (`idPessoa` ASC),
  INDEX `fk_EnsaioPessoa_PapelGrupo1_idx` (`idPapelGrupoPapelGrupo` ASC),
  CONSTRAINT `fk_PessoaEnsaio_Pessoa1`
    FOREIGN KEY (`idPessoa`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_PessoaEnsaio_Ensaio1`
    FOREIGN KEY (`idEnsaio`)
    REFERENCES `GrupoMusical`.`Ensaio` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_EnsaioPessoa_PapelGrupo1`
    FOREIGN KEY (`idPapelGrupoPapelGrupo`)
    REFERENCES `GrupoMusical`.`PapelGrupo` (`idPapelGrupo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`EventoPessoa`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`EventoPessoa` (
  `idEvento` INT NOT NULL,
  `idPessoa` INT NOT NULL,
  `idTipoInstrumento` INT NOT NULL,
  `presente` TINYINT NOT NULL DEFAULT 0,
  `justificativaFalta` VARCHAR(200) BINARY NULL,
  `justificativaAceita` TINYINT NOT NULL DEFAULT 0,
  `status` ENUM('INSCRITO', 'DEFERIDO', 'INDEFERIDO') NOT NULL DEFAULT 'INSCRITO',
  `idPapelGrupoPapelGrupo` INT NOT NULL,
  PRIMARY KEY (`idEvento`, `idPessoa`),
  INDEX `fk_ApresentacaoPessoa_Pessoa1_idx` (`idPessoa` ASC),
  INDEX `fk_ApresentacaoPessoa_Apresentacao1_idx` (`idEvento` ASC),
  INDEX `fk_ApresentacaoPessoa_TipoInstrumento1_idx` (`idTipoInstrumento` ASC),
  INDEX `fk_EventoPessoa_PapelGrupo1_idx` (`idPapelGrupoPapelGrupo` ASC),
  CONSTRAINT `fk_ApresentacaoPessoa_Apresentacao1`
    FOREIGN KEY (`idEvento`)
    REFERENCES `GrupoMusical`.`Evento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoPessoa_Pessoa1`
    FOREIGN KEY (`idPessoa`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoPessoa_TipoInstrumento1`
    FOREIGN KEY (`idTipoInstrumento`)
    REFERENCES `GrupoMusical`.`TipoInstrumento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_EventoPessoa_PapelGrupo1`
    FOREIGN KEY (`idPapelGrupoPapelGrupo`)
    REFERENCES `GrupoMusical`.`PapelGrupo` (`idPapelGrupo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`ApresentacaoTipoInstrumento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`ApresentacaoTipoInstrumento` (
  `idApresentacao` INT NOT NULL,
  `idTipoInstrumento` INT NOT NULL,
  `quantidadePlanejada` INT NOT NULL DEFAULT 0,
  `quantidadeConfirmada` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`idApresentacao`, `idTipoInstrumento`),
  INDEX `fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx` (`idTipoInstrumento` ASC),
  INDEX `fk_ApresentacaoTipoInstrumento_Apresentacao1_idx` (`idApresentacao` ASC),
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_Apresentacao1`
    FOREIGN KEY (`idApresentacao`)
    REFERENCES `GrupoMusical`.`Evento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ApresentacaoTipoInstrumento_TipoInstrumento1`
    FOREIGN KEY (`idTipoInstrumento`)
    REFERENCES `GrupoMusical`.`TipoInstrumento` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`MaterialEstudo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`MaterialEstudo` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(200) NOT NULL,
  `link` VARCHAR(500) NOT NULL,
  `data` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `idGrupoMusical` INT NOT NULL,
  `idColaborador` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_MaterialEstudo_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  INDEX `fk_MaterialEstudo_Pessoa1_idx` (`idColaborador` ASC),
  CONSTRAINT `fk_MaterialEstudo_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_MaterialEstudo_Pessoa1`
    FOREIGN KEY (`idColaborador`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`ReceitaFinanceira`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`ReceitaFinanceira` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descricao` VARCHAR(100) NOT NULL,
  `dataInicio` DATE NOT NULL,
  `dataFim` DATE NOT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `idGrupoMusical` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_ReceitaFinanceira_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_ReceitaFinanceira_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`ReceitaFinanceiraPessoa`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`ReceitaFinanceiraPessoa` (
  `idReceitaFinanceira` INT NOT NULL,
  `idPessoa` INT NOT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `valorPago` DECIMAL(10,2) NOT NULL DEFAULT 0,
  `dataPagamento` DATETIME NOT NULL,
  `observacoes` VARCHAR(200) NULL,
  `status` ENUM('ABERTO', 'ENVIADO', 'PAGO', 'ISENTO') NOT NULL DEFAULT 'ABERTO',
  PRIMARY KEY (`idReceitaFinanceira`, `idPessoa`),
  INDEX `fk_ReceitaFinanceiraPessoa_Pessoa1_idx` (`idPessoa` ASC),
  INDEX `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1_idx` (`idReceitaFinanceira` ASC),
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1`
    FOREIGN KEY (`idReceitaFinanceira`)
    REFERENCES `GrupoMusical`.`ReceitaFinanceira` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ReceitaFinanceiraPessoa_Pessoa1`
    FOREIGN KEY (`idPessoa`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`Informativo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`Informativo` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `idGrupoMusical` INT NOT NULL,
  `idPessoa` INT NOT NULL,
  `mensagem` VARCHAR(2000) NOT NULL,
  `data` DATE NOT NULL,
  `entregarAssociadosAtivos` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  INDEX `fk_GrupoMusicalPessoa_Pessoa1_idx` (`idPessoa` ASC),
  INDEX `fk_GrupoMusicalPessoa_GrupoMusical1_idx` (`idGrupoMusical` ASC),
  CONSTRAINT `fk_GrupoMusicalPessoa_GrupoMusical1`
    FOREIGN KEY (`idGrupoMusical`)
    REFERENCES `GrupoMusical`.`GrupoMusical` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_GrupoMusicalPessoa_Pessoa1`
    FOREIGN KEY (`idPessoa`)
    REFERENCES `GrupoMusical`.`Pessoa` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetroles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetroles` (
  `Id` VARCHAR(767) NOT NULL,
  `Name` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) NULL DEFAULT NULL,
  `ConcurrencyStamp` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `RoleNameIndex` (`NormalizedName` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetroleclaims`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetroleclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `RoleId` VARCHAR(767) NOT NULL,
  `ClaimType` TEXT NULL DEFAULT NULL,
  `ClaimValue` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_AspNetRoleClaims_RoleId` (`RoleId` ASC),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `GrupoMusical`.`aspnetroles` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetusers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetusers` (
  `Id` VARCHAR(767) NOT NULL,
  `UserName` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedUserName` VARCHAR(256) NULL DEFAULT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedEmail` VARCHAR(256) NULL DEFAULT NULL,
  `EmailConfirmed` BIT(1) NOT NULL,
  `PasswordHash` TEXT NULL DEFAULT NULL,
  `SecurityStamp` TEXT NULL DEFAULT NULL,
  `ConcurrencyStamp` TEXT NULL DEFAULT NULL,
  `PhoneNumber` TEXT NULL DEFAULT NULL,
  `PhoneNumberConfirmed` BIT(1) NOT NULL,
  `TwoFactorEnabled` BIT(1) NOT NULL,
  `LockoutEnd` TIMESTAMP NULL DEFAULT NULL,
  `LockoutEnabled` BIT(1) NOT NULL,
  `AccessFailedCount` INT(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserNameIndex` (`NormalizedUserName` ASC),
  INDEX `EmailIndex` (`NormalizedEmail` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetuserclaims`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetuserclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `UserId` VARCHAR(767) NOT NULL,
  `ClaimType` TEXT NULL DEFAULT NULL,
  `ClaimValue` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_AspNetUserClaims_UserId` (`UserId` ASC),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `GrupoMusical`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetuserlogins`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetuserlogins` (
  `LoginProvider` VARCHAR(128) NOT NULL,
  `ProviderKey` VARCHAR(128) NOT NULL,
  `ProviderDisplayName` TEXT NULL DEFAULT NULL,
  `UserId` VARCHAR(767) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`),
  INDEX `IX_AspNetUserLogins_UserId` (`UserId` ASC),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `GrupoMusical`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetuserroles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetuserroles` (
  `UserId` VARCHAR(767) NOT NULL,
  `RoleId` VARCHAR(767) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `IX_AspNetUserRoles_RoleId` (`RoleId` ASC),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `GrupoMusical`.`aspnetroles` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `GrupoMusical`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


-- -----------------------------------------------------
-- Table `GrupoMusical`.`aspnetusertokens`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `GrupoMusical`.`aspnetusertokens` (
  `UserId` VARCHAR(767) NOT NULL,
  `LoginProvider` VARCHAR(128) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `Value` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `GrupoMusical`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
