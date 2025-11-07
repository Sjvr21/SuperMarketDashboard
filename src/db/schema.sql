-- ==============================================
-- DATABASE: company_portal
-- AUTHOR: Sergio Vélez Rosario
-- PROJECT: SuperMarketDashboard
-- DATE: 2025
-- ==============================================

-- 1️⃣ Create and select database
CREATE DATABASE IF NOT EXISTS company_portal;
USE company_portal;

-- 2️⃣ Users
CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(64) NOT NULL UNIQUE,
  passwordhash VARCHAR(256) NOT NULL,
  role ENUM('admin','user') DEFAULT 'user'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3️⃣ Inventory
CREATE TABLE IF NOT EXISTS inventory (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(128) NOT NULL,
  category VARCHAR(64),
  quantity INT DEFAULT 0,
  price DECIMAL(10,2) DEFAULT 0.00,
  last_updated TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4️⃣ Warehouse Items (stock available for orders)
CREATE TABLE IF NOT EXISTS warehouse_items (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(128) NOT NULL,
  category VARCHAR(64),
  price DECIMAL(10,2) NOT NULL,
  stock INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5️⃣ Inventory Logs (track quantity updates)
CREATE TABLE IF NOT EXISTS inventory_logs (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(64) NOT NULL,
  item_id INT NOT NULL,
  old_quantity INT NOT NULL,
  new_quantity INT NOT NULL,
  timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_inventory_logs_item
    FOREIGN KEY (item_id) REFERENCES inventory(id)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6️⃣ Orders (order header)
CREATE TABLE IF NOT EXISTS orders (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(64) NOT NULL,
  total_price DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  shipping DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  arrival_date DATETIME NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  status VARCHAR(32) NOT NULL DEFAULT 'Pending',
  INDEX (username),
  CONSTRAINT fk_orders_user_by_name
    FOREIGN KEY (username) REFERENCES users(username)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7️⃣ Order Items (details per order)
CREATE TABLE IF NOT EXISTS order_items (
  id INT AUTO_INCREMENT PRIMARY KEY,
  order_id INT NOT NULL,
  item_id INT NOT NULL,
  quantity INT NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  INDEX (order_id),
  INDEX (item_id),
  CONSTRAINT fk_order_items_order
    FOREIGN KEY (order_id) REFERENCES orders(id)
    ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT fk_order_items_item
    FOREIGN KEY (item_id) REFERENCES inventory(id)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 8️⃣ Warehouse Orders (records of warehouse activity)
CREATE TABLE IF NOT EXISTS warehouse_orders (
  id INT AUTO_INCREMENT PRIMARY KEY,
  warehouse_item_id INT NOT NULL,
  quantity INT NOT NULL,
  total DECIMAL(10,2) NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_warehouse_orders_item
    FOREIGN KEY (warehouse_item_id) REFERENCES warehouse_items(id)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9️⃣ Insert example data
INSERT INTO users (username, passwordhash, role) VALUES
('admin', 'hash_here', 'admin'),
('employee1', 'hash_here', 'user');

INSERT INTO inventory (name, category, quantity, price) VALUES
('Goya Rice 5lb', 'Grains', 100, 6.50),
('Mazola Oil 1L', 'Oils', 80, 5.75),
('Indulac Milk', 'Dairy', 60, 4.25),
('Corn Flakes Cereal', 'Breakfast', 50, 3.80);

INSERT INTO warehouse_items (name, category, price, stock) VALUES
('Goya Rice 25lb Bag', 'Grains', 24.99, 45),
('Mazola Oil 1 Gallon', 'Oils', 12.50, 60),
('Indulac Milk 1 Gallon', 'Dairy', 6.25, 40),
('Corn Flakes Family Pack', 'Breakfast', 8.50, 55);

-- Example order creation
INSERT INTO orders (username, total_price, shipping, arrival_date, status)
VALUES ('employee1', 0.00, 3.99, NOW() + INTERVAL 3 DAY, 'Pending');

SET @order_id = LAST_INSERT_ID();

INSERT INTO order_items (order_id, item_id, quantity, price)
VALUES
(@order_id, 1, 2, 6.50),
(@order_id, 2, 1, 5.75);

UPDATE orders
SET total_price = (
  SELECT SUM(quantity * price) + shipping
  FROM order_items WHERE order_id = @order_id
)
WHERE id = @order_id;
