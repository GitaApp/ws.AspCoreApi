using System;
using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Entities
{
    public class Container
    {
        public HeadData headData { get; set; }
        public LineData lineData { get; set; }
    }

    public class HeadData
    {
        public int headId { get; set; }
        [Required(ErrorMessage = "extHeadId is required")]
        public int extHeadId { get; set; }

        [Required(ErrorMessage = "extHeadId is required")]
        public string taskCode { get; set; }

        [Required(ErrorMessage = "carrierId is required")]
        public int carrierId { get; set; }

        [Required(ErrorMessage = "carrierName is required")]
        public string carrierName { get; set; }

        public string driverName { get; set; }
        public string plateNumber { get; set; }
        public string issueName { get; set; }
        public bool delete { get; set; }

    }

    public class LineData
    {
        [Required(ErrorMessage = "lineId is required")]
        public int lineId { get; set; }


        [Required(ErrorMessage = "extLineId is required")]
        public int extLineId { get; set; }

        [Required(ErrorMessage = "freightMandatorId is required")]
        public int freightMandatorId { get; set; }

        [Required(ErrorMessage = "freightMandatorName is required")]
        public string freightMandatorName { get; set; }

        public string freightMandatorPosNumber { get; set; }
        public int freightMandatorContactId { get; set; }
        public string freightMandatorContactName { get; set; }
        public string route { get; set; }
        public int containerTypeId { get; set; }
        public string containerNumber1 { get; set; }
        public string containerNumber2 { get; set; }
        public string setNumber { get; set; }
        public string storeNumber { get; set; }
        public string deliverNumber { get; set; }
        public string startTime { get; set; }
        public bool billable { get; set; }
        public string currency { get; set; }
       
        public List<Point> points { get; set; }
        public List<Good> goods { get; set; }
    }

    public class Point
    {
        public int id { get; set; }
        [Required(ErrorMessage = "extLineId is required")]
        public int extLineId { get; set; }
        [Required(ErrorMessage = "extPointId is required")]
        public int extPointId { get; set; }
        
        public int order { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string date { get; set; }
   
        public int statusCode { get; set; }
        public string comment { get; set; }
        public bool tracking { get; set; }
        public bool mustDocs { get; set; }
        public bool statusCheck { get; set; }
        public string reference { get; set; }
        
        public bool delete { get; set; }
    }



}

