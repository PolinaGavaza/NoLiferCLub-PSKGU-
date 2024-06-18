using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using NoLiferClub; // Используем пространство имен модели

public class Program
{


    static void Main(string[] args)
    {
        // Строка подключения к PostgreSQL
        var connectionString = "Host=localhost;Database=cybersportclubdb;Username=polingavaza;Password=1234567;Port=5432";
        var dbConnection = new DatabaseConnection(connectionString);
        //Авторизация пользователя
        if (!AuthorizeUser(dbConnection))
        {
            Console.WriteLine("Авторизация не удалась.");
            return;
        }
        //основной цикл программы
        while (true)
        {
            // Выводим меню выбора действий
            Console.WriteLine("1. Регистрация игрока");
            Console.WriteLine("2. Создание турнира");
            Console.WriteLine("3. Просмотр результатов турнира");
            Console.WriteLine("4. Удаление турнира");
            Console.WriteLine("5. Поиск турнира с фильтром");
            Console.WriteLine("6. Выход");

            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine(); // Считываем выбор пользователя

            switch (choice)
            {
                case "1":
                    RegisterPlayer(dbConnection); // Вызываем функцию регистрации игрока
                    break;
                case "2":
                    CreateTournament(dbConnection); // Вызываем функцию создания турнира
                    break;
                case "3":
                    ViewTournamentResults(dbConnection); // Вызываем функцию просмотра результатов
                    break;
                case "4":
                    DeleteTournament(dbConnection); // Вызываем функцию удаления турнира
                    break;
                case "5":
                    SearchTournaments(dbConnection); // Вызываем функцию поиска турнира
                    break;
                case "6":
                    return; // Выходим из программы
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
    ////// Функция авторизации пользователя
    static bool AuthorizeUser(DatabaseConnection dbConnection)
    {
        Console.WriteLine("Введите email:");
        var email = Console.ReadLine(); // Считываем email игрока

        Console.WriteLine("Введите пароль:");
        var password = Console.ReadLine(); // Считываем пароль игрока

        // Открываем подключение к базе данных
        using (var connection = dbConnection.GetConnection())
        {
            // Выполняем запрос на поиск пользователя по email
            using (var cmd = new NpgsqlCommand("SELECT PasswordHash FROM Player WHERE Email = @email", connection))
            {
                cmd.Parameters.AddWithValue("@email", email); // Добавляем параметр для email

                connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    try
                    {

                        if (reader.Read())
                        {
                            // Получаем хеш пароля из базы данных
                            var storedPasswordHash = reader.GetString(0);

                            // Проверяем, совпадает ли введенный пароль с хешем из базы данных
                            if (PasswordHasher.VerifyPassword(storedPasswordHash, password))
                            {
                                Console.WriteLine("Авторизация прошла успешно!");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Неверный пароль.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Пользователь с таким email не найден.");
                            return false;
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
                        return false;

                    }
                }
            }
        }
    }




    // Функция регистрации игрока
    static void RegisterPlayer(DatabaseConnection dbConnection)
    {
        Console.WriteLine("Введите имя игрока:");
        var name = Console.ReadLine(); // Считываем имя игрока

        Console.WriteLine("Введите email:");
        var email = Console.ReadLine(); // Считываем email игрока

        Console.WriteLine("Введите пароль:");
        var password = Console.ReadLine(); // Считываем пароль игрока

        // Хеширование пароля
        var passwordHash = PasswordHasher.HashPassword(password); // Хешируем пароль

        // Открываем подключение к базе данных
        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем соединение
                                   // Выполняем запрос на вставку данных в таблицу Player
                using (var cmd = new NpgsqlCommand("INSERT INTO Player (Name, Email, PasswordHash) VALUES (@name, @email, @passwordHash)", connection))
                {
                    cmd.Parameters.AddWithValue("@name", name); // Добавляем параметр для имени
                    cmd.Parameters.AddWithValue("@email", email); // Добавляем параметр для email
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash); // Добавляем параметр для хеша пароля


                    cmd.ExecuteNonQuery(); // Выполняем запрос
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);

            }
        }

        Console.WriteLine("Игрок зарегистрирован!");
    }


    // Функция создания турнира
    static void CreateTournament(DatabaseConnection dbConnection)
    {
        Console.WriteLine("Введите название турнира:");
        var name = Console.ReadLine(); // Считываем название турнира

        Console.WriteLine("Введите дату турнира (формат: YYYY-MM-DD):");
        var date = DateTime.Parse(Console.ReadLine()); // Считываем дату турнира

        Console.WriteLine("Выберите игру:");
        DisplayGames(dbConnection);

        Console.WriteLine("Введите ID игры:");
        var gameId = int.Parse(Console.ReadLine()); // Считываем ID игры

        Console.WriteLine("Выберите тип турнира (1 - Amateur, 2 - Official):");
        var type = (TournamentType)Enum.Parse(typeof(TournamentType), Console.ReadLine()); // Считываем тип турнира

        Console.WriteLine("Есть ли призовые? (1 - Да, 0 - Нет):");
        string input = Console.ReadLine();

        bool hasPrize = false;
        // Проверяем, является ли ввод пользователя цифрой "1" или "0"
        if (input == "1")
        {
            hasPrize = true;
        }
        else if (input == "0")
        {
            hasPrize = false;
        }
        else
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, введите 1 для 'Да' или 0 для 'Нет'.");
            // Возможно, вы захотите запросить ввод снова
        }



        Console.WriteLine("Выберите компьютерный зал:");
        DisplayComputerRooms(dbConnection);

        Console.WriteLine("Введите ID компьютерного зала:");
        var computerRoomId = int.Parse(Console.ReadLine()); // Считываем ID компьютерного зала


        // Открываем подключение к базе данных
        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем соединение

                // Выполняем запрос на вставку данных в таблицу Tournaments
                using (var cmd = new NpgsqlCommand("INSERT INTO Tournaments (Name, Date, GameId, Type, HasPrize, ComputerRoomId) VALUES (@name, @date, @gameId, @type, @hasPrize, @computerRoomId)", connection))
                {
                    cmd.Parameters.AddWithValue("@name", name); // Добавляем параметр для названия
                    cmd.Parameters.AddWithValue("@date", date); // Добавляем параметр для даты
                    cmd.Parameters.AddWithValue("@gameId", gameId); // Добавляем параметр для ID игры
                    cmd.Parameters.AddWithValue("@type", type.ToString()); // Добавляем параметр для типа турнира
                    cmd.Parameters.AddWithValue("@hasPrize", hasPrize); // Добавляем параметр для наличия призовых
                    cmd.Parameters.AddWithValue("@computerRoomId", computerRoomId); // Добавляем параметр для ID зала

                    // Выполняем запрос
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
                // ... (обработка ошибки)
            }
        }

