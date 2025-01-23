CREATE DATABASE IF NOT EXISTS EstateManagement;
USE EstateManagement;

CREATE TABLE User (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Role ENUM('Customer', 'EstateOwner', 'Admin') NOT NULL,
    ResetPasswordToken VARCHAR(255) NULL
);

CREATE TABLE Estate (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Location VARCHAR(255) NOT NULL,
    Title VARCHAR(255),
    PricePerNight DOUBLE NOT NULL,
    EstateOwnerId INT,
    FOREIGN KEY (EstateOwnerId) REFERENCES User(Id)
);

CREATE TABLE Room (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EstateId INT,
    RoomType VARCHAR(50),
    BedCount INT,
    MaxGuests INT,
    FOREIGN KEY (EstateId) REFERENCES Estate(Id)
);

CREATE TABLE Reservation (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CustomerId INT,
    EstateId INT,
    CheckInDate DATE,
    CheckOutDate DATE,
    TotalPrice DOUBLE,
    Status BOOLEAN,
    FOREIGN KEY (CustomerId) REFERENCES User(Id),
    FOREIGN KEY (EstateId) REFERENCES Estate(Id)
);

CREATE TABLE Review (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EstateId INT,
    Rating INT NOT NULL,
    Comment TEXT,
    Date DATE,
    AuthorId INT,
    FOREIGN KEY (EstateId) REFERENCES Estate(Id),
    FOREIGN KEY (AuthorId) REFERENCES User(Id)
);

CREATE TABLE Amenities (
    EstateId INT,
    AmenityName VARCHAR(255),
    PRIMARY KEY (EstateId, AmenityName),
    FOREIGN KEY (EstateId) REFERENCES Estate(Id)
);

CREATE TABLE Payment (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ReservationId INT,
    Amount DOUBLE NOT NULL,
    Method VARCHAR(50),
    Date DATE,
    Status INT,
    FOREIGN KEY (ReservationId) REFERENCES Reservation(Id)
);

CREATE TABLE EstatePhotos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EstateId INT,
    PhotoUrl VARCHAR(2083) NOT NULL
);

CREATE TABLE RoomPhotos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RoomId INT,
    PhotoUrl VARCHAR(2083) NOT NULL
);
