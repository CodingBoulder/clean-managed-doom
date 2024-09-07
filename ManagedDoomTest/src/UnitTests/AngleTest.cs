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
            for (int deg = -720; deg <= 720; deg++)
            {
                double expectedSin = Math.Sin(2 * Math.PI * deg / 360);
                double expectedCos = Math.Cos(2 * Math.PI * deg / 360);

                var angle = Angle.FromDegree(deg);
                double actualSin = Math.Sin(angle.ToRadian());
                double actualCos = Math.Cos(angle.ToRadian());

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
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(1440) - 720;
                int b = +a;
                int c = -a;

                var aa = Angle.FromDegree(a);
                Angle ab = +aa;
                Angle ac = -aa;

                {
                    double expectedSin = Math.Sin(2 * Math.PI * b / 360);
                    double expectedCos = Math.Cos(2 * Math.PI * b / 360);

                    double actualSin = Math.Sin(ab.ToRadian());
                    double actualCos = Math.Cos(ab.ToRadian());

                    Assert.Equal(expectedSin, actualSin, delta);
                    Assert.Equal(expectedCos, actualCos, delta);
                }

                {
                    double expectedSin = Math.Sin(2 * Math.PI * c / 360);
                    double expectedCos = Math.Cos(2 * Math.PI * c / 360);

                    double actualSin = Math.Sin(ac.ToRadian());
                    double actualCos = Math.Cos(ac.ToRadian());

                    Assert.Equal(expectedSin, actualSin, delta);
                    Assert.Equal(expectedCos, actualCos, delta);
                }
            }
        }

        [Fact]
        public void Abs()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(120) - 60;
                int b = Math.Abs(a);

                var aa = Angle.FromDegree(a);
                var ab = Angle.Abs(aa);

                double expectedSin = Math.Sin(2 * Math.PI * b / 360);
                double expectedCos = Math.Cos(2 * Math.PI * b / 360);

                double actualSin = Math.Sin(ab.ToRadian());
                double actualCos = Math.Cos(ab.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Addition()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(1440) - 720;
                int b = random.Next(1440) - 720;
                int c = a + b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                Angle fc = fa + fb;

                double expectedSin = Math.Sin(2 * Math.PI * c / 360);
                double expectedCos = Math.Cos(2 * Math.PI * c / 360);

                double actualSin = Math.Sin(fc.ToRadian());
                double actualCos = Math.Cos(fc.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Subtraction()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(1440) - 720;
                int b = random.Next(1440) - 720;
                int c = a - b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                Angle fc = fa - fb;

                double expectedSin = Math.Sin(2 * Math.PI * c / 360);
                double expectedCos = Math.Cos(2 * Math.PI * c / 360);

                double actualSin = Math.Sin(fc.ToRadian());
                double actualCos = Math.Cos(fc.ToRadian());

                Assert.Equal(expectedSin, actualSin, delta);
                Assert.Equal(expectedCos, actualCos, delta);
            }
        }

        [Fact]
        public void Multiplication1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                uint a = (uint)random.Next(30);
                uint b = (uint)random.Next(12);
                uint c = a * b;

                var fa = Angle.FromDegree(a);
                Angle fc = fa * b;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Multiplication2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                uint a = (uint)random.Next(30);
                uint b = (uint)random.Next(12);
                uint c = a * b;

                var fb = Angle.FromDegree(b);
                Angle fc = a * fb;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Division()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = (double)random.Next(360);
                uint b = (uint)(random.Next(30) + 1);
                double c = a / b;

                var fa = Angle.FromDegree(a);
                Angle fc = fa / b;

                Assert.Equal(c, fc.ToDegree(), delta);
            }
        }

        [Fact]
        public void Comparison()
        {
            var random = new Random(666);
            for (int i = 0; i < 10000; i++)
            {
                int a = random.Next(1140) - 720;
                int b = random.Next(1140) - 720;

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
