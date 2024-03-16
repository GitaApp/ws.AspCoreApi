using System;
using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Entities
{
	public class shipmentData
	{
        [Required(ErrorMessage = "extHeadId is required")]
        public int extHeadId { get; set; }

        [Required(ErrorMessage = "carrierExtId is required")]
        public int carrierExtId { get; set; }


        [Required(ErrorMessage = "taskCode is required")]
        [StringLength(50, ErrorMessage = "taskCode must be at most 50 characters")]
        public string taskCode { get; set; }

        [Required(ErrorMessage = "carrierName is required")]
        [StringLength(255, ErrorMessage = "carrierName must be at most 255 characters")]
        public string carrierName { get; set; }

        [Required(ErrorMessage = "truckLicensPlateNumber is required")]
        [StringLength(50, ErrorMessage = "carrierName must be at most 50 characters")]
        public string truckLicensPlateNumber { get; set; }


       
        [StringLength(255, ErrorMessage = "note must be at most 255 characters")]
        public string note { get; set; }


        public List<lineData> lineData { get; set; }
    }

    public class lineData
    {


        public int serviceId { get; set; }



        [Required(ErrorMessage = "extLineId is required")]
        public int extLineId { get; set; }

        [Required(ErrorMessage = "extLineId is freightMandatorExtId")]
        public int freightMandatorExtId { get; set; }

        [Required(ErrorMessage = "upload date is required")]
        [RegularExpression(@"^\d{4}\.\d{2}\.\d{2}$", ErrorMessage = "Date must be in yyyy.MM.dd format")]
        public string uploadDate { get; set; }

        [Required(ErrorMessage = "uploadDate date is required")]
        [RegularExpression(@"^\d{4}\.\d{2}\.\d{2}$", ErrorMessage = "Date must be in yyyy.MM.dd format")]
        public string unloadDate { get; set; }
        

        [Required(ErrorMessage = "freightMandatorName is required")]
        [StringLength(255, ErrorMessage = "freightMandatorName must be at most 255 characters")]
        public string freightMandatorName { get; set; }

        [Required(ErrorMessage = "referenceId is required")]
        
        [StringLength(50, ErrorMessage = "refereceId must be at most 50 characters. Example : 0759742-01-01")]
        public string referenceId { get; set; }

        [StringLength(50, ErrorMessage = "refOrderNum must be at most 25 characters. Example : 2209619/3")]
        public string refOrderNum { get; set; }

     
        [StringLength(50, ErrorMessage = "orderNum must be at most 50 characters.")]
        public string orderNum { get; set; }

     
        public string loadingPlaceName { get; set; }


        public string loadingPlaceCountry { get; set; }


        public string loadingPlaceZip { get; set; }

        
        public string loadingPlaceCity { get; set; }

      
        public string loadingPlaceAddress { get; set; }

        [StringLength(255, ErrorMessage = "loadingPlaceComment must be at most 255 characters")]
        public string loadingPlaceComment { get; set; }

        [Required(ErrorMessage = "loadingSequence is required")]
        public int loadingSequence { get; set; }


        public string unloadingPlaceName { get; set; }

        public string unloadingPlaceCountry { get; set; }


     
        public string unloadingPlaceZip { get; set; }


        public string unloadingPlaceCity { get; set; }


        public string unloadingPlaceAddress { get; set; }


        public string unloadingPlaceComment { get; set; }

        [Required(ErrorMessage = "unloadingSequence is required")]
        public int unloadingSequence { get; set; }

        [Required(ErrorMessage = "transport_L_Sequence is required")]
        public int transport_L_Sequence { get; set; }

        [Required(ErrorMessage = "loadingTracking is required")]
        public bool loadingTracking { get; set; }

        [Required(ErrorMessage = "unLoadingTracking is required")]
        public bool unLoadingTracking { get; set; }
    }


}

