using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;

static class Database
{
    private static DbConnection _conn;


    static Database()
    {
        SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
        builder.DataSource = "database.db";
        _conn = new SQLiteConnection(builder.ConnectionString);
        _conn.Open();
        Debug.WriteLine("Opened database");
    }

    public static void Close()
    {
        _conn.Close();
        Debug.WriteLine("Closed database");
    }


}

class Plant
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public int? Sortav { get; private set; }
    public int? Totav { get; private set; }
    public string? Color { get; private set; }

    private Plant(long id, string name, int? sortav, int? totav, string? color)
    {
        Id = id;
        Name = name;
        Sortav = sortav;
        Totav = totav;
        Color = color;
    }

    static Plant LoadFromReader()
    {

        return Plant();
    }
}