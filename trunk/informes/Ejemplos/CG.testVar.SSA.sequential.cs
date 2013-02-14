using System;
using Figures;

namespace TestVar.SSA
{
   class Test
   {
      var attribute;

      public void setAttribute(var p)
      {
         this.attribute = p;
      }

      public var getAttribute()
      {
         return this.attribute;
      }

      /*********** Sequential ***************/

      public static void testSequential()
      {
         var reference;

         reference = 3;
         double d = reference;

         if (d != 3)
            Environment.Exit(-1);

         reference = 3.3;
         d = reference;

         if (d != 3.3)
            Environment.Exit(-1);

         reference = "3";
         string s = reference;

         if (!(s.Equals("3")))
            Environment.Exit(-1);

         reference = new int[10];
         d = reference[3];

         if (d != 0)
            Environment.Exit(-1);
      }

      public void sequentialAttributes()
      {
         attribute = 3;

         //int a = this.attribute % 3;
         //if (a != 0)
         //   Environment.Exit(-1);

         //setAttribute(3.3);
         //double d = getAttribute() * 3;
         //if (d != 9.9)
         //   Environment.Exit(-1);

         //setAttribute("3");
         //a = this.attribute.Length;
         //if (a != 1)
         // Environment.Exit(-1);

         //attribute = new int[10];
         //this.attribute[3];
      }

      public static void testSequentialAttributes()
      {
         //Test test = new Test();
         //test.sequentialAttributes();
         //int n = test.attribute[0];
      }

      public static void Main()
      {
         Console.Write("{0}", 2);
         Console.WriteLine();
         Test.testSequential();
         //Test.testSequentialAttributes();
         new Test().sequentialAttributes();
      }
   }
}

