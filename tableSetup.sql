DROP TABLE IF EXISTS Invoices;
DROP TABLE IF EXISTS Accounts;

CREATE TABLE Accounts (
AccountId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
AccountNumber VARCHAR(50) NOT NULL UNIQUE,
CustomerName VARCHAR(100) NOT NULL
);


CREATE TABLE Invoices (
InvoiceId INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
AccountId INT REFERENCES Accounts(AccountId),
InvoiceDate DATE NOT NULL,
AmountDue DECIMAL(10,2) NOT NULL,
Status VARCHAR(20)
);



INSERT INTO Accounts (AccountNumber, CustomerName)
VALUES
('ACC-00001', 'testCustomer1'),
('ACC-12345', 'testCustomer2'),
('ACC-654321', 'testCustomer3'),
('A-5', 'testCustomer4'),
('02', 'testCustomer5');

INSERT INTO Invoices (AccountId, InvoiceDate, AmountDue, Status)
VALUES
(1, '2025-01-01', 10.50, 'Paid'),
(1, '2025-02-06', 99.99, 'Unpaid'),
(2, '2025-03-21', 8.55, 'Paid'),
(2, '2025-04-08', 9.85, 'Paid'),
(3, '2025-05-09', 15.15, 'Unpaid'),
(3, '2025-06-11', 19.99, 'Paid'),
(3, '2025-07-05', 10.00, 'Unpaid'),
(3, '2025-06-04', 20.00, 'Unpaid'),
(4, '2025-05-01', 98.50, 'Unpaid'),
(4, '2025-08-09', 18.85, 'Paid');
