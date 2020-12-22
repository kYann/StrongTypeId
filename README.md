# Introduction 
StrongTypeId prevents developer from using the wrong Id in their code.
Compatible with `Asp.Net Core routing`, `System.Text.Json`, `Newtonsoft.Json` and `NHibernate`

Record idea and TypeConverters are from [Thomas Levesque](https://twitter.com/thomaslevesque) [blog](https://thomaslevesque.com/2020/11/23/csharp-9-records-as-strongly-typed-ids-part-2-aspnet-core-route-and-query-parameters/)

Example :
```csharp

public class Product
{
    public int Id { get; set; }
}

public class Account
{
    public int Id { get; set; }
}

public void CreateItem(int productId, int accountId);

// Wrong but it compiles 😨
❎ CreateItem(account.Id, product.Id);

//////////////////////////////////////////
// With StrongTypeId, you would have
//////////////////////////////////////////

public record ProductId(int Value) : StrongTypeId(Value);

public class Product
{
    public ProductId Id { get; set; }
}

public record AccountId(int Value) : StrongTypeId(Value);

public class Account
{
    public AccountId Id { get; set; }
}

public void CreateItem(ProductId productId, AccountId accountId);

// Will not compile 👍
❌ CreateItem(account.Id, product.Id);

```

# How to use ?

```powershell
Install-Package StrongTypeId
```

Simply declare your identifiers like this :

```csharp
public record ProductId(int Value) : StrongTypeId(Value)
{
	// Needed if you wanna use your StrongTypeId in route parameters
	// Or you can use StrongTypeId.Generators to generate it
	public override string ToString() => base.ToString();
}
```

And use it like that :

```csharp
public class Product
{
    public ProductId Id { get; set; }
}
```

# What about json serialization ?

This library is compatible with `Newtonsoft.Json` and `System.Text.Json`.
We provide converters that will serialize the strong type to his Id type.
Also it will deserialize Id type to strong type.

## Newtonsoft.Json

```powershell
Install-Package StrongType.Newtonsoft.Json
```

In your Startup.cs, ConfigureServices
```csharp
services.AddMvc().AddNewtonsoftJson(options =>
    {
        // Add StrongTypeId Converter
        options.SerializerOptions.Converters.Add(new StrongTypeIdJsonConverter());
    });
```

## System.Text.Json

```powershell
Install-Package StrongTypeId.System.Text.Json
```

In your Startup.cs, ConfigureServices
```csharp
services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new StrongTypeIdJsonConverterFactory());
    });
```

# What about my ORM ?

This library is compatible with NHibernate. Entity Framework contribution are welcome !

## Fluent NHibernate

```powershell
Install-Package StrongTypeId.FluentNhibernate
```

```csharp
var fluentCfg = Fluently.Configure()
    .Mappings(m =>
    {
        m.FluentMappings.Conventions.Add<ConventionStrongTypeId>();
    });
```


# AspNet

When using strong type id in your route like this :

```csharp
var productId = new ProductId(5);
RedirectToAction("Index", new { id = productId });
```

The generated url, will use `productId.ToString()` in the Url.
By default C# create a `ToString()` method on `record` that will display :

`ProductId { Value = 5 }`

And the url will look like this :

`/Product/Index/ProductId { Value = 5 }` instead of `/Product/Index/5`

To prevent this, you have to override `ToString()` or use the source generator that will do it for you.

# GraphQL.Net

Register specific ScalarType for GraphQL.Net

```powershell
Install-Package StrongTypeId.GraphQL
```

Inside your Schema :

```csharp
using StrongType.GraphQL;

public class MySchema : Schema
{
	public MySchema(IHttpContextAccessor httpContextAccessor)
		: base(httpContextAccessor.HttpContext.RequestServices)
	{
		this.AddStrongTypeId<ProductId>();
	}
}
```

# GraphQL HotChocolate
```powershell
Install-Package StrongTypeId.HotChocolate
```

In your Startup.cs, ConfigureServices :

```csharp
using StrongType.HotChocolate;

...

services.AddGraphQLServer()
	.BindStrongTypeId<ProductId>()
```

# Generators
```powershell
Install-Package StrongTypeId.Generators
```

Then you must declare your record as partial :

```csharp
public partial record ProductId(int Value) : StrongTypeId(Value);
```

And the generator will generate the `ToString()` method at compilation.