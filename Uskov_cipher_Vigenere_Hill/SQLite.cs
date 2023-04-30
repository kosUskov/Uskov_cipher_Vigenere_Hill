using System.Data.SQLite;

namespace Uskov_cipher_Vigenere_Hill
{
    internal class SQLite
    {
        public static void Write(object obj)
        {
            using (var connection = new SQLiteConnection(@"Data Source = " + Environment.CurrentDirectory + @"\dataForCipherTelegram.db; Version=3;"))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = "DROP TABLE IF EXISTS allData;";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE allData (ID INTEGER PRIMARY KEY, mode INTEGER, keyVigenere TEXT, keyHill TEXT)";
                command.ExecuteNonQuery();
                foreach (var user in Program.Users)
                {
                    string keyVigenere = HelpFunction.SetDataToSQL(user.keyVigenere);
                    string keyHill = HelpFunction.SetDataToSQL(user.keyHill);
                    command.CommandText = $"INSERT INTO allData(ID, mode, keyVigenere, keyHill) VALUES({user.ID}, {user.mode}, '{keyVigenere}', '{keyHill}')";
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void Read()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\dataForCipherTelegram.db"))
            {
                using (var connection = new SQLiteConnection(@"Data Source = " + Environment.CurrentDirectory + @"\dataForCipherTelegram.db; Version=3;"))
                {
                    connection.Open();
                    var command = new SQLiteCommand();
                    command.Connection = connection;
                    command.CommandText = $"SELECT * FROM allData";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Program.Users.Add(new User_
                                {
                                    ID = Convert.ToInt32(reader.GetValue(0)),
                                    mode = Convert.ToInt32(reader.GetValue(1)),
                                    keyVigenere = HelpFunction.GetDataFromSql(reader.GetValue(2).ToString()),
                                    keyHill = HelpFunction.GetDataFromSql(reader.GetValue(3).ToString())
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
