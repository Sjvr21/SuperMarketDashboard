using System.Net;
using System.Collections;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class AnalyticsController
{
    private readonly AnalyticsService service;

    public AnalyticsController(AnalyticsService service)
    {
        this.service = service;
    }

    public async Task ViewDashboard(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        Console.WriteLine("[Analytics] Request started");

        if (!AuthUtils.ValidateToken(HttpUtils.GetCookie(req, "token"), out string username, out string role) || role != "admin")
        {
            Console.WriteLine("[Analytics] Unauthorized user, redirecting...");
            res.Redirect("/login?message=Unauthorized");
            res.Close();
            return;
        }

        try
        {
            Console.WriteLine("[Analytics] Fetching data from service...");

            var topStocked = await service.GetTopStockedProducts();
            Console.WriteLine("[Analytics] topStocked OK");

            var categoryValues = await service.GetValuePerCategory();
            Console.WriteLine("[Analytics] categoryValues OK");

            var recentUpdates = await service.GetRecentlyUpdatedItems();
            Console.WriteLine("[Analytics] recentUpdates OK");

            var lowStock = await service.GetLowStockItems();
            Console.WriteLine("[Analytics] lowStock OK");

            var bestOrdered = await service.GetBestOrderedProducts();
            Console.WriteLine("[Analytics] bestOrdered OK");

            var dailyCosts = await service.GetDailyCosts();
            Console.WriteLine("[Analytics] dailyCosts OK");

            string content = AnalyticsHtmlTemplates.Dashboard(
                topStocked, categoryValues, recentUpdates, lowStock, bestOrdered, dailyCosts
            );

            Console.WriteLine("[Analytics] Rendering HTML...");

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK,
                HtmlTemplates.Base("Inventory Analytics", content));

            Console.WriteLine("[Analytics] Response sent OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Analytics ERROR] " + ex.Message);
            string html = $"<h2>Error loading analytics</h2><pre>{WebUtility.HtmlEncode(ex.ToString())}</pre>";
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError,
                HtmlTemplates.Base("Error", html));
        }
    }

}
