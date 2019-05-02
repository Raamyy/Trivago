create or replace PROCEDURE Add_Review
(d in VARCHAR2, r in NUMBER, bn in NUMBER)
as
begin
INSERT INTO Review
(booking_number, description, rating)
VALUES (bn, d, r);
end;

create or replace PROCEDURE Delete_User
(uName in VARCHAR2, creditNumber in VARCHAR2)
as
BEGIN
DELETE FROM Website_User
WHERE user_name = uName;

DELETE FROM Credit_Card
WHERE credit_card_number = creditNumber;
END;

create or replace PROCEDURE GET_Review 
(booking_number_in in number,Description_out out VARCHAR2,
  Rating_out out NUMBER)
AS
BEGIN
  select description, rating
  into Description_out, Rating_out
  from review
  where booking_number = booking_number_in;
END;

create or replace PROCEDURE Get_Room_Reviews
(rn NUMBER, hn NUMBER, reviews out Sys_RefCursor)
as
begin
OPEN reviews for
SELECT *
FROM Review
WHERE booking_number = (
  SELECT booking_number
  FROM Define_Booking
  WHERE hotel_license_number = hn
  AND room_number = rn
);
end;

create or replace PROCEDURE Update_Hotel
(lNumber in NUMBER,nme in VARCHAR2, cty in VARCHAR2, ctry in VARCHAR2)
as
BEGIN
UPDATE Hotel
SET hotel_name = nme, city = cty, country = ctry 
WHERE license_number = lNumber;
END;

