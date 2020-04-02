--PROCEDURA SKŁADOWANA
CREATE PROCEDURE PromoteStudents @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRAN
        DECLARE @IdStudy INT = (SELECT IdStudy FROM Studies WHERE Name = @Studies);
        IF @IdStudy IS NULL
            BEGIN
                RAISERROR('IdStudies nie moze byc nullem', 10, 1);
            END
        DECLARE @idEnrollment INT = (SELECT IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1) AND IdStudy = @idStudy);
        IF @idEnrollment IS NULL
            BEGIN
                INSERT INTO Enrollment (Semester, IdStudy, StartDate) VALUES (@Semester, @idStudy, GETDATE());
                SET @idEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE Semester = (@Semester + 1) AND IdStudy = @idStudy);
            END
        UPDATE Student SET IdEnrollment = @idEnrollment WHERE IdEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE Semester = @Semester AND IdStudy = @idStudy);
END;
SELECT * FROM Enrollment WHERE IdEnrollment = @idEnrollment;
