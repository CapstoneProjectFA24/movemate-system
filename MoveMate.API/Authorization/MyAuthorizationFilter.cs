using Hangfire.Dashboard;

namespace MoveMate.API.Authorization;

public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public MyAuthorizationFilter()
    {
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return true; //httpContext.User.Identity.IsAuthenticated;
    }
}