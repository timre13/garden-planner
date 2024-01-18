using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

static class Database
{
    private static DbConnection _conn;

    static Database()
    {
        SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
        builder.DataSource = "data/database.db";
        _conn = new SQLiteConnection(builder.ConnectionString);
        _conn.Open();
        Debug.WriteLine("Opened database");
    }

    public static void Close()
    {
        var plant = GetPlantFromId(1);
        Debug.WriteLine(plant.Name);
        _conn.Close();
        Debug.WriteLine("Closed database");
    }

    public static SQLiteDataReader ExecReaderCmd(string command, IEnumerable<object>? pars=null)
    {
        Debug.WriteLine($"Executing reader command: \"{command}\"");
        using (var cmd = _conn.CreateCommand() as SQLiteCommand)
        {
            cmd!.CommandText = command;
            if (pars != null)
            {
                cmd!.Parameters.AddRange(
                pars.Select((x, i) => new SQLiteParameter(value: x, parameterName: "@" + i.ToString())).ToArray()
                //pars.Select((x, i) => new SQLiteParameter() {Value = x}).ToArray()
                );
            }
            return cmd.ExecuteReader();
        }
    }

    public static int ExecWriterCmd(string command, IEnumerable<object>? pars=null)
    {
        Debug.WriteLine($"Executing writer command: \"{command}\"");
        using (var cmd = _conn.CreateCommand() as SQLiteCommand)
        {
            cmd!.CommandText = command;
            if (pars != null)
            {
                cmd!.Parameters.AddRange(
                    pars.Select((x, i) => new SQLiteParameter(value: x, parameterName: i.ToString())).ToArray()
                );
            }
            int count = cmd.ExecuteNonQuery();
            Debug.WriteLine($"Modified {count} rows");
            return count;
        }
    }

    public static T? GetValOrNull<T>(in SQLiteDataReader reader, int col)
    {
        return reader.IsDBNull(col) ? default(T) : reader.GetFieldValue<T>(col);
    }

    public static Plant GetPlantFromId(long id)
    {
        var reader = ExecReaderCmd("SELECT * WHERE id = @0", new object[] { id });
        reader.Read();
        return Plant.LoadFromReader(reader);
    }

    public static List<Plant> GetAllPlantsOrdered()
    {
        var reader = ExecReaderCmd("SELECT * FROM plants ORDER BY name");
        List<Plant> output = new List<Plant>();
        while (reader.Read())
        {
            output.Add(Plant.LoadFromReader(reader));
        }
        return output.OrderBy(x => x.Name).ToList();
    }

    public static List<long> GetGoodNeighIds(long id)
    {
        var reader = ExecReaderCmd("SELECT * FROM good_neighbours WHERE plant1 = @0 OR plant2 @0", new object[] {id});
        List<long> output = new List<long>();
        while (reader.Read())
        {
            var plant1 = reader.GetInt64(0);
            var plant2 = reader.GetInt64(1);

            output.Add(plant1 == id ? plant2 : plant1);
        }
        return output;
    }
}

class Plant
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public long? Sortav { get; private set; }
    public long? Totav { get; private set; }
    public string? Color { get; private set; }

    private Plant(long id, string name, long? sortav, long? totav, string? color)
    {
        Id = id;
        Name = name;
        Sortav = sortav;
        Totav = totav;
        Color = color;
    }

    public static Plant LoadFromReader(SQLiteDataReader reader)
    {
        int index = 0;
        
        var id = reader.GetInt64(index++);
        var name = reader.GetString(index++);
        var sortav = Database.GetValOrNull<long>(reader, index++);
        var totav = Database.GetValOrNull<long>(reader, index++);
        var color = Database.GetValOrNull<string>(reader, index++);

        return new Plant(id, name, sortav, totav, color);
    }
}