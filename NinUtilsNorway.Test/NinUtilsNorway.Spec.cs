using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;

namespace NinUtilsNorway.Test;

public class NinUtilsNorwaySpec
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("01129955131", Gender.Male)]
    [TestCase("17912099997", Gender.Male)]
    [TestCase("29822099635", Gender.Female)]
    [TestCase("05840399895", Gender.Female)]
    [TestCase("12829499914", Gender.Male)]
    [TestCase("12905299938", Gender.Male)]
    [TestCase("21883649874", Gender.Female)]
    [TestCase("21929774873", Gender.Female)]
    [TestCase(null, Gender.Unknown)]
    public void GetGenderReturnsExpected(string nin, Gender expectedGender)
    {
        NinUtilsNorway.GetGender(nin).Should().Be(expectedGender);
    }

    [Test]
    [TestCase("57912075186", true)]
    [TestCase("69822075096", true)]
    [TestCase("57912075186", true)]
    [TestCase("45840375084", true)]
    [TestCase("21929774873", false)]
    public void GetIsDNumberReturnsExpected(string nin, bool expectedValue)
    {
        NinUtilsNorway.IsDNumber(nin).Should().Be(expectedValue);
    }

    /// <summary>
    /// These tests passed on 5th feb 2022 - we should adjust the expectedAge to make them pass later on. (mocking DateTime.Now is not fixed in this lib for tests)
    /// Generation of test nin can be done from here :
    /// <see href="https://norske-testdata.no/fnr/" />
    /// </summary>
    /// <param name="nin"></param>
    /// <param name="expectedAge"></param>
    /// <remarks>
    /// <see href="https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/test-for-konsumenter/"/>
    /// This web portal can also be used to compare - showing gender and age and birthdate.    /// 
    /// http://www.fnrinfo.no/Verktoy/Kontroll.aspx
    /// </remarks>
    [Test]
    [TestCase("27028334589", 38)]
    [TestCase("14025410751", 67)]
    [TestCase("09027418594", 47)]
    [TestCase("14116437741", 57)]
    [TestCase("05070164207", 20)]
    [TestCase("24061027242", 111)]
    [TestCase("30060107749", 120)]
    public void GetAgeReturnsExpected(string nin, int expectedAge)
    {
        NinUtilsNorway.GetAge(nin, new NowTimeProvider()).Should().Be(expectedAge); //note adjust these tests later on as they are valid on 05.02.2022 .. 
    }

    [Test]
    [TestCase("31129956715", true)]
    public void IsValidNinReturnsExpected(string nin, bool expected)
    {
        NinUtilsNorway.IsValidNin(nin).Should().Be(expected);
    }

    [Test]
    [TestCase("311299567", "15")]
    [TestCase("270283345", "89")]
    [TestCase("140254107", "51")]
    [TestCase("090274185", "94")]
    [TestCase("141164377", "41")]
    [TestCase("050701642", "07")]
    [TestCase("240610272", "42")]
    [TestCase("300601077", "49")]
    public void GetControlDigitsForNinReturnsExpected(string nin, string expected)
    {
        NinUtilsNorway.GetControlDigitsForNin(nin).Should().Be(expected);
    }

    [Test]
    [TestCase("04081629469", "04081916")]
    [TestCase("03061990452", "03062019")]
    [TestCase("54087619934", "14081976")]
    [TestCase("26027226891", "26021972")]
    [TestCase("99056326525", null)]
    public void GetBirthDateFromNin(string nin, string actual)
    {
        var dob = NinUtilsNorway.GetBirthDateFromNin(nin);
        var expectedDob = !string.IsNullOrWhiteSpace(actual) ? (DateTime?)DateTime.ParseExact(actual, "ddMMyyyy", CultureInfo.InvariantCulture) : null;
        if (actual == null)
        {
            Assert.IsNull(actual);
        }
        else
        {
            dob.Should().NotBeNull();
            // ReSharper disable once PossibleInvalidOperationException
            Math.Abs(Convert.ToInt32(dob?.Subtract(expectedDob!.Value).TotalHours)).Should().BeLessThan(1);
        }
    }

}

internal class NowTimeProvider : IDateTimeNowProvider
{
    public DateTime GetToday()
    {
        return DateTime.ParseExact("05.02.2022", "dd.MM.yyyy", null); //fix DateTime.Now to get predicatble test results.
    }
}
