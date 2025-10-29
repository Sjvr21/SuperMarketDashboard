using System;
using System.Net;
using System.Collections;

namespace SuperMarketDashboard;

public class App
{
    private HttpListener server;
    private HttpRouter router;

    public App()
    {
        string host = "http://127.0.0.1:8080/";
        server = new HttpListener();
        server.Prefixes.Add(host);
        router = new HttpRouter();

        Console.WriteLine("Server listening on " + host);

        // =======================
        // 🌐 Global Middleware
        // =======================
        router.Use(async (req, res, options) => await HttpUtils.ServeStaticFile(req, res, options));
        router.Use(async (req, res, options) => await HttpUtils.ReadRequestFormData(req, res, options));




        // =======================
        // 👤 Authentication Setup
        // =======================
        var userRepo = new MySqlUserRepository("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var userService = new UserService(userRepo);
        var authController = new AuthController(userService);
        var userController = new UserController(userService);

        // =======================
        // 🧍 User Management Routes
        // =======================
        router.AddGet("/users", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt); // Any logged user
            if (res.OutputStream.CanWrite)
                await userController.ViewAll(req, res, opt);
        });

        router.AddGet("/users/register", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt); // Admin only
            if (res.OutputStream.CanWrite)
                await userController.RegisterGet(req, res, opt);
        });

        router.AddPost("/users/register", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt); // Admin only
            if (res.OutputStream.CanWrite)
                await userController.RegisterPost(req, res, opt);
        });

        // =======================
        // 🔐 Auth Routes
        // =======================
        router.AddGet("/login", authController.LoginGet);
        router.AddPost("/login", authController.LoginPost);
        router.AddGet("/logout", authController.Logout);

        // =======================
        // 🏪 Inventory Setup
        // =======================
        var inventoryRepo = new MySqlInventoryRepository("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var inventoryService = new InventoryService(inventoryRepo);
        var inventoryController = new InventoryController(inventoryService);

        router.AddGet("/", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await inventoryController.ViewAll(req, res, opt);
        });

        router.AddPost("/inventory/edit", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await inventoryController.EditQuantity(req, res, opt);
        });

        // =======================
        // 🧾 Logs (Admin Only)
        // =======================
        var logRepo = new MySqlInventoryLogRepository("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var logService = new InventoryLogService(logRepo);
        var logsController = new LogsController(logService);

        router.AddGet("/logs", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt);
            if (res.OutputStream.CanWrite)
                await logsController.ViewAll(req, res, opt);
        });

        // =======================
        // 📦 Warehouse Setup
        // =======================
        var warehouseRepo = new MySqlWarehouseRepository("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var warehouseService = new WarehouseService(warehouseRepo);
        var warehouseController = new WarehouseController(warehouseService);

        router.AddGet("/warehouse", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await warehouseController.ViewAll(req, res, opt);
        });


        // =======================
        // 📊 Inventory Analytics (Admin)
        // =======================
        var analyticsService = new AnalyticsService("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var analyticsController = new AnalyticsController(analyticsService);

        router.AddGet("/analytics", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt);
            if (!res.OutputStream.CanWrite) return;
            await analyticsController.ViewDashboard(req, res, opt);
        });




        // =======================
        // 🧺 Orders Setup
        // =======================
        var orderRepo = new MySqlOrderRepository("Server=localhost;Database=supermarketdb;Uid=root;Pwd=Sergio23;");
        var orderService = new OrderService(orderRepo);
        var orderController = new OrderController(orderService, warehouseRepo);

        router.AddPost("/warehouse/add", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.AddToCart(req, res, opt);
        });

        router.AddPost("/orders/confirm", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.Confirm(req, res, opt);
        });

        router.AddGet("/orders", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.ViewAll(req, res, opt);
        });

        router.AddGet("/orders/view", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.ViewOrder(req, res, opt);
        });

        router.AddGet("/orders/mark", async (req, res, opt) =>
        {
            await Middleware.CheckAdmin(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.MarkDelivered(req, res, opt);
        });

        // =======================
        // 🛒 Cart Routes
        // =======================
        router.AddGet("/cart", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.ViewCart(req, res, opt);
        });

        router.AddPost("/cart/clear", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.ClearCart(req, res, opt);
        });

        router.AddPost("/cart/remove", async (req, res, opt) =>
        {
            await Middleware.CheckAuth(req, res, opt);
            if (res.OutputStream.CanWrite)
                await orderController.RemoveFromCart(req, res, opt);
        });
    }

    public void Run()
    {
        server.Start();
        while (true)
        {
            var context = server.GetContext();
            _ = router.HandleRequest(context, new Hashtable());
        }
    }
}
