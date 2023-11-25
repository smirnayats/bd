// Начало кода

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// Создаем класс для представления данных в таблице "Телефонная книга"
public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
}

// Создаем класс контекста базы данных, наследуясь от DbContext
public class PhoneBookContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PhoneBookDb;Trusted_Connection=True;")
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
    }
}

// Создаем класс для работы с БД
public class PhoneBookManager
{
    private PhoneBookContext _context;

    public PhoneBookManager()
    {
        _context = new PhoneBookContext();
        _context.Database.EnsureCreated();
    }

    // Метод для добавления контакта
    public void AddContact(Contact contact)
    {
        _context.Contacts.Add(contact);
        _context.SaveChanges();
    }

    // Метод для удаления контакта
    public void DeleteContact(int contactId)
    {
        var contact = _context.Contacts.Find(contactId);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            _context.SaveChanges();
        }
    }

    // Метод для обновления контакта
    public void UpdateContact(int contactId, string name, string phoneNumber)
    {
        var contact = _context.Contacts.Find(contactId);
        if (contact != null)
        {
            contact.Name = name;
            contact.PhoneNumber = phoneNumber;
            _context.SaveChanges();
        }
    }

    // Метод для выгрузки данных в виде JSON-файла
    public void ExportToJson(string filename)
    {
        var contacts = _context.Contacts.ToList();
        var json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
        File.WriteAllText(filename, json);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var phoneBookManager = new PhoneBookManager();

        // Примеры использования методов PhoneBookManager

        // Добавление данных
        var newContact = new Contact { Name = "Иван", PhoneNumber = "123456789" };
        phoneBookManager.AddContact(newContact);

        // Удаление данных
        int contactIdToDelete = 1;
        phoneBookManager.DeleteContact(contactIdToDelete);

        // Обновление данных
        int contactIdToUpdate = 2;
        string newName = "Петр";
        string newPhoneNumber = "987654321";
        phoneBookManager.UpdateContact(contactIdToUpdate, newName, newPhoneNumber);

        // Выгрузка данных в JSON-файл
        string exportFilename = "phonebook.json";
        phoneBookManager.ExportToJson(exportFilename);
    }
}

// Конец кода