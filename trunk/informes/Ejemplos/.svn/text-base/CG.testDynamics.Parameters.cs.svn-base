using System;
using Figures;

namespace Testing.Dynamics
{
   class Parameters
   {
      static int testDynamic(var dynParameter)
      {
         // * dynParameter is a dynamic reference
         // dynParameter: Circle\/Rectangle

         //Console.WriteLine(dynParameter.getRadius());
         //Console.WriteLine(dynParameter.getX());
         //dynParameter.getRadius() * dynParameter.getWidth();

         //return (dynParameter.getRadius() * dynParameter.getWidth());
         return (dynParameter.getX() * dynParameter.getY());
      }

      static int testStatic(var staParameter)
      {
         // * staParameter is a static reference
         // staParameter: Circle\/Rectangle

         return (staParameter.getX() * staParameter.getY());
         //staParameter.getX() * staParameter.getY();
      }

      static var circleOrRectangle(bool condition)
      {
         if (condition)
            return new Circle(1, 2, 10);
         else
            return new Rectangle(3, 4, 20, 30);
      }

      static void Main()
      {
         Console.WriteLine(testStatic(circleOrRectangle(false)));
         Console.WriteLine(testDynamic(circleOrRectangle(true)));
         
         //testStatic(circleOrRectangle(false)); // Rectangle
         //testDynamic(circleOrRectangle(true)); // Circle
         
      }
   }
}

