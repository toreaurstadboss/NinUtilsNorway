using FluentAssertions;
using NUnit.Framework;

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

}