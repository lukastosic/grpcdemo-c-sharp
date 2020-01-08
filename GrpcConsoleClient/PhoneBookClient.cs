using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PhoneBookServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GrpcConsoleClient
{
    /// <summary>
    /// Implementation of phonebook grpc client
    /// </summary>
    public class PhoneBookClient
    {
        /// <summary>
        /// Init channel and client
        /// </summary>
        /// <param name="serverUrl"></param>
        public PhoneBookClient(string serverUrl)
        {
            phoneBookChannel = GrpcChannel.ForAddress(serverUrl);
            phoneBookClient = new PhoneBook.PhoneBookClient(phoneBookChannel);
        }

        private GrpcChannel phoneBookChannel;
        private PhoneBook.PhoneBookClient phoneBookClient;

        /// <summary>
        /// Getting all contacts from rpc
        /// </summary>
        /// <returns></returns>
        internal async Task ShowAllContacts()
        {               
            try
            {
                // Execute rpc
                // Request is empty object so it is just created on the same line
                ContactsResponse contacts = await phoneBookClient.GetAllContactsAsync(new GetAllRequest());

                if (contacts != null && contacts.Contact != null && contacts.Contact.Count > 0)
                {
                    foreach (var contact in contacts.Contact)
                    {
                        UIHelper.PrintContact(contact);
                    }
                }
                else
                {
                    Console.WriteLine("There are no contacts");
                }
            }
            catch (RpcException rpcException)
            {
                Console.WriteLine("There was an error communicating with gRPC server");
                Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Search for contacts
        /// </summary>
        /// <returns>Show stream from gRPC server of found contacts</returns>
        internal async Task SearchForContact()
        {   
            // Prepare request object
            SearchRequest searchRequest = UIHelper.InputSearchParameters();

            // Prepare stream read
            using (var search = phoneBookClient.SearchContacts(searchRequest))
            {
                Console.WriteLine("Printing out stream of found contacts");

                try
                {
                    // Waiting for stream elements
                    // This loop will go on until server closes the stream
                    while (await search.ResponseStream.MoveNext())
                    {
                        var currentContact = search.ResponseStream.Current;
                        UIHelper.PrintContact(currentContact);
                    }
                }
                catch (RpcException rpcException)
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        internal async Task CreateNewContact()
        {
            ContactModel newContact = UIHelper.InputNewContact(true);
            Console.Write("Are you sure you want to push this data to gRPC server? (Y/N): ");
            var confirmation = Console.ReadKey();
            Console.WriteLine();
            if (confirmation.KeyChar == 'Y' || confirmation.KeyChar == 'y')
            {
                try
                {
                    var response = await phoneBookClient.CreateNewContactAsync(newContact);
                    Console.WriteLine("New contact is successfuly saved to the server.");
                    Console.WriteLine("Below you can see new contact information with assigned IDs from the server:");
                    UIHelper.PrintContact(response);
                }
                catch (RpcException rpcException)
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        internal async Task AddPhoneNumber()
        {
            var request = UIHelper.UIAddNewPhoneNumber();
            try
            {
                var response = await phoneBookClient.AddPhoneNumberAsync(request);
                Console.WriteLine("New contact is successfuly saved to the server.");
                Console.WriteLine("Below you can see contact with new phone number");
                UIHelper.PrintContact(response);
            }
            catch(RpcException rcpException)
            {
                if (rcpException.StatusCode == StatusCode.NotFound)
                {
                    Console.WriteLine($"Contact with ID={request.ContactID} was not found on the server.");
                }
                else
                {
                    Console.WriteLine("Exception executing RPC:");
                    Console.WriteLine(rcpException);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task DeleteContact()
        {
            DeleteContactRequest deleteRequest = new DeleteContactRequest();
            Console.WriteLine();
            Console.WriteLine("Enter contact ID: ");
            deleteRequest.ContactID = UIHelper.EnterInteger();

            try
            {
                Console.WriteLine($"Looking up contact with ID={deleteRequest.ContactID} on the server");
                ContactModel contact = await phoneBookClient.GetContactAsync(new GetContactRequest { ContactID = deleteRequest.ContactID });
                Console.WriteLine("Found contact:");
                UIHelper.PrintContact(contact);
                Console.Write("Are you sure you want to delete this contact? (Y/N): ");
                var confirmation = Console.ReadKey();
                Console.WriteLine();
                if (confirmation.KeyChar == 'Y' || confirmation.KeyChar == 'y')
                {
                    try
                    {
                        var response = await phoneBookClient.DeleteContactAsync(deleteRequest);
                        Console.WriteLine();
                        Console.WriteLine($"Server response: {response.Message}");
                    }
                    catch (RpcException rpcException)
                    {
                        Console.WriteLine("There was an error communicating with gRPC server");
                        Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (RpcException rpcException)
            {
                if (rpcException.StatusCode == StatusCode.NotFound)
                {
                    Console.WriteLine($"Could not find contact with ID={deleteRequest.ContactID}");
                }
                else
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task DeletePhoneNumber()
        {
            DeletePhoneNumberRequest deleteRequest = new DeletePhoneNumberRequest();
            Console.WriteLine();
            Console.WriteLine("Enter Phone Number ID: ");
            deleteRequest.NumberID = UIHelper.EnterInteger();

            try
            {
                Console.WriteLine($"Looking up phone number with ID={deleteRequest.NumberID} on the server");
                PhoneNumberModel phoneNumber = await phoneBookClient.GetPhoneNumberAsync(new GetPhoneNumberRequest { NumberID = deleteRequest.NumberID });
                Console.WriteLine("Found phone number:");
                UIHelper.PrintPhoneNumberMultiline(phoneNumber);
                Console.Write("Are you sure you want to delete this phone number? (Y/N): ");
                var confirmation = Console.ReadKey();
                Console.WriteLine();
                if (confirmation.KeyChar == 'Y' || confirmation.KeyChar == 'y')
                {
                    try
                    {
                        var response = await phoneBookClient.DeletePhoneNumberAsync(deleteRequest);
                        Console.WriteLine();
                        Console.WriteLine($"Number is deleted, this is contact details from the server: ");
                        UIHelper.PrintContact(response);
                    }
                    catch (RpcException rpcException)
                    {
                        Console.WriteLine("There was an error communicating with gRPC server");
                        Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (RpcException rpcException)
            {
                if (rpcException.StatusCode == StatusCode.NotFound)
                {
                    Console.WriteLine($"Could not find number with ID={deleteRequest.NumberID}");
                }
                else
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task UpdateContact()
        {            
            Console.WriteLine();
            Console.WriteLine("Enter contact ID: ");
            int contactID = UIHelper.EnterInteger();

            try
            {
                Console.WriteLine($"Looking up contact with ID={contactID} on the server");
                ContactModel contact = await phoneBookClient.GetContactAsync(new GetContactRequest { ContactID = contactID });
                Console.WriteLine("Found contact:");
                UIHelper.PrintContact(contact);
                Console.Write("Are you sure you want to edit this contact? (Y/N): ");
                var confirmation = Console.ReadKey();
                Console.WriteLine();
                if (confirmation.KeyChar == 'Y' || confirmation.KeyChar == 'y')
                {
                    var updateContact = UIHelper.InputNewContact(false);
                    updateContact.ContactID = contactID;

                    try
                    {
                        var response = await phoneBookClient.UpdateContactAsync(updateContact);
                        Console.WriteLine();
                        Console.WriteLine($"Updated contact: ");
                        UIHelper.PrintContact(response);
                    }
                    catch (RpcException rpcException)
                    {
                        Console.WriteLine("There was an error communicating with gRPC server");
                        Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (RpcException rpcException)
            {
                if (rpcException.StatusCode == StatusCode.NotFound)
                {
                    Console.WriteLine($"Could not find contact with ID={contactID}");
                }
                else
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task UpdatePhoneNumber()
        {
            DeletePhoneNumberRequest deleteRequest = new DeletePhoneNumberRequest();
            Console.WriteLine();
            Console.WriteLine("Enter Phone Number ID: ");
            int numberID = UIHelper.EnterInteger();

            try
            {
                Console.WriteLine($"Looking up phone number with ID={numberID} on the server");
                PhoneNumberModel phoneNumber = await phoneBookClient.GetPhoneNumberAsync(new GetPhoneNumberRequest { NumberID = numberID });
                Console.WriteLine("Found phone number:");
                UIHelper.PrintPhoneNumberMultiline(phoneNumber);
                Console.Write("Are you sure you want to delete this phone number? (Y/N): ");
                var confirmation = Console.ReadKey();
                Console.WriteLine();
                if (confirmation.KeyChar == 'Y' || confirmation.KeyChar == 'y')
                {
                    try
                    {
                        var phoneNumberRequest = UIHelper.InputPhoneNumber();
                        phoneNumberRequest.NumberID = numberID;
                        var response = await phoneBookClient.UpdatePhoneNumberAsync(phoneNumberRequest);
                        Console.WriteLine();
                        Console.WriteLine($"Number is updated, this is contact details from the server: ");
                        UIHelper.PrintContact(response);
                    }
                    catch (RpcException rpcException)
                    {
                        Console.WriteLine("There was an error communicating with gRPC server");
                        Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (RpcException rpcException)
            {
                if (rpcException.StatusCode == StatusCode.NotFound)
                {
                    Console.WriteLine($"Could not find number with ID={numberID}");
                }
                else
                {
                    Console.WriteLine("There was an error communicating with gRPC server");
                    Console.WriteLine($"Code: {rpcException.StatusCode}, Status: {rpcException.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

}
