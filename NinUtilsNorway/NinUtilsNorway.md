# Library NinUtilsNorway

## Namespaces

 - [NinUtilsNorway](##NinUtilsNorway) *(3)*

## NinUtilsNorway

### Gender

Gender enum : Unknown = 0 | Female = 1 | Male = 2

#### Fields

|Name|Summary|
|----|-------|
|Female|Female|
|Male|Male|
|Unknown|Unknown gender|
### IDateTimeNowProvider

Used for mocking date times

#### Methods

|Name|Summary|
|----|-------|
|GetToday()|Retrieves the Today value (DateTime.Today for example). Useful for mocking / unit testing.|
### NinUtilsNorway

Util methods for Nin (National identifier number) in Norway

#### Methods

|Name|Summary|
|----|-------|
|GetAge(System.String nin, NinUtilsNorway.IDateTimeNowProvider nowTimeProvider)|Calculates age from Nin|
|GetBirthDateFromNin(System.String nin)|Calculates birth date from nin based on rules from https://www.skatteetaten.no/person/folkeregister/fodsel-og-navnevalg/barn-fodt-i-norge/fodselsnummer/|
|GetControlDigitsForNin(System.String)||
|GetGender(System.String nin)|Resolves gender from nin. Rule is that the first six digits are the birth date DDMMYYYY followed by 3 'individual digits' (individnummer) and finally two control digits (kontrollsiffer). The third digit of 'individual digits' are the indicator for gender. If even number, female individual, if odd number, male individual.|
|IsDNumber(System.String nin)|Returns true if a person is having a D-number. A d-number is given to foreign workers in Norway as a temporary identifier during their work period. It is similar to a ordinary Nin (fødselsnummer), but for the first digit in the nin, we add 4. This gives 4,5,6,7 as possible digits for the first digits. A lot of other characteristics of D-number are similar to ordinary Nin, including the two control digits follow same rules.|
|IsDufNumber(System.String, NinUtilsNorway.IDateTimeNowProvider)|Checks if this is a DUF-number. These numbers are given by UDI The check is only checking it is a number with 12 digits. The number must also reside in UDI data systems, which this method do not check.|
|IsFHNumber(System.String nin)|FH numbers are developed by KITH as a proposal established as a standard 18.01.2010. It is similar to Nin fødselsnumre with 11 digits, and the first digit is 8 or 9. The numbers in position 2 - 9 are generated as random numbers. This standard conceals also gender, birthdate or which order the number is provided. The algorithms allows about 200 million numbers minus 17% of these due to incorrect control digits (last two digits). Examples of people getting a FH-number are tourists, newborn (infants), unconcious people not identified, unidentified people or similar reasons that a fødselsnummer Nin or D-Number is not available.|
|IsHelpNumber(System.String nin, System.Boolean useEightNineConventionFirstDigitConvention)|Checks if this is a help number H-number. The default convention for H-number is that it we add the number 4 to the third digit|
|IsValidNin(System.String nin)|Calculates validity of Nin according to modulo 11 algorithm.|
