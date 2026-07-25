SET SQL_SAFE_UPDATES = 0;

DELETE antigo
FROM dispositivopessoa antigo
INNER JOIN dispositivopessoa recente
    ON antigo.fcmToken = recente.fcmToken
    AND antigo.id < recente.id;

ALTER TABLE dispositivopessoa
MODIFY COLUMN fcmToken VARCHAR(512) NOT NULL;

ALTER TABLE dispositivopessoa
ADD CONSTRAINT uq_dispositivopessoa_fcmToken
UNIQUE (fcmToken);

SET SQL_SAFE_UPDATES = 1;