        Console.WriteLine("Турнир создан!");
    }
    static void DisplayGames(DatabaseConnection dbConnection)
    {
        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем подключение

                using (var cmd = new NpgsqlCommand("SELECT * FROM Games", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader.GetInt32(0)}. {reader.GetString(1)}");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);

            }
        }
    }

    static void DisplayComputerRooms(DatabaseConnection dbConnection)
    {
        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем подключение

                using (var cmd = new NpgsqlCommand("SELECT * FROM ComputerRooms", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader.GetInt32(0)}. {reader.GetString(1)}");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }

    //Функция просмотра результатов турнира
    static void ViewTournamentResults(DatabaseConnection dbConnection) 
    {
        Console.WriteLine("Введите ID турнира:");
        var tournamentId = int.Parse(Console.ReadLine()); // Считываем ID турнира

        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем подключение

                using (var cmd = new NpgsqlCommand("SELECT r.Place, p.Name, r.Points FROM Results r JOIN Player p ON r.PlayerId = p.PlayerId JOIN Tournaments t ON r.TournamentId = t.TournamentId WHERE t.TournamentId = @tournamentId", connection))
                {
                    cmd.Parameters.AddWithValue("@tournamentId", tournamentId); // Добавляем параметр для ID турнира

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Место: {reader.GetInt32(0)}, Игрок: {reader.GetString(1)}, Очки: {reader.GetInt32(2)}");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }

    // Функция удаления турнира
    static void DeleteTournament(DatabaseConnection dbConnection)
    {
        Console.WriteLine("Введите ID турнира для удаления:");
        var tournamentId = int.Parse(Console.ReadLine()); // Считываем ID турнира

        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем подключение

                // Запрос на удаление турнира
                using (var cmd = new NpgsqlCommand("DELETE FROM Tournaments WHERE TournamentId = @tournamentId", connection))
                {
                    cmd.Parameters.AddWithValue("@tournamentId", tournamentId); // Добавляем параметр для ID турнира

                    // Выполняем запрос
                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Турнир успешно удален.");
                    }
                    else
                    {
                        Console.WriteLine("Турнир с таким ID не найден.");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при удалении турнира: " + ex.Message);
            }
        }
    }
    // Функция поиска турниров с фильтром
    static void SearchTournaments(DatabaseConnection dbConnection)
    {
        Console.WriteLine("Введите критерий поиска (например, название турнира):");
        var searchTerm = Console.ReadLine(); // Считываем критерий поиска

        using (var connection = dbConnection.GetConnection())
        {
            try
            {
                connection.Open(); // Открываем подключение

                // Запрос на поиск турниров с фильтром
                using (var cmd = new NpgsqlCommand("SELECT * FROM Tournaments WHERE Name LIKE @searchTerm", connection))
                {
                    cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%"); // Добавляем параметр для фильтра

                    // Читаем данные из результата запроса
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("Найденные турниры:");
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader.GetInt32(0)}, Название: {reader.GetString(1)}, Дата: {reader.GetDateTime(2)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Турниры, соответствующие критерию поиска, не найдены.");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ошибка при поиске турниров: " + ex.Message);
            }
        }
    }
}


