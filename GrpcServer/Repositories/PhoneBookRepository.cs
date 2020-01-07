using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GrpcServer.Repositories
{
    public class PhoneBookRepository
    {
        private readonly ILogger<PhoneBookRepository> logger;

        public PhoneBookRepository(ILogger<PhoneBookRepository> logger)
        {
            this.logger = logger;
            logger.LogInformation("Generating 20 random contacts");
            InitContacts(20);
        }

        public List<ContactModel> Contacts { get; set; }

        private readonly Random rnd = new Random();

        #region Random seeds
        private List<string> FirstNamesSeed { get; } = new List<string>
            {
                "Kate", "Lara", "Lena", "Saskia", "Yasmin","Kathleen", "Mya", "Alexandra", "Connie", "Anthony", "Roman", "Adrian", "Zach", "Vincent", "Francis", "Bryan", "Sam", "Keaton", "Isaiah", "Victor"
            };

        private List<string> LastNamesSeed { get; } = new List<string>
            {
                "Roberts", "Hall", "Le", "Soto", "Murphy", "Pineda", "Haley", "Howe", "Molina", "Gilbert", "Johns", "Terry", "Lester", "Contreras", "Finley", "Douglas", "Reid", "Thornton", "McCann", "Valdez"
            };

        private List<string> AddressesSeed { get; } = new List<string>
            {
                "Beauchamp Rise", "Haywood Link", "Haddon Loke", "Bentleybridge Way", "Fitzroy Valley", "Brookside Terrace", "Gerddi'r Morfa", "Village Lanes", "Browns Village", "Precelly Crescent", "Fitzroy Dell", "Bellmeadow", "Bell Ridge", "Sandhurst Cliff", "Downing Piece", "Field Hills", "Pilgrims Ridings", "Beauchamp Trees", "Pembroke Garden", "Glenfield Walk"
            };

        private List<string> CitiesSeed { get; } = new List<string>
            {
                "Phurwoldgate", "Siormun With Leydamworth", "Malt Harbour", "Thampstreethbay", "St Sketel", "Tedulcaster", "Iogooncombe", "East Lakespockul", "Hotleek", "Lumbprai", "Niescoorte", "Saint Stroud", "Diecharl", "Park Noakslacrom", "Edgewelchester", "Port Lens", "West Cupa", "Red Xas", "New Thfrahope", "Grand Druth"
            };

        private List<string> CountriesSeed { get; } = new List<string>
            {
                "Siani", "Raqua Beibe", "Fasouthbiazam", "Frirungreecetire", "Eastern Mirymoaroclau"
            };

        #endregion

        /// <summary>
        /// Gives back next ID that should be used for the contact
        /// </summary>
        /// <returns>ID (as integer)</returns>
        public int NextContactID()
        {
            return this.Contacts.Max(c => c.ContactID) + 1;
        }

        /// <summary>
        /// Gives back next ID that should be used for the phone number
        /// </summary>
        /// <returns>ID (as integer)</returns>
        public int NextNumberID()
        {
            return this.Contacts.SelectMany(c => c.PhoneNumbers).Max(p => p.NumberID) + 1;
        }

        /// <summary>
        /// Add new contact (and its phone numbers if any) to the list.
        /// IDs will be auto assigned.
        /// </summary>
        /// <param name="contact">Contact model</param>
        /// <returns>Resulting contact wiht populated IDs</returns>
        public ContactModel AddContact(ContactModel contact)
        {
            contact.ContactID = NextContactID();
            var nextPhoneID = NextNumberID();
            foreach (var phone in contact.PhoneNumbers)
            {
                phone.NumberID = nextPhoneID;
                nextPhoneID++;
            }
            Contacts.Add(contact);
            return contact;
        }

        /// <summary>
        /// Find phone and corresponding contact based on the phone ID
        /// </summary>
        /// <param name="phoneID">Phone ID that you search for</param>
        /// <param name="outContact">Corresponding contact of that phone</param>
        /// <param name="outPhone">Found phone</param>
        public void FindContactAndPhoneNumber(int phoneID, out ContactModel outContact, out PhoneNumberModel outPhone)
        {
            outContact = null;
            outPhone = null;

            foreach (var contact in this.Contacts)
            {
                bool found = false;
                foreach (var phone in contact.PhoneNumbers)
                {
                    if (phone.NumberID == phoneID)
                    {
                        outPhone = phone;
                        outContact = contact;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
        }

        /// <summary>
        /// Find contact based on the ID
        /// </summary>
        /// <param name="contactID">ID of the contact</param>
        /// <returns>Found contact model (or null if not found)</returns>
        public ContactModel FindContact(int contactID)
        {
            return this.Contacts.FirstOrDefault(contact => contact.ContactID == contactID);
        }

        /// <summary>
        /// Find phone based on the ID
        /// </summary>
        /// <param name="phoneNumberID">ID of the phone</param>
        /// <returns>Found phone model (or null if not found)</returns>
        public PhoneNumberModel FindPhoneNumber(int phoneNumberID)
        {
            return this.Contacts
                .SelectMany(contact => contact.PhoneNumbers)
                .FirstOrDefault(phone => phone.NumberID == phoneNumberID);
        }

        /// <summary>
        /// Generate initial list of contacts with random data
        /// </summary>
        /// <param name="numberOfContacts">Number of contacts in the list</param>
        private void InitContacts(int numberOfContacts)
        {
            Contacts = new List<ContactModel>();
            Random rnd = new Random();
            int contactID = 1;
            int phoneID = 1;
            for (int i = 0; i < numberOfContacts; i++)
            {
                ContactModel contact = RandomContact(contactID);
                int phones = rnd.Next(1, 4);
                for (int j = 0; j < phones; j++)
                {
                    contact.PhoneNumbers.Add(RandomPhoneNumber(phoneID));
                    phoneID++;
                }
                Contacts.Add(contact);
                contactID++;
            }
        }

        /// <summary>
        /// Generate random contact
        /// </summary>
        /// <param name="id">ID that will be applied to the contact</param>
        /// <returns>Contact model</returns>
        private ContactModel RandomContact(int id)
        {
            return new ContactModel()
            {
                FirstName = this.FirstNamesSeed[rnd.Next(0, 20)],
                LastName = this.LastNamesSeed[rnd.Next(0, 20)],
                Address = $"{this.AddressesSeed[rnd.Next(0, 20)]} {rnd.Next(1, 100)}",
                Zipcode = rnd.Next(10000, 100000).ToString(),
                City = this.CitiesSeed[rnd.Next(0, 20)],
                Country = this.CountriesSeed[rnd.Next(0, 5)],
                ContactID = id
            };
        }

        /// <summary>
        /// Generate random phone number
        /// </summary>
        /// <param name="id">ID of that will be applied to the phone number</param>
        /// <returns>Phone number model</returns>
        private PhoneNumberModel RandomPhoneNumber(int id)
        {
            return new PhoneNumberModel()
            {
                Number = rnd.Next(6000000, 9999999).ToString(),
                PhoneType = (PhoneType)(rnd.Next(0, 3)),
                NumberID = id
            };
        }
    }
}
