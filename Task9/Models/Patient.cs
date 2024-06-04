using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Task9.Models
{
    public class Patient
    {
        public int IdPatient { get; set; }
        public string LastName { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}