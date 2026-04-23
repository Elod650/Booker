---
applyTo: '**/*.cs'
---

# C# Instructions

**MANDATORY: These standards must be followed for ALL C# code in this repository. Review this entire file before writing any C# code.**

## Syntax for every C# file:**

**REQUIRED for every C# file:**

- **File-scoped namespaces**: Use `namespace MyProject.Domain;` (not block syntax)
    - **Example**
    ```csharp
    // ✅ Correct
    namespace MyNamespace;
    public class MyClass
    {
       // Class implementation
    }
    
    // ❌ Incorrect
    namespace MyNamespace
    {
       public class MyClass
       {
          // Class implementation
       }
    }
    ```

- **Use Global Usings**: Every project should contains a `GlobalUsings.cs` file with `global using` directives. Place all `using` directives into these.
- **Example**
    ```csharp
    // ✅ Correct
    // In GlobalUsings.cs
    global using System;
    global using System.Collections.Generic;
    global using System.Linq;

    // ❌ Incorrect
    // In each .cs file
    using System;
    using System.Collections.Generic;
    using System.Linq;
    ```

- **Collection Initializers**: Use `[]` instead of `new List<T>()` when initializing collections.
    - **Example**
    ```csharp
    // ✅ Correct
    List<T> myList = [item1, item2, item3];
    
    // ❌ Incorrect
    List<T> myList = new List<T> { item1, item2, item3 };
    ```

- **Primary Constructors**: Use primary constructors unless aditional logic is required in the constructor body.
    - **Example**
    ```csharp
    // ✅ Correct
    public class MyClass(string name, int age)
    {
    
    }
    
    // ❌ Incorrect
    public class MyClass
    {
       private string Name;
       private int age
    
       public MyClass(string name, int age)
       {
          name = name;
          age = age;
       }
    }
    ```
    
- **`var` usage:** Use `var` when the type is apparent or on the right-hand side; use explicit types for built-in types (`int`, `string`, etc.).
    - **Example**
    ```csharp
    // ✅ Correct
    string name = "John";
    var Person = new Person(name, 30);
    
    // ❌ Incorrect
    var name = "John";
    Person person = new Person(name, 30);
    ```

- **Braces:** Opening brace on new line. Always use braces, even for single-line blocks.
    - **Example**
    ```csharp
    // ✅ Correct
    if (condition)
    {
        DoSomething();
    }
    
    // ❌ Incorrect
    if (condition) {
        DoSomething();
    }

    if (condition) DoSomething();

    if (condition) 
        DoSomething();
    ```

- **Access modifiers:** Always declare explicitly. Except public interface members.
    - **Example**
    ```csharp
    // ✅ Correct
    public class MyClass
    {
        private int _myField;
        protected string MyProperty { get; set; }
        internal void MyMethod() { }
    }
    
    // ❌ Incorrect
    class MyClass
    {
        int _myField;
        string MyProperty { get; set; }
        void MyMethod() { }
    }
    ```

- **`this.` qualification:** Use `this.` for fields and properties and methods. Parameters from primary constructors are not instance members and cannot be qualified with `this.`.
    - **Example**
    ```csharp
    // ✅ Correct
    public class MyClass
    {
        private int _myField;
        protected string MyProperty { get; set; }
        internal void MyMethod() 
        {
            this._myField = 10;
            this.MyProperty = "Hello";
            this.MyMethod();
        }
    }

    // ✅ Correct — primary constructor parameters are accessed without `this.`
    public class MyService(ILogger<MyService> logger, IOptions<MyOptions> options)
    {
        private readonly MyOptions _options = options.Value;

        public void DoWork()
        {
            logger.LogInformation("Working");
            this._options.ToString();
        }
    }
    
    // ❌ Incorrect
    public class MyClass
    {
        private int _myField;
        protected string MyProperty { get; set; }
        internal void MyMethod() 
        {
            _myField = 10;
            MyProperty = "Hello";
            MyMethod();
        }
    }
    ```

- **Interface naming:** Always prefix with `I` (enforced as error).
    - **Example**
    ```csharp
    // ✅ Correct
    public interface IMyInterface
    {
        void MyMethod();
    }
    
    // ❌ Incorrect
    public interface MyInterface
    {
        void MyMethod();
    }
    ```

- **Modifier order:** `public, private, protected, internal, file, static, extern, new, virtual, abstract, sealed, override, readonly, unsafe, required, volatile, async`
    - **Example**
    ```csharp
    // ✅ Correct
    public class MyClass
    {
        private static readonly int MyField;
        public async Task MyMethod() { }
    }
    
    // ❌ Incorrect
    public class MyClass
    {
        static private readonly int MyField;
        async public Task MyMethod() { }
    }
    ```

- **Use developer appsettings:** For any configuration values, use `appsettings.Development.json` with the Options pattern. Do not hardcode values in code or use environment variables directly. The `appsettings.json` file should only contain default values or placeholders.
    - **Example**
    ```csharp
    // ✅ Correct
    // In appsettings.json
    {
        "MyOptions": {
            "Option1": "",
            "Option2": 0
        }
    }
    // In appsettings.Development.json
    {
        "MyOptions": {
            "Option1": "Value1",
            "Option2": 10
        }
    }
    // In code
    public class MyService
    {
        private readonly MyOptions _options;
        
        public MyService(IOptions<MyOptions> options)
        {
            this._options = options.Value;
        }
    }
    
    // ❌ Incorrect
    public class MyService
    {
        private readonly string _option1 = "Value1";
        private readonly int _option2 = 10;
        
        public MyService()
        {
        }
    }

    // In appsettings.json
    {
        "MyOptions": {
            "Option1": "Value1",
            "Option2": 10
        }
    }

    // appsettings.Development.json is missing
    ```

- **Use `nameof()` for type-safe references:** When referencing types, members, or options keys, always use `nameof()` to avoid magic strings and ensure refactor safety.
    - **Example**
    ```csharp
    // ✅ Correct
    string className = nameof(MyClass);
    
    // ❌ Incorrect
    string className = "MyClass";
    ```

- **Don't use `else`:** Instead of using `else`, return early from the method to reduce nesting and improve readability.
    - **Example**
    ```csharp
    // ✅ Correct
    public void MyMethod()
    {
        if (!condition)
        {
            return;
        }
        
        DoSomething();
    }
    
    // ❌ Incorrect
    public void MyMethod()
    {
        if (condition)
        {
            DoSomething();
        }
        else
        {
            return;
        }
    }
    ```

- **Use `is` for null checks:** Use pattern matching with `is null` and `is not null` for null checks instead of `== null` or `!= null`.
    - **Example**
    ```csharp
    // ✅ Correct
    if (myObject is null)
    {
        // Handle null case
    }

    if (myObject is not null)
    {
        // Handle non-null case
    }
    
    // ❌ Incorrect
    if (myObject == null)
    {
        // Handle null case
    }

    if (myObject != null)
    {
        // Handle non-null case
    }
    ```