using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Data;
using WebAppDemo.Models;
using WebAppDemo.Services.EmailSender;
using X.PagedList;

namespace WebAppDemo.Controllers
{
    public class BookingsController : Controller
    {
        private readonly WebAppDemoContext _context;
        private readonly IMailService _mailService;

        public BookingsController(WebAppDemoContext context, IMailService _MailService)
        {
            _context = context;
            _mailService = _MailService;
        }

        // GET: Bookings
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.CitySortParm = sortOrder == "city_asc" ? "city_desc" : "city_asc";
            ViewBag.RegNumSortParm = sortOrder == "reg_asc" ? "reg_desc" : "reg_asc";
            ViewBag.PostcodeSortParm = sortOrder == "post_asc" ? "post_desc" : "post_asc";
            ViewBag.JobCatSortParm = sortOrder == "jobcat_asc" ? "jobcat_desc" : "jobcat_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var bookingsList = from b in _context.Booking
                               select b;
            if (!String.IsNullOrEmpty(searchString))
            {                
                bookingsList = bookingsList.Where(b => b.LastName.Contains(searchString)
                                       || b.FirstName.Contains(searchString)
                                       || b.Email.Contains(searchString)
                                       || b.AddressLine1.Contains(searchString)
                                       || b.City.Contains(searchString)
                                       || b.Postcode.Contains(searchString)
                                       || b.VehicleRegNumber.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.LastName);
                    break;
                case "name_asc":
                    bookingsList = bookingsList.OrderBy(b => b.LastName);
                    break;
                case "date_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.BookingStartDate);
                    break;
                case "date_asc":
                    bookingsList = bookingsList.OrderBy(b => b.BookingStartDate);
                    break;
                case "city_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.City);
                    break;
                case "city_asc":
                    bookingsList = bookingsList.OrderBy(b => b.City);
                    break;
                case "reg_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.VehicleRegNumber);
                    break;
                case "reg_asc":
                    bookingsList = bookingsList.OrderBy(b => b.VehicleRegNumber);
                    break;
                case "post_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.Postcode);
                    break;
                case "post_asc":
                    bookingsList = bookingsList.OrderBy(b => b.Postcode);
                    break;
                case "jobcat_desc":
                    bookingsList = bookingsList.OrderByDescending(b => b.JobCategory);
                    break;
                case "jobcat_asc":
                    bookingsList = bookingsList.OrderBy(b => b.JobCategory);
                    break;
                default:
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(bookingsList.ToPagedList(pageNumber, pageSize));            
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {            
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingStartDate,FirstName,LastName,Email,ContactNumber,AddressLine1,AddressLine2,City,Postcode,VehicleRegNumber,JobCategory,Comments")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();

                //send an email to the email address in the booking and show a confirmation
                MailRequest newMail = new MailRequest();
                newMail.Subject = "Form submitted";
                newMail.ToEmail = booking.Email;
                newMail.Body = _mailService.CreateEmailBody(booking);
                await _mailService.SendEmailAsync(newMail);

                TempData["bookingdetails"] = string.Format("Successful booking, you will receive a confirmation email shortly.", booking.BookingStartDate.ToString() );

                //create a json file containing the submitted data and save it to file system
                Helpers.Serialization.JsonSerialization.WriteToJsonFile(@"C:\newBookingjson.txt", booking);

                //create an xml file containing the submitted data and save it to file system
                Helpers.Serialization.XmlSerialization.WriteToXmlFile(@"C:\newBookingxml.txt", booking);

                //redirect to bookings view
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingStartDate,FirstName,LastName,Email,ContactNumber,AddressLine1,AddressLine2,City,Postcode,VehicleRegNumber,JobCategory,Comments")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'WebAppDemoContext.Booking'  is null.");
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Booking?.Any(e => e.Id == id)).GetValueOrDefault();
        }       
    }
}
