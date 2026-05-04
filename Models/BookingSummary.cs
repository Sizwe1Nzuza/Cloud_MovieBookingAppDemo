namespace MovieBookingAppDemo.Models
{
    public class BookingSummary
    {

    //      ViewModels are used to combine and present data from multiple models in a single structure for display purposes
    //      Instead of exposing database entities or using multiple navigation properties in the view, a ViewModel allows us to shape 
    //      only the required data for a specific screen
    //      A ViewModel is not a database entity and does not represent a table --> it exists for presentation purposes (show what the user wants to see)
    //      Doesn't need primary key since it's not a database entity

    //      viewModel --> taking some info from other models and putting it into this model so that we can use it for the search filter 




        // Attribute does not have to match respective model names, it just needs to be descriptive enough so that you don't confuse it with the database names

        //Movie info
        public string MovieTitle { get; set; }
        public string MovieGenre { get; set; }


        //cinema info
        public string CinemaName { get; set; }
        public string CinemaLocation { get; set; }


        //Booking Info
        public DateTime BookingDate { get; set; }
        public int BookingId { get; set; }

    }

}
