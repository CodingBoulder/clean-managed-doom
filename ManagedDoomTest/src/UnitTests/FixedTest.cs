using ManagedDoom;
using System;
using Xunit;

namespace ManagedDoomTest.UnitTests
{
    public class FixedTest
    {
        private static readonly double delta = 1.0E-3;

        [Fact]
        public void Conversion()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double da = 666 * random.NextDouble() - 333;
                float sa = (float)da;

                var fda = Fixed.FromDouble(da);
                var fsa = Fixed.FromFloat(sa);

                Assert.Equal(da, fda.ToDouble(), delta);
                Assert.Equal(sa, fsa.ToFloat(), delta);
            }
        }

        [Fact]
        public void Abs1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666) - 333;
                int b = Math.Abs(a);

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.Abs(fa);

                Assert.Equal(b, fb.ToDouble(), delta);
            }
        }

        [Fact]
        public void Abs2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 666 * random.NextDouble() - 333;
                double b = Math.Abs(a);

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.Abs(fa);

                Assert.Equal(b, fb.ToDouble(), delta);
            }
        }

        [Fact]
        public void Sign1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666) - 333;

                var fa = Fixed.FromDouble(a);

                Assert.Equal(+a, (+fa).ToDouble(), delta);
                Assert.Equal(-a, (-fa).ToDouble(), delta);
            }
        }

        [Fact]
        public void Sign2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 666 * random.NextDouble() - 333;

                var fa = Fixed.FromDouble(a);

                Assert.Equal(+a, (+fa).ToDouble(), delta);
                Assert.Equal(-a, (-fa).ToDouble(), delta);
            }
        }

        [Fact]
        public void Addition1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666) - 333;
                int b = random.Next(666) - 333;
                int c = a + b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                Fixed fc = fa + fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Addition2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 666 * random.NextDouble() - 333;
                double b = 666 * random.NextDouble() - 333;
                double c = a + b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                Fixed fc = fa + fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Subtraction1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666) - 333;
                int b = random.Next(666) - 333;
                int c = a - b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                Fixed fc = fa - fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Subtraction2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 666 * random.NextDouble() - 333;
                double b = 666 * random.NextDouble() - 333;
                double c = a - b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                Fixed fc = fa - fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Multiplication1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(66) - 33;
                int b = random.Next(66) - 33;
                int c = a * b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                Fixed fc = fa * fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Multiplication2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 66 * random.NextDouble() - 33;
                double b = 66 * random.NextDouble() - 33;
                double c = a * b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                Fixed fc = fa * fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Multiplication3()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(66) - 33;
                double b = 66 * random.NextDouble() - 33;
                double c = a * b;

                var fb = Fixed.FromDouble(b);
                Fixed fc = a * fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Multiplication4()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 66 * random.NextDouble() - 33;
                int b = random.Next(66) - 33;
                double c = a * b;

                var fa = Fixed.FromDouble(a);
                Fixed fc = fa * b;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Division1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(66) - 33;
                int b = (2 * random.Next(2) - 1) * (random.Next(33) + 33);
                double c = (double)a / b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                Fixed fc = fa / fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Division2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 66 * random.NextDouble() - 33;
                double b = (2 * random.Next(2) - 1) * (33 * random.NextDouble() + 33);
                double c = a / b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                Fixed fc = fa / fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Division3()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(66) - 33;
                double b = (2 * random.Next(2) - 1) * (33 * random.NextDouble() + 33);
                double c = a / b;

                var fb = Fixed.FromDouble(b);
                Fixed fc = a / fb;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Division4()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = 66 * random.NextDouble() - 33;
                int b = (2 * random.Next(2) - 1) * (random.Next(33) + 33);
                double c = a / b;

                var fa = Fixed.FromDouble(a);
                Fixed fc = fa / b;

                Assert.Equal(c, fc.ToDouble(), delta);
            }
        }

        [Fact]
        public void Bitshift()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666666) - 333333;
                int b = random.Next(16);
                int c = a << b;
                int d = a >> b;

                var fa = new Fixed(a);
                Fixed fc = fa << b;
                Fixed fd = fa >> b;

                Assert.Equal(c, fc.Data);
                Assert.Equal(d, fd.Data);
            }
        }

        [Fact]
        public void Comparison()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(5);
                int b = random.Next(5);

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);

                Assert.Equal(a == b, fa == fb);
                Assert.Equal(a != b, fa != fb);
                Assert.Equal(a < b, fa < fb);
                Assert.Equal(a > b, fa > fb);
                Assert.Equal(a <= b, fa <= fb);
                Assert.Equal(a >= b, fa >= fb);
            }
        }

        [Fact]
        public void MinMax()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(5);
                int b = random.Next(5);
                int min = Math.Min(a, b);
                int max = Math.Max(a, b);

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fmin = Fixed.Min(fa, fb);
                var fmax = Fixed.Max(fa, fb);

                Assert.Equal(min, fmin.ToDouble(), 1.0E-9);
                Assert.Equal(max, fmax.ToDouble(), 1.0E-9);
            }
        }

        [Fact]
        public void ToInt1()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                int a = random.Next(666);

                var fa = Fixed.FromDouble(a);
                int ffloor = fa.ToIntFloor();
                int fceiling = fa.ToIntCeiling();

                Assert.Equal(a, ffloor, 1.0E-9);
                Assert.Equal(a, fceiling, 1.0E-9);
            }
        }

        [Fact]
        public void ToInt2()
        {
            var random = new Random(666);
            for (int i = 0; i < 100; i++)
            {
                double a = (double)random.Next(666666) / 1000;
                double floor = Math.Floor(a);
                double ceiling = Math.Ceiling(a);

                var fa = Fixed.FromDouble(a);
                int ffloor = fa.ToIntFloor();
                int fceiling = fa.ToIntCeiling();

                Assert.Equal(floor, ffloor, 1.0E-9);
                Assert.Equal(ceiling, fceiling, 1.0E-9);
            }
        }
    }
}
