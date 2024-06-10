using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task9.Data;
using Task9.Models;
using Task9.DTO;
using System.Numerics;

namespace Task9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionDbContext _context;

        public PrescriptionsController(PrescriptionDbContext context)
        {
            _context = context;
        }

        // GET: api/Prescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions()
        {
            return await _context.Prescriptions.ToListAsync(); 
        }

        //for getting the patients
        // GET: api/Prescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pm => pm.Doctor)
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.Prescription_Medicaments)
                        .ThenInclude(pm => pm.Medicament)
                .FirstOrDefault(p => p.IdPatient == id);

            if (patient == null)
            {
                return BadRequest("Patient doesn't exist");
            }

            patient.Prescriptions = patient.Prescriptions.OrderBy(p => p.DueDate).ThenBy(p => p.Date).ToList();

            return Ok(patient);
        }

        // PUT: api/Prescriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescription(int id, Prescription prescription)
        {
            if (id != prescription.IdPrescription)
            {
                return BadRequest();
            }

            _context.Entry(prescription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrescriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Prescriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Prescription>> PostPrescription([FromBody] PrescriptionRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = _context.Patients.FirstOrDefault(p => p.IdPatient == request.IdPatient);
            if (patient == null)
            {
                patient = new Patient
                {
                    IdPatient = request.IdPatient,
                    LastName = request.LastName,
                    Birthdate = request.Birthdate,
                };

                _context.Patients.Add(patient);
            }

            if(request.Medicaments.Count > 10)
            {
                return BadRequest("A prescription cant include more than 10 medications");
            }
            if(request.DueDate < request.Date)
            {
                return BadRequest("Due date must be greater or equal to the date");
            }

            foreach( var medicament in request.Medicaments)
            {
                var existingMedication = _context.Medicaments.FirstOrDefault(p => p.IdMedicament == medicament.IdMedicament);
                if (medicament == null)
                {
                    return NotFound($"Medicament {medicament.IdMedicament} wasnt found");
                }
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.IdDoctor == request.IdDoctor);


            var prescription = new Prescription 
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdDoctor = request.IdDoctor,
                IdPatient = request.IdPatient,
                Patient = patient,
                Doctor = doctor,
                Prescription_Medicaments = request.Medicaments.Select(m => new Prescription_Medicament
                {
                    IdMedicament = m.IdMedicament,
                    Dose = m.Dose,
                    Details = m.Details
                }).ToList()
            };
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }

        // DELETE: api/Prescriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.IdPrescription == id);
        }
    }
}
