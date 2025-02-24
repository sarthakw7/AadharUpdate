using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AadharUpdateAPI.Models;

namespace AadharUpdateAPI.Services
{
    public class AadharService
    {
        private readonly string _xmlFilePath = "AadharDetails.xml";

        public List<AadharDetails> GetAll()
        {
            return ReadFromXml();
        }

        public AadharDetails GetById(int id)
        {
            return ReadFromXml().FirstOrDefault(x => x.Id == id);
        }

        public AadharDetails Add(AadharDetails aadhar)
        {
            var list = ReadFromXml();

            // Assign a new unique Id
            aadhar.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;

            list.Add(aadhar);
            WriteToXml(list);

            return aadhar;
        }

        public bool Update(int id, AadharDetails updatedAadhar)
        {
            var list = ReadFromXml();
            var existing = list.FirstOrDefault(x => x.Id == id);
            if (existing != null)
            {
                existing.Name = updatedAadhar.Name;
                existing.AadharNumber = updatedAadhar.AadharNumber;
                existing.DateOfBirth = updatedAadhar.DateOfBirth;
                existing.Address = updatedAadhar.Address;
                existing.MobileNumber = updatedAadhar.MobileNumber;
                existing.Email = updatedAadhar.Email;

                WriteToXml(list);
                return true;
            }
            return false;
        }

        public bool Delete(int id)
        {
            var list = ReadFromXml();
            var removed = list.RemoveAll(x => x.Id == id) > 0;
            if (removed)
            {
                WriteToXml(list);
            }
            return removed;
        }

        private List<AadharDetails> ReadFromXml()
        {
            if (!File.Exists(_xmlFilePath))
            {
                return new List<AadharDetails>();
            }

            var serializer = new XmlSerializer(typeof(List<AadharDetails>));
            using (var stream = new FileStream(_xmlFilePath, FileMode.Open))
            {
                return (List<AadharDetails>)serializer.Deserialize(stream);
            }
        }

        private void WriteToXml(List<AadharDetails> list)
        {
            var serializer = new XmlSerializer(typeof(List<AadharDetails>));
            using (var stream = new FileStream(_xmlFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, list);
            }
        }
    }
}
