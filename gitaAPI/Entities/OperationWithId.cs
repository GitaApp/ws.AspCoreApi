using System;
using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Entities
{
	public class OperartionWithId
    {
  

        [Required(ErrorMessage = "id is required")]

        public int id { get; set; }

   



}
}
