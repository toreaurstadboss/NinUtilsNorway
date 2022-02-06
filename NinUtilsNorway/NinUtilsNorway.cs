using System;
using System.Linq;

namespace NinUtilsNorway
{

    /// <summary>
    /// Util methods for Nin (National identifier number) in Norway
    /// </summary>
    /// <remarks>
    /// About change of Nin into PID standard as of 2032 : <see href="https://www.skatteetaten.no/en/deling/opplysninger/folkeregisteropplysninger/pid/"/>
    /// Sample test persons can be retrieved from here which was helpful in building the util methods.
    /// <see href="https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/test-for-konsumenter/"/>
    /// </remarks>
    /// See useful list of definitions here: 
    /// <see href="https://www.ehelse.no/standardisering/standarder/identifikatorer-for-personer"/>
    /// And DUF-numbers (UDI) : 
    /// <see href="https://www.udi.no/ord-og-begreper/duf-nummer/"/>
    /// Note - Gender calculation will not necessarily be possible after 2032, as you are not guaranteed that 
    /// Nin will contain correct gender information when PID is introduced. People will keep their Nin as before, 
    /// but the semantic of Gender where the ninth digit (last of the three digits of 'individual number' is even number means = FEMALE and odd number = MALE is halted after 2032.
    /// Newborns and new Nin (PID) will be gender-less, i.e. you cannot read gender out of Nin handed out after 2032. 
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
        /// Checks if this is a DUF-number. These numbers are given by UDI 
        /// The check is only checking it is a number with 12 digits. The number must also 
        /// reside in UDI data systems, which this method do not check.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Se notes from eHelse here: <see href="https://www.ehelse.no/standardisering/standarder/identifikatorer-for-personer#DUF-nummer"/></remarks>
        public static bool IsDufNumber(string duf, IDateTimeNowProvider dateTimeNowProvider = null)
        {
            duf = duf?.Trim();
            if (duf?.Length != 12)
            {
                return false;
            }
            if (!long.TryParse(duf, out var _))
            {
                return false;
            }

            int nowYear = dateTimeNowProvider != null ? dateTimeNowProvider.GetToday().Year : DateTime.Today.Year;

            short year = short.Parse(duf.Substring(0, 4));
            if (year < 1900 || year > nowYear)
            {
                return false; //first four digits are the year the asylum seeker applied application for permant residence in Norway. Must be at least after 1900 and not in the future as a basic check. 
            }

            return true;
        }

        /// <summary>
        /// Checks if this is a help number H-number. The default convention for H-number is that it we add 
        /// the number 4 to the third digit 
        /// </summary>
        /// <param name="useEightNineConvention">Use special convention that if the first digit is 8 or 9, it signals 
        /// a Help Number. Note - this usually designates a FH-help number instead</param>
        /// <returns></returns>
        public static bool IsHelpNumber(string nin, bool useEightNineConventionFirstDigitConvention = false)
        {
            nin = nin?.Trim();
            if (nin?.Length != 11)
            {
                return false; //even H-numbers are 11 digits.
            }
            if (useEightNineConventionFirstDigitConvention)
            {
                //this convention rather belongs to FH-numbers (see method IsFHNumber in this class) 
                if (nin[0] == '8' || nin[0] == '9')
                {
                    return true; //H-numbers beginning with 8 or 9 signals a H-number in a simple matter. 
                }
            }

            if (!long.TryParse(nin, out var _))
            {
                return false;
            }

            return new[] { '4', '5', '6', '7' }.Contains(nin[2]); //H-numbers add 4 to the third digit
        }

        /// <summary>
        /// FH numbers are developed by KITH as a proposal established as a standard 18.01.2010. It is similar to Nin 
        /// fødselsnumre with 11 digits, and the first digit is 8 or 9. The numbers in position 2 - 9 are generated
        /// as random numbers. This standard conceals also gender, birthdate or which order the number is provided.
        /// The algorithms allows about 200 million numbers minus 17% of these due to incorrect control digits (last two digits). 
        /// Examples of people getting a FH-number are tourists, newborn (infants), unconcious people not identified, 
        /// unidentified people or similar reasons that a fødselsnummer Nin or D-Number is not available. 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsFHNumber(string nin)
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

            if (nin[0] == '8' || nin[0] == '9')
            {
                return true; //H-numbers beginning with 8 or 9 signals a H-number in a simple matter. 
            }

            return false;
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
            return new[] { '4', '5', '6', '7' }.Contains(nin[0]); //D-numbers add 3 to the first digit 
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

            if (IsHelpNumber(nin))
            {
                return null; //H-numbers does not follow the Modulo 11 algorithm.        
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

            if (IsHelpNumber(nin) || IsFHNumber(nin) || IsDufNumber(nin))
            {
                //calculating age from help numbers are troublesome and is avoided. 
                //albeit you could calculate approximately via H-number if only the first digit is e.g. '8' or '9' and 
                //other parts of the Nin got standard setup. Usually H-numbers are just random generated after first digit 
                //and contain no info about age or gender (maybe rudimentary info such as approximate age in DUF numbers via application year).
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

