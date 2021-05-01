using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Stuffort.Model
{
    public class STask
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [MaxLength(120)]
        public string Name { get; set; }

        [Indexed]
        public int SubjectID { get; set; }

        public bool IsDone { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime AddedTime { get; set; }
    }
}
