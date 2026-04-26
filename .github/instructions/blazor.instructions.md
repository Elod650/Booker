---
applyTo: '**/*.razor'
---

# Blazor Instructions

**MANDATORY: These standards must be followed for ALL Blazor code in this repository. Review this entire file before writing any Blazor components.**

## Component Structure

### .razor Files

.razor files should contain **ONLY** the markup. Do **NOT** use `@code` blocks.

- **Example**
  ```razor
  <!-- ✅ Correct -->
  @page "/counter"
  @rendermode InteractiveWebAssembly
  
  <PageTitle>Counter</PageTitle>
  
  <h1>Counter</h1>
  <p role="status">Current count: @Count</p>
  <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
  
  @code {
      // ❌ This code block should NOT exist here
  }
  ```

  ```razor
  <!-- ✅ Correct -->
  @page "/counter"
  @rendermode InteractiveWebAssembly
  
  <PageTitle>Counter</PageTitle>
  
  <h1>Counter</h1>
  <p role="status">Current count: @Count</p>
  <button class="btn btn-primary" @onclick="IncrementCount">Click me</button>
  
  <!-- All C# code is in the partial class -->
  ```

### .razor.cs Partial Classes

All C# logic must be in a separate `ComponentName.razor.cs` file as a partial class.

- **Example**
  ```csharp
  // ✅ Correct - Counter.razor.cs
  namespace MyNamespace;
  
  [Route("/counter")]
  public partial class Counter
  {
      [Parameter]
      public int InitialValue { get; set; } = 0;
  
      private int Count { get; set; }
  
      protected override async Task OnInitializedAsync()
      {
          this.Count = this.InitialValue;
          await base.OnInitializedAsync();
      }
  
      private void IncrementCount()
      {
          this.Count++;
      }
  }
  ```

## Lifecycle Methods

### Use Async Versions When Possible

Always use async lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`, `OnAfterRenderAsync`) instead of their synchronous counterparts (`OnInitialized`, `OnParametersSet`, `OnAfterRender`). This applies even if you don't have async work to perform initially — it allows for future async operations without refactoring.

- **Example**
  ```csharp
  // ✅ Correct
  protected override async Task OnInitializedAsync()
  {
      var data = await this.LoadDataAsync();
      await base.OnInitializedAsync();
  }
  
  // ✅ Also Correct (no async work, but keeps door open)
  protected override async Task OnInitializedAsync()
  {
      await base.OnInitializedAsync();
  }
  
  // ❌ Incorrect
  protected override void OnInitialized()
  {
      var data = this.LoadData();
      base.OnInitialized();
  }
  ```

## .razor.cs File Organization

The members in `.razor.cs` files must be ordered as follows:

1. **Properties with `[Parameter]` attribute** — Component parameters first
2. **Properties** — Other public/private properties
3. **Fields** — Private backing fields
4. **OnInitializedAsync** — Initialization logic
5. **OnParametersSetAsync** — Parameter change handling
6. **OnAfterRenderAsync** — Post-render logic
7. **Other methods** — All remaining methods in logical order

- **Example**
  ```csharp
  namespace MyNamespace;
  
  public partial class MyComponent : ComponentBase
  {
      // 1. Properties with [Parameter]
      [Parameter]
      public string Title { get; set; } = string.Empty;
  
      [Parameter]
      public int MaxItems { get; set; } = 10;
  
      [Parameter]
      public EventCallback<string> OnItemSelected { get; set; }
  
      // 2. Properties
      public bool IsLoading { get; set; }
      public string? ErrorMessage { get; set; }
  
      // 3. Fields
      private List<Item> _items = [];
      private bool _initialized = false;
  
      // 4. OnInitializedAsync
      protected override async Task OnInitializedAsync()
      {
          this._items = await this.LoadItemsAsync();
          this._initialized = true;
          await base.OnInitializedAsync();
      }
  
      // 5. OnParametersSetAsync
      protected override async Task OnParametersSetAsync()
      {
          if (this._initialized)
          {
              this._items = await this.LoadItemsAsync();
          }
          await base.OnParametersSetAsync();
      }
  
      // 6. OnAfterRenderAsync
      protected override async Task OnAfterRenderAsync(bool firstRender)
      {
          if (firstRender)
          {
              await this.InitializeJsInteropAsync();
          }
          await base.OnAfterRenderAsync(firstRender);
      }
  
      // 7. Other methods
      private async Task LoadItemsAsync()
      {
          this.IsLoading = true;
          try
          {
              return await this.FetchItemsAsync();
          }
          catch (Exception ex)
          {
              this.ErrorMessage = ex.Message;
              return [];
          }
          finally
          {
              this.IsLoading = false;
          }
      }
  
      private async Task InitializeJsInteropAsync()
      {
          // JS interop logic
          await Task.CompletedTask;
      }
  
      private async Task FetchItemsAsync()
      {
          // Fetch logic
          return [];
      }
  
      private async Task OnItemClick(Item item)
      {
          await this.OnItemSelected.InvokeAsync(item.Id);
      }
  }
  ```

## Summary

| Rule | Requirement |
|------|-------------|
| Code in .razor files | ❌ No `@code` blocks |
| Code location | ✅ Separate `.razor.cs` partial class |
| Lifecycle methods | ✅ Use async versions (Async suffix) |
| File organization | ✅ Follow ordering: Parameters → Properties → Fields → OnInitializedAsync → OnParametersSetAsync → OnAfterRenderAsync → Other methods |
