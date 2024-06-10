namespace Task9.DTO
{
    public class PrescriptionRequestModel
    {
        public int IdDoctor { get; set; }
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<MedicamentViewModel> Medicaments { get; set; }
    }
}
