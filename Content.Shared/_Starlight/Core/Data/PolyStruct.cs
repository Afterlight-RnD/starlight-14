// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

//namespace Content.Shared._Starlight.Core.Data;

namespace Content.Shared.Starlight.Abstract
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public sealed class PolyStructAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class PolyStructInnerAttribute: System.Attribute
    {
    }
}


public interface IPolyStruct<T>
{
}

public record struct PolyStruct<T>(T Value) :
    IPolyStruct<T>
{
    public static PolyStruct<T> Chain(T value) => new(value);
}

public record struct PolyStruct<TSelf,T>(T Next,TSelf First) : IPolyStruct<TSelf>
{
    public static PolyStruct<TSelf,T> Chain(TSelf values) => new PolyStruct<TSelf,T>{Next,First};
}


public static class PolyStructHelpers
{
    extension<T, TSelf>()
    {
    }
}


public struct TestInner1;

public struct TestInner2;

public class Testing
{
    public static void Test()
    {
        PolyStruct<TestInner1,TestInner2> test;
    }
}
