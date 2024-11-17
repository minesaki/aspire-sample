# .NET Aspire の導入方法

.NET Aspire プロジェクトテンプレートをインストール
```
dotnet new install Aspire.ProjectTemplates
```

## .NET Aspire を導入した状態の新規プロジェクトを生成する場合
テンプレートを使用してプロジェクトを生成する。
```
dotnet new [template-name]

# [template-name] に現在指定できるもの：
#   aspire-starter (サンプルアプリを含む)
#   aspire (空のアプリ)
#   aspire-(xunit|mstest|nunit) (各種テストプロジェクト)
```

## 既存のプロジェクトに .NET Aspire を導入する場合

.NET Aspireで管理したいアプリケーションを作成
```
# プロジェクト（例：WebAPIアプリと、Blazorアプリ）
dotnet new webapi --output WebAPI
dotnet new blazor --output BlazorApp

# ソリューション
dotnet new sln --name AspireSample
dotnet sln add WebAPI BlazorApp

# その他
dotnet new gitignore
```

.NET Aspire を追加
```
# AppHost
dotnet new aspire-apphost -o Aspire.AppHost
dotnet sln add Aspire.AppHost

# AppHostにアプリプロジェクトへの参照を追加
dotnet add Aspire.AppHost reference WebAPI BlazorApp

# ServiceDefaults
dotnet new aspire-servicedefaults -o Aspire.ServiceDefaults
dotnet sln add Aspire.ServiceDefaults

# アプリプロジェクトにServiceDefaultsへの参照を追加
dotnet add WebAPI reference Aspire.ServiceDefaults
dotnet add BlazorApp reference Aspire.ServiceDefaults
```

.NET Aspire パッケージを使用する場合は適宜追加する

例：BlazorAppアプリにRedisによる出力キャッシュ機能を追加する
```
# BlazorAppに機能追加
dotnet add BlazorApp package Aspire.StackExchange.Redis.OutputCaching

# AppHostにRedisを追加（.NET Aspireにより、Docker上でRedisが起動される）
dotnet add Aspire.AppHost package Aspire.Hosting.Redis
```

ローカル実行
```
dotnet run --project Aspire.AppHost
# (以下も便利ですが、終了時にDockerコンテナが削除されなくなるようです)
# dotnet watch --project Aspire.AppHost
```

## Kubernetes へのデプロイ

準備中
