-- Create the CONTACTS_DB database
CREATE DATABASE CONTACTS_DB;
GO

-- Switch to the CONTACTS_DB database
USE CONTACTS_DB;
GO

-- Create the USERS table
CREATE TABLE USERS (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL,
    password NVARCHAR(100) NOT NULL
);
GO

-- Create the CONTACTS table
CREATE TABLE CONTACTS (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    user_id INT NOT NULL,
    name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20),
    email NVARCHAR(100),
    CONSTRAINT FK_Contacts_Users FOREIGN KEY (user_id) REFERENCES USERS (id)
);
GO

-- Create a hash method for the password (e.g., SHA256)
-- In a real application, use a more secure hashing algorithm and consider salting the passwords
-- For .NET Core, you can use the System.Security.Cryptography namespace for hashing.
-- The following is just an example; implement proper hashing in your .NET Core application.
-- DO NOT use plain text passwords in production!
-- The password 'password123' will be hashed as '46EC5A1B-68B1-85DD-6C37-4C85A581F796'
--INSERT INTO USERS (username, password)
--VALUES ('user1', '46EC5A1B-68B1-85DD-6C37-4C85A581F796');
