using MySqlConnector;
using PeanutButter.EasyArgs.Attributes;

namespace cleanup_xa_transactions
{
    public enum Actions
    {
        Rollback,
        Commit
    }

    public interface IOptions
    {
        [Description("The mysql host to connect to")]
        [Default("localhost")]
        string Host { get; set; }

        [Description("The port to connect on")]
        [Default(3306)]
        [Min(1)]
        [Max(65535)]
        int Port { get; set; }

        [Description("The user to connect with")]
        [Default("root")]
        string User { get; set; }

        [Description("The password to use")]
        string Password { get; set; }

        [Description("The action to take with a pending xa transaction")]
        [Default(Actions.Rollback)]
        Actions Action { get; set; }
    }

    public static class OptionsExtensions
    {
        public static string GenerateConnectionString(
            this IOptions opts
        )
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = opts.Host,
                Port = (uint)opts.Port,
                UserID = opts.User,
                Password = opts.Password,
                AllowUserVariables = true
            };
            return builder.ToString();
        }
    }
}