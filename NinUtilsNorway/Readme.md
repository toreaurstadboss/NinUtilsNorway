## NinUtilsNorway


## Summary 
**Util methods for Nin (National identifier number) in Norway** 

Note : Nin standards will be replaced by PID standard in 2032. Nin will be kept, but new 
Nins handed out will follow PID standard. 

The following types of Nin Numbers exists, basically five types of which ordinary Nin and 
D-number are the most typical. They all consist of 11 digits of which the last two are control 
digits. (usually Modulo 11 algorithm is used): 

- Ordinary Nin (fødselsnummer) 
- D-number (temporary given to foreign workers, may span multiple years) 
- Help numbers H-numbers (tourists, infants, unconcious people, unidentified people, etc)
- FH help numbers FH-numbers (similar to H-numbers)
- DUF-number (given to asylum seekers by UDI)

About change of Nin into PID standard as of 2032 : https://www.skatteetaten.no/en/deling/opplysninger/folkeregisteropplysninger/pid/
Sample test persons can be retrieved from here which was helpful in building the util methods.
https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/test-for-konsumenter

See useful list of definitions here: 
https://www.ehelse.no/standardisering/standarder/identifikatorer-for-personer
And DUF-numbers (UDI) : 
https://www.udi.no/ord-og-begreper/duf-nummer/
Note - Gender calculation will not necessarily be possible after 2032, as you are not guaranteed that 
Nin will contain correct gender information when PID is introduced. People will keep their Nin as before, 
but the semantic of Gender where the ninth digit (last of the three digits of 'individual number' is even number means = FEMALE and odd number = MALE is halted after 2032.
Newborns and new Nin (PID) will be gender-less, i.e. you cannot read gender out of Nin handed out after 2032. 

####  NinUtilsNorway.NinUtilsNorway.GetGender(System.String)"
            <summary>
            Resolves gender from nin. Rule is that the first six digits are the birth date
            DDMMYYYY followed by 3 'individual digits' (individnummer) and finally two
            control digits (kontrollsiffer). The third digit of 'individual digits' are the 
            indicator for gender. If even number, female individual, if odd number, male individual.
            </summary>
            <param name="nin"></param>
            <returns></returns>
            <remarks>Documentation about Norwegian Nin structure is here<see href="https://www.skatteetaten.no/person/folkeregister/fodsel-og-navnevalg/barn-fodt-i-norge/fodselsnummer/"/></remarks>
        </member>
#### NinUtilsNorway.NinUtilsNorway.IsDufNumber(System.String,NinUtilsNorway.IDateTimeNowProvider)
            <summary>
            Checks if this is a DUF-number. These numbers are given by UDI 
            The check is only checking it is a number with 12 digits. The number must also 
            reside in UDI data systems, which this method do not check.
            </summary>
            <returns></returns>
            <remarks>Se notes from eHelse here: <see href="https://www.ehelse.no/standardisering/standarder/identifikatorer-for-personer#DUF-nummer"/></remarks>
        </member>
####  NinUtilsNorway.NinUtilsNorway.IsHelpNumber(System.String,System.Boolean)
            <summary>
            Checks if this is a help number H-number. The default convention for H-number is that it we add 
            the number 4 to the third digit 
            </summary>
            <param name="useEightNineConvention">Use special convention that if the first digit is 8 or 9, it signals 
            a Help Number. Note - this usually designates a FH-help number instead</param>
            <returns></returns>
        </member>
#### NinUtilsNorway.NinUtilsNorway.IsFHNumber(System.String)
            <summary>
            FH numbers are developed by KITH as a proposal established as a standard 18.01.2010. It is similar to Nin 
            fødselsnumre with 11 digits, and the first digit is 8 or 9. The numbers in position 2 - 9 are generated
            as random numbers. This standard conceals also gender, birthdate or which order the number is provided.
            The algorithms allows about 200 million numbers minus 17% of these due to incorrect control digits (last two digits). 
            Examples of people getting a FH-number are tourists, newborn (infants), unconcious people not identified, 
            unidentified people or similar reasons that a fødselsnummer Nin or D-Number is not available. 
            </summary>
            <param name="number"></param>
            <returns></returns>
        </member>
#### M:NinUtilsNorway.NinUtilsNorway.IsDNumber(System.String)
            <summary>
            Returns true if a person is having a D-number. A d-number is given to foreign workers in 
            Norway as a temporary identifier during their work period. It is similar to a ordinary Nin (fødselsnummer), but 
            for the first digit in the nin, we add 4. This gives 4,5,6,7 as possible digits for the first digits.
            A lot of other characteristics of D-number are similar to ordinary Nin, including the two control digits follow same rules.
            </summary>
            <param name="nin"></param>
            <returns></returns>
        </member>

#### NinUtilsNorway.NinUtilsNorway.GetAge(System.String,NinUtilsNorway.IDateTimeNowProvider)
            <summary>
            Calculates age from Nin
            </summary>
            <param name="nin"></param>
            <param name="nowTimeProvider">Provide an implementation to override now time. 
            Useful for mocking</param>
            <returns></returns>
            <remarks>About individual numbers - the 7-9 digits of Nin - and rules of centuries. 
            See explanation here: <see href="https://no.wikipedia.org/wiki/F%C3%B8dselsnummer" /></remarks>


####  NinUtilsNorway.NinutilsNorway.GetControlDigitsForNin(string nin)    
         <summary>
        /// Nin are composed of two control digits at the end. We can calculate these digits. 
        /// Usage: pass in the first NINE digits of the Nin. The last two digits will then be calculated. 
        /// For given first nine digits of we calculate the control digits, last two digits of the nin
        //  Pass in the first nine digits. 11 - (the weighted sum modulo 11) is then returned for first control digit
        //  k1. And the second control digit 2 is similarly calculated, but include the first control digit also as a 
        //  self correcting mechanism.
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        </summary>


#### NinUtilsNorway.NinUtilsNorway.IsValidNin(string nin)
        /// <summary>
        /// Calculates validity of Nin according to modulo 11 algorithm. 
        /// </summary>
        /// <param name="nin"></param>
        /// <returns></returns>
        /// <remarks><see href="http://www.fnrinfo.no/Teknisk/KontrollsifferSjekk.aspx"
        /// Example of a Modulo-11 algorithm mathematical basis is shown here: 
        /// <see href="http://www.pgrocer.net/Cis51/mod11.html"/>
        /// </remarks>
        /// </summary>


Finally, note about testing. See : 

https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/test-for-konsumenter/

For test data.

Also note that you can implement IDateTimeNowProvider to statically set "today date" for predicatable results while testing.  