using System;
using Figures;

namespace TestVar.StaticUnionTypes
{

   class A { public char ma() { return 'A'; } }
   class B : A { void mb() { } }
   class C : A { void mc() { } }

   class Class
   {
      var attribute;

      Class() { }

      Class(var parameter)
      {
         this.attribute = parameter;
      }

      void setAttribute(var a) { attribute = a; }
      var getAttribute() { return attribute; }

      static var m(var param)
      {
         if (3 > 10) return 10;
         return param;
      }

      var m1(var param)
      {
         if (3 > 10) return 10;
         return param;
      }

      static var m(var param1, var param2)
      {
         if (3 > 10) return param1;
         return param2;
      }

      public static void testStaticUnionArrays()
      {
         // * Array(int) \/ Array(double)  unifies to  Array(int \/ double) 
         var[] genericArray = m(new int[10], new double[10]);
         // * int \/ double
         //Console.WriteLine(genericArray.GetType());
         genericArray[0] = 64.9;
         double d = genericArray[0];
         //Console.WriteLine(genericArray[0]);
         if (d != 64.9)
            Environment.Exit(-1);
         
      }

      public static void testArithmetic()
      {
         // * int \/ char + int \/ double promotes to double
         double d = m(3, '3') + m(1, 3.3); // '3'->51
         if (d != 54.3)
            Environment.Exit(-1);

         // * string + int \/ string + string \/ char + bool \/ string + string\/ double promotes to string
         string s = " " + m(3, "3") + m("3", '4') + m(true, "2.2") + m("true", 6.6);
         if (!(s.Equals(" 342.26,6")))
            Environment.Exit(-1);

         bool b;
         // * int \/ char >= int \/ double is a boolean
         b = m(3, '3') <= m(1, 3.3);
         if (b)
            Environment.Exit(-1);

         // * bool \/ bool &&  bool \/ bool is a boolean
         b = m(true, false) && m(false, true);
         if (b)
            Environment.Exit(-1);
      }

      public static void Main()
      {
         Class.testStaticUnionArrays();
         Class.testArithmetic();
      }
   }
}

