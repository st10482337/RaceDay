-- Create Database
IF DB_ID('RaceDayDB') IS NULL
BEGIN
    CREATE DATABASE RaceDayDB;
END;

USE RaceDayDB;


/* =========================================================
   1. USER TABLE
   ========================================================= */

CREATE TABLE [USER]
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL,

    CONSTRAINT CK_User_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);


/* =========================================================
   2. USER_PROFILE TABLE
   ========================================================= */

CREATE TABLE USER_PROFILE
(
    ProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20) NULL,
    Address VARCHAR(255) NULL,
    ProfilePictureURL VARCHAR(255) NULL,

    CONSTRAINT FK_UserProfile_User
        FOREIGN KEY (UserID)
        REFERENCES [USER](UserID)
);


/* =========================================================
   3. EVENT TABLE
   ========================================================= */

CREATE TABLE EVENT
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NOT NULL,
    EventDate DATE NOT NULL,
    Location VARCHAR(150) NOT NULL,
    Distance VARCHAR(50) NOT NULL,
    EventType VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES [USER](UserID),

    CONSTRAINT CK_Event_Type
        CHECK (EventType IN ('Run', 'Walk', 'Cycle'))
);


/* =========================================================
   4. CATEGORY TABLE
   ========================================================= */

CREATE TABLE CATEGORY
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    CategoryType VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID)
        REFERENCES EVENT(EventID),

    CONSTRAINT CK_Category_Type
        CHECK (CategoryType IN ('Age', 'Distance'))
);


/* =========================================================
   5. ENROLMENT TABLE
   ========================================================= */

CREATE TABLE ENROLMENT
(
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Enrolled',

    CONSTRAINT FK_Enrolment_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES [USER](UserID),

    CONSTRAINT FK_Enrolment_Event
        FOREIGN KEY (EventID)
        REFERENCES EVENT(EventID),

    CONSTRAINT FK_Enrolment_Category
        FOREIGN KEY (CategoryID)
        REFERENCES CATEGORY(CategoryID),

    CONSTRAINT CK_Enrolment_Status
        CHECK (Status IN ('Enrolled', 'Cancelled', 'Completed')),

    CONSTRAINT UQ_Participant_Event
        UNIQUE (ParticipantID, EventID)
);


/* =========================================================
   6. RESULT TABLE
   ========================================================= */

CREATE TABLE RESULT
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    FinishingPosition INT NOT NULL,

    CONSTRAINT FK_Result_Enrolment
        FOREIGN KEY (EnrolmentID)
        REFERENCES ENROLMENT(EnrolmentID),

    CONSTRAINT CK_FinishingPosition
        CHECK (FinishingPosition > 0)
);


/* =========================================================
   INSERT SAMPLE USERS
   2 ORGANISERS + 2 PARTICIPANTS
   ========================================================= */

INSERT INTO [USER]
    (FirstName, LastName, Email, PasswordHash, Role)
VALUES
    ('Thabo', 'Mokoena', 'thabo@raceday.co.za',
     'HASHED_PASSWORD_001', 'Organiser'),

    ('Lerato', 'Dlamini', 'lerato@raceday.co.za',
     'HASHED_PASSWORD_002', 'Organiser'),

    ('Sipho', 'Nkosi', 'sipho@example.com',
     'HASHED_PASSWORD_003', 'Participant'),

    ('Naledi', 'Molefe', 'naledi@example.com',
     'HASHED_PASSWORD_004', 'Participant');


/* =========================================================
   INSERT USER PROFILES
   ========================================================= */

INSERT INTO USER_PROFILE
    (UserID, PhoneNumber, Address, ProfilePictureURL)
VALUES
    (1, '0711500819', 'Johannesburg, Gauteng', NULL),

    (2, '0722913257', 'Pretoria, Gauteng', NULL),

    (3, '0790715165', 'Soweto, Gauteng', NULL),

    (4, '0673635656', 'Midrand, Gauteng', NULL);


/* =========================================================
   INSERT 3 EVENTS
   ========================================================= */

INSERT INTO EVENT
    (OrganiserID, EventName, Description, EventDate,
     Location, Distance, EventType)
VALUES
    (1,
     'Johannesburg City Run',
     'Annual road running event through Johannesburg.',
     '2026-10-10',
     'Johannesburg',
     '10km',
     'Run'),

    (1,
     'Pretoria Community Walk',
     'Community walking event supporting healthy living.',
     '2026-11-07',
     'Pretoria',
     '5km',
     'Walk'),

    (2,
     'Gauteng Cycle Challenge',
     'Competitive road cycling event for cycling enthusiasts.',
     '2026-12-05',
     'Midrand',
     '21km',
     'Cycle');


/* =========================================================
   INSERT CATEGORIES FOR EACH EVENT
   ========================================================= */

INSERT INTO CATEGORY
    (EventID, CategoryName, CategoryType)
VALUES
    (1, 'Under 20', 'Age'),
    (1, 'Senior', 'Age'),

    (2, '5km Open', 'Distance'),
    (2, '5km Senior', 'Distance'),

    (3, '21km Open', 'Distance'),
    (3, '21km Senior', 'Distance');


/* =========================================================
   INSERT SAMPLE ENROLMENTS
   ========================================================= */

INSERT INTO ENROLMENT
    (ParticipantID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
    (3, 1, 1, '2026-09-01 10:00:00', 'Enrolled'),

    (4, 1, 2, '2026-09-01 11:00:00', 'Enrolled'),

    (3, 2, 3, '2026-09-02 09:30:00', 'Enrolled'),

    (4, 3, 5, '2026-09-03 14:00:00', 'Enrolled');


/* =========================================================
   INSERT SAMPLE RESULTS
   ========================================================= */

INSERT INTO RESULT
    (EnrolmentID, FinishTime, FinishingPosition)
VALUES
    (1, '01:05:32', 47),

    (2, '01:12:45', 63);


/* =========================================================
   DISPLAY TABLE CONTENTS
   ========================================================= */

SELECT * FROM [USER];

SELECT * FROM USER_PROFILE;

SELECT * FROM EVENT;

SELECT * FROM CATEGORY;

SELECT * FROM ENROLMENT; 

SELECT * FROM RESULT;
