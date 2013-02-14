using System;

namespace TestVar.Cast
{
   class Test
   {
      private var attribute;

      public void setAttribute(var attribute)
      {
         this.attribute = attribute;
      }

      var castToint()
      {
         int n = (int)this.attribute;
         return n;
      }

      static int testImplicit()
      {
         int n;
         Test test = new Test();

         test.setAttribute('a');
         n = test.castToint();

         return n;
      }

      public static void Main()
      {
         if (Test.testImplicit() != 45)
            Console.WriteLine(Test.testImplicit());
      }
   }
}

