using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        public DateTime BookingStartDate { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }        

        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required, MaxLength(7)]
        [Display(Name = "Registration Number")]
        public string VehicleRegNumber { get; set; }

        [Required]
        [Display(Name = "Job Category")]
        public JobCategoryOption JobCategory { get; set; }

        [MaxLength(500)]
        public string? Comments { get; set; }

        public enum JobCategoryOption
        {
            Warranty = 0,
            Breakdown = 1,

            [Display(Name = "Vehicle of Road")]
            VehicleOfRoad = 2
        }
    }
}
