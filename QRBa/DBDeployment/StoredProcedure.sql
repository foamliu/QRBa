USE QRBaDB;
DROP PROCEDURE IF EXISTS QRBaDB.AddAccount;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddAccount
(
    id INT,
    name NVARCHAR(256),
    email VARCHAR(128),
    statusId TINYINT
)
BEGIN

    INSERT INTO Account
    (
        Id,Name,Email,StatusId,InsertedDatetime,InsertedBy
    )
    VALUES
    (
        id,
        name,
        email,
        statusId,
        customerTypeId,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM Account WHERE Id = @id;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetAccount;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetAccount
(
    id int
)
BEGIN

    SELECT * FROM Account
    WHERE Id = id;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddIdentity
(
    memberName NVARCHAR(64),
    passwordHash NVARCHAR(256)
)
BEGIN

    INSERT INTO Identity
    (
        MemberName,
        PasswordHash,
        InsertedDatetime,
        InsertedBy
    )
    VALUES
    (
        memberName,
        passwordHash,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT
        MemberName,
        PasswordHash
    FROM Identity
    WHERE MemberName = memberName;
    
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetIdentity
(
    memberName NVARCHAR(64)
)
BEGIN

    SELECT
        MemberName,
        PasswordHash
    FROM Identity
    WHERE MemberName = memberName;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddAccountIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddAccountIdentity
(
    accountId INT,
    identityTypeId TINYINT,
    identityValue NVARCHAR(64)
)
BEGIN

    INSERT INTO AccountIdentity
    (
        AccountId, IdentityTypeId, IdentityValue, InsertedDatetime, InsertedBy
    )
    VALUES
    (
        accountId,
        identityTypeId,
        identityValue,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM AccountIdentity 
    WHERE AccountId = accountId
        AND IdentityTypeId = identityTypeId
        AND IdentityValue = identityValue;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetAccountByIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetAccountByIdentity
(
    identityTypeId TINYINT,
    identityValue NVARCHAR(64)
)
BEGIN

    SELECT * 
    FROM AccountIdentity ai
        JOIN Account a
        ON ai.AccountId = a.Id
    WHERE identityTypeId = IdentityTypeId
        AND identityValue = IdentityValue;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetNextAccountIdRange;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetNextAccountIdRange
(
    requester VARCHAR(64),
    rangeLength INT
)
BEGIN

    SELECT @maxDispatched := IFNULL(MAX(EndId), 0) FROM AccountIdAlloc FOR UPDATE;

    IF (rangeLength <= 0) THEN
        SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'Account ID range length must be positive!';
	END IF;
    
    INSERT INTO AccountIdAlloc
    (
        StartId
        ,EndId
        ,Requester
        ,InsertedDatetime
        ,InsertedBy
    )
    VALUES
    (
        @maxDispatched + 1
        ,@maxDispatched + rangeLength
        ,requester
        ,UTC_TIMESTAMP()
        ,CURRENT_USER()
    );
    
    SELECT @maxDispatched + 1 AS StartId,
           @maxDispatched + @rangeLength AS EndId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.Cleanup;
DELIMITER //
CREATE PROCEDURE QRBaDB.Cleanup
()
BEGIN

    -- For testing only, very dangerous!!!
    --
    DELETE FROM Account;
    DELETE FROM AccountIdAlloc;
    DELETE FROM AccountIdentity;
    DELETE FROM Event;
    DELETE FROM Identity;

END //
DELIMITER ;

