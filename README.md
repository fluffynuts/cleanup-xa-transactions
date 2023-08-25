# cleanup-xa-transactions

## Purpose:
To "do something" about MySQL XA transactions which have been left in limbo,
particularly for when attempting to upgrade a MySql database to a later version
of MySql, when you receive blocking errors like:
```
Upgrade cannot proceed due to an existing prepared XA transaction
```

## Why
XA transactions are a mission to work with. See https://www.percona.com/blog/how-to-deal-with-xa-transactions-recovery/ for an idea of the shenannigans
that are required to work with them. Better yet, that article doesn't mention
that the data field from `xa recover convert xid` may be larger than suggested
by the `gtrid_length` and `bqual_length` fields from that result - in particular,
I find mine are exactly 2x larger than the values provided there.

Splitting out these strings and manually rolling back or committing xa transactions
is tedious. Let's make the computer do it for us.

## Usage

Requirements:
- dotnet 7
- (optional) node (recommended v16+)

Either:
1. Build with your IDE of choice or 
2. Build with `npm run build` and use the output .exe, or
3. `npm start -- {options}` to run directly.

- run with `--help` for all options
- you probably only need `--password {password}` if you're intending to connect as `root` to a mysql server running on localhost on the default port
    - this will default to roll back outstanding XA transactions
