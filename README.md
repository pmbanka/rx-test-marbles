# rx-test-marbles

This is an experimental C# DSL for testing Rx streams. 

The syntax is inspired by [RxJS Marble Tests](https://github.com/ReactiveX/rxjs/blob/master/doc/writing-marble-tests.md), so all credits goes to them. Syntax for defining observables is identical.

Example usage (see more in the tests project):

```csharp
[Fact]
public void MergeObservablesWorksCorrectly()
{
    using (var s = new MarbleScheduler())
    {
        var e1 = s.Hot("----a--^--b-------c--|");
        var e2 = s.Hot(  "---d-^--e---------f-----|");
        var expected =        "---(be)----c-f-----|";
        s.ExpectObservable(e1.Merge(e2)).ToBe(expected);
    }
}
```

I'm not sure whether this DSL is something useful or not, hence no Nuget. If (for some weird reason) you would like to use this code, consider using Paket's [Github dependency](https://fsprojects.github.io/Paket/github-dependencies.html).
