using ManagedDoom;
using System;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class AngleTest
    {
        private static readonly double delta = 1.0E-3;

        [Fact]
        public void ToRadian()
        {
            Assert.Equal(0.00 * Math.PI, Angle.Ang0.ToRadian(), delta);
            Assert.Equal(0.25 * Math.PI, Angle.Ang45.ToRadian(), delta);
            Assert.Equal(0.50 * Math.PI, Angle.Ang90.ToRadian(), delta);
            Assert.Equal(1.00 * Math.PI, Angle.Ang180.ToRadian(), delta);
            Assert.Equal(1.50 * Math.PI, Angle.Ang270.ToRadian(), delta);
        }

        [Fact]
        public void FromDegrees()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var expectedSin = Math.Sin(2 * Math.PI * deg / 360);
                var expectedCos = Math.Cos(2 * Math.PI * deg / 360);

                var angle = Angle.FromDegree(deg);
                var actualSin = Math.Sin(angle.ToRadian());
                var actualCos = Math.Cos(angle.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void FromRadianToDegrees()
        {
            Assert.Equal(0, Angle.FromRadian(0.00 * Math.PI).ToDegree(), delta);
            Assert.Equal(45, Angle.FromRadian(0.25 * Math.PI).ToDegree(), delta);
            Assert.Equal(90, Angle.FromRadian(0.50 * Math.PI).ToDegree(), delta);
            Assert.Equal(180, Angle.FromRadian(1.00 * Math.PI).ToDegree(), delta);
            Assert.Equal(270, Angle.FromRadian(1.50 * Math.PI).ToDegree(), delta);
        }

        [Fact]
        public void Sign()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = +a;
                var c = -a;

                var aa = Angle.FromDegree(a);
                var ab = +aa;
                var ac = -aa;

                {
                    var expectedSin = Math.Sin(2 * Math.PI * b / 360);
                    var expectedCos = Math.Cos(2 * Math.PI * b / 360);

                    var actualSin = Math.Sin(ab.ToRadian());
                    var actualCos = Math.Cos(ab.ToRadian());

                    Assert.Equal(expectedSin, actualSin, delta);
                    Assert.Equal(expectedCos, actualCos, delta);
                }

                {
                    var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                    var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                    var actualSin = Math.Sin(ac.ToRadian());
                    var actualCos = Math.Cos(ac.ToRadian());

                    Assert.Equal(expectedSin, actualSin, delta);
                    Assert.Equal(expectedCos, actualCos, delta);
                }
            }
        }

        [Fact]
        public void Abs()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(120) - 60;
                var b = Math.Abs(a);

                var aa = Angle.FromDegree(a);
                var ab = Angle.Abs(aa);

                var expectedSin = Math.Sin(2 * Math.PI * b / 360);
                var expectedCos = Math.Cos(2 * Math.PI * b / 360);

                var actualSin = Math.Sin(ab.ToRadian());
                var actualCos = Math.Cos(ab.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Addition()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = random.Next(1440) - 720;
                var c = a + b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                var fc = fa + fb;

                var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                var actualSin = Math.Sin(fc.ToRadian());
                var actualCos = Math.Cos(fc.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Subtraction()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = random.Next(1440) - 720;
                var c = a - b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                var fc = fa - fb;

                var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                var actualSin = Math.Sin(fc.ToRadian());
                var actualCos = Math.Cos(fc.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Multiplication1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (uint)random.Next(30);
                var b = (uint)random.Next(12);
                var c = a * b;

                var fa = Angle.FromDegree(a);
                var fc = fa * b;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Multiplication2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (uint)random.Next(30);
                var b = (uint)random.Next(12);
                var c = a * b;

                var fb = Angle.FromDegree(b);
                var fc = a * fb;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Division()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (double)random.Next(360);
                var b = (uint)(random.Next(30) + 1);
                var c = a / b;

                var fa = Angle.FromDegree(a);
                var fc = fa / b;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Comparison()
        {
            var random = new Random(666);
            for (var i = 0; i < 10000; i++)
            {
                var a = random.Next(1140) - 720;
                var b = random.Next(1140) - 720;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);

                a = ((a % 360) + 360) % 360;
                b = ((b % 360) + 360) % 360;

                Assert.True((a == b) == (fa == fb));
                Assert.True((a != b) == (fa != fb));
                Assert.True((a < b) == (fa < fb));
                Assert.True((a > b) == (fa > fb));
                Assert.True((a <= b) == (fa <= fb));
                Assert.True((a >= b) == (fa >= fb));
            }
        }
    }
}
