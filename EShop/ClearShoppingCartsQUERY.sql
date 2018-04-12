UPDATE Users 
SET ShoppingCartId = NULL

DELETE FROM ShoppingCartProduct
DELETE FROM ShoppingCart

DELETE FROM Product
WHERE Price= 0.00