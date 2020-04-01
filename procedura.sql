--PROCEDURA SKŁADOWANA
CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRAN
        DECLARE @IdStudy INT = (SELECT IdStudy FROM Studies WHERE Name = @Studies);
        IF @IdStudies IS NULL
            BEGIN
                RAISEERROR ("IdStudies nie moze byc nullem", 1000, 1);
            END
        DECLARE @idEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1) AND IdStudy = @idStudy);
        IF @idEnrollment IS NULL
            BEGIN
                INSERT INTO Enrollment (Semester, IdStudy, StartDate) VALUES (@Semester, @idStudy, GETDATE());
                SET @idEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1) AND IdStudy = @idStudy);
            END
        UPDATE Students SET IdEnrollment = @idEnrollment WHERE IdEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE Semester = @Semester AND IdStudy = @idStudy);
    END
    COMMIT;
    SELECT * FROM Enrollment WHERE IdEnrollment = @idEnrollment;
END;