using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vittoria.Helpers
{
    class PlanColorCoverter : IValueConverter
    {
        private static SolidColorBrush Normal = new SolidColorBrush(Colors.White);
        private static SolidColorBrush NotVisited = new SolidColorBrush(Colors.LightGray);
        private static SolidColorBrush Errord = new SolidColorBrush(Colors.LightCoral);
        private static SolidColorBrush Success = new SolidColorBrush(Colors.LightGreen);
        private static SolidColorBrush FailedGoalWorkflow = new SolidColorBrush(Colors.LightSkyBlue);

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            var ar = (byte) value;

            switch (ar)
            {
                case 0:
                    return NotVisited;
                case 2:
                    return Errord;
                case 3:
                    return Success;
                case 4:
                    return FailedGoalWorkflow;
            }

            return Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
