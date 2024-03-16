using System;
using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Entities
{
	public class getShipmentDataWithTaskCode
    {
  

        [Required(ErrorMessage = "taskCode is required")]
        [StringLength(50, ErrorMessage = "taskCode must be at most 50 characters")]
        public string taskCode { get; set; }

   



}
}
