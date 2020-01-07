using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GrpcConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string phoneBookGrpcUrl = GetGrpcUrl();

            if (phoneBookGrpcUrl == null)
            {
                return;
            }

            PhoneBookClient client = new PhoneBookClient(phoneBookGrpcUrl);
            GreetClient greetClient = new GreetClient(phoneBookGrpcUrl);

            while (true)
            {
                Console.Clear();

                Console.WriteLine($"Phonebook gRPC server will be contacted on: {phoneBookGrpcUrl}");
                Console.WriteLine($"Greet received: {await greetClient.DoTheGreet("gRPC Developer")}");

                Console.WriteLine("1. List all contacts");
                Console.WriteLine("2. Search for contact");
                Console.WriteLine("3. Add new contact");
                Console.WriteLine("4. Add phone number to an existing contact");
                Console.WriteLine("5. Edit existing phone number");
                Console.WriteLine("6. Edit existing contact");
                Console.WriteLine("7. Delete phone number");
                Console.WriteLine("8. Delete contact");
                Console.WriteLine("9. EXIT");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadKey();
                Console.Clear();
                
                switch (choice.KeyChar)
                {
                    case '1':
                        await client.ShowAllContacts();
                        break;
                    case '2':
                        await client.SearchForContact();
                        break;
                    case '3':
                        await client.CreateNewContact();
                        break;
                    case '4':
                        await client.AddPhoneNumber();
                        break;
                    case '5':
                        await client.UpdatePhoneNumber();
                        break;
                    case '6':
                        await client.UpdateContact();
                        break;
                    case '7':
                        await client.DeletePhoneNumber();
                        break;
                    case '8':
                        await client.DeleteContact();
                        break;
                    case '9':
                        Console.WriteLine();
                        Console.WriteLine("bye bye");
                        await Task.Delay(2000);
                        return;
                    default:
                        break;
                }
                Console.ReadLine();
            }
        }

        static string GetGrpcUrl()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            try
            {
                return configuration.GetConnectionString("PhoneBookGrpcServer");
            }
            catch
            {
                Console.WriteLine("Could not find GRPC url in the settings");
                Console.WriteLine("Make sure you have property 'PhoneBookGrpcServer' under 'ConnectionStrings' section in appsettings.json");
                Console.ReadLine();
                return null;
            }
        }
    }
}
