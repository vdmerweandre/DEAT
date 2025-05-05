namespace DEAT.AdminUI.Services.Auth
{
    [Flags]
    public enum Permissions
    {
        None = 0,
        View = 1,
        Create = 2,
        Update = 4,
        Delete = 8,
        All = View | Create | Update | Delete
    }

    public static class RolePermissions
    {
        public static readonly Dictionary<string, Dictionary<string, Permissions>> DefaultPermissions = new()
        {
            {
                "Admin", new Dictionary<string, Permissions>
                {
                    { "Accounts", Permissions.All },
                    { "Transactions", Permissions.All },
                    { "StateChanges", Permissions.All },
                    { "EventLogs", Permissions.All }
                }
            },
            {
                "Accountant", new Dictionary<string, Permissions>
                {
                    { "Accounts", Permissions.View | Permissions.Create },
                    { "Transactions", Permissions.All },
                    { "StateChanges", Permissions.View },
                    { "EventLogs", Permissions.View }
                }
            },
            {
                "Auditor", new Dictionary<string, Permissions>
                {
                    { "Accounts", Permissions.View },
                    { "Transactions", Permissions.View },
                    { "StateChanges", Permissions.All },
                    { "EventLogs", Permissions.All }
                }
            },
            {
                "Viewer", new Dictionary<string, Permissions>
                {
                    { "Accounts", Permissions.View },
                    { "Transactions", Permissions.View },
                    { "StateChanges", Permissions.View },
                    { "EventLogs", Permissions.View }
                }
            }
        };
    }
} 