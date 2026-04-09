using System.Globalization;

namespace Scrooge_app.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert( object value , Type targetType , object parameter , CultureInfo culture )
        {
            if(value is string colorString)
            {
                return colorString.ToLower() switch
                {
                    "green" => Colors.Green,
                    "red" => Colors.Red,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack( object value , Type targetType , object parameter , CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}