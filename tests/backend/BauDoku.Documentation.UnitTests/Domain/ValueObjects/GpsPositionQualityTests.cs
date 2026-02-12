using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class GpsPositionQualityTests
{
    [Fact]
    public void CalculateQualityGrade_AccuracyBelow1m_ShouldReturnA()
    {
        var position = new GpsPosition(48.0, 11.0, null, 0.5, "rtk");
        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.A);
    }

    [Fact]
    public void CalculateQualityGrade_AccuracyBetween1And5m_ShouldReturnB()
    {
        var position = new GpsPosition(48.0, 11.0, null, 3.5, "internal_gps");
        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.B);
    }

    [Fact]
    public void CalculateQualityGrade_AccuracyBetween5And30m_ShouldReturnC()
    {
        var position = new GpsPosition(48.0, 11.0, null, 15.0, "internal_gps");
        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.C);
    }

    [Fact]
    public void CalculateQualityGrade_AccuracyAbove30m_ShouldReturnD()
    {
        var position = new GpsPosition(48.0, 11.0, null, 50.0, "internal_gps");
        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.D);
    }

    [Fact]
    public void CalculateQualityGrade_GoodSecondaryFactors_ShouldUpgradeByOneLevel()
    {
        // Base: 4m → B (grade 1), bonus: HDOP<2 + sats>=8 + correction = 3, net +1 → A (grade 0)
        var position = new GpsPosition(48.0, 11.0, null, 4.0, "dgnss",
            correctionService: "SAPOS-EPS", satelliteCount: 10, hdop: 1.5);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.A);
    }

    [Fact]
    public void CalculateQualityGrade_BadSecondaryFactors_ShouldDowngradeByOneLevel()
    {
        // Base: 15m → C (grade 2), penalty: HDOP>5 + sats<4 = 2, net -2 clamped to -1 → D (grade 3)
        var position = new GpsPosition(48.0, 11.0, null, 15.0, "internal_gps",
            satelliteCount: 3, hdop: 6.0);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.D);
    }

    [Fact]
    public void CalculateQualityGrade_AdjustmentCappedAtPlusOne()
    {
        // Base: 0.5m → A (grade 0), penalty: HDOP>5 + sats<4 = 2, net -2 clamped to -1 → B (grade 1)
        var position = new GpsPosition(48.0, 11.0, null, 0.5, "rtk",
            satelliteCount: 3, hdop: 6.0);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.B);
    }

    [Fact]
    public void CalculateQualityGrade_AdjustmentCappedAtMinusOne()
    {
        // Base: 50m → D (grade 3), bonus: HDOP<2 + sats>=8 + correction = 3, net +3 clamped to +1 → C (grade 2)
        var position = new GpsPosition(48.0, 11.0, null, 50.0, "dgnss",
            correctionService: "SAPOS-EPS", satelliteCount: 12, hdop: 1.0);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.C);
    }

    [Fact]
    public void CalculateQualityGrade_NoSecondaryFactors_ShouldReturnBaseGrade()
    {
        var position = new GpsPosition(48.0, 11.0, null, 3.5, "internal_gps");
        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.B);
    }

    [Fact]
    public void CalculateQualityGrade_CannotGoBelowA()
    {
        // Base: A (grade 0), bonus: all three = net +1 clamped to +1 → still A (grade 0, clamped)
        var position = new GpsPosition(48.0, 11.0, null, 0.5, "rtk",
            correctionService: "SAPOS-HEPS", satelliteCount: 14, hdop: 0.8);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.A);
    }

    [Fact]
    public void CalculateQualityGrade_CannotGoBelowD()
    {
        // Base: D (grade 3), penalty: HDOP>5 + sats<4 = net -2 clamped to -1 → still D (grade 3, clamped to max 3)
        var position = new GpsPosition(48.0, 11.0, null, 50.0, "internal_gps",
            satelliteCount: 2, hdop: 8.0);

        position.CalculateQualityGrade().Should().Be(GpsQualityGrade.D);
    }
}
