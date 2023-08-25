using System;
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
);
foreach (var row in rows)
{
    var rawData = row.Data.RegexReplace("^0x", "");
    var totalBytes = row.GTRID_Length + row.BQUAL_Length;
    var multiplier = (decimal) rawData.Length / (decimal) (totalBytes);
    if ((int) multiplier != multiplier)
    {
        Console.Error.WriteLine($"Non-integer multiplier found - unable to process tx with data '{row.Data}'");
        continue;
    }

    var p1 = rawData.Substring(0, row.GTRID_Length * (int)multiplier);
    var p2 = rawData.Substring(row.GTRID_Length * (int)multiplier);
    var p3 = row.FormatId;
    Console.WriteLine($"{opts.Action} tx '{row.Data}'");
    await conn.ExecuteAsync($"xa {opts.Action} X'{p1}',X'{p2}',{p3}");
}