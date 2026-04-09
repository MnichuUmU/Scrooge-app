using SQLite;


namespace Scrooge_app.Databases
{
    [Table("people")]
    public class People
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("balance")]
        public decimal Balance { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("mail")]
        public string Mail { get; set; }

        [Column("facebook")]
        public string Facebook { get; set; }

        [Column("instagram")]
        public string Instagram { get; set; }

        [Column("family")]
        public string Family { get; set; }

        [Column("pfp")]
        public string ProfilePick { get; set; } = "user.png";

        [Ignore]
        public string FullName => $"{Name} {Surname}";
    }
}
