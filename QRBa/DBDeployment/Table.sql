use QRBaDB;

DROP TABLE IF EXISTS QRBaDB.Identity;
CREATE TABLE Identity
(
    MemberName NVARCHAR(64) NOT NULL PRIMARY KEY,
    PasswordHash NVARCHAR(256) NOT NULL,
    InsertedDatetime DATETIME NOT NULL,
    InsertedBy NVARCHAR(64) NOT NULL,
    UpdatedDatetime DATETIME NULL,
    UpdatedBy NVARCHAR(64) NULL
);

DROP TABLE IF EXISTS QRBaDB.Account;
CREATE TABLE Account
(
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL,
    Email VARCHAR(128) NOT NULL,
    StatusId TINYINT NOT NULL,
    InsertedDatetime DATETIME NOT NULL,
    InsertedBy NVARCHAR(64) NOT NULL,
    UpdatedDatetime DATETIME NULL,
    UpdatedBy NVARCHAR(64) NULL
);

DROP TABLE IF EXISTS QRBaDB.AccountIdAlloc;
CREATE TABLE AccountIdAlloc
(
    StartId bigint NOT NULL,
    EndId   bigint NOT NULL,
    Requester varchar(64) NOT NULL,    
    InsertedDatetime datetime NOT NULL,
    InsertedBy varchar(64) NOT NULL,
    
    PRIMARY KEY 
    (
        EndId ASC
    )
);

DROP TABLE IF EXISTS QRBaDB.AccountIdentity;
CREATE TABLE AccountIdentity
(
    AccountId BIGINT NOT NULL,
    IdentityTypeId TINYINT NOT NULL,
    IdentityValue NVARCHAR(64) NOT NULL,
    InsertedDatetime DATETIME NOT NULL,
    InsertedBy NVARCHAR(64) NOT NULL,
    PRIMARY KEY (AccountId, IdentityTypeId, IdentityValue)
);

DROP TABLE IF EXISTS QRBaDB.Code;
CREATE TABLE Code
(
    AccountId INT NOT NULL,
    CodeId INT NOT NULL,    
    CodeTypeId TINYINT NOT NULL,
    CodeRectangle VARCHAR(32) NULL,
    BackgroundContentType VARCHAR(32) NULL,
    Payload NVARCHAR(2048) NULL,
    InsertedDatetime DATETIME NOT NULL,
    InsertedBy NVARCHAR(64) NOT NULL,
    UpdatedDatetime DATETIME NULL,
    UpdatedBy NVARCHAR(64) NULL,
    PRIMARY KEY (AccountId, CodeId)
);

DROP TABLE IF EXISTS QRBaDB.Event;
CREATE TABLE Event
(
    AccountId INT NOT NULL,
    CodeId INT NOT NULL,  
    EventTypeId TINYINT NOT NULL,
    Payload NVARCHAR(2048) NULL,
    InsertedDatetime DATETIME NOT NULL,
    InsertedBy NVARCHAR(64) NOT NULL,
    PRIMARY KEY(AccountId, CodeId, EventTypeId, InsertedDatetime)
);
