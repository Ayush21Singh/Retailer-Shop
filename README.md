# Ashish General Store Backend

ASP.NET Core backend powering an admin dashboard for Ashish General Store. Features include product/category/user CRUD, JWT authentication,
and purchase/sell analytics with customizable time periods. Built with Entity Framework Core and MySQL.

## Features
- **Admin Authentication**: Secure login/logout with JWT and refresh tokens.
- **Inventory Management**: CRUD operations for products, categories, and users.
- **Analytics Dashboard**: Bar chart widget for purchase vs. sell stats per item (day/week/month/custom).
- **Database**: MySQL with EF Core for ORM.

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [Git](https://git-scm.com/)
- [Retailer Shop UI](https://github.com/Ayush21Singh/Ashish-General-Store-UI)

## Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/AshishGeneralStore.git
   cd AshishGeneralStore
