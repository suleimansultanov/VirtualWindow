using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    [DataContract]
    public class PaymentExtraParamsDto
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }
        
        [DataMember(Name = "middleName")]
        public string MiddleName { get; set; }
        
        [DataMember(Name = "lastName")]
        public string LastName { get; set; }
        
        [DataMember(Name = "nick")]
        public string Nick { get; set; }
        
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        
        [DataMember(Name = "address")]
        public string Address { get; set; }
        
        [DataMember(Name = "comment")]
        public string Comment { get; set; }
        
        [DataMember(Name = "birthDate")]
        public string BirthDate { get; set; }
    }
}