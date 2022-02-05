﻿using System;
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




    }

}

