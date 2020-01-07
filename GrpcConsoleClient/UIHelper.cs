using PhoneBookServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcConsoleClient
{
    public static class UIHelper
    {
        public static SearchModel InputSearchParameters()
        {
            SearchModel searchModel = new SearchModel();
            searchModel.FirstName = StringInputSameLine("Enter part of the first name (Enter for empty)");
            searchModel.LastName = StringInputSameLine("Enter part of the last name (Enter for empty)");
            return searchModel;
        }

        public static ContactModel InputNewContact(bool inputPhone)
        {
            ContactModel newContact = new ContactModel();

            newContact.FirstName = StringInputSameLine("First name");
            newContact.LastName = StringInputSameLine("Last name");
            newContact.Country = StringInputSameLine("Country");
            newContact.City = StringInputSameLine("City");
            newContact.Zipcode = StringInputSameLine("Zipcode");
            newContact.Address = StringInputSameLine("Address");

            if (inputPhone)
            {
                while (true)
                {
                    Console.Write("Do you want to add new phone number? (Y/N)");
                    var newPhone = Console.ReadKey();
                    Console.WriteLine();
                    if (newPhone.KeyChar == 'Y' || newPhone.KeyChar == 'y')
                    {                        
                        newContact.PhoneNumbers.Add(InputPhoneNumber());
                    }
                    else break;
                }
                Console.WriteLine();
            }

            return newContact;
        }

        public static PhoneNumberModel InputPhoneNumber()
        {
            PhoneNumberModel phoneModel = new PhoneNumberModel();
            phoneModel.Number = StringInputSameLine("Phone number");
            phoneModel.PhoneType = InputPhoneType();
            return phoneModel;
        }

        public static PhoneType InputPhoneType()
        {
            while (true)
            {
                Console.Write("1 - HOME, 2 - MOBILE, 3 - WORK: ");
                var phoneType = Console.ReadKey();
                Console.WriteLine();
                if (phoneType.KeyChar == '1')
                {
                    return PhoneType.Home;
                }
                else if (phoneType.KeyChar == '2')
                {
                    return PhoneType.Mobile;

                }
                else if (phoneType.KeyChar == '3')
                {
                    return PhoneType.Work;
                }
            }
        }

        public static string StringInputSameLine(string inputMessage)
        {
            Console.Write($"{inputMessage}: ");
            return Console.ReadLine();
        }

        public static AddPhoneNumberRequest UIAddNewPhoneNumber()
        {
            AddPhoneNumberRequest request = new AddPhoneNumberRequest();
            Console.WriteLine();
            Console.Write("Contact ID: ");
            request.ContactID = EnterInteger();
            request.PhoneType = InputPhoneType();
            Console.Write("Enter phone number: ");
            request.Number = Console.ReadLine();
            return request;
        }

        public static int EnterInteger()
        {
            while (true)
            {
                string inputID = Console.ReadLine();
                int parsedInput;
                if (int.TryParse(inputID, out parsedInput))
                {
                    return parsedInput;
                }
            }
        }

        public static void PrintContact(ContactModel contact)
        {
            Console.WriteLine($"{contact.ContactID}");
            Console.WriteLine($"{contact.FirstName} {contact.LastName}");
            Console.WriteLine($"{contact.Country} {contact.City} {contact.Zipcode} {contact.Address}");
            Console.WriteLine("Phone numbers: ");
            foreach (var phone in contact.PhoneNumbers)
            {
                Console.WriteLine($"{phone.NumberID}: {phone.PhoneType} {phone.Number}");
            }
            Console.WriteLine("================================");
        }

        public static void PrintPhoneNumber(PhoneNumberModel phone)
        {
            Console.WriteLine($"Phone number ID: {phone.NumberID}");
            Console.WriteLine($"Phone number type: {phone.PhoneType}");
            Console.WriteLine($"Phone number: {phone.Number}");
        }
    }
}
