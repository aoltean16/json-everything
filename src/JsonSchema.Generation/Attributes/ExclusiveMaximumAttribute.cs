﻿using System;
using Json.More;
using Json.Schema.Generation.Intents;

namespace Json.Schema.Generation;

/// <summary>
/// Applies an `exclusiveMaximum` keyword.
/// </summary>
/// <remarks>
/// The `value` parameter is provided in the constructor as a `double` but stored as a `decimal`
/// because `decimal` is not a valid attribute parameter type.
/// As such, to prevent overflows, the value is clamped to the `decimal` range prior to being converted.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field |
                AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = true)]
public class ExclusiveMaximumAttribute : ConditionalAttribute, INestableAttribute, IAttributeHandler
{
	/// <summary>
	/// The exclusive maximum.
	/// </summary>
	public decimal Value { get; }

	/// <summary>
	/// The index of the parameter to which the attribute should apply. Default is -1 to indicate the root.
	/// </summary>
	public int GenericParameter { get; set; } = -1;

	/// <summary>
	/// Creates a new <see cref="ExclusiveMaximumAttribute"/> instance.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <remarks>
	/// The <paramref name="value"/> parameter is provided as a `double` but stored as a `decimal`
	/// because `decimal` is not a valid attribute parameter type.
	/// As such, to prevent overflows, the value is clamped to the `decimal` range prior to being converted.
	/// </remarks>
	public ExclusiveMaximumAttribute(double value)
	{
		Value = value.ClampToDecimal();
	}

	void IAttributeHandler.AddConstraints(SchemaGenerationContextBase context, Attribute attribute)
	{
		if (!context.Type.IsNumber() && !context.Type.IsNullableNumber()) return;

		context.Intents.Add(new ExclusiveMaximumIntent(Value));
	}
}