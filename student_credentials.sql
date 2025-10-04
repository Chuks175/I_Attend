#DROP DATABASE IF exists Students_Credentials;
#CREATE DATABASE students_credentials;
#USE students_credentials;


 #CREATE TABLE Details (
  #   Id INT NOT NULL,
 #    User_Names VARCHAR(50) NOT NULL,
 #    Department VARCHAR(50) NOT NULL,
 #    Course_code  VARCHAR(10),
 #    Matric_Number VARCHAR(50) NOT NULL UNIQUE,
 #    Email VARCHAR(100) NOT NULL UNIQUE,
 #    PRIMARY KEY(Id)
 #)
 
DROP DATABASE IF EXISTS Students_Credentials;
CREATE DATABASE students_credentials;
USE students_credentials;

CREATE TABLE Details (
    Id INT AUTO_INCREMENT NOT NULL,
    UserNames VARCHAR(50) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    Course_code VARCHAR(10) NOT NULL,
    Matric_Number VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL, -- Removed UNIQUE constraint
    ImageData LONGBLOB NULL,
    PRIMARY KEY(Id)
);
#CREATE TABLE Course_List(
#Course_code VARCHAR(50) PRIMARY KEY,
#start_time TIME	NOT NULL,
#Late_time_threshold TIME NOT NULL
#);

CREATE TABLE ECE5251 (
    id INT AUTO_INCREMENT PRIMARY KEY,
    Matric_Number VARCHAR(50) NOT NULL,
    date DATE NOT NULL,
    time TIME NOT NULL,
    status ENUM('Present', 'Late') NOT NULL,
    FOREIGN KEY (Matric_Number) REFERENCES Details(Matric_Number)
);