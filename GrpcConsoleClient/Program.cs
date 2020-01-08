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
            // Get url to the gRPC server from the appsettings.json file
            string phoneBookGrpcUrl = GetGrpcUrl();

            // If not found - exit from the application
            if (phoneBookGrpcUrl == null)
            {   
                Environment.Exit(404);
            }

            // Init client implementations of gRPC
            PhoneBookClient client = new PhoneBookClient(phoneBookGrpcUrl);
            GreetClient greetClient = new GreetClient(phoneBookGrpcUrl);
            BuddyGuyClient buddyGuyClient = new BuddyGuyClient(phoneBookGrpcUrl);



            while (true)
            {
                Console.Clear();


                await buddyGuyClient.StartBuddyGuy();


                Console.WriteLine($"Phonebook gRPC server will be contacted on: {phoneBookGrpcUrl}");
                Console.WriteLine($"\r\nGreet received: {await greetClient.DoTheGreet("gRPC Developer")}");
                Console.WriteLine("\r\nMake your choice");
                Console.WriteLine("\t1. List all contacts");
                Console.WriteLine("\t2. Search for contact");
                Console.WriteLine("\t3. Add new contact");
                Console.WriteLine("\t4. Add phone number to an existing contact");
                Console.WriteLine("\t5. Edit existing phone number");
                Console.WriteLine("\t6. Edit existing contact");
                Console.WriteLine("\t7. Delete phone number");
                Console.WriteLine("\t8. Delete contact");
                Console.WriteLine("\t9. EXIT");
                Console.Write("\r\nEnter your choice: ");
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
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Used to get connection string from appsettings.json
        /// Connection string should be located under the property named "PhoneBookGrpcServer" within "ConnectionStrings" section
        /// </summary>
        /// <returns>connection string</returns>
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
