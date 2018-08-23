using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Linq
{
    class GamerDataContext:DataContext
    {
        public static readonly string DefaultConnectionString = @"Server=localhost;Database=RoboGamer;Trusted_Connection=True;";
        public GamerDataContext(string connectionString = null) 
            : base(connectionString ?? DefaultConnectionString)
        {
        }

        public Table<Step> Steps;
        public Table<Hsv> Hsvs;
    }

    [Table(Name = "Step")]
    public class Step
    {
        [Column(IsPrimaryKey = true)]
        public Guid Id = Guid.NewGuid();
        [Column]
        public string Game;
        [Column]
        public string Source;
        [Column]
        public string Target;
        [Column]
        public string Action;
        [Column]
        public DateTime Time = DateTime.UtcNow;
    }
    [Table(Name = "Hsv")]
    public class Hsv
    {
        [Column(IsPrimaryKey = true)]
        public string Shot;
        [Column]
        public int H;
        [Column]
        public int S;
        [Column]
        public int V;
    }
}
