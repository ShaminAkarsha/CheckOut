namespace HandyCraftsAdapterWebAPI.Models
{
    public class HandyCrafts
    {
        public string name { get; set; }                
        public string code { get; set; }                
        public decimal price { get; set; }              
        public string description { get; set; }        
        public string material { get; set; }            
        public string category { get; set; }            
        public string size { get; set; }                
        public string color { get; set; }              
        public string coverImage { get; set; }     
        public List<string> galleryImages { get; set; } 
    }
}
