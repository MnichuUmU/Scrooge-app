using SQLite;

namespace Scrooge_app.Databases
{
    public class LogListDisplayModel : Logs
    {
        public string Color => (Amount > 0) ? "green" : "red";
        public bool HaveProof => !string.IsNullOrEmpty(Proof);
    }
    public class LocalDbService
    {
        private const string DB_NAME = "db_Scrooge.db3";
        private SQLiteAsyncConnection _connection;

        private async Task Init()
        {
            if(_connection != null) return;

            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory , DB_NAME));

            //creating tables
            await _connection.CreateTableAsync<People>();
            await _connection.CreateTableAsync<Logs>();


            var count = await _connection.Table<People>().CountAsync();
            if(count == 0)
            {
                var newPerson = new People { Name = "Imie" , Surname = "Test" , Balance = 0 , Phone = "4812345678" , Mail = "test@gmail.com" , Facebook = "www.facebook.com" , Instagram = "www.instagram.com" , Family = "48234567890" };
                await _connection.InsertAsync(newPerson);

                var newLogs = new List<Logs>
                {
                    new Logs {Date = new DateTime(2025, 12, 31), LogName  = "Na rower", Description="potrzebował teochę pieniędzy na rower", Amount=300, Due = new DateTime(2026, 06, 08) , PersonId = 1},
                    new Logs {Date = DateTime.Now, LogName = "Na rower - zwrot", Description="potrzebował teochę pieniędzy na rower", Amount=-300, Due = new DateTime(2026, 06, 08), PersonId = 1}
                };
                await _connection.InsertAllAsync(newLogs);
            }

        }

        //////////////////////////////////////////////////////
        /// METHOD FOR People
        //////////////////////////////////////////////////////

        public async Task<List<People>> GetPeople()
        {
            await Init();
            return await _connection.Table<People>().ToListAsync();
        }

        public async Task<People> GetPerson()
        {
            await Init();
            return await _connection.Table<People>().FirstAsync();
        }

        public async Task UpdateBalance( int id , decimal balance )
        {
            await Init();
            string sql = "UPDATE people SET balance = balance + ? WHERE id = ?;";
            await _connection.ExecuteAsync(sql , balance , id);
        }
        public async Task AddPerson( People person )
        {
            await Init();
            await _connection.InsertAsync(person);
        }


        //public async Task UpdateBalanceNoInput(People person)
        //{
        //    await Init();
        //    string sql_get_ba = "De"
        //    decimal balance = await _connection.ExecuteAsync()
        //    string sql_update = "UPDATE people SET balance = ? WHERE id = ?;";
        //    await _connection.ExecuteAsync(sql_update , balance);
        //}

        public async Task DeletePerson( People person )
        {
            await Init();
            string sql = "DELETE FROM logs WHERE person_id = ?;";
            await _connection.ExecuteAsync(sql , person.Id);
            await _connection.DeleteAsync(person);
        }


        //////////////////////////////////////////////////////
        /// METHOD FOR Logs
        //////////////////////////////////////////////////////
        public async Task<List<LogListDisplayModel>> GetLogsList( int id )
        {
            await Init();
            string sql = "SELECT log_id, log_name, amount, date, due, description, proof, person_id " +
                "FROM logs " +
                "WHERE person_id = ? " +
                "ORDER BY date DESC;";
            return await _connection.QueryAsync<LogListDisplayModel>(sql , id);
        }
        public async Task AddLog( Logs log )
        {
            await Init();
            await UpdateBalance(log.PersonId , log.Amount);
            await _connection.InsertAsync(log);
        }

        public async Task DeleteLog( Logs log )
        {
            await Init();
            await _connection.DeleteAsync(log);
        }
        public async Task DeleteLogById( int id , int? person_id , decimal? balance )
        {
            await Init();
            person_id ??= -1;
            balance ??= 0;
            if(person_id != -1)
            {
                await _connection.ExecuteAsync("UPDATE people SET balance = balance - ? WHERE id = ?;" , balance , person_id);
            }
            await _connection.ExecuteAsync("DELETE FROM logs WHERE log_id = ?" , id);

        }

    }
}

