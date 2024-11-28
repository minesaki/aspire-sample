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

# AppHostにRedisを追加（.NET Aspireにより、Docker上で起動される）
dotnet add Aspire.AppHost package Aspire.Hosting.Redis
```

例：APIからDB（例としてPostgreSQL）に接続する
```
# AppHostにPostgreSQLを追加（.NET Aspireにより、Docker上で起動される）
dotnet add Aspire.AppHost package Aspire.Hosting.PostgreSQL

# WebAPIに機能追加
dotnet add WebAPI package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
```

ローカル実行
```
dotnet run --project Aspire.AppHost

# (以下も便利ですが、終了時にDockerコンテナが削除されなくなるようです)
# dotnet watch --project Aspire.AppHost
```

## Azure Container Apps へのデプロイ

（動作は未確認。下記は調査結果で、後日確認予定）
```
# ソリューションルートで
azd init
# Use code in the current directory を選択
# .NET Aspire が検知されることを確認して、 Confirm and continue initializing my app を選択
# 任意の環境名を指定する

# bicepファイルを生成する（infraフォルダに生成される）
azd infra synth

# デプロイ
azd up
# サブスクリプション、リージョンを選択すると、デプロイされる
```

## Aspir8 を使用した Kubernetes へのデプロイ

Distribution を作成（旧 Docker Registry。Dockerイメージを置く場所が他にある場合は不要）
```
docker run -d -p 6000:5000 --name registry registry:latest
```

Aspir8 インストール
```
dotnet tool install -g aspirate
```

### 以降の作業は AppHost プロジェクトに移動した状態で実施する
```
cd Aspire.AppHost
```

Aspire8 初期化
```
aspirate init -cr localhost:6000 -ct latest --disable-secrets true --non-interactive
```
AppHost プロジェクト内に Aspir8 の設定ファイル aspirate.json が生成される。  

※なお、本来、秘匿情報はSecretsとしてデプロイするのが望ましい。そうするには `--disable-secrets true` の指定を除いて実行する。ただし、パスワードの指定が必要なため `--non-interactive` も指定できなくなる。

コンテナイメージのビルド、マニフェスト生成
```
aspirate generate --image-pull-policy Always --include-dashboard true --disable-secrets true --non-interactive
```
コンテナイメージがビルドされて、Distributionにプッシュされる。
また、AppHostプロジェクト内の aspirate-output フォルダにKubernetesマニフェストが出力される。  

※Secretsを生成する場合は `--disable-secrets true --non-interactive` を除いて実行する。Secretsの情報は暗号化されるため、パスワードの指定を求められる。
また、運用環境では既存のRedisやDBに接続するように指定した場合、その接続文字列の入力もこの時点で求められる。入力内容は暗号化されて aspirate-state.json に保存されるので、バージョン管理が可能。

#### 注意点
2024/11/17現在、以下の問題がある
- ダッシュボードのマニフェストも生成されるが、コンテナイメージのバージョンが9.0ではなく、8.0になっている。8.0でも問題なく動作するようだが、手動で9.0に修正した。

コンテキストをデプロイ先に切り替え
```
# コンテキストの一覧と現在のコンテキストを確認
kubectl config get-contexts

# コンテキストの切り替え
# kubectl config use-context <context>
# 例: Docker DesktopのKubernetesクラスタ（docker-desktopコンテキスト）に切り替え
kubectl config use-context docker-desktop
```

Kubernetesへのデプロイ
```
aspirate apply --non-interactive
```
※Secretsを生成した場合は、生成時に指定したパスワードの入力を求められる。

#### 注意点
- 生成されるServiceのマニフェストがClusterIPのため、このままではクラスタ外からアクセスする手段がない。環境に応じてLoadBalancerやIngressを別途デプロイする。
  - Docker DesktopのKubernetesクラスタ用のLoadBalancerを手動作成した。 `kubectl apply -f Aspire.AppHost/additional-manifest/docker-desktop` でデプロイ可能。


デプロイの削除
```
aspirate destroy --non-interactive
```
