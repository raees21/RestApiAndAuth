INSERT INTO UserType ([Type]) 
VALUES ('Administrator'), ('Buyer');

INSERT INTO OrderType ([Type]) 
VALUES ('Collection'), ('Delivery');

INSERT INTO OrderStatus ([Status]) 
VALUES ('Pending'), ('Confirmed'), ('In Progress'), ('Ready for collection'), ('Collected'), ('Shipped'), ('Delivered');

INSERT INTO FootSide (Side) 
VALUES ('Left'), ('Right');

INSERT INTO ShoeSize ([Size], Code) 
VALUES 
('13.5', 'UK'),
('13', 'UK'),
('12.5', 'UK'),
('12', 'UK'),
('11.5', 'UK'),
('11', 'UK'),
('10.5', 'UK'),
('10', 'UK'),
('9.5', 'UK'),
('9', 'UK'),
('8.5', 'UK'),
('8', 'UK'),
('7.5', 'UK'),
('7', 'UK'),
('6.5', 'UK'),
('6', 'UK'),
('13.5', 'US'),
('13', 'US'),
('12.5', 'US'),
('12', 'US'),
('11.5', 'US'),
('11', 'US'),
('10.5', 'US'),
('10', 'US'),
('9.5', 'US'),
('9', 'US'),
('8.5', 'US'),
('8', 'US'),
('7.5', 'US'),
('7', 'US'),
('6.5', 'US'),
('6', 'US');

INSERT INTO ProductType ([Type]) 
VALUES ('Flipflop'), ('Running'), ('Trail'), ('Sneaker');

INSERT INTO Gender ([Name]) 
VALUES ('Male'), ('Female'), ('Other');
