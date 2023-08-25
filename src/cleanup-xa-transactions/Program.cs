using System;
using System.Threading;
using cleanup_xa_transactions;
using Dapper;
using MySqlConnector;
using PeanutButter.EasyArgs;
using PeanutButter.Utils;

var opts = args.ParseTo<IOptions>();
using var conn = new MySqlConnection(
    opts.GenerateConnectionString()
);

var rows = await conn.QueryAsync<XaRecoverRow>(
    "xa recover convert xid;"
).ToArrayAsync();
if (rows.Length == 0)
{
    Console.WriteLine("Nothing to do");
    return;
}

var errored = false;
Console.WriteLine($"{rows.Length} transactions found...");
foreach (var row in rows)
{
    var rawData = row.Data.RegexReplace("^0x", "");
    var totalBytes = row.GTRID_Length + row.BQUAL_Length;
    var multiplier = (decimal) rawData.Length / (decimal) (totalBytes);
    if ((int) multiplier != multiplier)
    {
        Console.Error.WriteLine($"Non-integer multiplier found - unable to process tx with data '{row.Data}'");
        errored = true;
        continue;
    }

    var p1 = rawData.Substring(0, row.GTRID_Length * (int) multiplier);
    var p2 = rawData.Substring(row.GTRID_Length * (int) multiplier);
    var p3 = row.FormatId;
    var sql = $"xa {opts.Action} X'{p1}',X'{p2}',{p3}";
    Status.Start($"{opts.Action} tx '{row.Data}'");
    try
    {
        await conn.ExecuteAsync(sql);
        Status.Ok();
    }
    catch (Exception e)
    {
        Status.Fail();
        errored = true;
        Console.WriteLine($"Unable to run sql:\n{sql}\n:{e.Message}");
    }
}

Console.WriteLine(
    errored
        ? "finished with errors (see above)"
        : "done"
);