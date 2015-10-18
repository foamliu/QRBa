DELIMITER GO
CREATE PROCEDURE AddAccount
(
    id BIGINT,
    name NVARCHAR(256),
    email VARCHAR(128),
    statusId TINYINT,
    customerTypeId TINYINT
)
BEGIN

	SELECT @now = UTC_TIMESTAMP();
    DECLARE vc_inserted_by VARCHAR(128) = SUSER_SNAME();

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
        @dt_now,
        @vc_inserted_by
    );

    SELECT * FROM Account WHERE Id = @id;

END
GO