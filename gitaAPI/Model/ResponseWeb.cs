using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace gitaAPI.Model
{
    public class ResponseWeb
    {


        public string? token { get; set; }
        public DateTime? expiration { get; set; }
    }
  
}




