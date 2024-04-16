using EFCore.Snowflake.Metadata;
using EFCore.Snowflake.Metadata.Internal;
using EFCore.Snowflake.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

public static class SnowflakePropertyBuilderExtensions
{
    public static IConventionSequenceBuilder? HasSequence(
        this IConventionPropertyBuilder propertyBuilder,
        string? name,
        string? schema,
        bool fromDataAnnotation = false)
    {
        if (!propertyBuilder.CanSetSequence(name, schema, fromDataAnnotation))
        {
            return null;
        }

        propertyBuilder.Metadata.SetSequenceName(name, fromDataAnnotation);
        propertyBuilder.Metadata.SetSequenceSchema(schema, fromDataAnnotation);

        return name == null
            ? null
            : propertyBuilder.Metadata.DeclaringType.Model.Builder.HasSequence(name, schema, fromDataAnnotation);
    }

    public static bool CanSetSequence(
        this IConventionPropertyBuilder propertyBuilder,
        string? name,
        string? schema,
        bool fromDataAnnotation = false)
    {
        Check.NullButNotEmpty(name, nameof(name));
        Check.NullButNotEmpty(schema, nameof(schema));

        return propertyBuilder.CanSetAnnotation(SnowflakeAnnotationNames.SequenceName, name, fromDataAnnotation)
               && propertyBuilder.CanSetAnnotation(SnowflakeAnnotationNames.SequenceSchema, schema, fromDataAnnotation);
    }


    public static IConventionPropertyBuilder? HasIdentityColumnSeed(
        this IConventionPropertyBuilder propertyBuilder,
        long? seed,
        bool fromDataAnnotation = false)
    {
        if (propertyBuilder.CanSetIdentityColumnSeed(seed, fromDataAnnotation))
        {
            propertyBuilder.Metadata.SetIdentitySeed(seed, fromDataAnnotation);
            return propertyBuilder;
        }

        return null;
    }

    public static bool CanSetIdentityColumnSeed(
        this IConventionPropertyBuilder propertyBuilder,
        long? seed,
        bool fromDataAnnotation = false)
        => propertyBuilder.CanSetAnnotation(SnowflakeAnnotationNames.IdentitySeed, seed, fromDataAnnotation);

    public static IConventionPropertyBuilder? HasIdentityColumnIncrement(
        this IConventionPropertyBuilder propertyBuilder,
        int? increment,
        bool fromDataAnnotation = false)
    {
        if (propertyBuilder.CanSetIdentityColumnIncrement(increment, fromDataAnnotation))
        {
            propertyBuilder.Metadata.SetIdentityIncrement(increment, fromDataAnnotation);
            return propertyBuilder;
        }

        return null;
    }

    public static bool CanSetIdentityColumnIncrement(
        this IConventionPropertyBuilder propertyBuilder,
        int? increment,
        bool fromDataAnnotation = false)
        => propertyBuilder.CanSetAnnotation(SnowflakeAnnotationNames.IdentityIncrement, increment, fromDataAnnotation);

    public static PropertyBuilder UseIdentityColumn(
        this PropertyBuilder propertyBuilder,
        long seed = 1,
        int increment = 1)
    {
        var property = propertyBuilder.Metadata;
        property.SetValueGenerationStrategy(SnowflakeValueGenerationStrategy.AutoIncrement);
        property.SetIdentitySeed(seed);
        property.SetIdentityIncrement(increment);
        property.SetSequenceName(null);
        property.SetSequenceSchema(null);

        return propertyBuilder;
    }

    public static PropertyBuilder UseIdentityColumn(
        this PropertyBuilder propertyBuilder,
        int seed,
        int increment = 1)
        => propertyBuilder.UseIdentityColumn((long)seed, increment);

    public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(
        this PropertyBuilder<TProperty> propertyBuilder,
        long seed = 1,
        int increment = 1)
        => (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder, seed, increment);

    public static PropertyBuilder<TProperty> UseIdentityColumn<TProperty>(
        this PropertyBuilder<TProperty> propertyBuilder,
        int seed,
        int increment = 1)
        => (PropertyBuilder<TProperty>)UseIdentityColumn((PropertyBuilder)propertyBuilder, (long)seed, increment);

    public static IConventionPropertyBuilder? HasValueGenerationStrategy(
        this IConventionPropertyBuilder propertyBuilder,
        SnowflakeValueGenerationStrategy? valueGenerationStrategy,
        bool fromDataAnnotation = false)
    {
        if (propertyBuilder.CanSetAnnotation(
                SnowflakeAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy, fromDataAnnotation))
        {
            propertyBuilder.Metadata.SetValueGenerationStrategy(valueGenerationStrategy, fromDataAnnotation);
            if (valueGenerationStrategy != SnowflakeValueGenerationStrategy.AutoIncrement)
            {
                propertyBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                propertyBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
                propertyBuilder.HasSequence(null, null, fromDataAnnotation);
            }

            if (valueGenerationStrategy != SnowflakeValueGenerationStrategy.Sequence)
            {
                propertyBuilder.HasIdentityColumnSeed(null, fromDataAnnotation);
                propertyBuilder.HasIdentityColumnIncrement(null, fromDataAnnotation);
            }

            return propertyBuilder;
        }

        return null;
    }
}
