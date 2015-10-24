USE QRBaDB;
DROP PROCEDURE IF EXISTS QRBaDB.AddAccount;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddAccount
(
    param_id INT,
    param_name NVARCHAR(256),
    param_email VARCHAR(128),
    param_statusId TINYINT
)
BEGIN

    INSERT INTO Account
    (
        Id,Name,Email,StatusId,InsertedDatetime,InsertedBy
    )
    VALUES
    (
        param_id,
        param_name,
        param_email,
        param_statusId,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM Account WHERE Id = id;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetAccount;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetAccount
(
    param_id int
)
BEGIN

    SELECT * FROM Account
    WHERE Id = param_id;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.UpdateAccount;
DELIMITER //
CREATE PROCEDURE QRBaDB.UpdateAccount
(
    param_id BIGINT,
    param_name NVARCHAR(256),
    param_email VARCHAR(128),
    param_statusId TINYINT
)
BEGIN

    UPDATE Account
    SET 
        Name = IFNULL(param_name, Name),
        Email = IFNULL(param_email, Email),
        UpdatedDatetime = UTC_TIMESTAMP(),
        UpdatedBy = CURRENT_USER()
    WHERE Id = param_id;

    SELECT * FROM Account
    WHERE Id = param_id;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddIdentity
(
    param_memberName NVARCHAR(64),
    param_passwordHash NVARCHAR(256)
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
        param_memberName,
        param_passwordHash,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT
        MemberName,
        PasswordHash
    FROM Identity
    WHERE MemberName = param_memberName;
    
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetIdentity
(
    param_memberName NVARCHAR(64)
)
BEGIN

    SELECT
        MemberName,
        PasswordHash
    FROM Identity
    WHERE MemberName = param_memberName;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddAccountIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddAccountIdentity
(
    param_accountId INT,
    param_identityTypeId TINYINT,
    param_identityValue NVARCHAR(64)
)
BEGIN

    INSERT INTO AccountIdentity
    (
        AccountId, IdentityTypeId, IdentityValue, InsertedDatetime, InsertedBy
    )
    VALUES
    (
        param_accountId,
        param_identityTypeId,
        param_identityValue,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM AccountIdentity 
    WHERE AccountId = param_accountId
        AND IdentityTypeId = param_identityTypeId
        AND IdentityValue = param_identityValue;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetAccountByIdentity;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetAccountByIdentity
(
    param_identityTypeId TINYINT,
    param_identityValue NVARCHAR(64)
)
BEGIN

    SELECT * 
    FROM AccountIdentity ai
        JOIN Account a
        ON ai.AccountId = a.Id
    WHERE param_identityTypeId = IdentityTypeId
        AND param_identityValue = IdentityValue;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetNextAccountIdRange;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetNextAccountIdRange
(
    param_requester VARCHAR(64),
    param_rangeLength INT
)
BEGIN

    SELECT @maxDispatched := IFNULL(MAX(EndId), 0) FROM AccountIdAlloc FOR UPDATE;

    IF (param_rangeLength <= 0) THEN
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
        ,@maxDispatched + param_rangeLength
        ,param_requester
        ,UTC_TIMESTAMP()
        ,CURRENT_USER()
    );
    
    SELECT @maxDispatched + 1 AS StartId,
           @maxDispatched + param_rangeLength AS EndId;

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

DROP PROCEDURE IF EXISTS QRBaDB.AddCode;
DELIMITER //
CREATE PROCEDURE AddCode
(
    param_accountId INT,
    param_codeTypeId TINYINT,
    param_codeRectangle VARCHAR(32),
    param_backgroundContentType VARCHAR(32),
    param_payload NVARCHAR(2048)
)
BEGIN

    SELECT @nextCodeId := IFNULL(MAX(CodeId), 0) + 1
    FROM Code
    WHERE AccountId = accountId;

    INSERT INTO Code
    (
        AccountId, CodeId, CodeTypeId, CodeRectangle, 
        BackgroundContentType, Payload, InsertedDatetime, InsertedBy
    )
    VALUES
    (
        param_accountId,
        @nextCodeId,
        param_codeTypeId,
        param_codeRectangle,
        param_backgroundContentType,
        param_payload,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM Code 
    WHERE AccountId = accountId
		AND CodeId = @nextCodeId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetCode;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetCode
(
	param_accountId INT,
    param_codeId INT
)
BEGIN

    SELECT * FROM Code
    WHERE AccountId = param_accountId
		AND CodeId = param_codeId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.UpdateCode;
DELIMITER //
CREATE PROCEDURE QRBaDB.UpdateCode
(
    param_accountId INT,
    param_codeId INT,
    param_codeTypeId TINYINT,
    param_codeRectangle VARCHAR(32),    
    param_backgroundContentType VARCHAR(32),
    param_payload NVARCHAR(2048)
)
BEGIN

	IF (param_payload = '') THEN 
		SET param_payload = NULL;
	END IF;

    UPDATE QRBaDB.Code
    SET 
        CodeTypeId = IFNULL(param_codeTypeId, CodeTypeId),
        CodeRectangle = IFNULL(param_codeRectangle, CodeRectangle),
        BackgroundContentType = IFNULL(param_backgroundContentType, BackgroundContentType),
        Payload = IFNULL(param_payload, Payload),
        UpdatedDatetime = UTC_TIMESTAMP(),
        UpdatedBy = CURRENT_USER()
    WHERE AccountId = param_accountId
		AND CodeId = param_codeId;

    SELECT * FROM QRBaDB.Code
    WHERE AccountId = param_accountId
		AND CodeId = param_codeId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetCodes;
DELIMITER //
CREATE PROCEDURE QRBaDB.GetCodes
(
	param_accountId INT
)
BEGIN

    SELECT * FROM Code
    WHERE AccountId = param_accountId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddComment;
DELIMITER //
CREATE PROCEDURE AddComment
(
    param_accountId INT,
    param_content NVARCHAR(1024)
)
BEGIN

    SELECT @nextCommentId := IFNULL(MAX(CommentId), 0) + 1
    FROM Comment
    WHERE AccountId = accountId;

    INSERT INTO Comment
    (
        AccountId, CommentId, Content, InsertedDatetime, InsertedBy
    )
    VALUES
    (
        param_accountId,
        @nextCommentId,
        param_content,
        UTC_TIMESTAMP(),
        CURRENT_USER()
    );

    SELECT * FROM Comment 
    WHERE AccountId = accountId
		AND CommentId = @nextCommentId;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.GetComments;
DELIMITER //
CREATE PROCEDURE GetComments
(
)
BEGIN

    SELECT * FROM Comment
    ORDER BY InsertedDatetime DESC
	LIMIT 0, 20;

END //
DELIMITER ;

DROP PROCEDURE IF EXISTS QRBaDB.AddEvent;
DELIMITER //
CREATE PROCEDURE QRBaDB.AddEvent
(
    param_accountId INT,
    param_codeId INT,
    param_eventTypeId TINYINT,
    payload NVARCHAR(2048)
)
BEGIN

    SET @dtUtcNow := UTC_TIMESTAMP();

    INSERT INTO QRBaDB.Event
    (
        AccountId, CodeId, EventTypeId, Payload, InsertedDatetime, InsertedBy
    )
    VALUES
    (
        param_accountId,
        param_codeId,
        param_eventTypeId,
        payload,
        @dtUtcNow,
        CURRENT_USER()
    );
    
    SELECT * FROM QRBaDB.Event
    WHERE AccountId = param_accountId
		AND CodeId = param_codeId
        AND EventTypeId = param_eventTypeId
        AND InsertedDatetime = @dtUtcNow;

END //
DELIMITER ;

