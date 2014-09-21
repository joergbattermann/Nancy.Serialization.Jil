# Nancy.Serialization.Jil [![NuGet Version](http://img.shields.io/nuget/v/Nancy.Serialization.Jil.svg?style=flat)](https://www.nuget.org/packages/Nancy.Serialization.Jil/) [![Build status](https://ci.appveyor.com/api/projects/status/8m1agvwx1qukrrc1)](https://ci.appveyor.com/project/jbattermann/nancy-serialization-jil)


Implementations of the ISerialization and IBodyDeserializer interfaces, based on [Jil](https://github.com/kevin-montrose/Jil), for [Nancy](http://nancyfx.org). Because a little bit more [Performance](https://github.com/kevin-montrose/Jil/blob/master/README.md#benchmarks) never hurts.

## Usage

Start of by installing the [Nancy.Serialization.Jil](https://www.nuget.org/packages/Nancy.Serialization.Jil/) NuGet package:

`PM> Install-Package Nancy.Serialization.Jil`

When Nancy detects that the `JilSerializer` and `JilBodyDeserializer` types are available in the AppDomain of your application, it will assume you want to use them, rather than the default ones.

### Customization & Jil Options

By default [Jil's Options](https://github.com/kevin-montrose/Jil#configuration) are set to `Options.ISO8601IncludeInherited` for both, Serialization and De-Serialization. If you want to change that behaviour, choose one of the many [Configuration Options](https://github.com/kevin-montrose/Jil/blob/master/README.md#configuration) available and change the Default one used like that:

```
    // change only the JilSerializer's Options
    Nancy.Serialization.Jil.JilSerializer.Options = Options.ISO8601PrettyPrintNoHashing;
    
    // change only the JilBodyDeserializer's Options
    Nancy.Serialization.Jil.JilBodyDeserializer.Options = Options.ISO8601PrettyPrintNoHashingIncludeInherited;
    
    // or for both:
    Nancy.Serialization.Jil.JilSerializer.Options = Nancy.Serialization.Jil.JilBodyDeserializer.Options = Options.ISO8601PrettyPrintNoHashing; // that's usually what you'd like I guess
```

### (De-)Serializing nested Object Graphs

Jil, by default, does not (serialize) nested object graphs and ignores inheritance and its structure (Json.Net however does). In order to get the same behaviour, the default [Options](https://github.com/kevin-montrose/Jil#configuration) has been switched from `Options.Default` to `Options.ISO8601IncludeInherited` (see https://github.com/kevin-montrose/Jil/issues/71 over at Jil's repository for details). If you don't want or need inherited members to be serialized, you can easily switch this back following the instructions above.

## Limitations

As this library is based on [Jil](https://github.com/kevin-montrose/Jil), all its Features but also Limitations also apply here: .Net >= 4.5 is required, but even more so please take a close look at its [List of supported types](https://github.com/kevin-montrose/Jil/blob/master/README.md#supported-types) and how to [configure and tailor Jil](https://github.com/kevin-montrose/Jil/blob/master/README.md#configuration) to your specific environment and use case. However, one thing I've noticed is that Jil seems to require a public, empty .ctor for the types to be deserialized. Keep that in mind when designing your DTOs.

### Changes between Nancy.Serialization.Jil and *.JsonNet
As I haven't used [Nancy.Serialization.JsonNet](https://github.com/NancyFx/Nancy.Serialization.JsonNet) myself, I cannot say much about Nancy.Serialization.Jil's being a simple drop-in replacement, but Jil's Default set of options (see above) does come with a differen Json notation compared to Json.Net when it comes to Property-Names. When porting the tests over, I noticed the original json test fixtures ones had property names like .someProperty (even though the classes propery names were in fact .SomeProperty). Jil on the other hand keeps these properties at their original case, meaning a property named .SomeProperty remains SomeProperty inside the serialized Json.

## Thanks

This is basically a port of [Nancy.Serialization.JsonNet](https://github.com/NancyFx/Nancy.Serialization.JsonNet) but replacing the [Json.NET](http://json.codeplex.com/) specific parts with Jil's ones and adjusting the test correspondingly. So thanks to Andreas Håkansson, Steven Robbins and the other original contributors.

Moreover thanks alot to [Kevin](https://github.com/kevin-montrose) for his awesome work on [Jil](https://github.com/kevin-montrose/Jil).

## Copyright

Copyright © 2014 Jörg Battermann

## License

Nancy.Serialization.Jil is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to [LICENSE.md](https://github.com/jbattermann/Nancy.Serialization.Jil/blob/master/LICENSE.md) for more information.
