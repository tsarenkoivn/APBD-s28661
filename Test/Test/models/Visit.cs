using System;

namespace Task4.models
{
    public class Visit
    {
        // id of visists [Guid], datesofvisits [DateTime], AnimalId, Description, Price
        public Guid id { get; set; } = Guid.NewGuid();
        public DateTime dateofvisits { get; set; }
        public int animalId { get; set; }
        public string descriprtion { get; set; }
        public float price { get; set; }
    }
}
