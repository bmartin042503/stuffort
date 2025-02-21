﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Stuffort.Model
{
    [Table("STask")]
    public class STask
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }

        [Indexed]
        public int SubjectID { get; set; }

        [MaxLength(50)]
        public string SubjectName { get; set; }

        public bool IsDone { get; set; }

        public bool IsDeadline { get; set; }
        public DateTimeOffset DeadLine { get; set; }
        public DateTimeOffset AddedTime { get; set; }
        public DateTimeOffset Finished { get; set; }

        [Ignore]
        public STask Instance { get { return this; } }
    }
}
