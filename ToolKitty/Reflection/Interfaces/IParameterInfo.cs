//-----------------------------------------------------------------------
// <copyright file="C:\Development\Projects\ToolKitty\ToolKitty\Reflection\Interfaces\IParameterInfo.cs" company="">
//     Author:  
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace System.Reflection
{
    public interface IParameterInfo
    {
        Type Type { get; }

        object Data { get; }

        string Name { get; }
    }
}
