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
        /// Nin are composed of two control digits at the end. We can calculate these digits. 
        /// Usage: pass in the first NINE digits of the Nin. The last two digits will then be calculated. 
        /// For given first nine digits of we calculate the control digits, last two digits of the nin
        //  Pass in the first nine digits. 11 - (the weighted sum modulo 11) is then returned for first control digit
        //  k1. And the second control digit 2 is similarly calculated, but include the first control digit also as a 
        //  self correcting mechanism.
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        public static string GetControlDigitsForNin(string nin)
        {
            nin = nin?.Trim();
            if (nin?.Length != 9)
            {
                return null;
            }

            nin += "00"; //adjust temporarily for D-number check

            if (!long.TryParse(nin, out var _))
            {
                return null;
            }

            if (IsDNumber(nin))
            {
                nin = (byte.Parse(nin[0].ToString()) - 4).ToString() + nin.Skip(1);
            }

            nin = nin.Substring(0, nin.Length - 2);

            byte d1 = byte.Parse(nin[0].ToString());
            byte d2 = byte.Parse(nin[1].ToString());
            byte m1 = byte.Parse(nin[2].ToString());
            byte m2 = byte.Parse(nin[3].ToString());
            byte y1 = byte.Parse(nin[4].ToString());
            byte y2 = byte.Parse(nin[5].ToString());
            byte i1 = byte.Parse(nin[6].ToString());
            byte i2 = byte.Parse(nin[7].ToString());
            byte i3 = byte.Parse(nin[8].ToString());

            //Formula is documented here: https://no.wikipedia.org/wiki/F%C3%B8dselsnummer
            int k1 = 11 - ((3 * d1 + 7 * d2 + 6 * m1 + 1 * m2 + 8 * y1 + 9 * y2 + 4 * i1 + 5 * i2 + 2 * i3) % 11);
            int k2 = 11 - ((5 * d1 + 4 * d2 + 3 * m1 + 2 * m2 + 7 * y1 + 6 * y2 + 5 * i1 + 4 * i2 + 3 * i3 + 2 * k1) % 11);
            if (k1 == 11)
            {
                k1 = 0; //must be clamped to single digit in these cases
            }
            if (k2 == 11)
            {
                k2 = 0; //must be clamped to single digit in these cases
            }
            if (k1 == 10 || k2 == 10)
            {
                return null; //illeagal to have 10 as control digit, must be single digit. 
            }
            return $"{k1}{k2}";
        }

        /// <summary>
        /// Calculates validity of Nin according to modulo 11 algorithm. 
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        /// <remarks><see href="http://www.fnrinfo.no/Teknisk/KontrollsifferSjekk.aspx"
        /// Example of a Modulo-11 algorithm mathematical basis is shown here: 
        /// <see href="http://www.pgrocer.net/Cis51/mod11.html"/>
        /// </remarks>
        public static bool IsValidNin(string nin)
        {
            nin = nin?.Trim();
            if (nin?.Length != 11)
            {
                return false;
            }

            if (!long.TryParse(nin, out var _))
            {
                return false;
            }

            if (IsDNumber(nin))
            {
                nin = (byte.Parse(nin[0].ToString()) - 4).ToString() + nin.Skip(1);
            }

            int k1 = 0, k2 = 0; //weighted sums be 
            int[] k1_weights = new int[] { 3, 7, 6, 1, 8, 9, 4, 5, 2, 1 };
            foreach (var item in nin.Select((digit, index) => (digit, index)))
            {
                if (item.index == 10)
                {
                    break; //only considering first 10 digits of nin
                }
                k1 += int.Parse(item.digit.ToString()) * k1_weights[item.index];
            }
            if (k1 % 11 != 0)
            {
                return false; //k1 must be divisible by 11!
            }
            int[] k2_weights = new int[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };
            foreach (var item in nin.Select((digit, index) => (digit, index)))
            {             
                k2 += int.Parse(item.digit.ToString()) * k2_weights[item.index];
            }
            if (k2 % 11 != 0)
            {
                return false;
            }

            return true; //k1 and k2 is now known to be both divisible with 11
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
                    int age = GetAge(birthDate, today);
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
                    int age = GetAge(birthDate, today);
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
                    int age = GetAge(birthDate, today);
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

