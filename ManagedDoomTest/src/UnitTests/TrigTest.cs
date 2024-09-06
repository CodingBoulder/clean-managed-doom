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
            for (var deg = 1; deg < 180; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * (deg + 90) / 360;
                var expected = Math.Tan(radian);

                {
                    var actual = Trig.Tan(angle).ToDouble();
                    var delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.Equal(expected, actual, delta);
                }

                {
                    var actual = Trig.Tan(fineAngle).ToDouble();
                    var delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.Equal(expected, actual, delta);
                }
            }
        }

        [Fact]
        public void Sin()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * deg / 360;
                var expected = Math.Sin(radian);

                {
                    var actual = Trig.Sin(angle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }

                {
                    var actual = Trig.Sin(fineAngle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }
            }
        }

        [Fact]
        public void Cos()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * deg / 360;
                var expected = Math.Cos(radian);

                {
                    var actual = Trig.Cos(angle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }

                {
                    var actual = Trig.Cos(fineAngle).ToDouble();
                    Assert.Equal(expected, actual, 1.0E-3);
                }
            }
        }
    }
}
