Masterclass Blazor com André Baltieri - Balta.io

Utilizando Blazor com Dotnet 8.0


## Enhanced Navigation

A navegação do Blazor agora funciona como uma Single Page Application, mas renderiza como um Server Side Render, onde todo o corpo do HTML é gerado pelo servidor e devolvido para o usuário para que haja interação atualizada em tempo real


## Passando parâmetros para a página 

Passamos parâmetros para uma página da mesma forma que passamos no ASP.NET

```csharp
// O cifrão ($) está sendo usado apenas para ilustrar melhor o parâmetro dentro da rota neste exemplo, seu uso causará problemas ao código real
@page $"/home/{Parameter:int}"
```

Diferente do ASP.NET, no Blazor precisamos inserir o atribuito `[Parameter]` na propriedade que irá receber o parâmetro

```csharp
@body
{
    [Parameter]
    public int Parameter { get; set; } = 0;
    [Parameter]
    public string? Text { get; set; } = "Button";
}
```


## Componentes

A diferença primária de uma página para um componente é que um componente não possui a tag `@page`, mas ela vai agir exatamente como um componente criado com ASP.NET

Ao invés de passarmos o parâmetro na rota da URL nós podemos passá-lo direto em nosso componente, que através das propriedades definidas como parâmetros através do atributo `[Parameter]` lidarão com cada parâmetro recebido.

Esse componente é um arquivo `.razor` e ele será automaticamente integrado ao projeto e será reconhecido como uma tag para ser interpolada com a nossa página, por exemplo o arquivo `Incrementer.razor`

```csharp
// Demonstração da criação do componente no arquivo `Incrementer.razor`
<h3>Incrementer</h3>
<p role="status">Current count: @CurrentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">@Text</button>

@code {
    [Parameter]
    public int? CurrentCount { get; set; }
    [Parameter]
    public string? Text { get; set; } = "Click Me";

    private void IncrementCount()
    {
        CurrentCount++;
    }
}
```
```html
// Demonstração da utilização do componente Incrementer
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<Incrementer 
    CurrentCount="0"
    Text="Incrementar"
    @rendermode="InteractiveServer">
</Incrementer>

```

Note que foi criado um componente com a tag <Incrementer>, que é o nome do nosso arquivo, e suas duas propriedades "CurrentCount" e "Text", definidas pelo atributo [Parameter] são propriedades visiveis na nossa tag, e podem ser manipuladas pela nossa página.


## RenderMode

O Render Mode é o recurso do Blazor que irá definir a forma de renderização dos componentes.

Existem alguns modos de renderização: `StaticServer`, `InteractiveServer`, `InteractiveWebAssembly`, `InteractiveAuto`, informações podem ser obtidas na documentação da Microsoft.

Nós podemos definir o Render Mode ou para a página como um todo ou para componentes específicos, de maneira individual

```csharp
@rendermode InteractiveServer
```

```csharp
<Incrementer 
    CurrentCount="100"
    Text="Incrementar"
    @rendermode="InteractiveServer"
>
</Incrementer>
```

É recomendável sempre começar pelo nível mais baixo, ou seja, pelo nível de componente, e só após a utilização massiva do RenderMode por componentes individuais que devemos aplicar o RenderMode para toda a página.


## Stream Rendering

O Stream Rendering é um atributo do Blazor que nos auxiliará a fornecer uma interface que realmente responde para o usuário a cada nova requisição.

```csharp
@attribute [StreamRendering]
```

Ao invés de simplesmente aguardar, o programa irá bater no servidor, fazer uma nova renderização para o usuário para avisar que a aplicação não está travada enquanto está havendo a busca dos dados requisitados.

E o que o Stream Rendering faz é renderizar progressivamente a página final, sem perder nenhuma informação, colocando parte por parte da aplicação na tela, conforme o servidor for respondendo, o que ajuda muito em questões de `SEO`, já que nenhum dado será perdido.

Através de um simples if statement nós podemos controlar este fluxo de informações e escolher o que o usuário verá enquanto a aplicação está requisitando dados do servidor.


## Entity Framework e criação do banco de dados

Primeiro instalamos os seguintes pacotes - no caso estamos usando Sqlite, mas poderíamos estar usando SqlServer que o procedimento seria o mesmo.

Comandos:

`dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
`dotnet add package Microsoft.EntityFrameworkCore.Design`

Depois criamos a classe que representará o modelo do nosso banco de dados, onde podemos já aproveitar para inserir alguns atributos que irão definir cada coluna deste modelo.

```csharp
using System.ComponentModel.DataAnnotations;

namespace BlazorTemp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Insira no máximo 100 caracteres")]
        [MinLength(3, ErrorMessage = "Insira no mínimo 3 caracteres")]
        public string Title { get; set; } = String.Empty;
    }
}
```
Em seguida criamos nossa classe AppDbContext que herdará de DbContext do pacote EntityFramework, que será responsável por fazer a ligação com o migrations e efetivar a criação do nosso banco.

```csharp
using BlazorTemp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; } = null!;
    }
}
```

Em seguida adicionaremos um `DbContext` aos `Services` do nosso `builder` na classe principal `Program` do nosso projeto, passando uma `Connection String`[4].

```csharp
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite("DataSource=app.db;Cache=Shared"));
```

Para finalizar executamos o comando `dotnet ef migrations add v1` e um novo arquivo de extensão `.db` será gerado na solução do nosso projeto, além de uma pasta `Migrations`, com arquivos auto-gerados.


## Bind Value

O item `@bind-value` permite que liguemos determinado campo com uma propriedade do nosso formulário.

Então se criamos um classe chamada Category, que possui uma propriedade Title, podemos manipulá-la de um `InputText` através do `@bind-value`

```csharp
<EditForm Model="@Model" OnValidSubmit="OnSubmitAsync">
    <div class="mb-3">
        <label for="exampleInputEmail1" class="form-label">Email address</label>
        <InputText class="form-control" @bind-Value="Model.Title"/>
        <br />
        <button class="btn-success">Salvar</button>
    </div>
</EditForm>
```

Esse tipo ligação faz com que tudo que é feito no objeto vá para o bind-value e tudo que é feito no bind-value vá para o objeto, isso também é conhecido como `Two way binding`.

Lembrando que toda página ou componente que precisar de interatividae deve conter o `@rendermode`

Em seguida já podemos utilizar o componente `DataAnnotationsValidator`, que é um componente para ativar as validações, ou seja, tudo que tivermos de validação em nossos modos será automaticamente aplicado ao nosso campo.

```csharp
<EditForm Model="@Model" OnValidSubmit="OnSubmitAsync">
    <DataAnnotationsValidator />
    <div class="mb-3">
        <label for="exampleInputEmail1" class="form-label">Email address</label>
        <InputText class="form-control" @bind-Value="Model.Title"/>
        <br />
        <button class="btn-success">Salvar</button>
    </div>
</EditForm>
```

Também podemos adicionar um componente `ValidationSumary`, que é um resumo de tudo que deu errado conforme as validações aplicadas ao nosso formulário

```csharp
<EditForm Model="@Model" OnValidSubmit="OnSubmitAsync">
    <DataAnnotationsValidator />
    <ValidationSumary>
    <div class="mb-3">
        <label for="exampleInputEmail1" class="form-label">Email address</label>
        <InputText class="form-control" @bind-Value="Model.Title"/>
        <br />
        <button class="btn-success">Salvar</button>
    </div>
</EditForm>
```

Com as validações feitas no nosso modelo, o uso do `OnValidSubmit`, o `DataAnnotationsValidator`, `ValidationSumary` e o `@bind-Value` já temos um formulário bastante robusto, com funcionalidades e prevenção de erro com resposta para o usuário e com interação com o servidor.

Podemos substituir o `ValidationSumary` pelo `ValidationMessage` passando o parâmetro `For` e uma função anônima que retornará uma mensagem contendo o erro de validação obtido, caso o usuário não preencha o formulário corretamente.

```csharp
<EditForm Model="@Model" OnValidSubmit="OnSubmitAsync">
    <DataAnnotationsValidator />
    <div class="mb-3">
        <label for="exampleInputEmail1" class="form-label">Email address</label>
        <InputText class="form-control" @bind-Value="Model.Title"/>
        <div>
            <ValidationMessage For="@(()=> Model.Title)" />
        </div>
        <br />
        <button class="btn-success">Salvar</button>
    </div>
</EditForm>
```




### Bibliografia

[1] RenderModes............................https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0

[2] Curso de Entity Framework - Balta.io...........

[3] E-Book sobre Nulls - Balta.io............

[4] Connection String - .........................