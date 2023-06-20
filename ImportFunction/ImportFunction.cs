using ImportFunction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public class VehicleDataProcessor
{
    public void ProcessVehicleData(string csvFilePath)
    {
        List<Vehicle> vehicles = ReadAndRemoveDuplicates(csvFilePath);

        ExportCsvFilesByFuelType(vehicles);

        List<Vehicle> validRegistrations = FindVehiclesWithValidRegistrations(vehicles);

        int invalidRegistrationCount = CountVehiclesWithoutValidRegistrations(vehicles);

        Console.WriteLine($"Number of vehicles with invalid registrations: {invalidRegistrationCount}");
    }

    private List<Vehicle> ReadAndRemoveDuplicates(string csvFilePath)
    {
        List<Vehicle> vehicles = new List<Vehicle>();
        HashSet<string> uniqueRegistrations = new HashSet<string>();

        using (StreamReader reader = new StreamReader(csvFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                string registration = parts[0].Trim();

                if (!uniqueRegistrations.Contains(registration))
                {
                    uniqueRegistrations.Add(registration);

                    Vehicle vehicle = new Vehicle
                    {
                        Registration = registration,
                        Make = parts[1].Trim(),
                        Model = parts[2].Trim(),
                        FuelType = parts[3].Trim()
                        // Set other properties as needed
                    };

                    vehicles.Add(vehicle);
                }
            }
        }

        return vehicles;
    }

    private void ExportCsvFilesByFuelType(List<Vehicle> vehicles)
    {
        var vehiclesByFuelType = vehicles.GroupBy(v => v.FuelType);

        foreach (var group in vehiclesByFuelType)
        {
            string fuelType = group.Key;
            string csvFileName = $"Technical Test Data_{fuelType}.csv";

            using (StreamWriter writer = new StreamWriter(csvFileName))
            {
                foreach (Vehicle vehicle in group)
                {
                    string line = $"{vehicle.Registration},{vehicle.Make},{vehicle.Model},{vehicle.FuelType}";
                    writer.WriteLine(line);
                }
            }
        }
    }

    private List<Vehicle> FindVehiclesWithValidRegistrations(List<Vehicle> vehicles)
    {
        List<Vehicle> validRegistrations = vehicles.Where(v => Regex.IsMatch(v.Registration, @"^[A-Z]{2}\d{2}\s[A-Z]{3}$")).ToList();
        return validRegistrations;
    }

    private int CountVehiclesWithoutValidRegistrations(List<Vehicle> vehicles)
    {
        int invalidRegistrationCount = vehicles.Count(v => !Regex.IsMatch(v.Registration, @"^[A-Z]{2}\d{2}\s[A-Z]{3}$"));
        return invalidRegistrationCount;
    }
}

public class Program
{
    
    public static void Main()
    {       
        string csvFilePath = Path.Combine(GetProjectDirectory(), "Path", "Technical Test Data.csv");
        VehicleDataProcessor dataProcessor = new VehicleDataProcessor();
        dataProcessor.ProcessVehicleData(csvFilePath);
    }
    private static string GetProjectDirectory()
    {
        var startDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        return startDirectory;
    }
}
