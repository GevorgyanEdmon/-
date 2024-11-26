using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace SimpleDatabaseApp
{
    // Класс, представляющий запись в базе данных
    public class Record
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Класс для управления базой данных
    public class DatabaseManager : IDisposable
    {
        private readonly string _databasePath;
        private SQLiteConnection _connection;

        // Конструктор класса
        public DatabaseManager(string databasePath)
        {
            _databasePath = databasePath;
            SozdatBazuEsliNet(); // Создаем базу, если она еще не существует
            _connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
            _connection.Open(); // Открываем соединение с базой данных
        }

        // Создание базы данных, если она еще не существует
        private void SozdatBazuEsliNet()
        {
            if (!File.Exists(_databasePath))
            {
                SQLiteConnection.CreateFile(_databasePath);
                using var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
                connection.Open();
                ExecuteCommand("CREATE TABLE Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)", connection);
                Console.WriteLine("База данных создана.");
            }
        }

        // Выполнение SQL-команды с параметрами
        private void ExecuteCommand(string query, SQLiteConnection connection, Dictionary<string, object> parameters = null)
        {
            using var command = new SQLiteCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value); // Добавляем параметры в команду
                }
            }
            command.ExecuteNonQuery(); // Выполняем команду
        }

        // Добавление новой записи
        public void CreateRecord(Record record)
        {
            try
            {
                ExecuteCommand("INSERT INTO Records (Name) VALUES (@Name)", _connection, new Dictionary<string, object> { { "@Name", record.Name } });
                Console.WriteLine("Запись добавлена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        // Получение всех записей из базы данных
        public List<Record> GetAllRecords()
        {
            var records = new List<Record>();
            try
            {
                using var reader = new SQLiteCommand("SELECT * FROM Records", _connection).ExecuteReader();
                while (reader.Read())
                {
                    records.Add(new Record { Id = reader.GetInt32(0), Name = reader.GetString(1) }); // Читаем данные из базы и добавляем в список
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении записей: {ex.Message}");
            }
            return records;
        }

        // Обновление записи
        public void UpdateRecord(Record record)
        {
            try
            {
                ExecuteCommand("UPDATE Records SET Name = @Name WHERE Id = @Id", _connection,
                    new Dictionary<string, object> { { "@Name", record.Name }, { "@Id", record.Id } });
                Console.WriteLine("Запись обновлена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении записи: {ex.Message}");
            }
        }

        // Удаление записи по ID
        public void DeleteRecord(int id)
        {
            try
            {
                ExecuteCommand("DELETE FROM Records WHERE Id = @Id", _connection, new Dictionary<string, object> { { "@Id", id } });
                Console.WriteLine("Запись удалена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении записи: {ex.Message}");
            }
        }

        // Освобождаем ресурсы соединения
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    class Program
    {
        static void Main()
        {
            string databasePath = "database.db";
            using var dbManager = new DatabaseManager(databasePath);

            // Пример работы с базой данных
            dbManager.CreateRecord(new Record { Name = "Первый элемент" });
            dbManager.CreateRecord(new Record { Name = "Второй элемент" });

            var records = dbManager.GetAllRecords();
            foreach (var record in records)
            {
                Console.WriteLine($"Id: {record.Id}, Name: {record.Name}");
            }

            dbManager.UpdateRecord(new Record { Id = 1, Name = "Обновленный первый элемент" });

            dbManager.DeleteRecord(2);

            Console.ReadKey();
        }
    }
}
