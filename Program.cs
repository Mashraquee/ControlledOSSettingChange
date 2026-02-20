using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControlledOSSetting_Change
{
    internal class Program
    {
        private const string ConfigPath = "Config/osconfig.json";
        private const string AuditLogPath = "Logs/audit.log";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter command:");
            string input = Console.ReadLine();

            try
            {
                HandleCommand(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void HandleCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be empty.");

            string[] parts = command.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3 || parts[0] != "os" || parts[1] != "set-timezone")
                throw new ArgumentException("Invalid command format. Use: os set-timezone <Timezone>");

            string newTimezone = parts[2];

            ValidateTimezone(newTimezone);

            var config = LoadConfig();
            string oldTimezone = config.Timezone;

            if (oldTimezone == newTimezone)
            {
                Console.WriteLine("Timezone is already set to the specified value.");
                return;
            }

            // Simulate OS command
            Console.WriteLine($"[SIMULATION] Executing: timedatectl set-timezone {newTimezone}");

            // Persist config
            config.Timezone = newTimezone;
            SaveConfig(config);

            // Audit log
            LogAudit("operator", "set-timezone", oldTimezone, newTimezone);

            Console.WriteLine("Timezone updated successfully.");
        }

        static void ValidateTimezone(string timezone)
        {
            // Simple validation using TimeZoneInfo
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(timezone);
            }
            catch
            {
                throw new ArgumentException("Invalid timezone identifier.");
            }
        }

        static OSConfig LoadConfig()
        {
            if (!Directory.Exists("Config"))
                Directory.CreateDirectory("Config");

            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new OSConfig { Timezone = "UTC" };
                SaveConfig(defaultConfig);
                return defaultConfig;
            }

            string json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<OSConfig>(json);
        }

        static void SaveConfig(OSConfig config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }

        static void LogAudit(string user, string action, string oldValue, string newValue)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            string logEntry = $"{DateTime.UtcNow:O} | User: {user} | Action: {action} | {oldValue} -> {newValue}";
            File.AppendAllText(AuditLogPath, logEntry + Environment.NewLine);
        }
    }
}
class OSConfig
{
    public string Timezone { get; set; }
}


//# Controlled OS Setting Change with Audit Logging

//This project demonstrates how to perform a **controlled OS setting change** (simple, non-invasive) on an EGM-like system.  
//We simulate setting the **timezone** by writing to a configuration file and logging an audit trail.

//---

//## Features
//-  Controlled OS setting change (timezone)
//-  Input validation
//-  Configuration persistence (`config.json`)
//-  Audit logging (`audit.log`)
//-  Operator attribution and timestamp

//---

//## Example Command
//```bash
//os set-timezone Africa/Conakry

