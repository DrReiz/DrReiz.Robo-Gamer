﻿using LinqToDB;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.AndroidGamer.Wui
{
    public class GamerDataContext: LinqToDB.Data.DataConnection
    {
        public GamerDataContext(string connectionString = null)
            : base("SqlServer", connectionString ?? "RoboGamer")
        {
        }

        public ITable<Step> Steps => GetTable<Step>();
        public ITable<Hsv> Hsvs => GetTable<Hsv>();
        public ITable<Processing> Processings => GetTable<Processing>();
        public ITable<ShotCategory> ShotCategories => GetTable<ShotCategory>();

    }

    [Table(Name = "Step")]
    public class Step
    {
        [PrimaryKey]
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
        [PrimaryKey]
        public string Shot;
        [Column]
        public int H;
        [Column]
        public int S;
        [Column]
        public int V;
    }

    [Table(Name = "Processing")]
    public class Processing
    {
        [PrimaryKey]
        public Guid Id = Guid.NewGuid();
        [Column]
        public string Name;
        [Column]
        public DateTime ProcessedTime;
        [Column]
        public string Keys;

        [Column]
        public DateTime UpdateTime;
    }
    [Table(Name ="ShotCategory")]
    public class ShotCategory
    {
        [PrimaryKey]
        public Guid Id = Guid.NewGuid();
        [Column]
        public string Shot;
        [Column]
        public string Category;

        [Column]
        public DateTime ChangeTick = DateTime.UtcNow;
    }
}
