namespace SuperMarketDashboard;

public static class AuthViews
{
    public static string Login(string message = "")
    {
        return $@"
        <div class=""auth-page"">
        <form action=""/login"" method=""POST"" class=""auth-form"">
            <h2>🔐 Login to Dashboard</h2>

            <label for=""username"">Username</label>
            <input type=""text"" id=""username"" name=""username"" placeholder=""Enter your username"" required>

            <label for=""password"">Password</label>
            <input type=""password"" id=""password"" name=""password"" placeholder=""Enter your password"" required>

            <div class=""actions"">
            <input type=""submit"" value=""Login"">
            </div>

            <p class=""note"">Only authorized users can access the system.</p>
        </form>
        </div>

        ";
    }
}
