namespace SuperMarketDashboard;

public static class PaginationUtils
{
    public static string RenderPagination(string baseUrl, int currentPage, int totalPages)
    {
        if (totalPages <= 1)
            return "";

        string html = "<div class=\"pagination\">";

        // Previous
        if (currentPage > 1)
            html += $"<a href=\"{baseUrl}?page={currentPage - 1}\" class=\"page-btn\">« Prev</a>";
        else
            html += "<span class=\"page-btn disabled\">« Prev</span>";

        // Page numbers
        for (int i = 1; i <= totalPages; i++)
        {
            if (i == currentPage)
                html += $"<span class=\"page-btn active\">{i}</span>";
            else
                html += $"<a href=\"{baseUrl}?page={i}\" class=\"page-btn\">{i}</a>";
        }

        // Next
        if (currentPage < totalPages)
            html += $"<a href=\"{baseUrl}?page={currentPage + 1}\" class=\"page-btn\">Next »</a>";
        else
            html += "<span class=\"page-btn disabled\">Next »</span>";

        html += "</div>";

        return html;
    }
}
