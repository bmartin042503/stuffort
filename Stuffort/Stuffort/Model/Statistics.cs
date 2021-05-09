using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Stuffort.Model
{
    [Table("Statistics")]
    public class Statistics
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public TimeSpan Time { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int TaskID { get; set; }
        public DateTime Started { get; set; }
        public DateTime Finished { get; set; }
        public bool IsDone { get; set; }
        public bool TaskDisconnection { get; set; }

        [Ignore]
        public string TemporaryName { get; set; }

    }
}
