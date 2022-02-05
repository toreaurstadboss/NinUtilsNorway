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
    public void GetGenderReturnsExpected(string nin, Gender expectedGender)
    {
        NinUtilsNorway.GetGender(nin).Should().Be(expectedGender);


    }

}