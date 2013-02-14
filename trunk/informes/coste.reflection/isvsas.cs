using System;
using System.Reflection;

class A {
    public object m(object obj, int n) {
        return n*obj.GetHashCode();
    }

    public static long testIs(object implicitObject, long iterations) {
        long startTime = DateTime.Now.Ticks;
        A a;

        for (long i = 0; i < iterations; i++)
            if (implicitObject is A)
                a=((A)implicitObject)/*.m(implicitObject, (int)i)*/;

        return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
    }

    public static long testAs(object implicitObject, long iterations) {
        long startTime = DateTime.Now.Ticks;

        for (long i = 0; i < iterations; i++) {
            A a = implicitObject as A;
            object obj;
            if (a != null)
                //a.m(implicitObject, (int)i); 
                obj = a;
        }

        return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
    }


    public static void compareIsAs() {
        Console.WriteLine("{0}; {1}; {2}; {3}", "iterations", "is", "as", "% faster (as)");
        A obj = new A();
        long isTime, asTime;
        for (long iterations = 5000000, i = 0; i < 10; i++) {
            isTime = testIs(obj, iterations);
            asTime = testAs(obj, iterations);
            Console.WriteLine("{0}; {1}; {2}; {3}", iterations, isTime, asTime, (isTime * 100.0 / asTime - 100));
            iterations = iterations * 5;
        }
    }

   public static void Main() {
        compareIsAs();
    }

}