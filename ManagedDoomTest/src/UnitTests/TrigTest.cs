using ManagedDoom;
using System;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class TrigTest
    {
        [Fact]
        public void Tan()
        {
            for (int deg = 1; deg < 180; deg++)
            {
                var angle = Angle.FromDegree(deg);
                int fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                double radian = 2 * Math.PI * (deg + 90) / 360;
                double expected = Math.Tan(radian);

                {
                    double actual = Trig.Tan(angle).ToDouble();
                    double delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.Equal(expected, actual, delta);
                }

                {
                    double actual = Trig.Tan(fineAngle).ToDouble();
                    double delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.Equal(expected, actual, delta);
                }
            }
        }

        [Fact]
        public void Sin()
        {
            for (int deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                int fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                double radian = 2 * Math.PI * deg / 360;
                double expected = Math.Sin(radian);

                {
                    double actual = Trig.Sin(angle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }

                {
                    double actual = Trig.Sin(fineAngle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }
            }
        }

        [Fact]
        public void Cos()
        {
            for (int deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                int fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                double radian = 2 * Math.PI * deg / 360;
                double expected = Math.Cos(radian);

                {
                    double actual = Trig.Cos(angle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }

                {
                    double actual = Trig.Cos(fineAngle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }
            }
        }
    }
}
