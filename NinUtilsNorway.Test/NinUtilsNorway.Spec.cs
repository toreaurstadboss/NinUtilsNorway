using FluentAssertions;
using NUnit.Framework;
using System;

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
  
}

internal class NowTimeProvider : IDateTimeNowProvider
{
    public DateTime GetToday()
    {
        return DateTime.ParseExact("05.02.2022", "dd.MM.yyyy", null); //fix DateTime.Now to get predicatble test results.
    }
}
