using System;
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

    public static long GetLastInsertRowId()
    {
        var reader = ExecReaderCmd("SELECT LAST_INSERT_ROWID()");
        reader.Read();
        return reader.GetInt64(0);
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

    public static List<long> GetNeighIds(long id, bool isGoodNeigh)
    {
        var reader = ExecReaderCmd($"SELECT * FROM {(isGoodNeigh ? "good" : "bad")}_neighbours WHERE plant1 = @0 OR plant2 = @0", new object[] {id});
        List<long> output = new List<long>();
        while (reader.Read())
        {
            var plant1 = reader.GetInt64(0);
            var plant2 = reader.GetInt64(1);

            output.Add(plant1 == id ? plant2 : plant1);
        }
        return output;
    }

    public static List<Plant> GetNeighs(long id, bool isGoodNeigh)
    {
        var ids = GetNeighIds(id, isGoodNeigh)!;
        return ids.Select(plant => GetPlantFromId(plant)).ToList();
    }

    public static bool AreBadNeighs(long id1, long id2)
    {
        var reader = ExecReaderCmd("SELECT 1 FROM bad_neighbours WHERE (plant1 = @0 AND plant2 = @1) " +
            "OR (plant1 = @1 AND plant2 = @0)", new object[] { id1, id2 });
        return reader.HasRows;
    }

    public static bool AreGoodNeighs(long id1, long id2)
    {
        var reader = ExecReaderCmd("SELECT 1 FROM good_neighbours WHERE (plant1 = @0 AND plant2 = @1) " +
            "OR (plant1 = @1 AND plant2 = @0)", new object[] { id1, id2 });
        return reader.HasRows;
    }

    public static long? GetPlantIdFromName(in string name)
    {
        var reader = ExecReaderCmd("SELECT id FROM plants WHERE name = @0", new object[] { name });
        return reader.Read() ? reader.GetInt64(0) : null;
    }

    public static void AddPlantDefinition(ref Plant plant, in List<long> goodNeighIds, in List<long> badNeighIds)
    {
        if (plant == null)
            throw new ArgumentNullException("Plant is null");
        if (plant.Id != 0)
            throw new ArgumentException("Plant has an ID");
        if (plant.Name.Length == 0)
            throw new ArgumentException("Plant has empty `name`");
        if (plant.Sortav < 1)
            throw new ArgumentException("Plant has invalid `sortav`");
        if (plant.Totav < 1)
            throw new ArgumentException("Plant has invalid `totav`");
        if ((plant.Color?.Length ?? 0) == 0)
            throw new ArgumentException("Plant has invalid `color`");

        var count = ExecWriterCmd("INSERT INTO plants (name, sortavolsag, totavolsag, color) VALUES (@0, @1, @2, @3)",
            new object[] { plant.Name, plant.Sortav ?? 0, plant.Totav ?? 0, plant.Color ?? "#33dd55" });
        if (count != 1)
            throw new Exception("Failed to insert Plant");

        var plantId = GetLastInsertRowId();
        if (plantId == 0)
            throw new Exception("Failed to get LAST_INSERT_ROWID()");
        foreach (var id in goodNeighIds)
        {
            if (plantId == id || plantId == 0)
                throw new ArgumentException("Invalid id for neighbour");
            ExecWriterCmd("INSERT INTO good_neighbours (plant1, plant2) VALUES " +
                "(@0, @1)", new object[] { plantId, id }.Order() );
        }
        foreach (var id in badNeighIds)
        {
            if (plantId == id || plantId == 0)
                throw new ArgumentException("Invalid id for neighbour");
            ExecWriterCmd("INSERT INTO bad_neighbours (plant1, plant2) VALUES " +
                "(@0, @1)", new object[] { plantId, id }.Order());
        }
    }
}

public class Plant
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public long? Sortav { get; private set; }
    public long? Totav { get; private set; }
    public string? Color { get; private set; }

    public string SortavDisp { get => (Sortav is null or 0) ? "-" : Sortav!.ToString()!; }
    public string TotavDisp { get => (Totav is null or 0) ? "-" : Totav!.ToString()!; }

    public Plant(long id, string name, long? sortav, long? totav, string? color)
    {
        Id = id;
        Name = name;
        Sortav = sortav;
        Totav = totav;
        Color = color;
    }

    public Plant(string name, long? sortav, long? totav, string? color)
        : this(0, name, sortav, totav, color) { }

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