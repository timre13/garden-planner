using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace garden_planner
{
    public class Plant
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public long? Sortav { get; private set; }
        public long? Totav { get; private set; }
        public string? Color { get; private set; }

        public string SortavDisp { get => (Sortav is null or 0) ? "-" : Sortav!.ToString()!; }
        public string TotavDisp { get => (Totav is null or 0) ? "-" : Totav!.ToString()!; }

        public int Amount { get; set; } = 0;

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
}
