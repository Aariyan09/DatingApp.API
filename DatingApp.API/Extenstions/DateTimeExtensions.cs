namespace DatingApp.API.Extenstions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOnly)
        {
            var today = DateTime.UtcNow;

            var age = today.Year - dateOnly.Year;

            /**** BDAY FOR THIS YEAR STILL PENDING ***/
            if (dateOnly > today.AddYears(-age)) age--;
            /**** BDAY FOR THIS YEAR STILL PENDING ***/

            return age;
        }
    }
}
