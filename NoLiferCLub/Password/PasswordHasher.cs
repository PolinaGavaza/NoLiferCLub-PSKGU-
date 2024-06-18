using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Создаем объект SHA256
        using (SHA256 sha256 = SHA256.Create())
        {
            // Преобразуем пароль в байтовый массив
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Вычисляем хэш пароля
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

            // Преобразуем хэшированные байты в строку в формате Base64
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public static bool VerifyPassword(string password, string passwordHash)
    {
        // Хэшируем введенный пароль
        string hashedPassword = HashPassword(password);

        // Сравниваем хэшированные пароли
        return hashedPassword == passwordHash;
    }
}












