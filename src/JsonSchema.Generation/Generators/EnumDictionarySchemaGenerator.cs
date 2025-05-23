﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Json.Schema.Generation.Intents;

namespace Json.Schema.Generation.Generators;

internal class EnumDictionarySchemaGenerator : ISchemaGenerator
{
	public bool Handles(Type type)
	{
		if (!type.IsGenericType) return false;

		var generic = type.GetGenericTypeDefinition();
		if (generic != typeof(IDictionary<,>) &&
			generic != typeof(Dictionary<,>) &&
			generic != typeof(ConcurrentDictionary<,>))
			return false;

		var keyType = type.GenericTypeArguments[0];
		return keyType.IsEnum;
	}

	public void AddConstraints(SchemaGenerationContextBase context)
	{
		context.Intents.Add(new TypeIntent(SchemaValueType.Object));

		var keyType = context.Type.GenericTypeArguments[0];
		var keyContext = SchemaGenerationContextCache.Get(keyType);
		context.Intents.Add(new PropertyNamesIntent(keyContext));

		var valueType = context.Type.GenericTypeArguments[1];
		var valueTypeContext = SchemaGenerationContextCache.Get(valueType);
		var valueMemberContext = new MemberGenerationContext(valueTypeContext, []) { Parameter = 1 };
		context.Intents.Add(new AdditionalPropertiesIntent(valueMemberContext));

		valueMemberContext.GenerateIntents();
	}
}