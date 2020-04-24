﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Azure.Data
{
    public static class JsonSchemaParser
    {
        public static ModelSchema ParseFile(string filename)
        {
            var schemaJson = File.ReadAllText(filename);
            ModelSchema schema = ParseJson(schemaJson);
            return schema;
        }

        public static ModelSchema ParseJson(string schemaJson)
        {
            var schema = new Dictionary<string, ModelSchema.PropertySchema>();

            var document = JsonDocument.Parse(schemaJson);
            var root = document.RootElement;

            var requiredList = root.GetProperty("required");
            List<string> requiredProperties = new List<string>();
            foreach (var required in requiredList.EnumerateArray())
            {
                requiredProperties.Add(required.GetString());
            }

            var properties = root.GetProperty("properties");
            foreach (var property in properties.EnumerateObject())
            {
                var name = property.Name;
                var propertyDescriptor = property.Value;
                var type = propertyDescriptor.GetProperty("type").GetString();

                var clrType = ToClrType(type);
                var isRequired = requiredProperties.Contains(name);
                schema.Add(name, new ModelSchema.PropertySchema(clrType, name, isReadOnly: false, isRequired));
            }

            return new JsonSchema(schema);
        }

        private static Type ToClrType(string type)
        {
            switch (type)
            {
                case "string": return typeof(string);
                case "number": return typeof(int);
                case "array": return typeof(object[]);
                case "object": return typeof(object);
                default: throw new NotImplementedException(type);
            }
        }

        class JsonSchema : ModelSchema
        {
            Dictionary<string, PropertySchema> _properties;

            public JsonSchema(Dictionary<string, PropertySchema> properties)
                => _properties = properties;

            public override IEnumerable<string> PropertyNames => _properties.Keys;

            public override bool TryGetSchema(string propertyName, out PropertySchema schema)
            {
                return _properties.TryGetValue(propertyName, out schema);
            }
        }
    }
}