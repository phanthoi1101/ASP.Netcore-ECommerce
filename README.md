E-Commerce Web Application
Overview
This is an E-Commerce Web Application developed using ASP.NET Core MVC. The platform allows users to browse products, make purchases and manage orders. It also includes user authentication and role-based authorization for different user roles.

The application provides a secure and interactive experience where users can:

Browse Products: View a list of products, including details such as unit, price, description, and images.
Add to Cart & Checkout: Allows users to add, update, and remove products from the cart, and proceed to checkout.
User Authentication & Authorization: Users can login, and manage their accounts. Customers can place orders, while Admins can manage everything except orders, Managers can manage Categories, Customers, Products, Shippers, Suppliers, and Sales can manage Orders.
Admin Panel: The admin can manage all aspects of the platform, including adding, updating, and deleting products, managing user accounts, and overseeing platform operations, but they do not manage orders. Depending on the role, the manager can manage categories, customers, products, shippers, and suppliers, while the sales role is responsible for managing orders.
Features
Product Catalog: Users can view and search a list of available products.
Shopping Cart: Users can add, remove, and update items in the cart.
Checkout Process: Checkout process for order management.
User Authentication: Users can log in, and manage personal details.
Role-Based Authorization: Different access levels for customers and admins, managers, sales.
Technologies Used
Backend Framework: ASP.NET Core MVC
Authentication: ASP.NET Core Identity for user authentication and role management
Frontend: HTML, CSS, JavaScript
Database: SQL Server
Dependency Management: Entity Framework Core
Getting Started
To run this project locally, follow these steps:

Prerequisites
.NET 8 or higher
SQL Server
Visual Studio
Installation
Clone the repository:

git clone https://github.com/nhuytran122/ASP.NetCore-ECommerce.git
Set up the database:

Create a database named LiteCommerceDB and run the provided SQL script to set up the schema and sample data.

In appsettings.json, configure the connection string for your SQL Server database.

Build and run the project:

Use Visual Studio or run the project

Test Accounts
You can use the following test accounts to explore the features:

Customer Account:

Email: phanh@gmail.com
Password: 123456
Access: Customer role (can browse products, add to cart, checkout, and view order history).
Sale Account:

Email: lethimyna@mycompany.com
Password: 123456
Access: Sale role (can manage orders).
Manager Account:

Email: nguyentantu@mycompany.com
Password: 123456
Access: Manager role (can manage products, orders, users).
Admin Account:

Email: ducanh@gmail.com
Password: 123456
Access: Admin role (can manage everything: products, users, and more).
