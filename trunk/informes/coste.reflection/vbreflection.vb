Imports System

Public Class VBA
    Public Function m(ByVal obj As Object, ByVal n As Long) As Object
        Return n * obj.GetHashCode()
    End Function

    Public Shared Function testReflection(ByVal implicitObject As VBA, ByVal iterations As Long) As Long
        Dim startTime, i As Long
        Dim obj
        obj = implicitObject

        startTime = DateTime.Now.Ticks

        For i = 0 To iterations - 1
            obj.m(implicitObject, i)
        Next i

        Return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond
    End Function


    Public Shared Function testExplicit(ByVal implicitObject As VBA, ByVal iterations As Long) As Long
        Dim startTime, i As Long
        startTime = DateTime.Now.Ticks

        For i = 0 To iterations - 1
            implicitObject.m(implicitObject, i)
        Next i

        Return (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerMillisecond
    End Function



    Public Shared Sub compareMethodInvocation()
        Dim obj As VBA
        Dim explicitTime, reflectionTime, iterations, i As Long


        obj = New VBA()
        Console.WriteLine("{0}, {1}, {2}, {3}, {4}", "iterations explicit", "explicit", "iterations reflection", "reflection", "times faster (explicit)")
        iterations = 10000
        For i = 1 To 10
            explicitTime = testExplicit(obj, iterations)
            reflectionTime = testReflection(obj, iterations)
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}", iterations, explicitTime, iterations, reflectionTime, reflectionTime / explicitTime - 1)
            iterations = iterations * 5
        Next i
    End Sub


End Class

Module myModule
    Sub Main()
        VBA.compareMethodInvocation()
    End Sub
End Module
