using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Test2.Data;
using Test2.DTO;
using Test2.Models;

namespace Test2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly SubscriptionDbContext _context;

        public SubscriptionsController(SubscriptionDbContext context)
        {
            _context = context;
        }

        // GET: api/Subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
          if (_context.Subscriptions == null)
          {
              return NotFound();
          }
            return await _context.Subscriptions.ToListAsync();
        }

        // GET: api/Subscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
            var client = _context.Clients
                .Include(c => c.Sales)
                    .ThenInclude(s => s.Subscription)
                .FirstOrDefault(c => c.IdClient == id);

            if(client == null)
            {
                return BadRequest($"Client with id {id} not found");
            }

            var clientRequestModel = new ClientRequestModel
            {
                IdClient = client.IdClient,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = client.Phone,
                Subscriptions = client.Sales.Select(s => new SubscriptionViewModel
                {
                    IdSubscription = s.Subscription.IdSubscription,
                    Name = s.Subscription.Name,
                    EndTime = s.Subscription.EndTime,
                    AmountPaid = s.Subscription.Payments.Sum(p => p.Value)
                }).ToList(),
                TotalPaidAmount = client.Sales.Sum(s => s.Subscription.Payments.Sum(p => p.Value))
            };

            return Ok(clientRequestModel);
        }

        // PUT: api/Subscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscription(int id, Subscription subscription)
        {
            if (id != subscription.IdSubscription)
            {
                return BadRequest();
            }

            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(id))
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

        // POST: api/Subscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subscription>> PostSubscription([FromBody] PaymentRequestModel request)
        {
            var client = await _context.Clients.FindAsync(request.IdClient);
            if (client == null)
            {
                return NotFound("Client doesnt exist");
            }

            var subscription = await _context.Subscriptions.FindAsync(request.IdSubscription);
            if (subscription == null)
            {
                return NotFound("Subscription doesnt exist");
            }

            if (subscription.EndTime < DateTime.Now)
            {
                return BadRequest("Subscription is not active");
            }

            var existingPayment = await _context.Payments
                .Where(p => p.IdClient == request.IdClient && p.IdSubscription == request.IdSubscription)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();

            if (existingPayment != null && existingPayment.Date > DateTime.Now.AddMonths(-subscription.RenewalPeriod))
            {
                return BadRequest("This subscription is already paid");
            }

            decimal expectedAmount = subscription.Money;

            var activeDiscounts = _context.Discounts
                .Where(d => d.IdClient == request.IdClient && d.DateFrom <= DateTime.Now && d.DateTo >= DateTime.Now)
                .ToList();

            if (activeDiscounts.Any())
            {
                int totalDiscount = activeDiscounts.Sum(d => d.Value);
                totalDiscount = totalDiscount > 50 ? 50 : totalDiscount;
                expectedAmount -= expectedAmount * totalDiscount / 100;
            }

            if (request.PaymentAmount != expectedAmount)
            {
                return BadRequest("Payment amount does not match the price of the subscription");
            }

            var newPayment = new Payment
            {
                Date = DateTime.Now,
                IdClient = request.IdClient,
                IdSubscription = request.IdSubscription,
                Value = request.PaymentAmount
            };

            _context.Payments.Add(newPayment);
            await _context.SaveChangesAsync();

            return Ok(newPayment.IdPayment);
        }

        // DELETE: api/Subscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            if (_context.Subscriptions == null)
            {
                return NotFound();
            }
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionExists(int id)
        {
            return (_context.Subscriptions?.Any(e => e.IdSubscription == id)).GetValueOrDefault();
        }
    }
}
