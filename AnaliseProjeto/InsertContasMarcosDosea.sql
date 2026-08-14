
USE grupomusical; -- Altere se o nome do banco for diferente

-- =======================================================================
-- 1. CONTA: MARCOS DÓSEA ASSOCIADO (Role: ASSOCIADO | idPapelGrupo: 1)
-- CPF: 246.806.210-87
-- =======================================================================
INSERT INTO pessoa (cpf, nome, sexo, cep, rua, bairro, cidade, estado, telefone1, email, ativo, isentoPagamento, idGrupoMusical, idPapelGrupo, idManequim) 
VALUES ('24680621087', 'Marcos Dósea Associado', 'M', '49500000', 'Rua Fictícia', 'Centro', 'Itabaiana', 'SE', '(79)99999-0001', 'dosea.associado@teste.com', 1, 0, 1, 1, 1);

INSERT INTO aspnetusers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES (UUID(), '24680621087', '24680621087', 'dosea.associado@teste.com', 'DOSEA.ASSOCIADO@TESTE.COM', 1, 'AQAAAAIAAYagAAAAEPN+ZZZdZ01hzhtePun3D0JqOE1J1hBh8o9qBqf62LVDAlQfxnwjupWe1CGN+L6rcw==', UUID(), UUID(), 0, 0, 1, 0);

INSERT INTO aspnetuserroles (UserId, RoleId)
VALUES ((SELECT Id FROM aspnetusers WHERE Email = 'dosea.associado@teste.com'), '9d020009-99fd-48c8-b578-9deed019c83a');


-- =======================================================================
-- 2. CONTA: MARCOS DÓSEA ADM GRUPO (Role: ADMINISTRADOR GRUPO | idPapelGrupo: 3)
-- CPF: 647.810.590-31
-- =======================================================================
INSERT INTO pessoa (cpf, nome, sexo, cep, rua, bairro, cidade, estado, telefone1, email, ativo, isentoPagamento, idGrupoMusical, idPapelGrupo, idManequim) 
VALUES ('64781059031', 'Marcos Dósea ADM Grupo', 'M', '49500000', 'Rua Fictícia', 'Centro', 'Itabaiana', 'SE', '(79)99999-0002', 'dosea.admgrupo@teste.com', 1, 0, 1, 3, 1);

INSERT INTO aspnetusers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES (UUID(), '64781059031', '64781059031', 'dosea.admgrupo@teste.com', 'DOSEA.ADMGRUPO@TESTE.COM', 1, 'AQAAAAIAAYagAAAAEPN+ZZZdZ01hzhtePun3D0JqOE1J1hBh8o9qBqf62LVDAlQfxnwjupWe1CGN+L6rcw==', UUID(), UUID(), 0, 0, 1, 0);

INSERT INTO aspnetuserroles (UserId, RoleId)
VALUES ((SELECT Id FROM aspnetusers WHERE Email = 'dosea.admgrupo@teste.com'), '245dbc59-f7ad-490e-90e3-d0fabfda91dc');


-- =======================================================================
-- 3. CONTA: MARCOS DÓSEA ADM SISTEMA (Role: ADMINISTRADOR SISTEMA | idPapelGrupo: 4)
-- CPF: 041.731.710-72
-- =======================================================================
INSERT INTO pessoa (cpf, nome, sexo, cep, rua, bairro, cidade, estado, telefone1, email, ativo, isentoPagamento, idGrupoMusical, idPapelGrupo, idManequim) 
VALUES ('04173171072', 'Marcos Dósea ADM Sistema', 'M', '49500000', 'Rua Fictícia', 'Centro', 'Itabaiana', 'SE', '(79)99999-0003', 'dosea.admsistema@teste.com', 1, 0, 1, 4, 1);

INSERT INTO aspnetusers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES (UUID(), '04173171072', '04173171072', 'dosea.admsistema@teste.com', 'DOSEA.ADMSISTEMA@TESTE.COM', 1, 'AQAAAAIAAYagAAAAEPN+ZZZdZ01hzhtePun3D0JqOE1J1hBh8o9qBqf62LVDAlQfxnwjupWe1CGN+L6rcw==', UUID(), UUID(), 0, 0, 1, 0);

INSERT INTO aspnetuserroles (UserId, RoleId)
VALUES ((SELECT Id FROM aspnetusers WHERE Email = 'dosea.admsistema@teste.com'), '4e41763d-d54c-472a-ab46-dadabb2d8859');


-- =======================================================================
-- 4. CONTA: MARCOS DÓSEA REGENTE (Role: REGENTE | idPapelGrupo: 5)
-- CPF: 101.613.890-31
-- =======================================================================
INSERT INTO pessoa (cpf, nome, sexo, cep, rua, bairro, cidade, estado, telefone1, email, ativo, isentoPagamento, idGrupoMusical, idPapelGrupo, idManequim) 
VALUES ('10161389031', 'Marcos Dósea Regente', 'M', '49500000', 'Rua Fictícia', 'Centro', 'Itabaiana', 'SE', '(79)99999-0004', 'dosea.regente@teste.com', 1, 0, 1, 5, 1);

INSERT INTO aspnetusers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES (UUID(), '10161389031', '10161389031', 'dosea.regente@teste.com', 'DOSEA.REGENTE@TESTE.COM', 1, 'AQAAAAIAAYagAAAAEPN+ZZZdZ01hzhtePun3D0JqOE1J1hBh8o9qBqf62LVDAlQfxnwjupWe1CGN+L6rcw==', UUID(), UUID(), 0, 0, 1, 0);

INSERT INTO aspnetuserroles (UserId, RoleId)
VALUES ((SELECT Id FROM aspnetusers WHERE Email = 'dosea.regente@teste.com'), '66a8639c-b17f-4fcc-8416-2266188635d6');