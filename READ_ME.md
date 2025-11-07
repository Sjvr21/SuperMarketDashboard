# 🛒 SuperMarketDashboard

**SuperMarketDashboard** is a **Full-Stack** application for comprehensive supermarket management.  
It allows managing **inventory, warehouses, orders, users, and reports** using a modular architecture with a **.NET backend** and **MySQL** database.

---

## 📦 Main Features
- Complete **inventory management (CRUD)**
- **Warehouse** management
- **Order creation and tracking**
- **User roles** (Administrator / Employee)
- **Modern interface** with pagination, search, and responsive design
- **Relational MySQL database**
- Layered architecture: **Controller → Service → Repository**

---

## 🧰 Technologies Used

| Type | Technology |
|------|-------------|
| Backend Language | C# (.NET 8) |
| Database | MySQL 9.0.1 |
| Frontend | HTML, CSS, JavaScript |
| IDE | Visual Studio 2022 / VS Code |
| Version Control | GitHub |
| Operating System | macOS / Windows |

---

## ⚙️ Prerequisites

Before running the project, make sure you have installed:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)
- [MySQL 9.0+](https://dev.mysql.com/downloads/)
- [Node.js (optional, for dynamic frontend)](https://nodejs.org/)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
- Git (to clone the repository)

---

## 🗂️ Project Structure

```
SuperMarketDashboard/
│
├── backend/
│   ├── server.js
│   ├── db.js
│   ├── routes/
│   │   ├── auth.js
│   │   ├── dashboard.js
│   │   ├── warehouse.js
│   │   └── orders.js
│   ├── models/
│   └── package.json
│
├── frontend/
│   ├── index.html
│   ├── js/
│   │   ├── dashboard.js
│   │   ├── warehouse.js
│   │   ├── orders.js
│   │   └── pagination.js
│   ├── css/
│   │   └── dashboard.css
│   └── views/
│
└── README.md
```

---

## 🧩 Database Configuration

1. Open MySQL Workbench or your MySQL CLI.
2. Create the database: Using the Schema.sql on db folder.
3. Change the app.cs to ur db user, password.


## CREATE A USER FOR FIRST TIME LOG IN AS ADMIN
```bash
# Install bcrypt
npm install bcrypt

# Open Node.js
node

# Run this command
const bcrypt = require('bcrypt');
const password = 'yourpassword123';  // <-- change this
bcrypt.hash(password, 10).then(hash => console.log(hash));

# Copy the hash that appears
# Insert it manually in DB

INSERT INTO users (username, passwordhash, role)
VALUES ('admin', '$2b$10$REPLACE_WITH_YOUR_HASH', 'admin');
```



---

## 🚀 Running the Project

### 1. Backend (.NET / Node)
```bash
cd supermarketdashbaord
npm install     # Installs dependencies if Node is used
node dotnet run  # Starts the server at http://localhost:8080
```

### 2. Frontend (HTML + JS)
- Open `/login` directly in your browser  
  or use the **Live Server** extension in VS Code.

---


---

## 🏗️ Backend Architecture

```plaintext
Controller → Service → Repository → MySQL
```

- **Controller:** Handles HTTP requests  
- **Service:** Processes business logic  
- **Repository:** Executes SQL queries in MySQL  

This modular pattern enables maintainability, scalability, and code reuse.

---

## 👨‍💻 Developer

**Sergio Vélez Rosario**  
Universidad Interamericana de Puerto Rico - Bayamón Campus  
Course: COMP2053 – Full-Stack Web Development  
Year: 2025

---

## 🧾 License

Academic project for educational purposes.  
© 2025 Sergio Vélez Rosario. All rights reserved.
