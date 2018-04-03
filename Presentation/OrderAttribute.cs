using System;

[AttributeUsage( AttributeTargets.Class, Inherited = false )]
public sealed class OrderAttribute : Attribute
{
    #region Properties

    public Int32 Order { get; }

    #endregion

    #region Ctor

    public OrderAttribute( Int32 order ) => Order = order;

    #endregion
}