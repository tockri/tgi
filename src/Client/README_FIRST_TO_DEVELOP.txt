■■■■[1] このプロジェクトの開発方法■■■■

(1) HTMLはPHPを使って生成
    PHPをテンプレートエンジンとしてHTML生成に利用しています。
    !!! HTMLを直接編集しないでください。 !!!

(2) CSSはSASSを使って生成
    開発効率のため、SASSを使ってCSSを生成します。
    !!! www/css以下を直接編集しないでください。!!!

(3) JSはWebPackを使って生成
    開発効率のため、WebPackを使ってJSを生成します。
    !!! www/js 以下を直接編集しないでください。 !!!

(4) Gulpを使って開発
    上記すべてを、タスクランナー「Gulp」を利用して実行します。
    これにより、ファイルを保存した瞬間に必要な自動生成が実行され、
    ブラウザが自動的にリロードされます。
    とくに、SCSSファイルを保存してCSSを自動生成したときはページ全体のリロードはせず
    ブラウザのCSS定義だけが置き換わるため、高速に結果が確認できます。

(5) デバッグ用 gulpfile.js 変更点
5-1.'server'タスクの
        proxy: 'dev.xp'
    の行を有効にして、
        server: {baseDir: './www/'}
    の行をコメントアウトすると、http://localhost:3000→http://dev.xp/ にプロキシアクセスします。
    TGI2アプリを開発しているホストのIPアドレスをhostsファイルで「dev.xp」に割り当てて
    おけば便利です。
    コメントを逆にするとlocalhost:3000のサーバーはそのまま./wwwをルートとした簡易サーバーに
    なります。後述のmockjaxを有効にすると、TGI2アプリを起動しなくてもクライアントだけ開発できます。

5-2.'js'タスクの
        .pipe(uglify())
    の行をコメントアウトすると、生成されるJavaScriptが短縮化されないようになります。

5-3.'sass'タスクの
        outputStyle: 'compressed'
    の行をコメントアウトすると、生成されるCSSが短縮化されないようになります。

5-4.sourcemapsについては、メモリを食うので一応、外してありますが、使う場合はどうぞ。

(6) アプリとの通信をエミュレートするmockjaxを使っています。
    これは jQuery.ajax() の通信をハイジャックして、js/mockjax.js に記述してある
    レスポンスをスタブとして返すようにするライブラリです。
    js/modules/Prefs.js の require('../mockjax'); をコメントアウトすると、
    実際に http://(hostname)/api/ 以下にリクエストするようになります。

(7) デバッグ用 Prefs.js 変更点
    require('../mockjax') : 上記mockjaxを使用する。製品版はコメントアウト
    API_ROOT: API通信先のURL
    SOCKET_URL: WebSocket通信先のURL
    COLORMAP_URL: カラーマップ画像取得先のURL


(8) デバッグ用クラス
    Loggerクラス
    Logger.log(object, message) を利用して、ブラウザコンソールに出力してください。



■■■■[2] クライアント開発環境構築手順■■■■

## 任意のディレクトリで実行 ##

(1) npmをインストール
　- Windows : 
    https://nodejs.org/download/
    からnode.jsをダウンロード、インストール

  - Mac : 
    Homebrewを使用
    $ brew install npm

(2) Gulpをインストール
    (-gオプションでシステムにインストール）
    $ npm install -g gulp

(3) Bowerをインストール
    (-gオプションでシステムにインストール）
    $ npm install -g bower

(4) ローカルでPHPが実行できるようにしておく必要があります。
    ただHTMLを生成するだけなので、PHPのバージョンは5.3以上であれば特に制限はありません。
   - Windows
    http://windows.php.net/download/
    からインストーラをダウンロード、インストール後PATHを通す
    
   - Mac 
    Homebrewを使用　(要mbstring）
    $ brew install php


## ここからは、このファイルがあるディレクトリで実行 ##

(5) このプロジェクト用のモジュールをすべてインストール
    $ npm install
    $ bower install

(6) Netbeansを使う場合、プロジェクトの設定で[保存時にsassファイルをコンパイル]のチェックは外すこと。

(7) 開発タスクランナーgulpを実行
    $ gulp

    以下のタスクが自動で実行されます。
    ・jsファイルを保存→www/js以下にwebpackビルド→ブラウザ再読み込み
    ・sassファイルを保存→www/css以下にsassコンパイル→ブラウザ再読み込み
    ・www/*.phpを保存→ブラウザ再読み込み