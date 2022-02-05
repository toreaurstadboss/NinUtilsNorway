using System;
using System.Linq;

namespace NinUtilsNorway
{

    /// <summary>
    /// Util methods for Nin (National identifier number) in Norway
    /// </summary>
    /// <remarks>
    /// Sample test persons can be retrieved from here which was helpful in building the util methods.
    /// <see href="https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/test-for-konsumenter/"/>
    /// </remarks>
    public static class NinUtilsNorway
    {

        /// <summary>
        /// Resolves gender from nin. Rule is that the first six digits are the birth date
        /// DDMMYYYY followed by 3 'individual digits' (individnummer) and finally two
        /// control digits (kontrollsiffer). The third digit of 'individual digits' are the 
        /// indicator for gender. If even number, female individual, if odd number, male individual.
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        /// <remarks>Documentation about Norwegian Nin structure is here<see href="https://www.skatteetaten.no/person/folkeregister/fodsel-og-navnevalg/barn-fodt-i-norge/fodselsnummer/"/></remarks>
        public static Gender GetGender(string nin)
        {
            nin = nin?.Trim();
            if (nin?.Length != 11)
            {
                return Gender.Unknown;
            }
            if (!long.TryParse(nin, out var _))
            {
                return Gender.Unknown;
            }
            bool isFemale = byte.Parse(nin[8].ToString()) % 2 == 0;
            return isFemale ? Gender.Female : Gender.Male;
        }

        /// <summary>
        /// Returns true if a person is having a D-number. A d-number is given to foreign workers in 
        /// Norway as a temporary identifier during their work period. It is similar to a ordinary Nin (fødselsnummer), but 
        /// for the first digit in the nin, we add 4. This gives 4,5,6,7 as possible digits for the first digits.
        /// A lot of other characteristics of D-number are similar to ordinary Nin, including the two control digits follow same rules.
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        public static bool IsDNumber(string nin)
        {
            nin = nin?.Trim(); 
            if (nin?.Length != 11)
            {
                return false; 
            }
            return new[] { '4', '5', '6', '7' }.Contains(nin[0]);
        }

        /// <summary>
        /// Calculates age from Nin
        /// </summary>
        /// <param name="nin"></param>
        /// <param name="nowTimeProvider">Provide an implementation to override now time. 
        /// Useful for mocking</param>
        /// <returns></returns>
        /// <remarks>About individual numbers - the 7-9 digits of Nin - and rules of centuries. 
        /// See explanation here: <see href="https://no.wikipedia.org/wiki/F%C3%B8dselsnummer" /></remarks>
        public static int? GetAge(string nin, IDateTimeNowProvider nowTimeProvider = null)
        {
            nin = nin?.Trim(); 
            if (nin?.Length != 11)
            {
                return null;
            }
            if (!long.TryParse(nin, out var _))
            {
                return null;
            }

            short individualNumber = short.Parse(nin.Substring(6, 3));
            if (IsDNumber(nin))
            {
                nin = (byte.Parse(nin[0].ToString()) - 4).ToString() + nin.Skip(1);
            }

            string dayBorn = GetLeftPaddedValue(2, '0', nin.Substring(0, 2));
            string monthBorn = GetLeftPaddedValue(2, '0', nin.Substring(2, 2));
            string twodigitYearBorn = GetLeftPaddedValue(2, '0', nin.Substring(4, 2));

            DateTime birthDate;

            DateTime today = nowTimeProvider?.GetToday() ?? DateTime.Today; 

            if (individualNumber >= 500 && individualNumber <= 749)
            {
                if (DateTime.TryParseExact($"{dayBorn}.{monthBorn}.18{twodigitYearBorn}", "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
                {
                    int age = GetAge(birthDate, DateTime.Today);
                    if (age < 125 && age >= 0)
                    {
                        return age;
                    }
                }
            }

            if ((individualNumber >= 0 && individualNumber <= 499) || (individualNumber >= 900 && individualNumber <= 999))
            {
                if (DateTime.TryParseExact($"{dayBorn}.{monthBorn}.19{twodigitYearBorn}", "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
                {
                    int age = GetAge(birthDate, DateTime.Today);
                    if (age < 125 && age >= 0)
                    {
                        return age;
                    }
                }
            }

            if (individualNumber >= 500 && individualNumber <= 999)
            {
                if (DateTime.TryParseExact($"{dayBorn}.{monthBorn}.20{twodigitYearBorn}", "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
                {
                    int age = GetAge(birthDate, DateTime.Today);
                    if (age < 125 && age >= 0)
                    {
                        return age; 
                    }
                }
            }

            return null; 
        }

        private static string GetLeftPaddedValue(int totalWidth, char padChar, string input)
        {
            return input.PadLeft(totalWidth, padChar).ToString();
        }

        private static int GetAge(DateTime from, DateTime to)
        {
            var age = to.Year - from.Year;
            if (from.Date > to.AddYears(-age))
            {
                age--;
            }
            return age;
        }





    }

}

