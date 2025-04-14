// <copyright file="OperationStep.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk.Extensibility
{
    public enum OperationStep : ushort
    {
        PreOperation = 10,
        Operation = 20,
        PostOperation = 30
    }
}
