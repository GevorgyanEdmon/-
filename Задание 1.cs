using System;
using System.Collections.Generic;

// Класс для проверки скобок
public static class ProverkaSkobok
{
    // Метод проверки правильности последовательности скобок
    public static bool IsValid(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true; // Если строка пустая или null, возвращаем true
        }

        var stack = new Stack<char>(); // Стек для хранения открывающих скобок
        var pairs = new Dictionary<char, char> // Словарь пар скобок
        {
            { '(', ')' },
            { '{', '}' },
            { '[', ']' }
        };

        foreach (char c in input)
        {
            if (pairs.ContainsKey(c))
            {
                stack.Push(c); // Если символ - открывающая скобка, добавляем её в стек
            }
            else if (pairs.ContainsValue(c))
            {
                if (stack.Count == 0 || pairs[stack.Pop()] != c)
                {
                    return false; // Если стек пуст или верхняя скобка не соответствует закрывающей, возвращаем false
                }
            }
        }

        return stack.Count == 0; // Возвращаем true, если стек пуст
    }
}

// Основной класс программы
public class Programma
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введите последовательность скобок:");
        string input = Console.ReadLine(); // Читаем ввод пользователя

        bool result = ProverkaSkobok.IsValid(input);
        Console.WriteLine($"Результат: {result}"); // Выводим результат проверки

        // Дополнительный запрос на продолжение (опционально)
        Console.WriteLine("Проверить еще одну последовательность? (да/нет)");
        string answer = Console.ReadLine().ToLower(); // Читаем ответ пользователя

        while (answer == "да")
        {
            Console.WriteLine("Введите последовательность скобок:");
            input = Console.ReadLine(); // Читаем новую последовательность скобок
            result = ProverkaSkobok.IsValid(input);
            Console.WriteLine($"Результат: {result}"); // Выводим результат проверки
            Console.WriteLine("Проверить еще одну последовательность? (да/нет)");
            answer = Console.ReadLine().ToLower(); // Читаем ответ пользователя
        }

        Console.WriteLine("Программа завершена."); // Завершаем программу
    }
}
