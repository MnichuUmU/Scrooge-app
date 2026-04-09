using SQLite;


namespace Scrooge_app.Databases
{
    [Table("logs")]
    public class Logs
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("log_id")]
        public int LogId { get; set; }

        [Column("person_id")]
        public int PersonId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("log_name")]
        public string LogName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }
        
        [Column("due")]
        public DateTime Due { get; set; }

        [Column("proof")]
        public string? Proof { get; set; }

    }
}
