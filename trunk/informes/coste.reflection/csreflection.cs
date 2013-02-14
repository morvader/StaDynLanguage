using System;
using System.Reflection;

class A {
    public object m(object obj, int n) {
        return n * obj.GetHashCode();
    }

    public static long testReflection(A implicitObject, long iterations) {
        long startTime = DateTime.Now.Ticks;

        for (long i = 0; i < iterations; i++) {
            implicitObject.GetType().GetMethod("m").Invoke(implicitObject, new object[] { implicitObject, (int)i });
        }

        return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
    }

    public static long testExplicit(A implicitObject, long iterations) {
        long startTime = DateTime.Now.Ticks;

        for (long i = 0; i < iterations; i++) {
            implicitObject.m(implicitObject, (int)i);
        }

        return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond;
    }


    public static void compareMethodInvocation() {
        Console.WriteLine("{0}; {1}; {2}; {3}; {4}", "iterations", "cs.explicit", "cs.reflection", "vb.explicit", "vb.reflection");
        A obj = new A();
        VBA vbObj = new VBA();
        long csExplicitTime, csReflectionTime, vbExplicitTime, vbReflectionTime;
        for (long iterations = 100000, i = 0; i < 10; i++) {
            csExplicitTime = testExplicit(obj, iterations);
            csReflectionTime = testReflection(obj, iterations);
            vbExplicitTime = VBA.testExplicit(vbObj, iterations);
            vbReflectionTime = VBA.testReflection(vbObj, iterations);
            Console.WriteLine("{0}; {1}; {2}; {3}; {4}", iterations, csExplicitTime, csReflectionTime, vbExplicitTime, vbReflectionTime);
            iterations = iterations * 2;
        }
    }

    public static void Main() {
        compareMethodInvocation();
    }

}


/*
Milisegundos que cuesta de más llamar por reflexión 0,00701625
Milisegundos que lleva hacer un "as" 0,0000078496

El número de "as" que se pueden hacer hasta que se obtenga el 
tipo correcto y merezca la pena es reflexion/as-1: 893
*/